using REST.Interfaces;
using Crypt = BCrypt.Net.BCrypt;
namespace REST.Services
{
    public class HashingService : IHashingService
    {
        public string GetHash(string password){
            return Crypt.HashPassword(password);
        }
        public bool IsMatched(string password,string hashedPassword){
            return Crypt.Verify(password, hashedPassword);
        }
    }
}