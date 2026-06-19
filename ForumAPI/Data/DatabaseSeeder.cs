using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

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

            SeedNguoiDung(connection);
            SeedChuyenMuc(connection);
            SeedThe(connection);
            SeedCauHoi(connection);
            SeedCauTraLoi(connection);
            SeedBinhChon(connection);
            SeedBinhLuan(connection);
            SeedThongBao(connection);
            SeedToCao(connection);
        }

        private void SeedNguoiDung(IDbConnection connection)
        {
            var users = new[]
            {
                new UserSeed("Quản trị viên", "admin@tvu.edu.vn", "admin123", "Admin", 1, 12),

                new UserSeed("Nguyễn Minh Quân", "1101220001@st.tvu.edu.vn", "quan123", "User", 1, 12),
                new UserSeed("Trần Thanh Trúc", "1101220002@st.tvu.edu.vn", "truc123", "User", 1, 11),
                new UserSeed("Lâm Tấn Hưng", "1101220003@st.tvu.edu.vn", "hung123", "User", 1, 10),
                new UserSeed("Huỳnh Bảo Ngân", "1101220004@st.tvu.edu.vn", "ngan123", "User", 1, 10),
                new UserSeed("Phạm Gia Khang", "1101220005@st.tvu.edu.vn", "khang123", "User", 1, 9),

                new UserSeed("Võ Thành Đạt", "1101230006@st.tvu.edu.vn", "dat123", "User", 1, 9),
                new UserSeed("Đặng Minh Thư", "1101230007@st.tvu.edu.vn", "thu123", "User", 1, 8),
                new UserSeed("Lê Hoàng Phúc", "1101230008@st.tvu.edu.vn", "phuc123", "User", 1, 8),
                new UserSeed("Bùi Quốc An", "1101230009@st.tvu.edu.vn", "an123", "User", 1, 7),
                new UserSeed("Nguyễn Thảo Vy", "1101230010@st.tvu.edu.vn", "vy123", "User", 1, 7),

                new UserSeed("Trịnh Nhật Long", "1101240011@st.tvu.edu.vn", "long123", "User", 1, 6),
                new UserSeed("Cao Minh Nhật", "1101240012@st.tvu.edu.vn", "nhat123", "User", 1, 6),
                new UserSeed("Lê Gia Bảo", "1101240013@st.tvu.edu.vn", "bao123", "User", 1, 5),
                new UserSeed("Phan Kim Chi", "1101240014@st.tvu.edu.vn", "chi123", "User", 1, 5),
                new UserSeed("Võ Mỹ Duyên", "1101240015@st.tvu.edu.vn", "duyen123", "User", 1, 4),

                new UserSeed("Đỗ Quốc Việt", "1101250016@st.tvu.edu.vn", "viet123", "User", 1, 4),
                new UserSeed("Nguyễn Hải Đăng", "1101250017@st.tvu.edu.vn", "dang123", "User", 1, 3),
                new UserSeed("Trần Anh Khoa", "1101250018@st.tvu.edu.vn", "khoa123", "User", 1, 3),
                new UserSeed("Phạm Ngọc Mai", "1101250019@st.tvu.edu.vn", "mai123", "User", 1, 2),
                new UserSeed("Huỳnh Tuấn Kiệt", "1101250020@st.tvu.edu.vn", "kiet123", "User", 1, 2),
                new UserSeed("Đặng Khánh Linh", "1101250021@st.tvu.edu.vn", "linh123", "User", 1, 1),

                // Hai tài khoản này cố tình bị khóa để test Ban/Bỏ ban.
                new UserSeed("Nguyễn Văn Toàn", "1101250022@st.tvu.edu.vn", "toan123", "User", 0, 6),
                new UserSeed("Lê Thanh Tùng", "1101240023@st.tvu.edu.vn", "tung123", "User", 0, 5)
            };

            foreach (var user in users)
            {
                var email = user.Email.Trim().ToLowerInvariant();
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                connection.Execute(@"
                    INSERT INTO NGUOIDUNG
                        (HoTen, Email, MatKhauHash, VaiTro, TrangThai, NgayTao, NgayCapNhat)
                    SELECT
                        @HoTen, @Email, @MatKhauHash, @VaiTro, @TrangThai, @NgayTao, @NgayCapNhat
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM NGUOIDUNG
                        WHERE lower(trim(Email)) = lower(trim(@Email))
                    );
                ", new
                {
                    user.HoTen,
                    Email = email,
                    MatKhauHash = passwordHash,
                    user.VaiTro,
                    user.TrangThai,
                    NgayTao = Date(user.DaysAgo, 8),
                    NgayCapNhat = Date(0, 8)
                });

                connection.Execute(@"
                    UPDATE NGUOIDUNG
                    SET
                        HoTen = @HoTen,
                        MatKhauHash = @MatKhauHash,
                        VaiTro = @VaiTro,
                        TrangThai = @TrangThai,
                        NgayCapNhat = @NgayCapNhat
                    WHERE lower(trim(Email)) = lower(trim(@Email));
                ", new
                {
                    user.HoTen,
                    Email = email,
                    MatKhauHash = passwordHash,
                    user.VaiTro,
                    user.TrangThai,
                    NgayCapNhat = Date(0, 8)
                });
            }
        }

        private void SeedChuyenMuc(IDbConnection connection)
        {
            var categories = new[]
            {
                new CategorySeed("Lập trình Web", "Thảo luận về HTML, CSS, JavaScript, Vue và giao diện web."),
                new CategorySeed("Cơ sở dữ liệu", "Các vấn đề về SQLite, SQL Server, thiết kế bảng, khóa chính và khóa ngoại."),
                new CategorySeed("Lập trình .NET", "ASP.NET Core, Minimal API, Dapper, middleware và tổ chức backend."),
                new CategorySeed("JavaScript", "Cú pháp, xử lý bất đồng bộ, fetch API, module và lỗi thường gặp."),
                new CategorySeed("Vue.js", "Component, props, emits, router, state và Element Plus."),
                new CategorySeed("API & Backend", "Thiết kế API, kiểm thử bằng Swagger/Postman và xử lý lỗi backend."),
                new CategorySeed("An toàn thông tin", "JWT, phân quyền, hash mật khẩu và bảo vệ tài khoản người dùng."),
                new CategorySeed("Cấu trúc dữ liệu & Giải thuật", "Mảng, danh sách, cây, đồ thị, tìm kiếm, sắp xếp và tối ưu thuật toán."),
                new CategorySeed("Công cụ lập trình", "Git, GitHub, Visual Studio Code, Kiro IDE, Postman và môi trường phát triển."),
                new CategorySeed("Chia sẻ kinh nghiệm học tập", "Kinh nghiệm học môn chuyên ngành, thực tập, làm đồ án và làm việc nhóm."),
                new CategorySeed("Hỏi đáp chung", "Các câu hỏi tổng quát chưa thuộc chuyên mục cụ thể.")
            };

            foreach (var category in categories)
            {
                connection.Execute(@"
                    INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa, TrangThai)
                    SELECT @TenChuyenMuc, @MoTa, 1
                    WHERE NOT EXISTS (
                        SELECT 1 FROM CHUYENMUC WHERE TenChuyenMuc = @TenChuyenMuc
                    );
                ", category);

                connection.Execute(@"
                    UPDATE CHUYENMUC
                    SET MoTa = @MoTa,
                        TrangThai = 1
                    WHERE TenChuyenMuc = @TenChuyenMuc;
                ", category);
            }
        }

        private void SeedThe(IDbConnection connection)
        {
            var tags = new[]
            {
                "dotnet",
                "vue",
                "sqlite",
                "dapper",
                "api",
                "jwt",
                "javascript",
                "database",
                "sql",
                "frontend",
                "backend",
                "debug"
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

                connection.Execute(@"
                    UPDATE THE
                    SET MoTa = @MoTa,
                        TrangThai = 1
                    WHERE TenThe = @TenThe;
                ", new
                {
                    TenThe = tag,
                    MoTa = $"Thẻ liên quan đến {tag}"
                });
            }
        }

        private void SeedCauHoi(IDbConnection connection)
        {
            var questions = new[]
            {
                new QuestionSeed(
                    "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite",
                    "Mình đang dùng .NET Minimal API, Dapper và SQLite. Khi thêm câu hỏi thì bị lỗi FOREIGN KEY constraint failed. Nguyên nhân thường là gì và kiểm tra như thế nào?",
                    "Cơ sở dữ liệu",
                    "1101220001@st.tvu.edu.vn",
                    63,
                    0,
                    7,
                    new[] { "sqlite", "database", "dapper", "debug" }),

                new QuestionSeed(
                    "Minimal API khác gì Controller trong ASP.NET Core?",
                    "Mình thấy project dùng Minimal API thay vì Controller. Hai cách này khác nhau thế nào và khi làm đồ án nên chọn hướng nào?",
                    "Lập trình .NET",
                    "1101220002@st.tvu.edu.vn",
                    45,
                    0,
                    7,
                    new[] { "dotnet", "api", "backend" }),

                new QuestionSeed(
                    "Dapper truyền tham số như thế nào để tránh SQL Injection?",
                    "Mình muốn hiểu cách Dapper truyền tham số vào câu SQL. Nếu nối chuỗi trực tiếp thì có nguy hiểm không?",
                    "API & Backend",
                    "1101220003@st.tvu.edu.vn",
                    58,
                    0,
                    7,
                    new[] { "dapper", "sql", "backend", "api" }),

                new QuestionSeed(
                    "JWT nên lưu ở localStorage hay cookie?",
                    "Sau khi đăng nhập, backend trả JWT token. Frontend nên lưu token ở localStorage, sessionStorage hay cookie thì hợp lý hơn?",
                    "An toàn thông tin",
                    "1101220004@st.tvu.edu.vn",
                    72,
                    0,
                    6,
                    new[] { "jwt", "frontend", "backend" }),

                new QuestionSeed(
                    "Vue Router không chuyển trang sau khi đăng nhập",
                    "Sau khi login thành công mình đã lưu token, nhưng router không chuyển về trang chủ. Mình nên kiểm tra route guard hay hàm login trước?",
                    "Vue.js",
                    "1101220005@st.tvu.edu.vn",
                    66,
                    0,
                    6,
                    new[] { "vue", "frontend", "jwt", "debug" }),

                new QuestionSeed(
                    "Element Plus pagination dùng thế nào cho danh sách câu hỏi?",
                    "Mình muốn phân trang danh sách câu hỏi, câu trả lời và bình luận. Nên làm phân trang ở frontend hay backend?",
                    "Vue.js",
                    "1101230006@st.tvu.edu.vn",
                    40,
                    0,
                    6,
                    new[] { "vue", "frontend" }),

                new QuestionSeed(
                    "Soft Delete nên dùng IsDeleted hay xóa vật lý?",
                    "Trong hệ thống diễn đàn, khi người dùng xóa câu hỏi hoặc câu trả lời thì nên xóa vật lý hay dùng cột IsDeleted?",
                    "Cơ sở dữ liệu",
                    "1101230007@st.tvu.edu.vn",
                    88,
                    0,
                    6,
                    new[] { "database", "backend", "sql" }),

                new QuestionSeed(
                    "Khi nào cần tạo bảng trung gian nhiều-nhiều?",
                    "Ví dụ câu hỏi có nhiều thẻ và một thẻ có nhiều câu hỏi. Trường hợp này có cần bảng trung gian không?",
                    "Cơ sở dữ liệu",
                    "1101230008@st.tvu.edu.vn",
                    34,
                    0,
                    5,
                    new[] { "database", "sql" }),

                new QuestionSeed(
                    "Làm sao kiểm tra user có quyền sửa câu hỏi?",
                    "Mình muốn user chỉ sửa được câu hỏi của chính mình, còn admin có quyền quản lý. Nên kiểm tra ở endpoint hay service?",
                    "An toàn thông tin",
                    "1101230009@st.tvu.edu.vn",
                    51,
                    0,
                    5,
                    new[] { "jwt", "backend", "api" }),

                new QuestionSeed(
                    "Cách tổ chức thư mục Vue cho dễ quản lý",
                    "Project có nhiều view, component và file api. Mình nên chia thư mục như thế nào để dễ đọc code và bảo trì?",
                    "Lập trình Web",
                    "1101230010@st.tvu.edu.vn",
                    39,
                    0,
                    5,
                    new[] { "vue", "frontend" }),

                new QuestionSeed(
                    "Postman và Swagger khác nhau thế nào khi test API?",
                    "Mình đang test API bằng Swagger, nhưng nhiều bạn dùng Postman. Hai công cụ này khác nhau ra sao?",
                    "Công cụ lập trình",
                    "1101240011@st.tvu.edu.vn",
                    29,
                    0,
                    5,
                    new[] { "api", "debug" }),

                new QuestionSeed(
                    "Git branch nên chia như thế nào khi làm Scrum?",
                    "Nhóm mình dùng Scrum và GitHub. Nên chia branch main, develop, feature như thế nào cho dễ quản lý?",
                    "Công cụ lập trình",
                    "1101240012@st.tvu.edu.vn",
                    47,
                    0,
                    4,
                    new[] { "debug" }),

                new QuestionSeed(
                    "Middleware trong ASP.NET Core chạy theo thứ tự nào?",
                    "Mình chưa hiểu app.UseAuthentication và app.UseAuthorization phải đặt ở đâu trong Program.cs.",
                    "Lập trình .NET",
                    "1101240013@st.tvu.edu.vn",
                    55,
                    0,
                    4,
                    new[] { "dotnet", "backend", "jwt" }),

                new QuestionSeed(
                    "Lỗi 401 Unauthorized dù đã gửi Bearer token",
                    "Mình gọi API có gắn Authorization Bearer token nhưng vẫn bị 401. Có thể do token sai claim hay cấu hình JWT không?",
                    "API & Backend",
                    "1101240014@st.tvu.edu.vn",
                    62,
                    0,
                    4,
                    new[] { "jwt", "api", "debug", "backend" }),

                new QuestionSeed(
                    "Nên học .NET trước hay học Vue trước?",
                    "Mình muốn làm fullstack nhưng chưa biết nên học backend .NET trước hay frontend Vue trước.",
                    "Chia sẻ kinh nghiệm học tập",
                    "1101240015@st.tvu.edu.vn",
                    81,
                    0,
                    4,
                    new[] { "dotnet", "vue", "frontend", "backend" }),

                new QuestionSeed(
                    "v-model không cập nhật dữ liệu trong form Vue",
                    "Form của mình dùng v-model nhưng khi submit thì dữ liệu không đổi. Trường hợp này nên kiểm tra reactive hay ref?",
                    "Vue.js",
                    "1101250016@st.tvu.edu.vn",
                    36,
                    0,
                    3,
                    new[] { "vue", "frontend", "debug" }),

                new QuestionSeed(
                    "Cách hiển thị thông báo khi có người trả lời câu hỏi",
                    "Mình muốn khi có người trả lời câu hỏi thì chủ câu hỏi nhận thông báo. Thiết kế bảng thông báo như thế nào?",
                    "API & Backend",
                    "1101250017@st.tvu.edu.vn",
                    44,
                    0,
                    3,
                    new[] { "api", "backend", "database" }),

                new QuestionSeed(
                    "Tố cáo nội dung nên lưu những thông tin gì?",
                    "Mình muốn làm chức năng tố cáo câu hỏi, câu trả lời và bình luận. Bảng tố cáo nên có các cột nào?",
                    "An toàn thông tin",
                    "1101250018@st.tvu.edu.vn",
                    52,
                    0,
                    3,
                    new[] { "database", "backend", "api" }),

                new QuestionSeed(
                    "Làm sao ẩn nội dung của user bị khóa?",
                    "Nếu admin khóa một user thì các câu hỏi và câu trả lời của user đó có nên ẩn khỏi trang chủ không?",
                    "An toàn thông tin",
                    "1101250019@st.tvu.edu.vn",
                    38,
                    0,
                    3,
                    new[] { "backend", "jwt", "database" }),

                new QuestionSeed(
                    "Cách viết báo cáo phần kiểm thử hệ thống",
                    "Mình cần viết test case cho đăng nhập, đăng câu hỏi, trả lời, bình luận, vote và admin. Nên trình bày thế nào?",
                    "Chia sẻ kinh nghiệm học tập",
                    "1101250020@st.tvu.edu.vn",
                    74,
                    0,
                    3,
                    new[] { "debug" }),

                new QuestionSeed(
                    "Thuật toán tìm kiếm câu hỏi nên dùng LIKE hay full-text search?",
                    "Hiện tại mình tìm kiếm bằng LIKE theo tiêu đề, nội dung và tag. Cách này có ổn cho đồ án không?",
                    "Cơ sở dữ liệu",
                    "1101250021@st.tvu.edu.vn",
                    49,
                    0,
                    2,
                    new[] { "database", "sql", "api" }),

                new QuestionSeed(
                    "Lỗi CORS khi frontend gọi API backend",
                    "Frontend Vue chạy ở port 5173, backend chạy ở port 5182. Khi gọi API thì bị CORS. Nên sửa ở đâu?",
                    "Lập trình Web",
                    "1101220001@st.tvu.edu.vn",
                    57,
                    0,
                    2,
                    new[] { "api", "frontend", "backend", "debug" }),

                new QuestionSeed(
                    "Làm sao tạo avatar chữ cái đầu cho user?",
                    "Mình muốn nếu user chưa có ảnh đại diện thì hiển thị chữ cái đầu của họ tên. Làm ở frontend hay backend?",
                    "Lập trình Web",
                    "1101220002@st.tvu.edu.vn",
                    27,
                    0,
                    2,
                    new[] { "frontend", "vue" }),

                new QuestionSeed(
                    "Khi nào nên dùng computed trong Vue?",
                    "Mình đang dùng computed để lọc danh sách câu hỏi và câu trả lời. Có khác gì method không?",
                    "Vue.js",
                    "1101220003@st.tvu.edu.vn",
                    33,
                    0,
                    2,
                    new[] { "vue", "javascript", "frontend" }),

                new QuestionSeed(
                    "Fetch API xử lý lỗi 400 và 500 như thế nào?",
                    "Khi backend trả lỗi BadRequest hoặc Internal Server Error, frontend nên lấy message và hiển thị ra sao?",
                    "JavaScript",
                    "1101220004@st.tvu.edu.vn",
                    42,
                    0,
                    2,
                    new[] { "javascript", "api", "frontend", "debug" }),

                new QuestionSeed(
                    "Cách kiểm tra dữ liệu seed đã chạy chưa",
                    "Mình muốn biết DatabaseSeeder.cs đã insert dữ liệu mẫu vào SQLite hay chưa. Có thể kiểm tra bằng SQL nào?",
                    "Cơ sở dữ liệu",
                    "1101220005@st.tvu.edu.vn",
                    25,
                    0,
                    1,
                    new[] { "sqlite", "database", "debug" }),

                new QuestionSeed(
                    "Nên chia service và repository như thế nào?",
                    "Trong backend .NET, mình đang chia Endpoint, Service, Repository. Cách chia trách nhiệm như vậy đã ổn chưa?",
                    "Lập trình .NET",
                    "1101230006@st.tvu.edu.vn",
                    53,
                    0,
                    1,
                    new[] { "dotnet", "backend", "api" }),

                new QuestionSeed(
                    "Đặt tên API tiếng Việt không dấu có ổn không?",
                    "API của mình có các route như cauhoi, cautraloi, binhluan. Cách đặt tên này có phù hợp cho đồ án không?",
                    "API & Backend",
                    "1101230007@st.tvu.edu.vn",
                    31,
                    0,
                    1,
                    new[] { "api", "backend" }),

                new QuestionSeed(
                    "Làm sao tránh user vote nhiều lần?",
                    "Mình muốn một user chỉ được vote một lần cho một câu hỏi hoặc câu trả lời. Database nên ràng buộc thế nào?",
                    "Cơ sở dữ liệu",
                    "1101230008@st.tvu.edu.vn",
                    46,
                    0,
                    1,
                    new[] { "database", "sql", "backend" }),

                new QuestionSeed(
                    "Nên trình bày Sprint trong báo cáo Scrum như thế nào?",
                    "Nhóm mình có nhiều sprint như Auth, Question, Answer, Admin và Test. Khi viết báo cáo nên mô tả sao cho gọn?",
                    "Chia sẻ kinh nghiệm học tập",
                    "1101230009@st.tvu.edu.vn",
                    64,
                    0,
                    1,
                    new[] { "debug" }),

                new QuestionSeed(
                    "Câu hỏi này bị xóa mềm để test admin khôi phục",
                    "Đây là câu hỏi mẫu đã xóa mềm, dùng để demo chức năng admin xem nội dung đã xóa và khôi phục.",
                    "Hỏi đáp chung",
                    "1101240011@st.tvu.edu.vn",
                    12,
                    1,
                    1,
                    new[] { "debug", "database" }),

                new QuestionSeed(
                    "Câu hỏi của user bị khóa để test ẩn nội dung",
                    "Đây là câu hỏi của tài khoản đang bị khóa. Nội dung này dùng để kiểm tra public repository có ẩn nội dung của user bị khóa hay không.",
                    "Hỏi đáp chung",
                    "1101250022@st.tvu.edu.vn",
                    18,
                    0,
                    1,
                    new[] { "backend", "debug" }),

                new QuestionSeed(
                    "Nội dung spam quảng cáo khóa học không phù hợp",
                    "Mua khóa học lập trình giá rẻ, liên hệ ngay để được giảm giá. Nội dung này dùng để demo tố cáo và xử lý vi phạm.",
                    "Hỏi đáp chung",
                    "1101240023@st.tvu.edu.vn",
                    8,
                    1,
                    2,
                    new[] { "debug" }),

                new QuestionSeed(
                    "Câu hỏi chưa có trả lời về học JavaScript cơ bản",
                    "Mình mới học JavaScript, chưa rõ nên học DOM, fetch hay async await trước. Mong mọi người góp ý.",
                    "JavaScript",
                    "1101250019@st.tvu.edu.vn",
                    21,
                    0,
                    0,
                    new[] { "javascript", "frontend" }),

                new QuestionSeed(
                    "Câu hỏi chưa có trả lời về cấu trúc dữ liệu",
                    "Khi nào nên dùng queue thay vì stack trong bài toán thực tế?",
                    "Cấu trúc dữ liệu & Giải thuật",
                    "1101250020@st.tvu.edu.vn",
                    17,
                    0,
                    0,
                    new[] { "debug" }),

                new QuestionSeed(
                    "Sắp xếp dữ liệu trong bảng admin nên làm ở đâu?",
                    "Nếu bảng admin có nhiều dữ liệu thì nên sort ở frontend hay backend?",
                    "Lập trình Web",
                    "1101250021@st.tvu.edu.vn",
                    19,
                    0,
                    0,
                    new[] { "frontend", "backend", "api" })
            };

            foreach (var question in questions)
            {
                var questionId = InsertQuestion(connection, question);
                GanTheChoCauHoi(connection, questionId, question.Tags);
            }
        }

        private void SeedCauTraLoi(IDbConnection connection)
        {
            var answers = new[]
            {
                new AnswerSeed("Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite", "1101220002@st.tvu.edu.vn", "Lỗi này thường do ID_ChuyenMuc hoặc ID_NguoiDung không tồn tại trong bảng cha. Bạn nên kiểm tra dữ liệu trong bảng CHUYENMUC và NGUOIDUNG trước khi insert.", 1, 0, 6),
                new AnswerSeed("Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite", "1101230008@st.tvu.edu.vn", "Bạn cũng nên bật PRAGMA foreign_keys = ON để SQLite kiểm tra khóa ngoại đúng cách khi chạy ứng dụng.", 0, 0, 6),

                new AnswerSeed("Minimal API khác gì Controller trong ASP.NET Core?", "1101220001@st.tvu.edu.vn", "Minimal API gọn hơn và phù hợp với project nhỏ hoặc API đơn giản. Controller rõ cấu trúc hơn khi hệ thống lớn.", 1, 0, 6),
                new AnswerSeed("Minimal API khác gì Controller trong ASP.NET Core?", "1101240013@st.tvu.edu.vn", "Với đồ án diễn đàn, dùng Minimal API vẫn ổn nếu bạn tách Endpoint, Service và Repository rõ ràng.", 0, 0, 5),

                new AnswerSeed("Dapper truyền tham số như thế nào để tránh SQL Injection?", "1101240012@st.tvu.edu.vn", "Bạn nên truyền tham số bằng object, ví dụ new { Email = email }. Không nên nối chuỗi trực tiếp vào SQL.", 1, 0, 6),
                new AnswerSeed("Dapper truyền tham số như thế nào để tránh SQL Injection?", "1101250016@st.tvu.edu.vn", "Dapper sẽ bind parameter vào câu truy vấn, giúp code dễ đọc và an toàn hơn so với string interpolation.", 0, 0, 5),

                new AnswerSeed("JWT nên lưu ở localStorage hay cookie?", "1101240014@st.tvu.edu.vn", "Nếu làm đồ án đơn giản thì localStorage dễ triển khai. Tuy nhiên thực tế cần cân nhắc XSS và có thể dùng cookie httpOnly.", 1, 0, 5),
                new AnswerSeed("JWT nên lưu ở localStorage hay cookie?", "1101250018@st.tvu.edu.vn", "Quan trọng là token phải có thời hạn hết hạn và backend phải kiểm tra chữ ký, issuer, audience nếu có cấu hình.", 0, 0, 5),

                new AnswerSeed("Vue Router không chuyển trang sau khi đăng nhập", "1101230006@st.tvu.edu.vn", "Bạn kiểm tra hàm router.push có được gọi sau khi setAuth hay không. Ngoài ra route guard có thể đang redirect ngược về login.", 1, 0, 5),
                new AnswerSeed("Vue Router không chuyển trang sau khi đăng nhập", "1101250019@st.tvu.edu.vn", "Nếu login API trả token nhưng user bị null thì phần lưu auth có thể chưa đúng cấu trúc response.", 0, 0, 4),

                new AnswerSeed("Element Plus pagination dùng thế nào cho danh sách câu hỏi?", "1101220003@st.tvu.edu.vn", "Nếu dữ liệu ít thì phân trang frontend đủ dùng. Nếu dữ liệu lớn thì backend nên trả page, pageSize và total.", 1, 0, 5),
                new AnswerSeed("Soft Delete nên dùng IsDeleted hay xóa vật lý?", "1101220004@st.tvu.edu.vn", "Soft delete phù hợp với diễn đàn vì admin có thể khôi phục nội dung và giữ lịch sử quản lý.", 1, 0, 5),
                new AnswerSeed("Soft Delete nên dùng IsDeleted hay xóa vật lý?", "1101250020@st.tvu.edu.vn", "Bạn cần lọc IsDeleted = 0 ở các API public, còn admin thì có thể xem cả nội dung đã xóa.", 0, 0, 4),

                new AnswerSeed("Khi nào cần tạo bảng trung gian nhiều-nhiều?", "1101230007@st.tvu.edu.vn", "Trường hợp câu hỏi và thẻ là quan hệ nhiều-nhiều nên cần bảng CauHoi_The gồm ID_CauHoi và ID_The.", 1, 0, 4),
                new AnswerSeed("Làm sao kiểm tra user có quyền sửa câu hỏi?", "1101240011@st.tvu.edu.vn", "Nên kiểm tra ở Service. Endpoint chỉ lấy userId từ token, còn Service kiểm tra chủ sở hữu trong database.", 1, 0, 4),
                new AnswerSeed("Cách tổ chức thư mục Vue cho dễ quản lý", "1101250017@st.tvu.edu.vn", "Bạn có thể chia src/api, src/components, src/views, src/utils. Cách này đang khá phù hợp cho project hiện tại.", 1, 0, 4),

                new AnswerSeed("Postman và Swagger khác nhau thế nào khi test API?", "1101220005@st.tvu.edu.vn", "Swagger tiện để xem tài liệu API trực tiếp. Postman mạnh hơn khi cần lưu collection và test nhiều môi trường.", 1, 0, 4),
                new AnswerSeed("Git branch nên chia như thế nào khi làm Scrum?", "1101230010@st.tvu.edu.vn", "Có thể dùng main để lưu bản ổn định, develop để tích hợp, feature/... cho từng chức năng.", 1, 0, 3),
                new AnswerSeed("Middleware trong ASP.NET Core chạy theo thứ tự nào?", "1101240014@st.tvu.edu.vn", "Thông thường cần đặt UseAuthentication trước UseAuthorization. Ngoài ra phải map endpoint sau khi cấu hình middleware.", 1, 0, 3),
                new AnswerSeed("Lỗi 401 Unauthorized dù đã gửi Bearer token", "1101220002@st.tvu.edu.vn", "Bạn kiểm tra token có đúng format Bearer token không, claim NameIdentifier có tồn tại không và secret key có khớp không.", 1, 0, 3),

                new AnswerSeed("Nên học .NET trước hay học Vue trước?", "1101230008@st.tvu.edu.vn", "Nếu mục tiêu backend thì học .NET trước. Nhưng vẫn nên biết Vue cơ bản để tự làm project fullstack cho CV.", 1, 0, 3),
                new AnswerSeed("Nên học .NET trước hay học Vue trước?", "1101250021@st.tvu.edu.vn", "Bạn có thể học song song nhưng nên chia ngày rõ ràng: backend trước, frontend theo sau để nối API.", 0, 0, 2),

                new AnswerSeed("v-model không cập nhật dữ liệu trong form Vue", "1101240013@st.tvu.edu.vn", "Nếu dùng reactive thì kiểm tra đúng tên field. Nếu dùng ref thì nhớ truy cập qua .value trong script.", 1, 0, 2),
                new AnswerSeed("Cách hiển thị thông báo khi có người trả lời câu hỏi", "1101220001@st.tvu.edu.vn", "Nên tạo bảng THONGBAO với ID_NguoiNhan, ID_NguoiTao, LoaiThongBao, TieuDe, NoiDung, Link và DaDoc.", 1, 0, 2),
                new AnswerSeed("Tố cáo nội dung nên lưu những thông tin gì?", "1101230009@st.tvu.edu.vn", "Bảng tố cáo nên có người tố cáo, loại đối tượng, ID đối tượng, lý do, mô tả, trạng thái và admin xử lý.", 1, 0, 2),
                new AnswerSeed("Làm sao ẩn nội dung của user bị khóa?", "1101240015@st.tvu.edu.vn", "Repository public nên join với NGUOIDUNG và lọc TrangThai = 1. Admin vẫn có thể xem để quản lý.", 1, 0, 2),

                new AnswerSeed("Cách viết báo cáo phần kiểm thử hệ thống", "1101220004@st.tvu.edu.vn", "Bạn nên lập bảng test case gồm mã test, chức năng, bước thực hiện, dữ liệu test, kết quả mong đợi và kết quả thực tế.", 1, 0, 2),
                new AnswerSeed("Thuật toán tìm kiếm câu hỏi nên dùng LIKE hay full-text search?", "1101250016@st.tvu.edu.vn", "Với đồ án nhỏ, LIKE theo tiêu đề, nội dung và tag là ổn. Full-text search có thể đưa vào hướng phát triển.", 1, 0, 1),
                new AnswerSeed("Lỗi CORS khi frontend gọi API backend", "1101230006@st.tvu.edu.vn", "Bạn cần cấu hình CORS trong Program.cs, cho phép origin của Vite như http://localhost:5173.", 1, 0, 1),
                new AnswerSeed("Làm sao tạo avatar chữ cái đầu cho user?", "1101240011@st.tvu.edu.vn", "Phần này nên làm ở frontend bằng cách lấy chữ cái đầu của họ tên. Backend chỉ cần trả HoTen.", 1, 0, 1),
                new AnswerSeed("Khi nào nên dùng computed trong Vue?", "1101250018@st.tvu.edu.vn", "computed phù hợp khi dữ liệu phụ thuộc vào state và cần cache. method sẽ chạy lại mỗi lần render.", 1, 0, 1),
                new AnswerSeed("Fetch API xử lý lỗi 400 và 500 như thế nào?", "1101220005@st.tvu.edu.vn", "Bạn nên viết hàm request dùng chung. Nếu response không ok thì đọc message từ body và throw Error.", 1, 0, 1),
                new AnswerSeed("Cách kiểm tra dữ liệu seed đã chạy chưa", "1101230010@st.tvu.edu.vn", "Bạn có thể dùng SELECT COUNT(*) FROM NGUOIDUNG, CAUHOI, CAUTRALOI để kiểm tra nhanh dữ liệu mẫu.", 1, 0, 1),
                new AnswerSeed("Nên chia service và repository như thế nào?", "1101250017@st.tvu.edu.vn", "Repository chỉ truy vấn database, Service xử lý nghiệp vụ, Endpoint nhận request và trả response.", 1, 0, 1),
                new AnswerSeed("Đặt tên API tiếng Việt không dấu có ổn không?", "1101240012@st.tvu.edu.vn", "Trong đồ án có thể dùng cauhoi, cautraloi, binhluan để dễ hiểu. Nếu theo chuẩn quốc tế thì nên dùng tiếng Anh.", 1, 0, 1),
                new AnswerSeed("Làm sao tránh user vote nhiều lần?", "1101220002@st.tvu.edu.vn", "Bạn nên đặt UNIQUE(ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong) trong bảng BINHCHON.", 1, 0, 0),

                // Câu trả lời đã xóa mềm để test admin.
                new AnswerSeed("Sắp xếp dữ liệu trong bảng admin nên làm ở đâu?", "1101240023@st.tvu.edu.vn", "Câu trả lời này đã bị xóa mềm để demo chức năng quản lý câu trả lời.", 0, 1, 0)
            };

            foreach (var answer in answers)
            {
                InsertAnswer(connection, answer);
            }
        }

        private void SeedBinhChon(IDbConnection connection)
        {
            var activeUserIds = connection.Query<int>(@"
                SELECT ID_NguoiDung
                FROM NGUOIDUNG
                WHERE TrangThai = 1
                  AND VaiTro = 'User'
                ORDER BY ID_NguoiDung;
            ").ToList();

            var questions = connection.Query(@"
                SELECT ID_CauHoi, ID_NguoiDung
                FROM CAUHOI
                ORDER BY ID_CauHoi
                LIMIT 45;
            ").ToList();

            for (var i = 0; i < questions.Count; i++)
            {
                var questionId = Convert.ToInt32(questions[i].ID_CauHoi);
                var ownerId = Convert.ToInt32(questions[i].ID_NguoiDung);
                var voteCount = 2 + (i % 4);

                for (var j = 0; j < voteCount && j < activeUserIds.Count; j++)
                {
                    var voterId = activeUserIds[(i + j + 3) % activeUserIds.Count];

                    if (voterId == ownerId)
                    {
                        continue;
                    }

                    var value = ((i + j) % 9 == 0) ? -1 : 1;
                    InsertVote(connection, voterId, "CAUHOI", questionId, value, j % 4);
                }
            }

            var answers = connection.Query(@"
                SELECT ID_CauTraLoi, ID_NguoiDung
                FROM CAUTRALOI
                ORDER BY ID_CauTraLoi
                LIMIT 60;
            ").ToList();

            for (var i = 0; i < answers.Count; i++)
            {
                var answerId = Convert.ToInt32(answers[i].ID_CauTraLoi);
                var ownerId = Convert.ToInt32(answers[i].ID_NguoiDung);
                var voteCount = 1 + (i % 3);

                for (var j = 0; j < voteCount && j < activeUserIds.Count; j++)
                {
                    var voterId = activeUserIds[(i + j + 5) % activeUserIds.Count];

                    if (voterId == ownerId)
                    {
                        continue;
                    }

                    var value = ((i + j) % 11 == 0) ? -1 : 1;
                    InsertVote(connection, voterId, "CAUTRALOI", answerId, value, j % 3);
                }
            }
        }

        private void SeedBinhLuan(IDbConnection connection)
        {
            var comments = new[]
            {
                new CommentSeed("CAUHOI", "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite", "1101220002@st.tvu.edu.vn", "Bạn thử kiểm tra ID chuyên mục có tồn tại trong bảng CHUYENMUC chưa nha.", 0, 5),
                new CommentSeed("CAUHOI", "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite", "1101220001@st.tvu.edu.vn", "Cảm ơn bạn, đúng là mình gửi sai ID_ChuyenMuc nên bị lỗi.", 0, 5),
                new CommentSeed("CAUTRALOI", "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite", "1101230008@st.tvu.edu.vn", "Mình bổ sung thêm là SQLite cần bật foreign key khi mở connection.", 0, 5),

                new CommentSeed("CAUHOI", "JWT nên lưu ở localStorage hay cookie?", "1101240014@st.tvu.edu.vn", "Câu này hay, mình cũng đang phân vân khi làm đồ án.", 0, 4),
                new CommentSeed("CAUTRALOI", "JWT nên lưu ở localStorage hay cookie?", "1101250018@st.tvu.edu.vn", "Nếu đưa vào báo cáo thì có thể nêu localStorage là giải pháp đơn giản nhưng còn hạn chế.", 0, 4),

                new CommentSeed("CAUHOI", "Vue Router không chuyển trang sau khi đăng nhập", "1101230006@st.tvu.edu.vn", "Bạn kiểm tra route.query.redirect nữa nha.", 0, 4),
                new CommentSeed("CAUTRALOI", "Vue Router không chuyển trang sau khi đăng nhập", "1101220005@st.tvu.edu.vn", "Mình từng lỗi vì setAuth xong nhưng chưa cập nhật state user.", 0, 4),

                new CommentSeed("CAUHOI", "Soft Delete nên dùng IsDeleted hay xóa vật lý?", "1101230008@st.tvu.edu.vn", "Theo mình dùng IsDeleted hợp lý hơn vì admin còn khôi phục được.", 0, 4),
                new CommentSeed("CAUTRALOI", "Soft Delete nên dùng IsDeleted hay xóa vật lý?", "1101250020@st.tvu.edu.vn", "Đúng rồi, API public nhớ lọc IsDeleted = 0.", 0, 3),

                new CommentSeed("CAUHOI", "Git branch nên chia như thế nào khi làm Scrum?", "1101220001@st.tvu.edu.vn", "Nhóm mình cũng đang dùng main, develop và feature.", 0, 3),
                new CommentSeed("CAUHOI", "Nên học .NET trước hay học Vue trước?", "1101250021@st.tvu.edu.vn", "Nếu đi thực tập backend thì .NET nên ưu tiên hơn.", 0, 2),
                new CommentSeed("CAUHOI", "Cách viết báo cáo phần kiểm thử hệ thống", "1101240015@st.tvu.edu.vn", "Bạn có thể thêm cột trạng thái Pass/Fail vào bảng test case.", 0, 2),
                new CommentSeed("CAUHOI", "Lỗi CORS khi frontend gọi API backend", "1101230007@st.tvu.edu.vn", "Nhớ kiểm tra đúng port frontend đang chạy là 5173 hay port khác.", 0, 1),
                new CommentSeed("CAUTRALOI", "Lỗi CORS khi frontend gọi API backend", "1101250016@st.tvu.edu.vn", "Cấu hình CORS phải đặt trước Map endpoints nha.", 0, 1),

                new CommentSeed("CAUHOI", "Tố cáo nội dung nên lưu những thông tin gì?", "1101230010@st.tvu.edu.vn", "Nên có trạng thái để admin xử lý từng tố cáo.", 0, 2),
                new CommentSeed("CAUTRALOI", "Tố cáo nội dung nên lưu những thông tin gì?", "1101220003@st.tvu.edu.vn", "Phần này demo admin rất rõ nếu có đủ trạng thái.", 0, 2),

                new CommentSeed("CAUHOI", "Câu hỏi này bị xóa mềm để test admin khôi phục", "1101250017@st.tvu.edu.vn", "Bình luận này cũng dùng để test nội dung đã xóa.", 1, 1),
                new CommentSeed("CAUHOI", "Nội dung spam quảng cáo khóa học không phù hợp", "1101220004@st.tvu.edu.vn", "Nội dung này nên bị tố cáo vì không phù hợp với diễn đàn.", 1, 2),
                new CommentSeed("CAUHOI", "Câu hỏi chưa có trả lời về học JavaScript cơ bản", "1101220005@st.tvu.edu.vn", "Mình cũng quan tâm câu hỏi này, mong có bạn trả lời thêm.", 0, 0),
                new CommentSeed("CAUHOI", "Câu hỏi chưa có trả lời về cấu trúc dữ liệu", "1101230006@st.tvu.edu.vn", "Queue thường dùng khi xử lý theo thứ tự vào trước ra trước.", 0, 0),

                new CommentSeed("CAUHOI", "Sắp xếp dữ liệu trong bảng admin nên làm ở đâu?", "1101240011@st.tvu.edu.vn", "Nếu dữ liệu ít thì sort frontend ổn, dữ liệu lớn thì nên để backend.", 0, 0),
                new CommentSeed("CAUHOI", "Làm sao tránh user vote nhiều lần?", "1101250018@st.tvu.edu.vn", "Unique constraint là cách chắc chắn nhất.", 0, 0),
                new CommentSeed("CAUTRALOI", "Làm sao tránh user vote nhiều lần?", "1101250021@st.tvu.edu.vn", "Phần này nhớ xử lý cả trường hợp đổi vote từ up sang down.", 0, 0)
            };

            foreach (var comment in comments)
            {
                InsertComment(connection, comment);
            }
        }

        private void SeedThongBao(IDbConnection connection)
        {
            var notifications = new[]
            {
                new NotificationSeed("1101220001@st.tvu.edu.vn", "1101220002@st.tvu.edu.vn", "ANSWER", "Câu hỏi của bạn có câu trả lời mới", "Trần Thanh Trúc đã trả lời câu hỏi về lỗi FOREIGN KEY trong SQLite.", QuestionLink(connection, "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite"), 0, 0, 1),
                new NotificationSeed("1101220001@st.tvu.edu.vn", "1101230008@st.tvu.edu.vn", "COMMENT_QUESTION", "Câu hỏi của bạn có bình luận mới", "Có người bình luận vào câu hỏi về lỗi FOREIGN KEY constraint failed.", QuestionLink(connection, "Lỗi FOREIGN KEY constraint failed khi thêm câu hỏi trong SQLite"), 0, 0, 1),
                new NotificationSeed("1101220002@st.tvu.edu.vn", "1101230006@st.tvu.edu.vn", "ANSWER", "Câu hỏi của bạn có câu trả lời mới", "Có câu trả lời mới trong câu hỏi Vue Router không chuyển trang.", QuestionLink(connection, "Vue Router không chuyển trang sau khi đăng nhập"), 0, 0, 1),
                new NotificationSeed("1101220004@st.tvu.edu.vn", "1101240014@st.tvu.edu.vn", "COMMENT_QUESTION", "Có bình luận mới", "Có người bình luận vào câu hỏi JWT nên lưu ở localStorage hay cookie.", QuestionLink(connection, "JWT nên lưu ở localStorage hay cookie?"), 0, 0, 1),
                new NotificationSeed("1101250018@st.tvu.edu.vn", "admin@tvu.edu.vn", "REPORT_WARNING", "Quản trị viên đã nhắc nhở nội dung", "Nội dung của bạn đã được quản trị viên nhắc nhở sau khi có tố cáo.", QuestionLink(connection, "Tố cáo nội dung nên lưu những thông tin gì?"), 0, 0, 1),
                new NotificationSeed("1101240023@st.tvu.edu.vn", "admin@tvu.edu.vn", "ACCOUNT_LOCK", "Tài khoản của bạn đã bị khóa", "Tài khoản của bạn đã bị quản trị viên khóa do vi phạm quy định diễn đàn.", null, 0, 0, 1),

                new NotificationSeed("1101230007@st.tvu.edu.vn", "1101220004@st.tvu.edu.vn", "ANSWER", "Câu hỏi của bạn có câu trả lời mới", "Có câu trả lời mới trong câu hỏi Soft Delete.", QuestionLink(connection, "Soft Delete nên dùng IsDeleted hay xóa vật lý?"), 1, 0, 3),
                new NotificationSeed("1101240015@st.tvu.edu.vn", "1101230008@st.tvu.edu.vn", "ANSWER", "Câu hỏi của bạn có câu trả lời mới", "Có người góp ý về lộ trình học .NET và Vue.", QuestionLink(connection, "Nên học .NET trước hay học Vue trước?"), 1, 0, 2),
                new NotificationSeed("1101250019@st.tvu.edu.vn", "admin@tvu.edu.vn", "SYSTEM", "Chào mừng đến với CET FORIT", "Bạn có thể đặt câu hỏi, trả lời, bình luận và theo dõi thông báo tại diễn đàn.", null, 1, 0, 2),
                new NotificationSeed("1101250020@st.tvu.edu.vn", "1101240015@st.tvu.edu.vn", "COMMENT_QUESTION", "Có bình luận mới", "Có người bình luận vào câu hỏi về kiểm thử hệ thống.", QuestionLink(connection, "Cách viết báo cáo phần kiểm thử hệ thống"), 0, 0, 0),
                new NotificationSeed("1101250021@st.tvu.edu.vn", "1101240011@st.tvu.edu.vn", "COMMENT_QUESTION", "Có bình luận mới", "Có người bình luận vào câu hỏi sắp xếp dữ liệu trong bảng admin.", QuestionLink(connection, "Sắp xếp dữ liệu trong bảng admin nên làm ở đâu?"), 0, 0, 0),
                new NotificationSeed("1101250022@st.tvu.edu.vn", "admin@tvu.edu.vn", "ACCOUNT_LOCK", "Tài khoản của bạn đã bị khóa", "Tài khoản bị khóa nên nội dung công khai sẽ bị ẩn khỏi trang chủ.", null, 1, 0, 4),
                new NotificationSeed("1101240023@st.tvu.edu.vn", "admin@tvu.edu.vn", "REPORT_DELETE", "Nội dung của bạn đã bị xử lý", "Một nội dung vi phạm đã bị quản trị viên xóa mềm.", QuestionLink(connection, "Nội dung spam quảng cáo khóa học không phù hợp"), 0, 0, 1)
            };

            foreach (var notification in notifications)
            {
                InsertNotification(connection, notification);
            }
        }

        private void SeedToCao(IDbConnection connection)
        {
            var adminId = GetUserId(connection, "admin@tvu.edu.vn");

            var reports = new[]
            {
                new ReportSeed("1101220001@st.tvu.edu.vn", "CAUHOI", "Nội dung spam quảng cáo khóa học không phù hợp", "Spam / quảng cáo", "Nội dung có tính chất quảng cáo, không đúng mục đích diễn đàn.", "RESOLVED", adminId, "Đã xóa mềm nội dung quảng cáo.", 2, 1),
                new ReportSeed("1101220002@st.tvu.edu.vn", "CAUHOI", "Tố cáo nội dung nên lưu những thông tin gì?", "Nội dung cần kiểm tra", "Cần admin xem xét cách trình bày nội dung này.", "REMINDED", adminId, "Đã nhắc nhở người đăng chỉnh sửa cách trình bày.", 2, 1),
                new ReportSeed("1101230006@st.tvu.edu.vn", "CAUHOI", "JWT nên lưu ở localStorage hay cookie?", "Nội dung dễ gây hiểu nhầm", "Câu hỏi có nội dung tranh luận về bảo mật, cần kiểm tra thêm.", "REJECTED", adminId, "Nội dung là câu hỏi học tập hợp lệ nên bỏ qua tố cáo.", 3, 2),
                new ReportSeed("1101240011@st.tvu.edu.vn", "CAUHOI", "Câu hỏi chưa có trả lời về học JavaScript cơ bản", "Nội dung chưa rõ ràng", "Câu hỏi hơi chung, đề nghị kiểm tra.", "PENDING", null, null, 0, null),
                new ReportSeed("1101250018@st.tvu.edu.vn", "CAUTRALOI", "Sắp xếp dữ liệu trong bảng admin nên làm ở đâu?", "Câu trả lời chưa phù hợp", "Câu trả lời có thể gây nhầm lẫn cho người mới học.", "PENDING", null, null, 0, null),
                new ReportSeed("1101250021@st.tvu.edu.vn", "BINHLUAN", "Nội dung spam quảng cáo khóa học không phù hợp", "Ngôn từ không phù hợp", "Bình luận liên quan nội dung spam cần kiểm tra.", "PENDING", null, null, 0, null)
            };

            foreach (var report in reports)
            {
                InsertReport(connection, report);
            }
        }

        private int InsertQuestion(IDbConnection connection, QuestionSeed question)
        {
            var existingId = connection.ExecuteScalar<int?>(@"
                SELECT ID_CauHoi
                FROM CAUHOI
                WHERE TieuDe = @TieuDe
                LIMIT 1;
            ", new { question.TieuDe });

            var userId = GetUserId(connection, question.Email);
            var categoryId = GetChuyenMucId(connection, question.ChuyenMuc);
            var ngayTao = Date(question.DaysAgo, 9 + question.DaysAgo % 8);
            var ngayCapNhat = question.IsDeleted == 1
                ? Date(Math.Max(question.DaysAgo - 1, 0), 16)
                : null;

            if (existingId != null)
            {
                connection.Execute(@"
                    UPDATE CAUHOI
                    SET
                        ID_NguoiDung = @ID_NguoiDung,
                        ID_ChuyenMuc = @ID_ChuyenMuc,
                        NoiDung = @NoiDung,
                        TrangThai = 1,
                        LuotXem = @LuotXem,
                        NgayTao = @NgayTao,
                        NgayCapNhat = @NgayCapNhat,
                        IsDeleted = @IsDeleted
                    WHERE ID_CauHoi = @ID_CauHoi;
                ", new
                {
                    ID_CauHoi = existingId.Value,
                    ID_NguoiDung = userId,
                    ID_ChuyenMuc = categoryId,
                    question.NoiDung,
                    question.LuotXem,
                    NgayTao = ngayTao,
                    NgayCapNhat = ngayCapNhat,
                    question.IsDeleted
                });

                return existingId.Value;
            }

            return connection.ExecuteScalar<int>(@"
                INSERT INTO CAUHOI
                    (ID_NguoiDung, ID_ChuyenMuc, TieuDe, NoiDung, TrangThai, LuotXem, NgayTao, NgayCapNhat, IsDeleted)
                VALUES
                    (@ID_NguoiDung, @ID_ChuyenMuc, @TieuDe, @NoiDung, 1, @LuotXem, @NgayTao, @NgayCapNhat, @IsDeleted);

                SELECT last_insert_rowid();
            ", new
            {
                ID_NguoiDung = userId,
                ID_ChuyenMuc = categoryId,
                question.TieuDe,
                question.NoiDung,
                question.LuotXem,
                NgayTao = ngayTao,
                NgayCapNhat = ngayCapNhat,
                question.IsDeleted
            });
        }

        private int InsertAnswer(IDbConnection connection, AnswerSeed answer)
        {
            var questionId = GetQuestionId(connection, answer.QuestionTitle);
            var userId = GetUserId(connection, answer.Email);

            var existingId = connection.ExecuteScalar<int?>(@"
                SELECT ID_CauTraLoi
                FROM CAUTRALOI
                WHERE ID_CauHoi = @ID_CauHoi
                  AND ID_NguoiDung = @ID_NguoiDung
                  AND NoiDung = @NoiDung
                LIMIT 1;
            ", new
            {
                ID_CauHoi = questionId,
                ID_NguoiDung = userId,
                answer.NoiDung
            });

            var ngayTao = Date(answer.DaysAgo, 10 + answer.DaysAgo % 6);
            var ngayCapNhat = answer.IsDeleted == 1
                ? Date(Math.Max(answer.DaysAgo - 1, 0), 17)
                : null;

            if (existingId != null)
            {
                connection.Execute(@"
                    UPDATE CAUTRALOI
                    SET
                        DaChapNhan = @DaChapNhan,
                        IsDeleted = @IsDeleted,
                        NgayTao = @NgayTao,
                        NgayCapNhat = @NgayCapNhat
                    WHERE ID_CauTraLoi = @ID_CauTraLoi;
                ", new
                {
                    ID_CauTraLoi = existingId.Value,
                    answer.DaChapNhan,
                    answer.IsDeleted,
                    NgayTao = ngayTao,
                    NgayCapNhat = ngayCapNhat
                });

                return existingId.Value;
            }

            return connection.ExecuteScalar<int>(@"
                INSERT INTO CAUTRALOI
                    (ID_CauHoi, ID_NguoiDung, NoiDung, DaChapNhan, IsDeleted, NgayTao, NgayCapNhat)
                VALUES
                    (@ID_CauHoi, @ID_NguoiDung, @NoiDung, @DaChapNhan, @IsDeleted, @NgayTao, @NgayCapNhat);

                SELECT last_insert_rowid();
            ", new
            {
                ID_CauHoi = questionId,
                ID_NguoiDung = userId,
                answer.NoiDung,
                answer.DaChapNhan,
                answer.IsDeleted,
                NgayTao = ngayTao,
                NgayCapNhat = ngayCapNhat
            });
        }

        private void InsertVote(IDbConnection connection, int userId, string loaiDoiTuong, int idDoiTuong, int giaTri, int daysAgo)
        {
            connection.Execute(@"
                INSERT OR IGNORE INTO BINHCHON
                    (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, GiaTri, NgayTao)
                VALUES
                    (@ID_NguoiDung, @LoaiDoiTuong, @ID_DoiTuong, @GiaTri, @NgayTao);

                UPDATE BINHCHON
                SET GiaTri = @GiaTri,
                    NgayCapNhat = @NgayCapNhat
                WHERE ID_NguoiDung = @ID_NguoiDung
                  AND LoaiDoiTuong = @LoaiDoiTuong
                  AND ID_DoiTuong = @ID_DoiTuong;
            ", new
            {
                ID_NguoiDung = userId,
                LoaiDoiTuong = loaiDoiTuong,
                ID_DoiTuong = idDoiTuong,
                GiaTri = giaTri,
                NgayTao = Date(daysAgo, 14),
                NgayCapNhat = Date(0, 14)
            });
        }

        private void InsertComment(IDbConnection connection, CommentSeed comment)
        {
            var targetId = comment.LoaiDoiTuong == "CAUHOI"
                ? GetQuestionId(connection, comment.TargetQuestionTitle)
                : GetFirstAnswerId(connection, comment.TargetQuestionTitle);

            var userId = GetUserId(connection, comment.Email);

            connection.Execute(@"
                INSERT INTO BINHLUAN
                    (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, NoiDung, IsDeleted, NgayTao, NgayCapNhat)
                SELECT
                    @ID_NguoiDung, @LoaiDoiTuong, @ID_DoiTuong, @NoiDung, @IsDeleted, @NgayTao, @NgayCapNhat
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM BINHLUAN
                    WHERE ID_NguoiDung = @ID_NguoiDung
                      AND LoaiDoiTuong = @LoaiDoiTuong
                      AND ID_DoiTuong = @ID_DoiTuong
                      AND NoiDung = @NoiDung
                );

                UPDATE BINHLUAN
                SET IsDeleted = @IsDeleted,
                    NgayTao = @NgayTao,
                    NgayCapNhat = @NgayCapNhat
                WHERE ID_NguoiDung = @ID_NguoiDung
                  AND LoaiDoiTuong = @LoaiDoiTuong
                  AND ID_DoiTuong = @ID_DoiTuong
                  AND NoiDung = @NoiDung;
            ", new
            {
                ID_NguoiDung = userId,
                comment.LoaiDoiTuong,
                ID_DoiTuong = targetId,
                comment.NoiDung,
                comment.IsDeleted,
                NgayTao = Date(comment.DaysAgo, 15),
                NgayCapNhat = comment.IsDeleted == 1 ? Date(0, 16) : null
            });
        }

        private void InsertNotification(IDbConnection connection, NotificationSeed notification)
        {
            var receiverId = GetUserId(connection, notification.ReceiverEmail);
            int? creatorId = null;

            if (!string.IsNullOrWhiteSpace(notification.CreatorEmail))
            {
                creatorId = GetUserId(connection, notification.CreatorEmail);
            }

            connection.Execute(@"
                INSERT INTO THONGBAO
                    (ID_NguoiNhan, ID_NguoiTao, LoaiThongBao, TieuDe, NoiDung, Link, DaDoc, DaXoa, NgayTao)
                SELECT
                    @ID_NguoiNhan, @ID_NguoiTao, @LoaiThongBao, @TieuDe, @NoiDung, @Link, @DaDoc, @DaXoa, @NgayTao
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM THONGBAO
                    WHERE ID_NguoiNhan = @ID_NguoiNhan
                      AND TieuDe = @TieuDe
                      AND COALESCE(Link, '') = COALESCE(@Link, '')
                );

                UPDATE THONGBAO
                SET
                    ID_NguoiTao = @ID_NguoiTao,
                    LoaiThongBao = @LoaiThongBao,
                    NoiDung = @NoiDung,
                    DaDoc = @DaDoc,
                    DaXoa = @DaXoa,
                    NgayTao = @NgayTao
                WHERE ID_NguoiNhan = @ID_NguoiNhan
                  AND TieuDe = @TieuDe
                  AND COALESCE(Link, '') = COALESCE(@Link, '');
            ", new
            {
                ID_NguoiNhan = receiverId,
                ID_NguoiTao = creatorId,
                notification.LoaiThongBao,
                notification.TieuDe,
                notification.NoiDung,
                notification.Link,
                notification.DaDoc,
                notification.DaXoa,
                NgayTao = Date(notification.DaysAgo, 18)
            });
        }

        private void InsertReport(IDbConnection connection, ReportSeed report)
        {
            var reporterId = GetUserId(connection, report.ReporterEmail);
            var targetId = report.LoaiDoiTuong == "CAUHOI"
                ? GetQuestionId(connection, report.TargetTitle)
                : report.LoaiDoiTuong == "CAUTRALOI"
                    ? GetFirstAnswerId(connection, report.TargetTitle)
                    : GetFirstCommentId(connection, report.TargetTitle);

            connection.Execute(@"
                INSERT INTO TOCAO
                    (ID_NguoiToCao, LoaiDoiTuong, ID_DoiTuong, LyDo, MoTa, TrangThai, ID_AdminXuLy, GhiChuXuLy, NgayTao, NgayXuLy)
                SELECT
                    @ID_NguoiToCao, @LoaiDoiTuong, @ID_DoiTuong, @LyDo, @MoTa, @TrangThai, @ID_AdminXuLy, @GhiChuXuLy, @NgayTao, @NgayXuLy
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM TOCAO
                    WHERE ID_NguoiToCao = @ID_NguoiToCao
                      AND LoaiDoiTuong = @LoaiDoiTuong
                      AND ID_DoiTuong = @ID_DoiTuong
                      AND LyDo = @LyDo
                );

                UPDATE TOCAO
                SET
                    MoTa = @MoTa,
                    TrangThai = @TrangThai,
                    ID_AdminXuLy = @ID_AdminXuLy,
                    GhiChuXuLy = @GhiChuXuLy,
                    NgayTao = @NgayTao,
                    NgayXuLy = @NgayXuLy
                WHERE ID_NguoiToCao = @ID_NguoiToCao
                  AND LoaiDoiTuong = @LoaiDoiTuong
                  AND ID_DoiTuong = @ID_DoiTuong
                  AND LyDo = @LyDo;
            ", new
            {
                ID_NguoiToCao = reporterId,
                report.LoaiDoiTuong,
                ID_DoiTuong = targetId,
                report.LyDo,
                report.MoTa,
                report.TrangThai,
                ID_AdminXuLy = report.AdminId,
                report.GhiChuXuLy,
                NgayTao = Date(report.DaysAgo, 11),
                NgayXuLy = report.HandledDaysAgo == null ? null : Date(report.HandledDaysAgo.Value, 17)
            });
        }

        private void GanTheChoCauHoi(IDbConnection connection, int idCauHoi, string[] tags)
        {
            foreach (var tag in tags)
            {
                var idThe = connection.ExecuteScalar<int?>(
                    "SELECT ID_The FROM THE WHERE TenThe = @TenThe LIMIT 1;",
                    new { TenThe = tag }
                );

                if (idThe == null)
                {
                    continue;
                }

                connection.Execute(@"
                    INSERT OR IGNORE INTO CauHoi_The (ID_CauHoi, ID_The)
                    VALUES (@ID_CauHoi, @ID_The);
                ", new
                {
                    ID_CauHoi = idCauHoi,
                    ID_The = idThe.Value
                });
            }
        }

        private int GetUserId(IDbConnection connection, string email)
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

        private int GetChuyenMucId(IDbConnection connection, string tenChuyenMuc)
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

        private int GetQuestionId(IDbConnection connection, string title)
        {
            var questionId = connection.ExecuteScalar<int?>(@"
                SELECT ID_CauHoi
                FROM CAUHOI
                WHERE TieuDe = @TieuDe
                LIMIT 1;
            ", new { TieuDe = title });

            if (questionId == null)
            {
                throw new InvalidOperationException($"Không tìm thấy câu hỏi: {title}");
            }

            return questionId.Value;
        }

        private int GetFirstAnswerId(IDbConnection connection, string questionTitle)
        {
            var answerId = connection.ExecuteScalar<int?>(@"
                SELECT ctl.ID_CauTraLoi
                FROM CAUTRALOI ctl
                JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
                WHERE ch.TieuDe = @TieuDe
                ORDER BY ctl.ID_CauTraLoi
                LIMIT 1;
            ", new { TieuDe = questionTitle });

            if (answerId == null)
            {
                throw new InvalidOperationException($"Không tìm thấy câu trả lời thuộc câu hỏi: {questionTitle}");
            }

            return answerId.Value;
        }

        private int GetFirstCommentId(IDbConnection connection, string questionTitle)
        {
            var questionId = GetQuestionId(connection, questionTitle);

            var commentId = connection.ExecuteScalar<int?>(@"
                SELECT ID_BinhLuan
                FROM BINHLUAN
                WHERE LoaiDoiTuong = 'CAUHOI'
                  AND ID_DoiTuong = @ID_DoiTuong
                ORDER BY ID_BinhLuan
                LIMIT 1;
            ", new { ID_DoiTuong = questionId });

            if (commentId == null)
            {
                throw new InvalidOperationException($"Không tìm thấy bình luận thuộc câu hỏi: {questionTitle}");
            }

            return commentId.Value;
        }

        private string QuestionLink(IDbConnection connection, string title)
        {
            return $"/questions/{GetQuestionId(connection, title)}";
        }

        private static string Date(int daysAgo, int hour = 9)
        {
            return DateTime.Today
                .AddDays(-daysAgo)
                .AddHours(hour)
                .ToString("yyyy-MM-dd HH:mm:ss");
        }

        private sealed record UserSeed(
            string HoTen,
            string Email,
            string Password,
            string VaiTro,
            int TrangThai,
            int DaysAgo
        );

        private sealed record CategorySeed(
            string TenChuyenMuc,
            string MoTa
        );

        private sealed record QuestionSeed(
            string TieuDe,
            string NoiDung,
            string ChuyenMuc,
            string Email,
            int LuotXem,
            int IsDeleted,
            int DaysAgo,
            string[] Tags
        );

        private sealed record AnswerSeed(
            string QuestionTitle,
            string Email,
            string NoiDung,
            int DaChapNhan,
            int IsDeleted,
            int DaysAgo
        );

        private sealed record CommentSeed(
            string LoaiDoiTuong,
            string TargetQuestionTitle,
            string Email,
            string NoiDung,
            int IsDeleted,
            int DaysAgo
        );

        private sealed record NotificationSeed(
            string ReceiverEmail,
            string? CreatorEmail,
            string LoaiThongBao,
            string TieuDe,
            string NoiDung,
            string? Link,
            int DaDoc,
            int DaXoa,
            int DaysAgo
        );

        private sealed record ReportSeed(
            string ReporterEmail,
            string LoaiDoiTuong,
            string TargetTitle,
            string LyDo,
            string MoTa,
            string TrangThai,
            int? AdminId,
            string? GhiChuXuLy,
            int DaysAgo,
            int? HandledDaysAgo
        );
    }
}

