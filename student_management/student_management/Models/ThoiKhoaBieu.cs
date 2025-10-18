using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class ThoiKhoaBieu
{
    public int Id { get; set; }

    public int MaLhp { get; set; }

    public int Thu { get; set; }

    public int TietBatDau { get; set; }

    public int SoTiet { get; set; }

    public string? PhongHoc { get; set; }

    public virtual LopHocPhan MaLhpNavigation { get; set; } = null!;
}
