using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Enums;
using REST.Interfaces;
using REST.Models;

namespace REST.Services
{
    public class UserService : IUserService
    {
        ApplicationDbContext _context;
        IHashingService _hashingService;
        public UserService(ApplicationDbContext context,IHashingService hashingService){
            _context = context;
            _hashingService = hashingService;
        }
        public async Task<DbSet<User>> GetUsers(){
            return _context.Users;
        }
        public async Task<User?> GetByUsername(string? username){
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User?> GetByEmail(string? email){
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetById(int id){
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<UserStatus> Delete(int id){
            var user = await GetById(id);
            if(user == null)
               return UserStatus.NotFound;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(); 

            return UserStatus.OK;
        }
        public async Task<UserStatus> Edit(User credentials,int id){
            var user = await GetById(id);
            if(user == null)
                return UserStatus.NotFound;

            user.Username = credentials.Username;
            user.Password = credentials.Password;
            user.Email = credentials.Email;
            await _context.SaveChangesAsync();

            return UserStatus.OK; 
        }
        public async Task<UserStatus> Register(User credentials){
            var user = _context.Users.FirstOrDefault(u => u.Username == credentials.Username || u.Email == credentials.Email);
            if(user != null)
                return UserStatus.Found;
            
            credentials.Password = _hashingService.GetHash(credentials.Password);
            _context.Add(credentials);
            await _context.SaveChangesAsync();
            return UserStatus.OK;
        }
        public async Task<User?> Login(User credentials){
            var user = await GetByUsername(credentials.Username);
            if(user != null && user.Username == credentials.Username  && 
            _hashingService.IsMatched(credentials.Password,user.Password))
            {
                return user;
            }
            return null;
        }
    }
}