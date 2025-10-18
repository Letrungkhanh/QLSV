using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class ThongBao
{
    public int MaTb { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime NgayDang { get; set; }

    public string MaGv { get; set; } = null!;

    public int? MaLhp { get; set; }

    public virtual GiaoVien MaGvNavigation { get; set; } = null!;

    public virtual LopHocPhan? MaLhpNavigation { get; set; }
}
