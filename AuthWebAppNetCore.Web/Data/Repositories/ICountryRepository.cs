using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Data.Repositories
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        IQueryable GetCountriesWithCities();

        Task<Country> GetCountryWithCitiesAsync(int id);

        Task<City> GetCityAsync(int id);

        Task AddCityAsync(CityViewModel model);

        Task<int> UpdateCityAsync(City city);

        Task<int> DeleteCityAsync(City city);

        IEnumerable<SelectListItem> GetComboCountries();

        /// <summary>
        /// Cities que pertenecen a cierto pais
        /// </summary>
        /// <param name="conuntryId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetComboCities(int conuntryId);

        /// <summary>
        /// Las relaciones en EF funcionan con compatibilidad para bases de datos no relacionales,
        /// entoces por ejemplo, <see cref="City"/> no tiene relacion con <see cref="Country"/>, pero
        /// <see cref="Country"/> tiene relaciones con <see cref="City"/>.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        Task<Country> GetCountryAsync(City city);
    }
}
