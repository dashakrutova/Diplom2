using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models.Database
{
    public class ResetPasswordModel
    {
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
