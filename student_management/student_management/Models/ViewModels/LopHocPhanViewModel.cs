using System;

namespace student_management.Models.ViewModels
{
    public class LopHocPhanViewModel
    {
        public int MaLhp { get; set; }
        public string TenLhp { get; set; } = "";
        public string GiangVien { get; set; } = "";
        public int SiSoToiDa { get; set; }
        public int SiSoHienTai { get; set; }
        public string TrangThai { get; set; } = "";
        public string TrangThaiDangKy { get; set; } = "Chưa đăng ký";

        // Thông tin môn học
        public string TenMonHoc { get; set; } = "";
        public string MaMh { get; set; } = "";
        public int SoTinChi { get; set; }

        // Điểm tổng kết (nếu có)
        public decimal? DiemTongKet { get; set; } = null;
    }
}
