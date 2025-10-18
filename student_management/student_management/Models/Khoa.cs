using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class Khoa
{
    public string MaKhoa { get; set; } = null!;

    public string TenKhoa { get; set; } = null!;

    public string? MaVien { get; set; }

    public virtual ICollection<GiaoVien> GiaoViens { get; set; } = new List<GiaoVien>();

    public virtual ICollection<LopChinhQuy> LopChinhQuies { get; set; } = new List<LopChinhQuy>();

    public virtual Vien? MaVienNavigation { get; set; }

    public virtual ICollection<MonHoc> MonHocs { get; set; } = new List<MonHoc>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
