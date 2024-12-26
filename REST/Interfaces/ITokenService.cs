using REST.Models;

namespace REST.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJWT(User user);
    }
}