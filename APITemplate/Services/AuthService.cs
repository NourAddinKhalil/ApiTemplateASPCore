using APITemplate.Constants;
using APITemplate.DBModels;
using APITemplate.Helpers;
using APITemplate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APITemplate.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var auth = new AuthModel();

            var isEmail = IsEmail(model.UserName);

            ApplicationUser user;


            if (isEmail)
            {
                user = await _userManager.FindByEmailAsync(model.UserName);

                if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    throw new CustomAPIExceptionHelper("Invalid Email Or Password!.", StatusCodes.Status400BadRequest);
                }
            } 
            else
            {
                user = await _userManager.FindByNameAsync(model.UserName);

                if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    throw new CustomAPIExceptionHelper("Invalid UserName Or Password!.", StatusCodes.Status400BadRequest);
                }
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            auth.Email = user.Email;
            auth.ExpiresOn = jwtSecurityToken.ValidTo;
            auth.IsAuthenticated = true;
            auth.Roles = userRoles.ToList();
            auth.UserName = user.UserName;
            auth.FullName = user.FullName;
            auth.Id = user.Id;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return auth;
        }

        private static bool IsEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            var auth = new AuthModel();

            if (await IsEmailFound(model.Email))
            {
                throw new CustomAPIExceptionHelper("Email is Already Exisit!.", StatusCodes.Status400BadRequest);
            }

            if (await IsUserNameFound(model.UserName))
            {
                throw new CustomAPIExceptionHelper("UserName is Already Exisit!.", StatusCodes.Status400BadRequest);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
            };

            var results = await _userManager.CreateAsync(user, model.Password);

            if (!results.Succeeded)
            {
                var message = "";
                foreach (var error in results.Errors)
                {
                    message += error.Description + ", ";
                }
                // Same as Substring(0, auth.Message.Length - 3)
                message = message[0..^3];
                throw new CustomAPIExceptionHelper($"Somthing Went wrong!\n{message}", StatusCodes.Status500InternalServerError);
            }

            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);

            auth.Email = user.Email;
            auth.ExpiresOn = jwtSecurityToken.ValidTo;
            auth.IsAuthenticated = true;
            auth.Roles = new List<string> { "User" };
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            auth.UserName = user.UserName;
            auth.FullName = user.FullName;
            auth.Id = user.Id;

            return auth;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var claims = new []
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(_jwt.DurationInDays)).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);
            
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        private async Task<bool> IsEmailFound(string email)
        {
            bool output = await _userManager.FindByEmailAsync(email) is not null;

            return output;
        }

        private async Task<bool> IsUserNameFound(string username)
        {
            bool output = await _userManager.FindByNameAsync(username) is not null;

            return output;
        } 

        public async Task<ApplicationUser> AssginRoleAsync(UserRoles role, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            var isRoleExists = await _roleManager.RoleExistsAsync(role.ToString());

            if (user == null || !isRoleExists)
            {
                throw new CustomAPIExceptionHelper("Invalid User Or Role!.", StatusCodes.Status400BadRequest);
            }

            if (await _userManager.IsInRoleAsync(user, role.ToString()))
            {
                throw new CustomAPIExceptionHelper("User Already has this role!.", StatusCodes.Status208AlreadyReported);
            }

            var results = await _userManager.AddToRoleAsync(user, role.ToString());

            if (!results.Succeeded)
            {
                var message = "";
                foreach (var error in results.Errors)
                {
                    message += error.Description + ", ";
                }
                throw new CustomAPIExceptionHelper($"Somthing Went wrong!\n{message}", StatusCodes.Status500InternalServerError);
            }

            return user;
        }

        public async Task<ApplicationUser> RevokeRoleAsync(List<UserRoles> roles, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                throw new CustomAPIExceptionHelper("Invalid User!.", StatusCodes.Status400BadRequest);
            }

            if (roles.Count == 0)
            {
                throw new CustomAPIExceptionHelper("Must Provide Roles!.", StatusCodes.Status400BadRequest);
            }

            var rolesString = roles.Select(x => x.ToString()).ToArray();

            foreach (var role in rolesString)
            {
                var isRoleExists = await _roleManager.RoleExistsAsync(role);

                if (!isRoleExists)
                {
                    throw new CustomAPIExceptionHelper($"Invalid Role {role}!.", StatusCodes.Status400BadRequest);
                }
            }

            var results = await _userManager.RemoveFromRolesAsync(user, rolesString);

            if (!results.Succeeded)
            {
                var message = "";
                foreach (var error in results.Errors)
                {
                    message += error.Description + ", ";
                }
                throw new CustomAPIExceptionHelper($"Somthing Went wrong!\n{message}", StatusCodes.Status500InternalServerError);
            }

            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null)
            {
                throw new CustomAPIExceptionHelper("Invalid User!.", StatusCodes.Status400BadRequest);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles is null || userRoles.Count <= 0)
            {
                throw new CustomAPIExceptionHelper("This User does not have roles yet!.", StatusCodes.Status404NotFound);
            }

            return userRoles.ToList();
        }
    }
}
