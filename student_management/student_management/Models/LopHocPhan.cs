using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class LopHocPhan
{
    public int MaLhp { get; set; }

    public string? TenLhp { get; set; }

    public string MaMh { get; set; } = null!;

    public string? MaGv { get; set; }

    public int HocKy { get; set; }

    public string NamHoc { get; set; } = null!;

    public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    public virtual GiaoVien? MaGvNavigation { get; set; }

    public virtual MonHoc MaMhNavigation { get; set; } = null!;

    public virtual ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = new List<ThoiKhoaBieu>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
