using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class BangDiem
{
    public string MaSv { get; set; } = null!;

    public int MaLhp { get; set; }

    public decimal? DiemChuyenCan { get; set; }

    public decimal? DiemGiuaKy { get; set; }

    public decimal? DiemCuoiKy { get; set; }

    public decimal? DiemTongKet { get; set; }

    public virtual DangKyHoc DangKyHoc { get; set; } = null!;
}
