-- 1. Tạo bảng CHUYENMUC và THE
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

-- 2. Tạo bảng CAUHOI (Tham chiếu Khóa ngoại)
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

-- 3. Tạo bảng trung gian CauHoi_The
CREATE TABLE IF NOT EXISTS CauHoi_The (
    ID_CauHoi INTEGER NOT NULL,
    ID_The INTEGER NOT NULL,
    PRIMARY KEY (ID_CauHoi, ID_The),
    FOREIGN KEY (ID_CauHoi) REFERENCES CAUHOI(ID_CauHoi),
    FOREIGN KEY (ID_The) REFERENCES THE(ID_The)
);

-- 4. Thêm 3 dòng dữ liệu mẫu cho Frontend có cái để hiển thị
INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa) VALUES ('Lập trình Web', 'Thảo luận về Vue.js, React, ASP.NET, PHP...');
INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa) VALUES ('Cơ sở dữ liệu', 'Các vấn đề liên quan đến SQL Server, SQLite, MongoDB...');
INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa) VALUES ('Chia sẻ kinh nghiệm', 'Kinh nghiệm học tập, thực tập và định hướng nghề nghiệp IT.');


--=========================SỬA================================
DELETE FROM CHUYENMUC;
DELETE FROM sqlite_sequence WHERE name='CHUYENMUC';
INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa) VALUES ('Lập trình Web', 'Thảo luận về Vue.js, React, ASP.NET, PHP...');
INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa) VALUES ('Cơ sở dữ liệu', 'Các vấn đề liên quan đến SQL Server, SQLite, MongoDB...');
INSERT INTO CHUYENMUC (TenChuyenMuc, MoTa) VALUES ('Chia sẻ kinh nghiệm', 'Kinh nghiệm học tập, thực tập và định hướng nghề nghiệp IT.');
SELECT * FROM CHUYENMUC;