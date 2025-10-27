using System;

namespace student_management.Models.ViewModels
{
    public class LichSuDiemDanhViewModel
    {
        public int MaLhp { get; set; }
        public string TenMonHoc { get; set; } = string.Empty;
        public DateOnly NgayDiemDanh { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }
}
