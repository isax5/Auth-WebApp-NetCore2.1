using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Data.Repositories;
using AuthWebAppNetCore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Controllers
{
    /// <summary>
    /// Controlador para <see cref="Country"/> y <see cref="City"/>
    /// </summary>
    public class CountriesController : Controller
    {
        private readonly ICountryRepository countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            var countryId = await countryRepository.DeleteCityAsync(city);
            return RedirectToAction($"{nameof(Details)}/{countryId}");
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                var countryId = await countryRepository.UpdateCityAsync(city);
                if (countryId != 0)
                {
                    return RedirectToAction($"{nameof(Details)}/{countryId}");
                }
            }

            return View(city);
        }

        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            var model = new CityViewModel { CountryId = country.Id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                await countryRepository.AddCityAsync(model);
                return RedirectToAction($"{nameof(Details)}/{model.CountryId}");
            }

            return View(model);
        }

        public IActionResult Index()
        {
            return View(countryRepository.GetCountriesWithCities());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await countryRepository.GetCountryWithCitiesAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                await countryRepository.CreateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            await countryRepository.DeleteAsync(country);
            return RedirectToAction(nameof(Index));
        }
    }
}
