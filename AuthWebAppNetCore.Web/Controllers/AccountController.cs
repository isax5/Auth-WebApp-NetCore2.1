using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Data.Repositories;
using AuthWebAppNetCore.Web.Helpers;
using AuthWebAppNetCore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// For Url.Action
        /// </summary>
        public static string RouteController = "Account";

        private readonly IUserHelper userHelper;
        private readonly IMailHelper mailHelper;
        private readonly ICountryRepository countryRepository;
        private readonly IConfiguration configuration;

        public AccountController(
            IUserHelper userHelper,
            IMailHelper mailHelper,
            ICountryRepository countryRepository,
            IConfiguration configuration) // Access to Configuration file (appsettings.json)
        {
            this.userHelper = userHelper;
            this.mailHelper = mailHelper;
            this.countryRepository = countryRepository;
            this.configuration = configuration;
        }

        /// <summary>
        /// GET Login User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// POST Login User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result =
                    await userHelper.LoginAsync(model.Username, model.Password, model.RememberMe);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        // Direccion que te manda a login, por ejemplo el intento de ver algo solo para usuarios con login
                        // https://localhost:44344/Account/Login?ReturnUrl=%2FProducts
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to login.");
            return View(model);
        }

        /// <summary>
        /// Logout web User
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// GET for create new account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterNewUserViewModel
            {
                Countries = countryRepository.GetComboCountries(),
                Cities = countryRepository.GetComboCities(0)
            };

            return View(model);
        }

        /// <summary>
        /// POST for create a new account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    var city = await countryRepository.GetCityAsync(model.CityId);

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        City = city
                    };

                    var result = await userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    var myToken = await userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = Url.Action(nameof(ConfirmEmail), AccountController.RouteController, new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    mailHelper.SendMail(model.Username, "Auth-Web-App Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");
                    ViewBag.Message = "The instructions to allow your user has been sent to email.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "The username is already registered.");
            }

            return View(model);
        }

        /// <summary>
        /// Confirm Email, this is the link that the user received in the email when he create a new account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        /// <summary>
        /// GET for Change user preferences
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ChangeUser()
        {
            var user = await userHelper.GetUserByEmailAsync(User.Identity.Name);
            var model = new ChangeUserViewModel();

            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Address = user.Address;
            model.PhoneNumber = user.PhoneNumber;

            if (user != null) // this is not necesary
            {
                if (user.City != null)
                {
                    var country = await countryRepository.GetCountryAsync(user.City);
                    if (country != null)
                    {
                        model.CountryId = country.Id;
                        model.Cities = countryRepository.GetComboCities(country.Id);
                        model.Countries = countryRepository.GetComboCountries();
                        model.CityId = user.City.Id;
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// POST for Change user preferences
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    var city = await countryRepository.GetCityAsync(model.CityId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.City = city;

                    var respose = await userHelper.UpdateUserAsync(user);
                    if (respose.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, respose.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }

        /// <summary>
        /// List of Cities using AJAX for <see cref="Register"/> and <see cref="ChangeUser"/>
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var country = await countryRepository.GetCountryWithCitiesAsync(countryId);
            return Json(country.Cities.OrderBy(c => c.Name));
        }

        /// <summary>
        /// GET for change password
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// POST for change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    IdentityResult result = await userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(ChangeUser));
                        //return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }

        #region API Methods
        /// <summary>
        /// Create a TOKEN for user from API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            // Dont forget "[FromBody]"
            if (ModelState.IsValid)
            {
                User user = await userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        Claim[] claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        // Access to appsettings.json // GroupName:SubGroupName:...
                        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
                        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        JwtSecurityToken token = new JwtSecurityToken(
                            configuration["Tokens:Issuer"],
                            configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddYears(10),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest(); // Respuesta 400
        }
        #endregion

        #region Recover Password
        /// <summary>
        /// GET for Recover password using Email
        /// </summary>
        /// <returns></returns>
        public IActionResult RecoverPassword()
        {
            return View();
        }

        /// <summary>
        /// POST for Recover password using Email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action(
                    nameof(ResetPassword),
                    AccountController.RouteController,
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                mailHelper.SendMail(model.Email, "Auth-Web-App Password Reset", $"<h1>Shop Password Reset</h1>" +
                    $"To reset the password click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");
                ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return View();

            }

            return View(model);
        }

        /// <summary>
        /// GET for Reset Password after <see cref="RecoverPassword"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        /// <summary>
        /// POST for Reset Password after <see cref="RecoverPassword"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successful.";
                    return View();
                }

                ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }
        #endregion

        #region Users management
        /// <summary>
        /// List of users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            var users = await userHelper.GetAllUsersAsync();
            return View(users);
        }

        /// <summary>
        /// Put user like <see cref="Roles.Admin"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AdminOff(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await userHelper.RemoveUserFromRoleAsync(user, Roles.Admin);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Put user like not <see cref="Roles.Admin"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AdminOn(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await userHelper.AddUserToRoleAsync(user, Roles.Admin);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await userHelper.DeleteUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        #endregion
        /// <summary>
        /// NOT AUTHORIZED web view
        /// </summary>
        /// <returns></returns>
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}
