using NotesApi.Data;
using NotesApi.Dtos;
using Microsoft.AspNetCore.Identity;
using NotesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesApi.Services;

public class UserService : IUserService
{
    private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();
    private readonly AppDbContext _dbContext;
    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public User Create(CreateUserRequest request)
    {
        User user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _hasher.HashPassword(null!, request.Password)
        };

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return user;
    }

    public User? Delete(int id)
    {
        User? user = _dbContext.Users.FirstOrDefault(currentUser => currentUser.Id == id);

        if (user == null)
        {
            return null;
        }

        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();

        return user;
    }

    public IEnumerable<User> GetAll()
    {
        return _dbContext.Users;
    }

    public User? GetByEmail(string email)
    {
        return _dbContext.Users.FirstOrDefault(currentUser => currentUser.Email == email);
    }
    

    public User? GetById(int id)
    {
        return _dbContext.Users.FirstOrDefault(currentUser => currentUser.Id == id);
    }

    public User? Update(int id, UpdateUserRequest request)
    {
        User? user = _dbContext.Users.FirstOrDefault(currentUser => currentUser.Id == id);

        if (user == null)
        {
            return null;
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.PasswordHash = _hasher.HashPassword(null!, request.Password);

        _dbContext.SaveChanges();

        return user;
    }
}