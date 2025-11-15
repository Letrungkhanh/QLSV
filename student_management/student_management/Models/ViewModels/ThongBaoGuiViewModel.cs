using System.ComponentModel.DataAnnotations;

namespace student_management.Models.ViewModels
{
    public class ThongBaoGuiViewModel
    {
        public int MaLHP { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề thông báo")]
        [StringLength(200)]
        public string TieuDe { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập nội dung thông báo")]
        [DataType(DataType.MultilineText)]
        public string NoiDung { get; set; } = null!;
    }
}
