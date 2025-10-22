using System.Collections.Generic;

namespace student_management.Models.ViewModels
{
    public class LopHocPhanViewModel
    {
        public LopHocPhan LopHocPhan { get; set; }           // Thông tin lớp học phần
        public IEnumerable<SinhVien> SinhViens { get; set; } // Danh sách sinh viên
    }
}
