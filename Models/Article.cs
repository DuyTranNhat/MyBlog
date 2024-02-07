using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Migration_EF.Models
{
    public class Article
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(255)]
        public string Content { set; get; }
    }
}
