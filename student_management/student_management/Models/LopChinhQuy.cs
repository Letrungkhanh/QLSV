using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class LopChinhQuy
    {
        [Key]
        [Display(Name = "Mã lớp")]
        [Required(ErrorMessage = "Vui lòng nhập mã lớp chính quy")]
        public string MaLop { get; set; } = null!;

        [Display(Name = "Tên lớp")]
        [Required(ErrorMessage = "Vui lòng nhập tên lớp chính quy")]
        public string? TenLop { get; set; }

        [Display(Name = "Khoa trực thuộc")]
        [Required(ErrorMessage = "Vui lòng chọn khoa trực thuộc")]
        public string? MaKhoa { get; set; }

        // 🔹 Navigation Properties
        [Display(Name = "Khoa trực thuộc")]
        public virtual Khoa? MaKhoaNavigation { get; set; }

        [Display(Name = "Danh sách sinh viên trong lớp")]
        public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
    }
}
