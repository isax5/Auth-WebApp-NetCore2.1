using System.ComponentModel.DataAnnotations;

namespace AuthWebAppNetCore.Web.Data.Entities
{
    public class City : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Name { get; set; }
    }
}
