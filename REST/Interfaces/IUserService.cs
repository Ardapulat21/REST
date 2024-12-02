using Microsoft.EntityFrameworkCore;
using REST.Enums;
using REST.Models;

namespace REST.Interfaces
{
    public interface IUserService
    {
        public Task<DbSet<User>> GetUsers();
        public Task<User?> GetById(int id);
        public Task<UserStatus> Add(User credentials);
        public Task<UserStatus> Delete(int id);
        public Task<UserStatus> Edit(User credentials,int id);
        public Task<User?> GetByUsername(string? username);
    }
}