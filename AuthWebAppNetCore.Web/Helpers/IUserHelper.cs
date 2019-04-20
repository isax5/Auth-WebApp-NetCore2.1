using AuthWebAppNetCore.Web.Data.Entities;
using AuthWebAppNetCore.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthWebAppNetCore.Web.Helpers
{
    public interface IUserHelper
    {
        /// <summary>
        /// Necesary for controllers and other thinks
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Add new user, it need <see cref="GenerateEmailConfirmationTokenAsync(User)"/> and <see cref="ConfirmEmailAsync(User, string)"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> AddUserAsync(User user, string password);

        /// <summary>
        /// Login User. this function need <see cref="LoginViewModel.Username"/> and <see cref="LoginViewModel.Password"/> to workd
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<SignInResult> LoginAsync(string userName, string password, bool rememberMe);

        /// <summary>
        /// Logout 
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync();

        /// <summary>
        /// Update the user properties
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IdentityResult> UpdateUserAsync(User user);

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        /// <summary>
        /// Check if the exist a role or create if it does not exist
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task CheckRoleAsync(string roleName);

        /// <summary>
        /// Check if the user and password is correct, this is for API. It need User and Password to work
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        /// <summary>
        /// Add user to Role. Check <see cref="Roles"/> and <see cref="CheckRoleAsync(string)"/>.
        /// Opposite to <see cref="RemoveUserFromRoleAsync(User, string)"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task AddUserToRoleAsync(User user, string roleName);

        /// <summary>
        /// True or false if the user has some Role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> IsUserInRoleAsync(User user, string roleName);

        #region Tokens and emails mnagement
        /// <summary>
        /// Generates an email confirmation token for the specified user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        /// <summary>
        /// Validates that an email confirmation token matches the specified user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        /// <summary>
        /// Get User by user's  Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(string userId);
        #endregion

        #region Password reset management
        /// <summary>
        ///  Generates a password reset token for the specified user, using the configured
        ///  password reset token provider
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GeneratePasswordResetTokenAsync(User user);

        /// <summary>
        /// Resets the user's password to the specified newPassword after validating the
        /// given password reset token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
        #endregion

        #region For Users management
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Remove user from role. Opposite to <see cref="AddUserToRoleAsync(User, string)"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task RemoveUserFromRoleAsync(User user, string roleName);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task DeleteUserAsync(User user);
        #endregion
    }
}
