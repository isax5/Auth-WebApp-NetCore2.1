using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthWebAppNetCore.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }

        /// <summary>
        /// City of the user
        /// </summary>
        public City City { get; set; }

        [NotMapped] // Lectura y escritura que No se va a DB
        [Display(Name = "Is Admin?")]
        public bool IsAdmin { get; set; }

        // Property Display personalized
        [Display(Name = "Phone Number")]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

        [Display(Name = "Email Confirmed")]
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }
    }

    /// <summary>
    /// Type of user, you can create how many you want
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
    }
}
