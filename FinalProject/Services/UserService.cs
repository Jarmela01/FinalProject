using LOAN_API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Users.Data;
using Users.Domain;


namespace FinalProject.Services
{
    public interface IUserService
    {
        User Login(User model);
        List<Loan> GetById(int id);
        string GenerateToken(User user,int id);
    }
    public class UserService : IUserService
    {
        private readonly UsersContext _usersContext;
        private readonly AppSettings _appSettings;

        public UserService(UsersContext usersContext,IOptions<AppSettings> appSettings)
        {
            _usersContext = usersContext;
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(User user,int id)
        {
            var users = _usersContext.Users.ToList();
         
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor            
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, users[id].Id.ToString()),
                    new Claim(ClaimTypes.Role, users[id].Roles.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public List<Loan> GetById(int id)
        {
            var loans = _usersContext.Loans.ToList();
            var loan = loans.Where(x => x.UserId == id).ToList();

            return loan;
        }

        public User Login(User user)
        {
        if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return null;
            }
            var userLogin = _usersContext.Users.SingleOrDefault(x => x.UserName == user.UserName);
            user.Password = PasswordHasher.HashPassword(user.Password);
            

            if (userLogin == null)
            
                return null;
            
            if (userLogin.Password != user.Password)
            {
                return null;
            }

            return userLogin;
        }

        
    }
    
}
