using System.Linq;
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
         [HttpPost("Login")]
         public async Task<ActionResult<AppUser>> Login(Logindto logindto){
             var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == logindto.Username);
             if (user == null) return Unauthorized("The username does not exist!");
             using var hmac = new HMACSHA512(user.PasswordSalt);
             var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.password));

             for( int i = 0; i < computeHash.Length; i++){
                 if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password!");
             }
             return user;

         }

         public async Task<bool> UsernameExist(string username){
             return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
         }

    }
}