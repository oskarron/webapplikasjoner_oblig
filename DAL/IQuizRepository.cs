using oblig.Models;

namespace oblig.DAL;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAllAsync();
    Task<Quiz?> GetByIdAsync(int id);
    Task AddAsync(Quiz quiz);
    Task<bool> UpdateAsync(Quiz quiz);
    Task DeleteAsync(int id);
}
