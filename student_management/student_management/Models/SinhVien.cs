using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class SinhVien
{
    public string MaSv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? MaLop { get; set; }

    public string? MaKhoa { get; set; }

    public int? NamNhapHoc { get; set; }
    public string? Anh { get; set; }


    public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    public virtual Khoa? MaKhoaNavigation { get; set; }

    public virtual LopChinhQuy? MaLopNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
