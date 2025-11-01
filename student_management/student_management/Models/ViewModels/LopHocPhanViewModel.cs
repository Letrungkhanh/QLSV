using System;

namespace student_management.Models.ViewModels
{
    public class LopHocPhanViewModel
    {
        public int MaLhp { get; set; }
        public string TenLhp { get; set; } = "";
        public string GiangVien { get; set; } = "";
        public int SiSoToiDa { get; set; }          // sĩ số tối đa
        public int SiSoHienTai { get; set; }        // sĩ số hiện tại
        public string TrangThai { get; set; } = "";

        // ✅ Thêm dòng này để xác định trạng thái đăng ký của sinh viên
        public string TrangThaiDangKy { get; set; } = "Chưa đăng ký";
    }
}
