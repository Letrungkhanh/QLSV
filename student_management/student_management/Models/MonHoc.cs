using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class MonHoc
{
    public string MaMh { get; set; } = null!;

    public string TenMh { get; set; } = null!;

    public int SoTinChi { get; set; }
    public string? Anh { get; set; }

    public string? MaKhoa { get; set; }

    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();

    public virtual Khoa? MaKhoaNavigation { get; set; }
}
