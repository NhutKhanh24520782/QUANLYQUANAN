CREATE DATABASE QLQuanAn;
GO
USE QLQuanAn;
GO

-- 1. NGƯỜI DÙNG
CREATE TABLE NGUOIDUNG (
    MaNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    MatKhau NVARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(20) CHECK (VaiTro IN (N'Bep', N'PhucVu', N'Admin')), -- Admin do admin tạo
    HoTen NVARCHAR(100) NOT NULL,
    SDT NVARCHAR(15),
    Email NVARCHAR(100),
    TrangThai BIT DEFAULT 1,
    NgayTao DATETIME DEFAULT GETDATE()
);

-- 2. LOẠI MÓN
CREATE TABLE LOAIMON (
    MaLoaiMon INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(200),
    TrangThai BIT DEFAULT 1
);

-- 3. MÓN ĂN (bỏ cột HinhAnh)
CREATE TABLE MENUITEMS (
    MaMon INT IDENTITY(1,1) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    Gia DECIMAL(12,2) NOT NULL,
    MoTa NVARCHAR(200),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ConMon', N'HetMon')),
    MaLoaiMon INT,
    FOREIGN KEY (MaLoaiMon) REFERENCES LOAIMON(MaLoaiMon)
);

-- 4. BÀN ĂN
CREATE TABLE BAN (
    MaBanAn INT IDENTITY(1,1) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL,
    SoChoNgoi INT,
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Trong', N'DangSuDung', N'DaDat')),
    MaNhanVien INT NULL,
    FOREIGN KEY (MaNhanVien) REFERENCES NGUOIDUNG(MaNguoiDung)
);

-- 5. HÓA ĐƠN
CREATE TABLE HOADON (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaBanAn INT,
    MaNV INT,
    Ngay DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ChuaThanhToan', N'DaThanhToan', N'Huy')),
    PhuongThucThanhToan NVARCHAR(50),
    TongTien DECIMAL(12,2) DEFAULT 0,
    GhiChu NVARCHAR(200),
    FOREIGN KEY (MaBanAn) REFERENCES BAN(MaBanAn),
    FOREIGN KEY (MaNV) REFERENCES NGUOIDUNG(MaNguoiDung)
);

-- 6. CHI TIẾT HÓA ĐƠN
CREATE TABLE CTHD (
    MaHD INT,
    MaMon INT,
    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(12,2),
    Gia AS (SoLuong * DonGia) PERSISTED,
    PRIMARY KEY (MaHD, MaMon),
    FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MENUITEMS(MaMon)
);

-- 7. ĐƠN HÀNG (cho nhân viên bếp)
CREATE TABLE DONHANG (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    MaBanAn INT,
    MaNVOrder INT,
    NgayOrder DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ChoXacNhan', N'DangCheBien', N'HoanThanh', N'Huy')),
    FOREIGN KEY (MaBanAn) REFERENCES BAN(MaBanAn),
    FOREIGN KEY (MaNVOrder) REFERENCES NGUOIDUNG(MaNguoiDung)
);

-- 8. TIN NHẮN (chat giữa nhân viên)
CREATE TABLE TINNHAN (
    MaTin INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoiGui INT NOT NULL,
    MaNguoiNhan INT NOT NULL,
    NoiDung NVARCHAR(500),
    ThoiGian DATETIME DEFAULT GETDATE(),
    DaDoc BIT DEFAULT 0,
    FOREIGN KEY (MaNguoiGui) REFERENCES NGUOIDUNG(MaNguoiDung),
    FOREIGN KEY (MaNguoiNhan) REFERENCES NGUOIDUNG(MaNguoiDung)
);

-- 9. BÁO CÁO (cho admin)
CREATE TABLE BAOCAO (
    MaBaoCao INT IDENTITY(1,1) PRIMARY KEY,
    LoaiBaoCao NVARCHAR(50),
    NgayTao DATETIME DEFAULT GETDATE(),
    MaNguoiTao INT,
    FOREIGN KEY (MaNguoiTao) REFERENCES NGUOIDUNG(MaNguoiDung)
);

insert into HOADON (MaNV, MaBanAn)
values ('2','2')
insert into BAN (TenBan)
values ('Ban 2')

select * from HOADON

delete from HOADON
where MaHD = '4'