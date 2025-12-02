
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
    MaDonHang INT NULL, 
    Ngay DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'ChuaThanhToan', N'DaThanhToan', N'Huy')),
    PhuongThucThanhToan NVARCHAR(50),
    TongTien DECIMAL(12,2) DEFAULT 0,
    GhiChu NVARCHAR(200),
    FOREIGN KEY (MaBanAn) REFERENCES BAN(MaBanAn),
    FOREIGN KEY (MaNV) REFERENCES NGUOIDUNG(MaNguoiDung)
);
ALTER TABLE HOADON 
ADD CONSTRAINT FK_HOADON_DONHANG 
FOREIGN KEY (MaDonHang) REFERENCES DONHANG(MaDonHang);
-- 6. CHI TIẾT HÓA ĐƠN
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
CREATE TABLE THANHTOAN (
    -- Khóa chính
    MaGiaoDich INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Thông tin hóa đơn (liên kết với HOADON)
    MaHD INT NOT NULL,
    
    -- Thông tin nhân viên xử lý thanh toán
    MaNhanVien INT NOT NULL,
    
    -- Phương thức thanh toán (chỉ 2 loại theo HOADON)
    PhuongThucThanhToan NVARCHAR(20) NOT NULL 
        CHECK (PhuongThucThanhToan IN (N'TienMat', N'ChuyenKhoan')),
    
    -- Thông tin số tiền
    SoTienThanhToan DECIMAL(12,2) NOT NULL CHECK (SoTienThanhToan > 0),
    SoTienNhan DECIMAL(12,2) NULL, -- Chỉ dùng cho tiền mặt
    SoTienThua DECIMAL(12,2) NULL, -- Chỉ dùng cho tiền mặt
    
    -- Trạng thái giao dịch
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ThanhCong' 
        CHECK (TrangThai IN (N'ThanhCong', N'ThatBai', N'Huy')),
    
    -- Thông tin giao dịch ngân hàng (chỉ dùng cho chuyển khoản)
    MaGiaoDichNganHang NVARCHAR(255) NULL,
    QRCodeData NVARCHAR(MAX) NULL,
    
    -- Thời gian
    ThoiGianTao DATETIME DEFAULT GETDATE(),
    ThoiGianThanhToan DATETIME DEFAULT GETDATE(),
    
    -- Ghi chú
    GhiChu NVARCHAR(500) NULL,
    
    -- Khóa ngoại
    FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD) ON DELETE CASCADE,
    FOREIGN KEY (MaNhanVien) REFERENCES NGUOIDUNG(MaNguoiDung)
);
-- BẢNG CHI TIẾT ĐƠN HÀNG (QUAN TRỌNG cho cả bếp và phục vụ)
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
            N'ChoXacNhan',      -- Chờ xác nhận
            N'DangCheBien',     -- Đang chế biến
            N'HoanThanh',       -- Hoàn thành
            N'CoVanDe',         -- Có vấn đề (hết nguyên liệu...)
            N'Huy'              -- Hủy
        )),
    
    -- Thông tin từ bếp
    GhiChuBep NVARCHAR(200),      -- Lý do delay, đề xuất thay thế
    MaNhanVienCheBien INT NULL,   -- Ai đang chế biến
    UuTien INT DEFAULT 1,         -- Độ ưu tiên (1-5)
    
    -- Thời gian tracking
    ThoiGianBatDau DATETIME NULL,
    ThoiGianHoanThanh DATETIME NULL,
    ThoiGianDuKien DATETIME NULL, -- Thời gian dự kiến hoàn thành
    
    -- Khóa ngoại
    FOREIGN KEY (MaDonHang) REFERENCES DONHANG(MaDonHang) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MENUITEMS(MaMon),
    FOREIGN KEY (MaNhanVienCheBien) REFERENCES NGUOIDUNG(MaNguoiDung)
);

-- Tạo index cho hiệu năng
CREATE INDEX IX_CHITIET_DONHANG_TrangThai ON CHITIET_DONHANG(TrangThai);
CREATE INDEX IX_CHITIET_DONHANG_MaDonHang ON CHITIET_DONHANG(MaDonHang);

