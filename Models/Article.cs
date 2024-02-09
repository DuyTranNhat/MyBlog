using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Migration_EF.Models
{
    public class Article
    {
        [Key]
        public int ID { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tiêu đề tối thiểu 3 ký tự và ít hơn 100 ký tự!")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} không được để trống!")]
        [DataType(DataType.Date)]
        [DisplayName("Ngày công chiếu")]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "{0} không được để trống!")]
        [Column(TypeName = "nvarchar")]
        [StringLength(255)]
        [DisplayName("Nội dung")]
        public string Content { set; get; }
    }
}
