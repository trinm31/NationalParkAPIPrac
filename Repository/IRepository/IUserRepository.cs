using NationalParkAPI.Models;

namespace NationalParkAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IUniqueUser(string username);
        User Authentication(string username, string password);
        User Register(string username, string password);
    }
}