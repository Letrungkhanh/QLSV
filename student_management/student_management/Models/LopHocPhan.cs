using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace student_management.Models
{
    [Table("LopHocPhan")]
    public partial class LopHocPhan
    {
        [Key]
        [Display(Name = "Mã lớp học phần")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLhp { get; set; }

        [Display(Name = "Tên lớp học phần")]
        [Required(ErrorMessage = "Vui lòng nhập tên lớp học phần")]
        public string? TenLhp { get; set; }

        [Display(Name = "Mã môn học")]
        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        public string MaMh { get; set; } = null!;

        [Display(Name = "Giảng viên phụ trách")]
        [Required(ErrorMessage = "Vui lòng chọn giảng viên phụ trách")]
        public string? MaGv { get; set; }

        [Display(Name = "Học kỳ")]
        [Range(1, 3, ErrorMessage = "Học kỳ phải nằm trong khoảng từ 1 đến 3")]
        public int HocKy { get; set; }

        [Display(Name = "Trạng thái lớp học phần")]
        public string TrangThai { get; set; } = "Chờ duyệt";

        [Display(Name = "Năm học")]
        [Required(ErrorMessage = "Vui lòng nhập năm học (VD: 2024-2025)")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Năm học phải có định dạng: 2024-2025")]
        public string NamHoc { get; set; } = null!;
        public int SiSoToiDa { get; set; }
        public int SiSoHienTai { get; set; } = 0;
        // 🔹 Navigation Properties
        [Display(Name = "Danh sách sinh viên đăng ký")]
        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

        [Display(Name = "Giảng viên phụ trách")]
        public virtual GiaoVien? MaGvNavigation { get; set; }

        [Display(Name = "Môn học")]
        public virtual MonHoc? MaMhNavigation { get; set; }


        [Display(Name = "Thời khóa biểu")]
        public virtual ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = new List<ThoiKhoaBieu>();

        [Display(Name = "Thông báo lớp học phần")]
        public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
    }
}
