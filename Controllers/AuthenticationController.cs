using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ExpensesAPI.Data;
using ExpensesAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExpensesAPI.Controllers
{  
    //[Route("api/[controller]")] 
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthenticationController(AppDbContext context)
        {
            _context = context;
        }

        [Route("login")]
        [HttpPost]
        public ActionResult Login([FromBody] User user)
        {
            if(string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Enter userName and password");
            
            try
            {
                var exists = _context.Users.Any(n => n.UserName == user.UserName && n.Password == user.Password);
                if (exists) return Ok(CreateToken(user));

                return BadRequest("Bad Credentials");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [Route("register")]
        [HttpPost]
        public ActionResult Register([FromBody] User user)
        {
            try
            {
                var exists = _context.Users.Any(n => n.UserName == user.UserName);

                if (exists)
                {
                    return BadRequest("User already exists");
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(CreateToken(user));

            }
            catch (Exception ex)
            {                
                throw;
            }
           
        }

        private JwtPackage CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.UserName) });

            const string secretKey = "your secret key goes here";
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken)tokenHandler.CreateJwtSecurityToken
                (
                  subject: claims,
                  signingCredentials:signingCredentials
                );

            var tokenString = tokenHandler.WriteToken(token);
            return new JwtPackage()
            {
                UserName = user.UserName,
                Token = tokenString
            };
        }
    }
}

public class JwtPackage
{
    public string Token { get; set; }
    public string UserName { get; set; }
}