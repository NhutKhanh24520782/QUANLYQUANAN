/***************************************************************
  File: QuanLyQuanAn_cleaned.sql
  Mô tả: Toàn bộ script tạo database (tables, indexes, procs, trigger)
         Đã sắp xếp thứ tự tạo và thêm DROP IF EXISTS + GO cho an toàn
***************************************************************/

-- =========================
-- 1. XÓA (nếu đã tồn tại) để chạy idempotent
--    Lưu ý: phải drop trigger/procedure trước, sau đó drop table theo thứ tự phụ thuộc (reverse deps)
-- =========================
IF OBJECT_ID('dbo.trg_GhiLichSuTrangThai', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_GhiLichSuTrangThai;
GO

IF OBJECT_ID('dbo.sp_ThongKeHieuSuatDauBep','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ThongKeHieuSuatDauBep;
GO
IF OBJECT_ID('dbo.sp_TopMonPhobien','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_TopMonPhobien;
GO
IF OBJECT_ID('dbo.sp_ThongKeTongQuanBep','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ThongKeTongQuanBep;
GO

-- Drop tables in reverse dependency order
IF OBJECT_ID('dbo.LICHSU_TRANGTHAI_MON','U') IS NOT NULL DROP TABLE dbo.LICHSU_TRANGTHAI_MON;
IF OBJECT_ID('dbo.CHITIET_DONHANG','U') IS NOT NULL DROP TABLE dbo.CHITIET_DONHANG;
IF OBJECT_ID('dbo.CTHD','U') IS NOT NULL DROP TABLE dbo.CTHD;
IF OBJECT_ID('dbo.THANHTOAN','U') IS NOT NULL DROP TABLE dbo.THANHTOAN;
IF OBJECT_ID('dbo.BAOCAO','U') IS NOT NULL DROP TABLE dbo.BAOCAO;
IF OBJECT_ID('dbo.TINNHAN','U') IS NOT NULL DROP TABLE dbo.TINNHAN;
IF OBJECT_ID('dbo.HOADON','U') IS NOT NULL DROP TABLE dbo.HOADON;
IF OBJECT_ID('dbo.DONHANG','U') IS NOT NULL DROP TABLE dbo.DONHANG;
IF OBJECT_ID('dbo.BAN','U') IS NOT NULL DROP TABLE dbo.BAN;
IF OBJECT_ID('dbo.MENUITEMS','U') IS NOT NULL DROP TABLE dbo.MENUITEMS;
IF OBJECT_ID('dbo.LOAIMON','U') IS NOT NULL DROP TABLE dbo.LOAIMON;
IF OBJECT_ID('dbo.NGUOIDUNG','U') IS NOT NULL DROP TABLE dbo.NGUOIDUNG;
GO

-- =========================
-- 2. TẠO BẢNG (thứ tự hợp lệ cho khóa ngoại)
-- =========================

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
GO

-- 2. LOẠI MÓN
CREATE TABLE LOAIMON (
    MaLoaiMon INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(200),
    TrangThai BIT DEFAULT 1
);
GO

-- 3. MÓN ĂN (bỏ cột HinhAnh)
CREATE TABLE MENUITEMS (
    MaMon INT IDENTITY(1,1) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    Gia DECIMAL(12,2) NOT NULL,
    MoTa NVARCHAR(200),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ConMon', N'HetMon')),
    MaLoaiMon INT NULL,
    FOREIGN KEY (MaLoaiMon) REFERENCES LOAIMON(MaLoaiMon)
);
GO

-- 4. BÀN ĂN
CREATE TABLE BAN (
    MaBanAn INT IDENTITY(1,1) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL,
    SoChoNgoi INT,
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Trong', N'DangSuDung', N'DaDat')),
    MaNhanVien INT NULL,
    FOREIGN KEY (MaNhanVien) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- 5. ĐƠN HÀNG (cho nhân viên bếp)  -- DONHANG cần trước HOADON nếu HOADON có FK -> MaDonHang
CREATE TABLE DONHANG (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    MaBanAn INT NULL,
    MaNVOrder INT NULL,
    NgayOrder DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ChoXacNhan', N'DangCheBien', N'HoanThanh', N'Huy')),
    FOREIGN KEY (MaBanAn) REFERENCES BAN(MaBanAn),
    FOREIGN KEY (MaNVOrder) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- 6. HÓA ĐƠN
CREATE TABLE HOADON (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaBanAn INT NULL,
    MaNV INT NULL,
    MaDonHang INT NULL, 
    Ngay DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ChuaThanhToan', N'DaThanhToan', N'Huy')),
    PhuongThucThanhToan NVARCHAR(50),
    TongTien DECIMAL(12,2) DEFAULT 0,
    GhiChu NVARCHAR(200),
    FOREIGN KEY (MaBanAn) REFERENCES BAN(MaBanAn),
    FOREIGN KEY (MaNV) REFERENCES NGUOIDUNG(MaNguoiDung),
    FOREIGN KEY (MaDonHang) REFERENCES DONHANG(MaDonHang)
);
GO

-- 7. CHI TIẾT HÓA ĐƠN
CREATE TABLE CTHD (
    MaHD INT,
    MaMon INT,
    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(12,2),
    Gia AS (SoLuong * DonGia) PERSISTED,
    GhiChu NVARCHAR(200) NULL,
    PRIMARY KEY (MaHD, MaMon),
    FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MENUITEMS(MaMon)
);
GO

-- 8. BẢNG CHI TIẾT ĐƠN HÀNG (QUAN TRỌNG cho cả bếp và phục vụ)
CREATE TABLE CHITIET_DONHANG (
    MaChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Thông tin đơn hàng
    MaDonHang INT NOT NULL,
    
    -- Thông tin món ăn
    MaMon INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(12,2) NOT NULL,
    
    -- Ghi chú từ khách (khi order)
    GhiChuKhach NVARCHAR(200),
    
    -- TRẠNG THÁI TỪNG MÓN (quan trọng!)
    TrangThai NVARCHAR(20) DEFAULT N'ChoXacNhan' 
        CHECK (TrangThai IN (
            N'ChoXacNhan',
            N'DangCheBien',
            N'HoanThanh',
            N'CoVanDe',
            N'Huy'
        )),
    
    -- Thông tin từ bếp
    GhiChuBep NVARCHAR(200),
    MaNhanVienCheBien INT NULL,
    UuTien INT DEFAULT 1,
    
    -- Thời gian tracking
    ThoiGianBatDau DATETIME NULL,
    ThoiGianHoanThanh DATETIME NULL,
    ThoiGianDuKien DATETIME NULL,
    
    -- Khóa ngoại
    FOREIGN KEY (MaDonHang) REFERENCES DONHANG(MaDonHang) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MENUITEMS(MaMon),
    FOREIGN KEY (MaNhanVienCheBien) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- 9. BẢNG LỊCH SỬ THAY ĐỔI TRẠNG THÁI MÓN - DÙNG CHO THỐNG KÊ HIỆU SUẤT
CREATE TABLE LICHSU_TRANGTHAI_MON (
    MaLichSu INT IDENTITY(1,1) PRIMARY KEY,
    
    MaChiTiet INT NOT NULL,
    TrangThaiCu NVARCHAR(20) NULL,
    TrangThaiMoi NVARCHAR(20) NOT NULL,
    MaNhanVienThucHien INT NOT NULL,
    ThoiGianThayDoi DATETIME DEFAULT GETDATE(),
    GhiChu NVARCHAR(500) NULL,
    ThoiGianXuLy INT NULL,
    
    FOREIGN KEY (MaChiTiet) REFERENCES CHITIET_DONHANG(MaChiTiet),
    FOREIGN KEY (MaNhanVienThucHien) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- 10. TIN NHẮN (chat giữa nhân viên)
CREATE TABLE TINNHAN (
    MaTinNhan INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoiGui INT NOT NULL,
    MaNguoiNhan INT NOT NULL,
    NoiDung NVARCHAR(500),
    ThoiGian DATETIME DEFAULT GETDATE(),
    DaDoc BIT DEFAULT 0,
    FOREIGN KEY (MaNguoiGui) REFERENCES NGUOIDUNG(MaNguoiDung),
    FOREIGN KEY (MaNguoiNhan) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- 11. BÁO CÁO (cho admin)
CREATE TABLE BAOCAO (
    MaBaoCao INT IDENTITY(1,1) PRIMARY KEY,
    LoaiBaoCao NVARCHAR(50),
    NgayTao DATETIME DEFAULT GETDATE(),
    MaNguoiTao INT NULL,
    FOREIGN KEY (MaNguoiTao) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- 12. THANHTOAN
CREATE TABLE THANHTOAN (
    MaGiaoDich INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaNhanVien INT NOT NULL,
    PhuongThucThanhToan NVARCHAR(20) NOT NULL 
        CHECK (PhuongThucThanhToan IN (N'TienMat', N'ChuyenKhoan')),
    SoTienThanhToan DECIMAL(12,2) NOT NULL CHECK (SoTienThanhToan > 0),
    SoTienNhan DECIMAL(12,2) NULL,
    SoTienThua DECIMAL(12,2) NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ThanhCong' 
        CHECK (TrangThai IN (N'ThanhCong', N'ThatBai', N'Huy')),
    MaGiaoDichNganHang NVARCHAR(255) NULL,
    QRCodeData NVARCHAR(MAX) NULL,
    ThoiGianTao DATETIME DEFAULT GETDATE(),
    ThoiGianThanhToan DATETIME DEFAULT GETDATE(),
    GhiChu NVARCHAR(500) NULL,
    FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD) ON DELETE CASCADE,
    FOREIGN KEY (MaNhanVien) REFERENCES NGUOIDUNG(MaNguoiDung)
);
GO

-- =========================
-- 3. INDEX cho hiệu năng
-- =========================
CREATE INDEX IX_CHITIET_DONHANG_TrangThai ON CHITIET_DONHANG(TrangThai);
CREATE INDEX IX_CHITIET_DONHANG_MaDonHang ON CHITIET_DONHANG(MaDonHang);

CREATE INDEX IX_LICHSU_TRANGTHAI_MON_ThoiGian ON LICHSU_TRANGTHAI_MON(ThoiGianThayDoi);
CREATE INDEX IX_LICHSU_TRANGTHAI_MON_MaChiTiet ON LICHSU_TRANGTHAI_MON(MaChiTiet);
CREATE INDEX IX_LICHSU_TRANGTHAI_MON_MaNhanVien ON LICHSU_TRANGTHAI_MON(MaNhanVienThucHien);
GO

-- =========================
-- 4. STORED PROCEDURES (DROP IF EXISTS rồi tạo)
-- =========================

-- sp_ThongKeHieuSuatDauBep
IF OBJECT_ID('dbo.sp_ThongKeHieuSuatDauBep','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ThongKeHieuSuatDauBep;
GO

CREATE PROCEDURE dbo.sp_ThongKeHieuSuatDauBep
    @TuNgay DATETIME,
    @DenNgay DATETIME,
    @MaNhanVien INT = NULL -- NULL = tất cả đầu bếp
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        nd.MaNguoiDung,
        nd.HoTen,
        COUNT(DISTINCT dh.MaDonHang) AS TongDon,
        SUM(CASE WHEN dh.TrangThai = N'HoanThanh' THEN 1 ELSE 0 END) AS DonHoanThanh,
        COUNT(ct.MaChiTiet) AS TongMon,
        SUM(CASE WHEN ct.TrangThai = N'HoanThanh' THEN 1 ELSE 0 END) AS MonHoanThanh,
        AVG(DATEDIFF(MINUTE, ls.ThoiGianThayDoi, ls2.ThoiGianThayDoi)) AS ThoiGianTrungBinh
    FROM NGUOIDUNG nd
    LEFT JOIN CHITIET_DONHANG ct ON nd.MaNguoiDung = ct.MaNhanVienCheBien
    LEFT JOIN DONHANG dh ON ct.MaDonHang = dh.MaDonHang
    LEFT JOIN LICHSU_TRANGTHAI_MON ls ON ct.MaChiTiet = ls.MaChiTiet AND ls.TrangThaiMoi = N'DangCheBien'
    LEFT JOIN LICHSU_TRANGTHAI_MON ls2 ON ct.MaChiTiet = ls2.MaChiTiet AND ls2.TrangThaiMoi = N'HoanThanh'
    WHERE nd.VaiTro = N'Bep'
        AND dh.NgayOrder BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay)
        AND (@MaNhanVien IS NULL OR nd.MaNguoiDung = @MaNhanVien)
    GROUP BY nd.MaNguoiDung, nd.HoTen
    ORDER BY TongDon DESC;
END
GO

-- sp_TopMonPhobien
IF OBJECT_ID('dbo.sp_TopMonPhobien','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_TopMonPhobien;
GO

CREATE PROCEDURE dbo.sp_TopMonPhobien
    @TuNgay DATETIME,
    @DenNgay DATETIME,
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP(@Top)
        mi.MaMon,
        mi.TenMon,
        SUM(ct.SoLuong) AS SoLuong,
        COUNT(DISTINCT ct.MaDonHang) AS SoDon,
        lm.TenLoai
    FROM CHITIET_DONHANG ct
    JOIN MENUITEMS mi ON ct.MaMon = mi.MaMon
    JOIN DONHANG dh ON ct.MaDonHang = dh.MaDonHang
    LEFT JOIN LOAIMON lm ON mi.MaLoaiMon = lm.MaLoaiMon
    WHERE dh.NgayOrder BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay)
    GROUP BY mi.MaMon, mi.TenMon, lm.TenLoai
    ORDER BY SoLuong DESC;
END
GO

-- sp_ThongKeTongQuanBep
IF OBJECT_ID('dbo.sp_ThongKeTongQuanBep','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ThongKeTongQuanBep;
GO

CREATE PROCEDURE dbo.sp_ThongKeTongQuanBep
    @TuNgay DATETIME,
    @DenNgay DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TongDon INT = 0, @DonHoanThanh INT = 0, @TongMon INT = 0, @ThoiGianTB DECIMAL(10,2) = 0;

    SELECT @TongDon = COUNT(*)
    FROM DONHANG
    WHERE NgayOrder BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay);

    SELECT @DonHoanThanh = COUNT(*)
    FROM DONHANG
    WHERE TrangThai = N'HoanThanh'
        AND NgayOrder BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay);

    SELECT @TongMon = SUM(ct.SoLuong)
    FROM CHITIET_DONHANG ct
    JOIN DONHANG dh ON ct.MaDonHang = dh.MaDonHang
    WHERE dh.NgayOrder BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay);

    SELECT @ThoiGianTB = AVG(DATEDIFF(MINUTE, ls.ThoiGianThayDoi, ls2.ThoiGianThayDoi))
    FROM LICHSU_TRANGTHAI_MON ls
    JOIN LICHSU_TRANGTHAI_MON ls2 ON ls.MaChiTiet = ls2.MaChiTiet
    WHERE ls.TrangThaiMoi = N'DangCheBien'
        AND ls2.TrangThaiMoi = N'HoanThanh'
        AND ls.ThoiGianThayDoi BETWEEN @TuNgay AND DATEADD(DAY, 1, @DenNgay);

    SELECT 
        @TongDon AS TongDon,
        @DonHoanThanh AS DonHoanThanh,
        @TongMon AS TongMon,
        @ThoiGianTB AS ThoiGianTrungBinh;
END
GO

-- =========================
-- 5. TRIGGER: Tự động ghi lịch sử khi thay đổi trạng thái món
-- =========================

IF OBJECT_ID('dbo.trg_GhiLichSuTrangThai', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_GhiLichSuTrangThai;
GO

CREATE TRIGGER dbo.trg_GhiLichSuTrangThai
ON CHITIET_DONHANG
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý khi có thay đổi trạng thái
    IF UPDATE(TrangThai)
    BEGIN
        INSERT INTO LICHSU_TRANGTHAI_MON (
            MaChiTiet, 
            TrangThaiCu, 
            TrangThaiMoi, 
            MaNhanVienThucHien, 
            GhiChu
        )
        SELECT 
            i.MaChiTiet,
            d.TrangThai AS TrangThaiCu,
            i.TrangThai AS TrangThaiMoi,
            i.MaNhanVienCheBien,
            i.GhiChuBep
        FROM inserted i
        INNER JOIN deleted d ON i.MaChiTiet = d.MaChiTiet
        WHERE i.TrangThai != d.TrangThai
            AND i.TrangThai IN (N'DangCheBien', N'HoanThanh', N'CoVanDe', N'Huy');
    END
END
GO