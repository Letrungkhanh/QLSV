using System;
using System.Collections.Generic;

namespace student_management.Models;

public partial class DangKyHoc
{
    public string MaSv { get; set; } = null!;

    public int MaLhp { get; set; }

    public DateTime NgayDangKy { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual BangDiem? BangDiem { get; set; }

    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public virtual LopHocPhan MaLhpNavigation { get; set; } = null!;

    public virtual SinhVien MaSvNavigation { get; set; } = null!;

    public string? KetQua { get; set; } // "Hoàn thành", "Không đạt", null (chưa kết thúc)

}
