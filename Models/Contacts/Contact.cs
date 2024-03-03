using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Migration_EF.Models.Contacts
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Required(ErrorMessage = "{0}'s required")]
        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "{0}'s required")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Phải là địa chỉ email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public DateTime DateSent { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [StringLength(50)]
        [Phone(ErrorMessage = "Your Phone isn't correct")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
    }
}
