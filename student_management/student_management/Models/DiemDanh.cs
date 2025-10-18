using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class DiemDanh
{
    public int Id { get; set; }

    public string MaSv { get; set; } = null!;

    public int MaLhp { get; set; }

    public DateOnly NgayDiemDanh { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual DangKyHoc DangKyHoc { get; set; } = null!;
}
