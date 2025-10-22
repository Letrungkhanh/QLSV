using System.Collections.Generic;

namespace student_management.Models.ViewModels
{
    public class NhapDiemViewModel
    {
        public int MaLHP { get; set; }
        public string TenLHP { get; set; } = "";
        public List<SinhVienDiemItem> SinhViens { get; set; } = new();
    }

    public class SinhVienDiemItem
    {
        public string MaSV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public double? DiemChuyenCan { get; set; }
        public double? DiemGiuaKy { get; set; }
        public double? DiemCuoiKy { get; set; }
        public double? DiemTongKet { get; set; }
    }
}
