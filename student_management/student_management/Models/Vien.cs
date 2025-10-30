using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class Vien
    {
        [Key]
        [Display(Name = "Mã viện")]
        [Required(ErrorMessage = "Vui lòng nhập mã viện")]
        public string MaVien { get; set; } = null!;

        [Display(Name = "Tên viện")]
        [Required(ErrorMessage = "Vui lòng nhập tên viện")]
        public string TenVien { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Danh sách khoa thuộc viện")]
        public virtual ICollection<Khoa> Khoas { get; set; } = new List<Khoa>();
    }
}
