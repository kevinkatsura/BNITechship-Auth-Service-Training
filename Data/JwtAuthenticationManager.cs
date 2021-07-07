using AlphaAuthService.Helpers;
using AlphaAuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAuthService.Data
{
    public class JwtAuthenticationManager : IUser
    {
        private AppSettings _appSettings;
        private UserManager<ApplicationUser> _userManager;
        private BNITechshipContext _db;

        public JwtAuthenticationManager(IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, BNITechshipContext db)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _db = db;
        }

        public async Task<User> GenerateToken(User user) {
            
            //MEMBUAT JWT TOKEN
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), /*DURASI TOKEN*/
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = string.Empty;
            user.FirstName = string.Empty;
            user.LastName = string.Empty;

            return user;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var userFind = await _userManager.CheckPasswordAsync(await _userManager.FindByNameAsync(username), password);
            if (!userFind)
                return null;

            var user = new User
            {
                UserName = username
            };

            return await GenerateToken(user);
        }

        
        public async Task DeleteAccount(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            var results = await _userManager.Users.ToListAsync();
            return results;
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            var result = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return result;
        }

        public async Task Registration(User user)
        {
            try
            {
                var validateUsername = await _userManager.FindByNameAsync(user.UserName);
                if (validateUsername != null)
                {
                    throw new Exception("Username telah digunakan.");
                }
                var _user = new ApplicationUser()
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                var result = await _userManager.CreateAsync(_user,user.Password);
                if(!result.Succeeded)
                {
                    throw new Exception("Gagal menambahkan data user");
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task UpdatePassword(string user, string CurrentPassword, string NewPassword, string ConfirmNewPassword)
        {
            try
            {
                var userFind = await _userManager.CheckPasswordAsync(await _userManager.FindByNameAsync(user), CurrentPassword);
                if (!userFind)
                {
                    throw new Exception("Current password tidak sesuai.");
                }
                if (!NewPassword.Equals(ConfirmNewPassword))
                {
                    throw new Exception("Password baru dengan konfirmasi password baru tidak sesuai.");
                }
                ApplicationUser tUser = await _db.Users.Where(u => u.UserName == user).FirstOrDefaultAsync();
                tUser.PasswordHash = _userManager.PasswordHasher.HashPassword(tUser,NewPassword);
                _db.SaveChanges();
                /*await _userManager.ChangePasswordAsync(await _userManager.FindByNameAsync(user), CurrentPassword, ConfirmNewPassword);*/
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }  
        }
    }
}
