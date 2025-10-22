using System.Collections.Generic;

namespace student_management.Models.ViewModels
{
    public class DiemDanhItem
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string TrangThai { get; set; } = "Có mặt";
    }

    public class DiemDanhViewModel
    {
        public int MaLHP { get; set; }
        public string TenLHP { get; set; }
        public List<DiemDanhItem> SinhViens { get; set; } = new List<DiemDanhItem>();
    }
}
