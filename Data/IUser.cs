using AlphaAuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaAuthService.Data
{
    public interface IUser
    {
        Task<User> Authenticate(string username, string password);
        Task Registration(User user);
    }
}
