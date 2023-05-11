using LOAN_API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Users.Data;
using Users.Domain;
using XAct.Users;

namespace FinalProject.Services
{
    public interface IAccountantService
    {
        Accountant Login(Accountant accountant);
        string GenerateToken(Accountant accountant);

    }
    public class AccountantService : IAccountantService
    {
        private readonly UsersContext _usersContext;
        private readonly AppSettings _appSettings;

        public AccountantService(UsersContext usersContext, IOptions<AppSettings> appSettings)
        {
            _usersContext = usersContext;
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(Accountant accountant)
        {
            var accountants = _usersContext.Accountant.ToList();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, accountants[0].Id.ToString()),
                    new Claim(ClaimTypes.Role, accountants[0].Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public Accountant Login(Accountant accountant)
        {
            if (string.IsNullOrEmpty(accountant.UserName) || string.IsNullOrEmpty(accountant.Password))
            {
                return null;
            }
            var userLogin = _usersContext.Accountant.SingleOrDefault(x => x.UserName == accountant.UserName);


            if (userLogin == null)
                return null;

            if (userLogin.Password != accountant.Password)
            {
                return null;
            }

            return userLogin;
        }
    }
}
