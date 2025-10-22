namespace student_management.Models.ViewModels
{
    public class ThongKeDiemDanhViewModel
    {
        public int MaLHP { get; set; }
        public List<ThongKeDiemDanhItem> DanhSach { get; set; } = new();
    }

    public class ThongKeDiemDanhItem
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public int SoBuoiCoMat { get; set; }
        public int SoBuoiVang { get; set; }
    }
}
