using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace student_management.Models
{
    public partial class SinhVien
    {
        [Key]
        [Display(Name = "Mã sinh viên")]
        [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
        public string MaSv { get; set; } = null!;

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ và tên sinh viên")]
        public string HoTen { get; set; } = null!;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateOnly? NgaySinh { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Lớp chính quy")]
        [Required(ErrorMessage = "Vui lòng chọn lớp chính quy")]
        public string? MaLop { get; set; }

        [Display(Name = "Khoa trực thuộc")]
        [Required(ErrorMessage = "Vui lòng chọn khoa trực thuộc")]
        public string? MaKhoa { get; set; }

        [Display(Name = "Năm nhập học")]
        [Range(2000, 2100, ErrorMessage = "Năm nhập học không hợp lệ (2000 - 2100)")]
        public int? NamNhapHoc { get; set; }

        [Display(Name = "Ảnh sinh viên")]
        public string? Anh { get; set; }

        // 🔹 Quan hệ
        [Display(Name = "Danh sách đăng ký học")]
        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

        [Display(Name = "Khoa trực thuộc")]
        public virtual Khoa? MaKhoaNavigation { get; set; }

        [Display(Name = "Lớp chính quy")]
        public virtual LopChinhQuy? MaLopNavigation { get; set; }

        [Display(Name = "Tài khoản sinh viên")]
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    }
}
