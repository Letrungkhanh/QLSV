using System.Collections.Generic;

namespace student_management.Models.ViewModels
{
    public class DkiHocIndexViewModel
    {
        public string TenKhoa { get; set; } = "";
        public List<LopHocPhanViewModel> LopChuaHoc { get; set; } = new();
        public List<LopHocPhanViewModel> LopDangHoc { get; set; } = new();
        public List<LopHocPhanViewModel> LopHoanThanh { get; set; } = new();
    }
}
