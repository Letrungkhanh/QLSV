using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class TaiKhoan
{
    public int Id { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int MaVaiTro { get; set; }

    public string? MaGv { get; set; }

    public string? MaSv { get; set; }

    public bool TrangThai { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual GiaoVien? MaGvNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
