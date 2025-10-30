using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class GiaoVien
    {
        [Key]
        [Display(Name = "Mã giảng viên")]
        [Required(ErrorMessage = "Vui lòng nhập mã giảng viên")]
        public string MaGv { get; set; } = null!;

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên giảng viên")]
        public string HoTen { get; set; } = null!;

        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateOnly? NgaySinh { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Khoa trực thuộc")]
        [Required(ErrorMessage = "Vui lòng chọn khoa trực thuộc")]
        public string? MaKhoa { get; set; }

        [Display(Name = "Ảnh giảng viên")]
        public string? Anh { get; set; }

        // 🔹 Quan hệ với các bảng khác
        [Display(Name = "Danh sách lớp học phần giảng dạy")]
        public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();

        [Display(Name = "Khoa trực thuộc")]
        public virtual Khoa? MaKhoaNavigation { get; set; }

        [Display(Name = "Tài khoản giảng viên")]
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

        [Display(Name = "Thông báo của giảng viên")]
        public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
    }
}
