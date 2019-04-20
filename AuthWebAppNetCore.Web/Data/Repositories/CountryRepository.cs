using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Data.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext context;

        public CountryRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public async Task AddCityAsync(CityViewModel model)
        {
            var country = await GetCountryWithCitiesAsync(model.CountryId);
            if (country == null)
            {
                return;
            }

            country.Cities.Add(new City { Name = model.Name });
            context.Countries.Update(country);
            await context.SaveChangesAsync();
        }

        public async Task<int> DeleteCityAsync(City city)
        {
            var country = await context.Countries.Where(c => c.Cities.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();
            if (country == null)
            {
                return 0;
            }

            context.Cities.Remove(city);
            await context.SaveChangesAsync();
            return country.Id;
        }

        public IQueryable GetCountriesWithCities()
        {
            return context.Countries
                .Include(c => c.Cities)
                .OrderBy(c => c.Name);
        }

        public async Task<Country> GetCountryWithCitiesAsync(int id)
        {
            return await context.Countries
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await context.Countries.Where(c => c.Cities.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();
            if (country == null)
            {
                return 0;
            }

            context.Cities.Update(city);
            await context.SaveChangesAsync();
            return country.Id;
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await context.Cities.FindAsync(id);
        }

        public IEnumerable<SelectListItem> GetComboCountries()
        {
            var list = context.Countries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).OrderBy(l => l.Text).ToList();

            // Al iniciar nos aparecera esto en el ComboBox y tiene que ser el 0 en value
            // ya que el rango configurado para country es desde 1
            list.Insert(0, new SelectListItem
            {
                Text = "(Select a country...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboCities(int conuntryId)
        {
            var country = context.Countries.Find(conuntryId);
            var list = new List<SelectListItem>();
            if (country != null)
            {
                list = country.Cities.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).OrderBy(l => l.Text).ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a city...)",
                Value = "0"
            });

            return list;
        }

        public async Task<Country> GetCountryAsync(City city)
        {
            return await context.Countries
                .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
                .Take(1)
                .FirstOrDefaultAsync();
        }
    }
}
