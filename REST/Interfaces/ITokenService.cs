namespace REST.Interfaces{
    public interface ITokenService
    {
        public string GenerateJWT(string username);
    }
}