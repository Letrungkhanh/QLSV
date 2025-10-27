using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class GiaoVien
{
    public string MaGv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? MaKhoa { get; set; }
    public string? Anh { get; set; }


    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();

    public virtual Khoa? MaKhoaNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
