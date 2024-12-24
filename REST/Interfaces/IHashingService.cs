namespace REST.Interfaces
{
    public interface IHashingService
    {
        public string GetHash(string password);
        public bool IsMatched(string password,string hashedPassword);
    }
}