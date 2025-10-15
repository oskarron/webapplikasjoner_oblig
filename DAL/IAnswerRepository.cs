using oblig.Models;

namespace oblig.DAL;

public interface IAnswerRepository
{
    Task<IEnumerable<Answer>> GetAllAsync();
    Task<Answer?> GetByIdAsync(int id);
    Task AddAsync(Answer answer);
    Task<bool> UpdateAsync(Answer answer);
    Task<bool> DeleteAsync(int id);
}
