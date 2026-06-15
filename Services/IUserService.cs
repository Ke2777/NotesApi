using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Services;
public interface IUserService
{
    IEnumerable<User> GetAll();
    User? GetByEmail(string email);
    User? GetById(int id);
    User Create(CreateUserRequest request);
    User? Delete(int id);
    User? Update(int id, UpdateUserRequest request);
}