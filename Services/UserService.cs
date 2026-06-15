using NotesApi.Data;
using NotesApi.Dtos;
using NotesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace NotesApi.Services;

public class UserService : IUserService
{
    public User Create(CreateUserRequest request)
    {
        throw new NotImplementedException();
    }

    public User? Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public User? GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public User? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public User? Update(int id, UpdateUserRequest request)
    {
        throw new NotImplementedException();
    }
}