using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class MonHoc
    {
        [Key]
        [Display(Name = "Mã môn học")]
        [Required(ErrorMessage = "Vui lòng nhập mã môn học")]
        public string MaMh { get; set; } = null!;

        [Display(Name = "Tên môn học")]
        [Required(ErrorMessage = "Vui lòng nhập tên môn học")]
        public string TenMh { get; set; } = null!;

        [Display(Name = "Số tín chỉ")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải nằm trong khoảng từ 1 đến 10")]
        [Required(ErrorMessage = "Vui lòng nhập số tín chỉ của môn học")]
        public int SoTinChi { get; set; }

        [Display(Name = "Ảnh minh họa")]
        public string? Anh { get; set; }

        [Display(Name = "Khoa phụ trách")]
        [Required(ErrorMessage = "Vui lòng chọn khoa phụ trách")]
        public string? MaKhoa { get; set; }

        // 🔹 Quan hệ với các bảng khác
        [Display(Name = "Danh sách lớp học phần")]
        public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();

        [Display(Name = "Khoa phụ trách")]
        public virtual Khoa? MaKhoaNavigation { get; set; }
    }
}
