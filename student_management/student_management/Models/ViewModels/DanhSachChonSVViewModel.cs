using System.Collections.Generic;

namespace student_management.Models.ViewModels
{
    public class DanhSachChonSVViewModel
    {
        public int MaLHP { get; set; }
        public List<SinhVienChonItem> SinhViens { get; set; } = new();
    }

    public class SinhVienChonItem
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public bool DaChon { get; set; }
    }
}
