using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserHelper(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await userManager.ConfirmEmailAsync(user, token);
        }

        public async Task DeleteUserAsync(User user)
        {
            await userManager.DeleteAsync(user);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await userManager.Users
                .Include(u => u.City)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            return users.Select(async u =>
            {
                if (u != null)
                    u.IsAdmin = await IsUserInRoleAsync(u, Roles.Admin);

                return u;
            })
            .Select(t => t.Result)
            .ToList();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            // This code does not charge City
            //return await userManager.FindByEmailAsync(email);

            return await userManager.Users
                .Where(u => u.Email.Equals(email))
                .Include(u => u.City)
                .Take(1)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            // This code does not charge City
            //return await userManager.FindByIdAsync(userId);

            return await userManager.Users
                .Where(u => u.Id.Equals(userId))
                .Include(u => u.City)
                .Take(1)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserInRoleAsync(User user, string nombreRol)
        {
            return await userManager.IsInRoleAsync(user, nombreRol);
        }

        public async Task<SignInResult> LoginAsync(string userName, string password, bool rememberMe)
        {
            return await signInManager.PasswordSignInAsync(
                userName,
                password,
                rememberMe,
                false);
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task RemoveUserFromRoleAsync(User user, string roleName)
        {
            await userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await signInManager.CheckPasswordSignInAsync(
                user,
                password,
                false); // Failure counter
        }
    }
}
