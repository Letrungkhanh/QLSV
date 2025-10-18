using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class Vien
{
    public string MaVien { get; set; } = null!;

    public string TenVien { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<Khoa> Khoas { get; set; } = new List<Khoa>();
}
