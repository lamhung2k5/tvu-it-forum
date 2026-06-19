using Dapper;
using ForumAPI.Data;

namespace ForumAPI.Data
{
    public class DatabaseSeeder
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseSeeder(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Seed()
        {
            using var connection = _connectionFactory.CreateConnection();

            // Luôn seed tài khoản demo, chuyên mục và thẻ.
            // Các hàm này có kiểm tra trùng nên chạy nhiều lần vẫn an toàn.
            SeedNguoiDung(connection);
            SeedChuyenMucVaThe(connection);

            // Chỉ bỏ qua phần nội dung nếu đã có câu hỏi demo cụ thể.
            // Không dùng COUNT(*) FROM CAUHOI vì có thể database chỉ có câu hỏi cũ hoặc đã xóa mềm.
            var demoContentExists = connection.ExecuteScalar<int>(@"
                SELECT COUNT(*)
                FROM CAUHOI
                WHERE TieuDe LIKE '%FOREIGN KEY constraint failed%';
            ");

            if (demoContentExists > 0)
            {
                return;
            }

            SeedCauHoi(connection);
            SeedCauTraLoi(connection);
            SeedBinhChon(connection);
            SeedBinhLuan(connection);
        }

        private void SeedNguoiDung(System.Data.IDbConnection connection)
        {
            var users = new[]
            {
                new { HoTen = "Quản trị viên", Email = "admin.tvu.forum@gmail.com", Password = "admin123", VaiTro = "Admin" },

                new { HoTen = "Nguyễn Minh Quân", Email = "1101220001@st.tvu.edu.vn", Password = "minhquan123", VaiTro = "User" },
                new { HoTen = "Trần Thanh Trúc", Email = "1101220002@st.tvu.edu.vn", Password = "thanhtruc123", VaiTro = "User" },
                new { HoTen = "Lê Hoàng Nam", Email = "1101230003@st.tvu.edu.vn", Password = "hoangnam123", VaiTro = "User" },
                new { HoTen = "Phạm Gia Huy", Email = "1101230004@st.tvu.edu.vn", Password = "giahuy123", VaiTro = "User" },
                new { HoTen = "Võ Ngọc Hân", Email = "1101240005@st.tvu.edu.vn", Password = "ngochan123", VaiTro = "User" }
            };

            foreach (var user in users)
            {
                connection.Execute(@"
                    INSERT INTO NGUOIDUNG (HoTen, Email, MatKhauHash, VaiTro, TrangThai, NgayTao)
                    SELECT @HoTen, @Email, @MatKhauHash, @VaiTro, 1, datetime('now')
                    WHERE NOT EXISTS (
                        SELECT 1 FROM NGUOIDUNG WHERE Email = @Email
                    );
                ", new
                {
                    user.HoTen,
                    user.Email,
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    user.VaiTro
                });
            }
        }

        private void SeedChuyenMucVaThe(System.Data.IDbConnection connection)
        {
            var chuyenMucs = new[]
            {
                new { Ten = "Lập trình Backend", MoTa = "Các vấn đề về .NET, API, Dapper, JWT." },
                new { Ten = "Lập trình Frontend", MoTa = "Vue, Element Plus, giao diện và xử lý API." },
                new { Ten = "Cơ sở dữ liệu", MoTa = "SQLite, SQL Server, thiết kế bảng và khóa ngoại." },
                new { Ten = "Quản lý dự án phần mềm", MoTa = "Scrum, Sprint, GitHub, Jira." },
                new { Ten = "Hỏi đáp chung", MoTa = "Các câu hỏi khác trong học tập và thực hành IT." }
            };

            foreach (var cm in chuyenMucs)
            {
                connection.Execute(@"
                    INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa, TrangThai)
                    SELECT @Ten, @MoTa, 1
                    WHERE NOT EXISTS (
                        SELECT 1 FROM CHUYENMUC WHERE TenChuyenMuc = @Ten
                    );
                ", cm);
            }

            var tags = new[]
            {
                "dotnet", "dapper", "sqlite", "jwt", "api",
                "vuejs", "element-plus", "frontend", "backend",
                "database", "scrum", "git", "github", "jira"
            };

            foreach (var tag in tags)
            {
                connection.Execute(@"
                    INSERT INTO THE (TenThe, MoTa, TrangThai)
                    SELECT @TenThe, @MoTa, 1
                    WHERE NOT EXISTS (
                        SELECT 1 FROM THE WHERE TenThe = @TenThe
                    );
                ", new
                {
                    TenThe = tag,
                    MoTa = $"Thẻ liên quan đến {tag}"
                });
            }
        }

        private void SeedCauHoi(System.Data.IDbConnection connection)
        {
            var backendId = GetChuyenMucId(connection, "Lập trình Backend");
            var databaseId = GetChuyenMucId(connection, "Cơ sở dữ liệu");
            var frontendId = GetChuyenMucId(connection, "Lập trình Frontend");

            var minhQuanId = GetUserId(connection, "1101220001@st.tvu.edu.vn");
            var thanhTrucId = GetUserId(connection, "1101220002@st.tvu.edu.vn");
            var hoangNamId = GetUserId(connection, "1101230003@st.tvu.edu.vn");

            var q1 = connection.ExecuteScalar<int>(@"
                INSERT INTO CAUHOI 
                    (ID_NguoiDung, ID_ChuyenMuc, TieuDe, NoiDung, TrangThai, LuotXem, NgayTao, IsDeleted)
                VALUES 
                    (@UserId, @ChuyenMucId, @TieuDe, @NoiDung, 1, 24, datetime('now', '-5 days'), 0);
                SELECT last_insert_rowid();
            ", new
            {
                UserId = minhQuanId,
                ChuyenMucId = backendId,
                TieuDe = "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite",
                NoiDung = "Mình đang dùng .NET Minimal API, Dapper và SQLite. Khi thêm câu hỏi thì bị lỗi FOREIGN KEY constraint failed. Nguyên nhân thường là gì và kiểm tra như thế nào?"
            });

            GanTheChoCauHoi(connection, q1, new[] { "sqlite", "dapper", "dotnet", "database" });

            var q2 = connection.ExecuteScalar<int>(@"
                INSERT INTO CAUHOI 
                    (ID_NguoiDung, ID_ChuyenMuc, TieuDe, NoiDung, TrangThai, LuotXem, NgayTao, IsDeleted)
                VALUES 
                    (@UserId, @ChuyenMucId, @TieuDe, @NoiDung, 1, 18, datetime('now', '-4 days'), 0);
                SELECT last_insert_rowid();
            ", new
            {
                UserId = thanhTrucId,
                ChuyenMucId = frontendId,
                TieuDe = "Vue Router không chuyển trang sau khi đăng nhập",
                NoiDung = "Sau khi login thành công mình đã lưu token vào localStorage nhưng router không chuyển về trang chủ. Mình nên kiểm tra ở đâu?"
            });

            GanTheChoCauHoi(connection, q2, new[] { "vuejs", "frontend", "jwt" });

            var q3 = connection.ExecuteScalar<int>(@"
                INSERT INTO CAUHOI 
                    (ID_NguoiDung, ID_ChuyenMuc, TieuDe, NoiDung, TrangThai, LuotXem, NgayTao, IsDeleted)
                VALUES 
                    (@UserId, @ChuyenMucId, @TieuDe, @NoiDung, 1, 31, datetime('now', '-3 days'), 0);
                SELECT last_insert_rowid();
            ", new
            {
                UserId = hoangNamId,
                ChuyenMucId = databaseId,
                TieuDe = "Soft Delete nên dùng IsDeleted hay xóa vật lý?",
                NoiDung = "Trong hệ thống diễn đàn, khi người dùng xóa câu hỏi hoặc câu trả lời thì nên xóa vật lý hay dùng cột IsDeleted?"
            });

            GanTheChoCauHoi(connection, q3, new[] { "database", "backend", "api" });
        }
        private void GanTheChoCauHoi(System.Data.IDbConnection connection, int idCauHoi, string[] tags)
        {
            foreach (var tag in tags)
            {
                var idThe = connection.ExecuteScalar<int>(
                    "SELECT ID_The FROM THE WHERE TenThe = @TenThe",
                    new { TenThe = tag }
                );

                connection.Execute(@"
                    INSERT OR IGNORE INTO CauHoi_The (ID_CauHoi, ID_The)
                    VALUES (@ID_CauHoi, @ID_The);
                ", new
                {
                    ID_CauHoi = idCauHoi,
                    ID_The = idThe
                });
            }
        }

        private void SeedCauTraLoi(System.Data.IDbConnection connection)
        {
            var thanhTrucId = GetUserId(connection, "1101220002@st.tvu.edu.vn");
            var hoangNamId = GetUserId(connection, "1101230003@st.tvu.edu.vn");

            var cauHoi1 = connection.ExecuteScalar<int>(
                "SELECT ID_CauHoi FROM CAUHOI WHERE TieuDe LIKE '%FOREIGN KEY%' LIMIT 1;"
            );

            connection.Execute(@"
                INSERT INTO CAUTRALOI 
                    (ID_CauHoi, ID_NguoiDung, NoiDung, DaChapNhan, IsDeleted, NgayTao)
                VALUES
                    (@ID_CauHoi, @ID_NguoiDung, @NoiDung, 1, 0, datetime('now', '-4 days'));
            ", new
            {
                ID_CauHoi = cauHoi1,
                ID_NguoiDung = thanhTrucId,
                NoiDung = "Lỗi này thường do ID_ChuyenMuc hoặc ID_NguoiDung không tồn tại trong bảng cha. Bạn nên kiểm tra dữ liệu trong bảng CHUYENMUC và NGUOIDUNG trước khi insert CAUHOI."
            });

            connection.Execute(@"
                INSERT INTO CAUTRALOI 
                    (ID_CauHoi, ID_NguoiDung, NoiDung, DaChapNhan, IsDeleted, NgayTao)
                VALUES
                    (@ID_CauHoi, @ID_NguoiDung, @NoiDung, 0, 0, datetime('now', '-3 days'));
            ", new
            {
                ID_CauHoi = cauHoi1,
                ID_NguoiDung = hoangNamId,
                NoiDung = "Ngoài ra bạn nên kiểm tra body gửi từ frontend hoặc Swagger có đúng tên field không. Nếu idChuyenMuc không bind được thì giá trị có thể thành 0."
            });
        }

        private void SeedBinhChon(System.Data.IDbConnection connection)
        {
            var user1 = GetUserId(connection, "1101220001@st.tvu.edu.vn");
            var user2 = GetUserId(connection, "1101220002@st.tvu.edu.vn");
            var user3 = GetUserId(connection, "1101230003@st.tvu.edu.vn");

            var cauHoi1 = connection.ExecuteScalar<int>(
                "SELECT ID_CauHoi FROM CAUHOI WHERE TieuDe LIKE '%FOREIGN KEY%' LIMIT 1;"
            );

            connection.Execute(@"
                INSERT OR IGNORE INTO BINHCHON 
                    (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, GiaTri, NgayTao)
                VALUES
                    (@UserId, 'CAUHOI', @ID_DoiTuong, 1, datetime('now'));
            ", new { UserId = user2, ID_DoiTuong = cauHoi1 });

            connection.Execute(@"
                INSERT OR IGNORE INTO BINHCHON 
                    (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, GiaTri, NgayTao)
                VALUES
                    (@UserId, 'CAUHOI', @ID_DoiTuong, 1, datetime('now'));
            ", new { UserId = user3, ID_DoiTuong = cauHoi1 });
        }

        private void SeedBinhLuan(System.Data.IDbConnection connection)
        {
            var minhQuanId = GetUserId(connection, "1101220001@st.tvu.edu.vn");
            var thanhTrucId = GetUserId(connection, "1101220002@st.tvu.edu.vn");

            var cauHoi1 = connection.ExecuteScalar<int>(
                "SELECT ID_CauHoi FROM CAUHOI WHERE TieuDe LIKE '%FOREIGN KEY%' LIMIT 1;"
            );

            connection.Execute(@"
                INSERT INTO BINHLUAN 
                    (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, NoiDung, IsDeleted, NgayTao)
                VALUES
                    (@UserId, 'CAUHOI', @ID_DoiTuong, @NoiDung, 0, datetime('now'));
            ", new
            {
                UserId = thanhTrucId,
                ID_DoiTuong = cauHoi1,
                NoiDung = "Bạn thử kiểm tra bảng CHUYENMUC đã có ID tương ứng chưa nha."
            });

            connection.Execute(@"
                INSERT INTO BINHLUAN 
                    (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, NoiDung, IsDeleted, NgayTao)
                VALUES
                    (@UserId, 'CAUHOI', @ID_DoiTuong, @NoiDung, 0, datetime('now'));
            ", new
            {
                UserId = minhQuanId,
                ID_DoiTuong = cauHoi1,
                NoiDung = "Cảm ơn bạn, đúng là mình gửi sai ID_ChuyenMuc nên bị lỗi."
            });
        }

        private int GetUserId(System.Data.IDbConnection connection, string email)
        {
            var userId = connection.ExecuteScalar<int?>(@"
                SELECT ID_NguoiDung
                FROM NGUOIDUNG
                WHERE Email = @Email
                LIMIT 1;
            ", new { Email = email });

            if (userId == null)
            {
                throw new InvalidOperationException($"Không tìm thấy người dùng có email: {email}");
            }

            return userId.Value;
        }

        private int GetChuyenMucId(System.Data.IDbConnection connection, string tenChuyenMuc)
        {
            var chuyenMucId = connection.ExecuteScalar<int?>(@"
                SELECT ID_ChuyenMuc
                FROM CHUYENMUC
                WHERE TenChuyenMuc = @TenChuyenMuc
                LIMIT 1;
            ", new { TenChuyenMuc = tenChuyenMuc });

            if (chuyenMucId == null)
            {
                throw new InvalidOperationException($"Không tìm thấy chuyên mục: {tenChuyenMuc}");
            }

            return chuyenMucId.Value;
        }
    }
}