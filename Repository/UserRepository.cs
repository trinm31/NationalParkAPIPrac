using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NationalParkAPI.Data;
using NationalParkAPI.Models;
using NationalParkAPI.Repository.IRepository;

namespace NationalParkAPI.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly Appsettings _appsettings;

        public UserRepository(ApplicationDbContext db, IOptions<Appsettings> appsettings)
        {
            _db = db;
            _appsettings = appsettings.Value;
        }

        public User Authentication(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);
            if (user == null)
            {
                return null;
            }

            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            user.Token = tokenhandler.WriteToken(token);
            return user;
        }

        public bool IUniqueUser(string username)
        {
            var user = _db.Users.SingleOrDefault(x => x.Username == username);

            // return null if user not found
            if (user == null)
                return true;

            return false;
        }

        public User Register(string username, string password)
        {
            User userObj = new User()
            {
                Username = username,
                Password = password,
                Role="Admin"
            };

            _db.Users.Add(userObj);
            _db.SaveChanges();
            userObj.Password = "";
            return userObj;
        }
    }
}