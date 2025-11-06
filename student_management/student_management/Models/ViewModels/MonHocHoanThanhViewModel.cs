namespace student_management.Models.ViewModels
{
    public class MonHocHoanThanhViewModel
    {
        public string MaMh { get; set; } = "";
        public string TenMh { get; set; } = "";
        public int SoTinChi { get; set; }
        public int LopDangMo { get; set; }
        public bool HoanThanh { get; set; } = false;
        public decimal Diem { get; set; } // Điểm tổng kết
        public bool VangQua30 { get; set; } // Vắng quá 30% hay không

        // Trạng thái đăng ký có thể được tái sử dụng ở đây nếu bạn muốn hiển thị chung
        public string TrangThaiDangKy { get; set; } = "";

        public List<(int MaLhp, string TenLhp)> LopHocPhanDaDangKy { get; set; } = new();


    }
}
