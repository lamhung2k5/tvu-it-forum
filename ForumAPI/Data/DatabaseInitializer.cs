using Dapper;

namespace ForumAPI.Data
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            // Sử dụng "using" để đảm bảo kết nối SQLite được đóng ngay sau khi chạy xong lệnh
            using var connection = _connectionFactory.CreateConnection();

            // Các câu lệnh bên dưới giúp database tự có đủ bảng khi chạy app ở máy mới.
            // Nếu bảng đã tồn tại thì SQLite sẽ bỏ qua, không tạo lại.
            var sql = @"
                CREATE TABLE IF NOT EXISTS NGUOIDUNG (
                    ID_NguoiDung INTEGER PRIMARY KEY AUTOINCREMENT,
                    HoTen TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    MatKhauHash TEXT NOT NULL,
                    AnhDaiDien TEXT,
                    VaiTro TEXT NOT NULL DEFAULT 'User',
                    TrangThai INTEGER NOT NULL DEFAULT 1,
                    NgayTao TEXT NOT NULL,
                    NgayCapNhat TEXT
                );

                CREATE TABLE IF NOT EXISTS CHUYENMUC (
                    ID_ChuyenMuc INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenChuyenMuc NVARCHAR(100) NOT NULL,
                    MoTa NVARCHAR(500) NULL,
                    TrangThai INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS THE (
                    ID_The INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenThe VARCHAR(50) NOT NULL UNIQUE,
                    MoTa NVARCHAR(250) NULL,
                    TrangThai INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS CAUHOI (
                    ID_CauHoi INTEGER PRIMARY KEY AUTOINCREMENT,
                    ID_NguoiDung INTEGER NOT NULL,
                    ID_ChuyenMuc INTEGER NOT NULL,
                    TieuDe NVARCHAR(250) NOT NULL,
                    NoiDung TEXT NOT NULL,
                    TrangThai INTEGER NOT NULL DEFAULT 0,
                    LuotXem INTEGER NOT NULL DEFAULT 0,
                    NgayTao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    NgayCapNhat TEXT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (ID_NguoiDung) REFERENCES NGUOIDUNG(ID_NguoiDung),
                    FOREIGN KEY (ID_ChuyenMuc) REFERENCES CHUYENMUC(ID_ChuyenMuc)
                );

                CREATE TABLE IF NOT EXISTS CauHoi_The (
                    ID_CauHoi INTEGER NOT NULL,
                    ID_The INTEGER NOT NULL,
                    PRIMARY KEY (ID_CauHoi, ID_The),
                    FOREIGN KEY (ID_CauHoi) REFERENCES CAUHOI(ID_CauHoi),
                    FOREIGN KEY (ID_The) REFERENCES THE(ID_The)
                );

                CREATE TABLE IF NOT EXISTS CAUTRALOI (
                    ID_CauTraLoi INTEGER PRIMARY KEY AUTOINCREMENT,
                    ID_CauHoi INTEGER NOT NULL,
                    ID_NguoiDung INTEGER NOT NULL,
                    NoiDung TEXT NOT NULL,
                    DaChapNhan INTEGER NOT NULL DEFAULT 0,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    NgayTao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    NgayCapNhat TEXT NULL,
                    FOREIGN KEY (ID_CauHoi) REFERENCES CAUHOI(ID_CauHoi),
                    FOREIGN KEY (ID_NguoiDung) REFERENCES NGUOIDUNG(ID_NguoiDung)
                );

                CREATE TABLE IF NOT EXISTS BINHCHON (
                    ID_BinhChon INTEGER PRIMARY KEY AUTOINCREMENT,
                    ID_NguoiDung INTEGER NOT NULL,
                    LoaiDoiTuong TEXT NOT NULL,
                    ID_DoiTuong INTEGER NOT NULL,
                    GiaTri INTEGER NOT NULL,
                    NgayTao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    NgayCapNhat TEXT NULL,
                    FOREIGN KEY (ID_NguoiDung) REFERENCES NGUOIDUNG(ID_NguoiDung),
                    CHECK (LoaiDoiTuong IN ('CAUHOI', 'CAUTRALOI')),
                    CHECK (GiaTri IN (1, -1)),
                    UNIQUE (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong)
                );

                CREATE TABLE IF NOT EXISTS BINHLUAN (
                    ID_BinhLuan INTEGER PRIMARY KEY AUTOINCREMENT,
                    ID_NguoiDung INTEGER NOT NULL,
                    LoaiDoiTuong TEXT NOT NULL,
                    ID_DoiTuong INTEGER NOT NULL,
                    NoiDung TEXT NOT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    NgayTao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    NgayCapNhat TEXT NULL,
                    FOREIGN KEY (ID_NguoiDung) REFERENCES NGUOIDUNG(ID_NguoiDung),
                    CHECK (LoaiDoiTuong IN ('CAUHOI', 'CAUTRALOI'))
                );

                CREATE TABLE IF NOT EXISTS THONGBAO (
                    ID_ThongBao INTEGER PRIMARY KEY AUTOINCREMENT,
                    ID_NguoiNhan INTEGER NOT NULL,
                    ID_NguoiTao INTEGER NULL,
                    LoaiThongBao TEXT NOT NULL,
                    TieuDe TEXT NOT NULL,
                    NoiDung TEXT NOT NULL,
                    Link TEXT NULL,
                    DaDoc INTEGER NOT NULL DEFAULT 0,
                    DaXoa INTEGER NOT NULL DEFAULT 0,
                    NgayTao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (ID_NguoiNhan) REFERENCES NGUOIDUNG(ID_NguoiDung),
                    FOREIGN KEY (ID_NguoiTao) REFERENCES NGUOIDUNG(ID_NguoiDung)
                );

                CREATE TABLE IF NOT EXISTS TOCAO (
                    ID_ToCao INTEGER PRIMARY KEY AUTOINCREMENT,
                    ID_NguoiToCao INTEGER NOT NULL,
                    LoaiDoiTuong TEXT NOT NULL,
                    ID_DoiTuong INTEGER NOT NULL,
                    LyDo TEXT NOT NULL,
                    MoTa TEXT NULL,
                    TrangThai TEXT NOT NULL DEFAULT 'PENDING',
                    ID_AdminXuLy INTEGER NULL,
                    GhiChuXuLy TEXT NULL,
                    NgayTao TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    NgayXuLy TEXT NULL,
                    FOREIGN KEY (ID_NguoiToCao) REFERENCES NGUOIDUNG(ID_NguoiDung),
                    FOREIGN KEY (ID_AdminXuLy) REFERENCES NGUOIDUNG(ID_NguoiDung),
                    CHECK (LoaiDoiTuong IN ('CAUHOI', 'CAUTRALOI', 'BINHLUAN')),
                    CHECK (TrangThai IN ('PENDING', 'REMINDED', 'RESOLVED', 'REJECTED'))
                );

            ";

            connection.Execute(sql);

            // Bổ sung cột mới cho database cũ đã tồn tại từ các phiên bản trước.
            EnsureColumn(connection, "THONGBAO", "DaXoa", "INTEGER NOT NULL DEFAULT 0");

            // Seed dữ liệu chuyên mục cơ bản nếu chưa có, tránh bị nhân bản khi app khởi động nhiều lần.
            var seedChuyenMucSql = @"
                INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa)
                SELECT 'Lập trình Web', 'Thảo luận về Vue.js, React, ASP.NET, PHP...'
                WHERE NOT EXISTS (SELECT 1 FROM CHUYENMUC WHERE TenChuyenMuc = 'Lập trình Web');

                INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa)
                SELECT 'Cơ sở dữ liệu', 'Các vấn đề liên quan đến SQL Server, SQLite, MongoDB...'
                WHERE NOT EXISTS (SELECT 1 FROM CHUYENMUC WHERE TenChuyenMuc = 'Cơ sở dữ liệu');

                INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa)
                SELECT 'Chia sẻ kinh nghiệm', 'Kinh nghiệm học tập, thực tập và định hướng nghề nghiệp IT.'
                WHERE NOT EXISTS (SELECT 1 FROM CHUYENMUC WHERE TenChuyenMuc = 'Chia sẻ kinh nghiệm');
            ";

            connection.Execute(seedChuyenMucSql);
        }

        private static void EnsureColumn(System.Data.IDbConnection connection, string tableName, string columnName, string definition)
        {
            var existing = connection.Query($"PRAGMA table_info({tableName});");

            foreach (var column in existing)
            {
                if (string.Equals((string)column.name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            connection.Execute($"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition};");
        }
    }
}
