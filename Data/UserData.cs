using AlphaAuthService.Helpers;
using AlphaAuthService.Models;
using Microsoft.AspNetCore.Identity;
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
    public class UserData : IUser
    {
        private AppSettings _appSettings;
        private UserManager<IdentityModel> _userManager;
        public UserData(IOptions<AppSettings> appSettings,UserManager<IdentityModel> userManager)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
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

        public async Task Registration(User user)
        {
            try
            {
                var _user = new IdentityModel()
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
    }
}
