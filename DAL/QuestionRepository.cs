using Microsoft.EntityFrameworkCore;
using oblig.Models;

namespace oblig.DAL;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _context;

    public QuestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Question>> GetAllAsync() =>
        await _context.Questions.Include(q => q.Answers).ToListAsync();

    public async Task<Question?> GetByIdAsync(int id) =>
        await _context.Questions.Include(q => q.Answers)
                                .FirstOrDefaultAsync(q => q.Id == id);

    public async Task AddAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Question question)
    {
        _context.Questions.Update(question);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question != null)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }
}
