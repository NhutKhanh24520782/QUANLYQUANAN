USE restaurant; -- Đảm bảo đang chọn đúng DB
GO

-- 1. Xóa bảng cũ nếu nó đã tồn tại
IF OBJECT_ID('dbo.THANHTOAN', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.THANHTOAN;
END
GO

-- 2. Tạo lại bảng với cấu trúc mới của bạn
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
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangXuLy' 
        CHECK (TrangThai IN (N'DangXuLy', N'ThanhCong', N'ThatBai', N'Huy')),
    
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
GO