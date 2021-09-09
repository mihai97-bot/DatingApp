using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet("register")]
        public async Task<ActionResult<AppUser>> Register(Registerdto registerdto){
            if (await UsernameExist(registerdto.Username)) return BadRequest("The username is taken!");
              using var hmac   = new HMACSHA512();
                var user = new AppUser{
                        UserName = registerdto.Username,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
                    PasswordSalt = hmac.Key
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
         }

         public async Task<bool> UsernameExist(string username){
             return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
         }

    }
}