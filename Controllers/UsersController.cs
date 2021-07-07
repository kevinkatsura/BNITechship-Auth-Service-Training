using AlphaAuthService.Data;
using AlphaAuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlphaAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUser _user;

        public UsersController(IUser user)
        {
            _user = user;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] User user) {
            try
            {
                await _user.Registration(user);
                return Ok("Proses Registrasi Berhasil.");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("Authentication")]
        public async Task<IActionResult> Authentication([FromBody] User userParam)
        {
            var user = await _user.Authenticate(userParam.UserName, userParam.Password);
            if (user == null)
                return BadRequest("Username/Password incorrect");
            return Ok(user);
        }

        [Authorize]
        [HttpGet("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordModel user) {
            try
            {
                await _user.UpdatePassword(User.Identity.Name, user.CurrentPassword, user.NewPassword, user.ConfirmNewPassword);
                return Ok("Password berhasil diupdate.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*        // GET: api/<UsersController>
                [HttpGet]
                public IEnumerable<string> Get()
                {
                    return new string[] { "value1", "value2" };
                }

                // GET api/<UsersController>/5
                [HttpGet("{id}")]
                public string Get(int id)
                {
                    return "value";
                }

                // POST api/<UsersController>
                [HttpPost]
                public void Post([FromBody] string value)
                {
                }

                // PUT api/<UsersController>/5
                [HttpPut("{id}")]
                public void Put(int id, [FromBody] string value)
                {
                }

                // DELETE api/<UsersController>/5
                [HttpDelete("{id}")]
                public void Delete(int id)
                {
                }*/
    }
}
