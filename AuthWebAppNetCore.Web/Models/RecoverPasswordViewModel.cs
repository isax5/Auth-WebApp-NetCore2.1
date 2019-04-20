using System.ComponentModel.DataAnnotations;

namespace AuthWebAppNetCore.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
