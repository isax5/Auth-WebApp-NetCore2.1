using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthWebAppNetCore.Web.Data.Entities
{
    public class Country : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }

        [Display(Name = "# Cities")]
        public int NumberCities => Cities == null ? 0 : Cities.Count;
    }
}
