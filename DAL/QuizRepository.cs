using Microsoft.EntityFrameworkCore;
using oblig.Models;

namespace oblig.DAL;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _context;
    public QuizRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Quiz>> GetAllAsync() =>
        await _context.Quizzes
                      .Include(q => q.Questions)
                      .ThenInclude(q => q.Answers)
                      .ToListAsync();

    public async Task<Quiz?> GetByIdAsync(int id) =>
        await _context.Quizzes
                      .Include(q => q.Questions)
                      .ThenInclude(q => q.Answers)
                      .FirstOrDefaultAsync(q => q.Id == id);

    public async Task AddAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Quiz quiz)
    {
        var existing = await _context.Quizzes.FindAsync(quiz.Id);
        if (existing == null) return false;

        _context.Entry(existing).CurrentValues.SetValues(quiz);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }
}
