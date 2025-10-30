using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class ThongBao
    {
        [Key]
        [Display(Name = "Mã thông báo")]
        public int MaTb { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề thông báo")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string TieuDe { get; set; } = null!;

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung thông báo")]
        [DataType(DataType.MultilineText)]
        public string NoiDung { get; set; } = null!;

        [Display(Name = "Ngày đăng")]
        [DataType(DataType.DateTime)]
        public DateTime NgayDang { get; set; } = DateTime.Now;

        [Display(Name = "Giảng viên đăng thông báo")]
        [Required(ErrorMessage = "Vui lòng chọn giảng viên đăng thông báo")]
        public string MaGv { get; set; } = null!;

        [Display(Name = "Lớp học phần liên quan")]
        public int? MaLhp { get; set; }

        // 🔹 Navigation Properties
        [Display(Name = "Giảng viên đăng thông báo")]
        public virtual GiaoVien MaGvNavigation { get; set; } = null!;

        [Display(Name = "Lớp học phần liên quan")]
        public virtual LopHocPhan? MaLhpNavigation { get; set; }
    }
}
