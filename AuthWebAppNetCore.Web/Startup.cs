using AuthWebAppNetCore.Web.Data;
using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Data.Repositories;
using AuthWebAppNetCore.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthWebAppNetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// Password Configuration
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true; // confirmar email con codigo enviado a este
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequiredLength = 6;
            })
            .AddDefaultTokenProviders() // For Token configuration
            .AddEntityFrameworkStores<DataContext>();
            /////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// DB Connection
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            /////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// Token Authentication
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                });
            /////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// Classes ingections
            // Ingection de un solo uso
            services.AddTransient<SeedDb>();

            // Ingection de uso guardado en memoria
            services.AddScoped<IUserHelper, UserHelper>(); // Helper de uso de usuarios
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IMailHelper, MailHelper>();
            /////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// Redirecctions
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized";
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });
            /////////////////////////////////////////////////////////////////////////////////////



            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            /////////////////////////////////////////////////////////////////////////////////////
            app.UseStatusCodePagesWithReExecute("/error/{0}"); // Diferent pages for diferent errors (404, 500..)
            /////////////////////////////////////////////////////////////////////////////////////

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            /////////////////////////////////////////////////////////////////////////////////////
            app.UseAuthentication(); // App require authentification
            /////////////////////////////////////////////////////////////////////////////////////

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
