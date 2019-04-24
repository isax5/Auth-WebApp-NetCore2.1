using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
        }

        /// <summary>
        /// Feed DB in first execution
        /// </summary>
        /// <returns></returns>
        public async Task SeedAsync()
        {
            // Check DB
            await context.Database.EnsureCreatedAsync();
            await CheckRoles();

            // Add Data

            if (!context.Countries.Any())
            {
                await AddCountriesAndCitiesAsync();
            }

            // Users for testing
            var user = await CheckUserAsync("admin@gmail.com", "Isaac", "Rebolledo", Roles.Admin);
            await CheckUserAsync("brad@gmail.com", "Brad", "Pit", Roles.Customer);
            await CheckUserAsync("angelina@gmail.com", "Angelina", "Jolie", Roles.Customer);
        }

        private async Task<User> CheckUserAsync(string userName, string firstName, string lastName, string role)
        {
            // Add user
            var user = await userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                user = await AddUser(userName, firstName, lastName, role);
            }

            var isInRole = await userHelper.IsUserInRoleAsync(user, role);
            if (!isInRole)
            {
                await userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private async Task<User> AddUser(string userName, string firstName, string lastName, string role)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = userName,
                UserName = userName,
                Address = "Calle Luna Calle Sol",
                PhoneNumber = "350 634 2747",
                City = context.Countries.FirstOrDefault().Cities.FirstOrDefault()
            };

            var result = await userHelper.AddUserAsync(user, "123456");
            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Could not create the user in seeder");
            }

            await userHelper.AddUserToRoleAsync(user, role);
            var token = await userHelper.GenerateEmailConfirmationTokenAsync(user);
            await userHelper.ConfirmEmailAsync(user, token);
            return user;
        }

        private async Task AddCountriesAndCitiesAsync()
        {
            AddCountry("Argentina", new string[] { "Córdoba", "Buenos Aires", "Rosario", "Tandil", "Salta", "Mendoza" });
            AddCountry("Bolivia", new string[] { "La Paz", "Sucre", "Potosi", "Cochabamba" });
            AddCountry("Brasil", new string[] { "Rio de Janeiro", "São Paulo", "Salvador", "Porto Alegre", "Curitiba", "Recife", "Belo Horizonte", "Fortaleza" });
            AddCountry("Chile", new string[] { "Santiago", "Valdivia", "Concepción", "Puerto Montt", "Temuco", "La Serena", "Chillán" });
            AddCountry("Colombia", new string[] { "Medellín", "Bogota", "Calí", "Barranquilla", "Bucaramanga", "Cartagena", "Pereira" });
            AddCountry("Ecuador", new string[] { "Quito", "Guayaquil", "Ambato", "Manta", "Loja", "Santo" });
            AddCountry("Estados Unidos", new string[] { "New York", "Los Ángeles", "Chicago", "Washington", "San Francisco", "Miami", "Boston" });
            AddCountry("Panamá", new string[] { "Chitré", "Santiago", "La Arena", "Agua Dulce", "Monagrillo", "Ciudad de Panamá", "Colón", "Los Santos" });
            AddCountry("Paraguay", new string[] { "Asunción", "Ciudad del Este", "Encarnación", "San  Lorenzo", "Luque", "Areguá" });
            AddCountry("Peru", new string[] { "Lima", "Arequipa", "Cusco", "Trujillo", "Chiclayo", "Iquitos" });
            AddCountry("México", new string[] { "Guadalajara", "Ciudad de México", "Monterrey", "Ciudad Obregón", "Hermosillo", "La Paz", "Culiacán", "Los Mochis" });
            AddCountry("Uruguay", new string[] { "Montevideo", "Punta del Este", "Colonia del Sacramento", "Las Piedras" });
            AddCountry("Venezuela", new string[] { "Caracas", "Valencia", "Maracaibo", "Ciudad Bolivar", "Maracay", "Barquisimeto" });
            await context.SaveChangesAsync();
        }

        private void AddCountry(string country, string[] cities)
        {
            var theCities = cities.Select(c => new City { Name = c }).ToList();
            context.Countries.Add(new Country
            {
                Cities = theCities,
                Name = country
            });
        }

        private async Task CheckRoles()
        {
            await userHelper.CheckRoleAsync(Roles.Admin);
            await userHelper.CheckRoleAsync(Roles.Customer);
        }
    }
}
