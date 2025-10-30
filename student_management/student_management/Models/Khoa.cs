using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class Khoa
    {
        [Key]
        [Display(Name = "Mã khoa")]
        [Required(ErrorMessage = "Vui lòng nhập mã khoa")]
        public string MaKhoa { get; set; } = null!;

        [Display(Name = "Tên khoa")]
        [Required(ErrorMessage = "Vui lòng nhập tên khoa")]
        public string TenKhoa { get; set; } = null!;

        [Display(Name = "Thuộc viện")]
        [Required(ErrorMessage = "Vui lòng chọn viện trực thuộc")]
        public string? MaVien { get; set; }

        [Display(Name = "Danh sách giảng viên")]
        public virtual ICollection<GiaoVien> GiaoViens { get; set; } = new List<GiaoVien>();

        [Display(Name = "Danh sách lớp chính quy")]
        public virtual ICollection<LopChinhQuy> LopChinhQuies { get; set; } = new List<LopChinhQuy>();

        [Display(Name = "Viện trực thuộc")]
        public virtual Vien? MaVienNavigation { get; set; }

        [Display(Name = "Danh sách môn học")]
        public virtual ICollection<MonHoc> MonHocs { get; set; } = new List<MonHoc>();

        [Display(Name = "Danh sách sinh viên")]
        public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
    }
}
