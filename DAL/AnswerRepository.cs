using Microsoft.EntityFrameworkCore;
using oblig.Models;

namespace oblig.DAL;

public class AnswerRepository : IAnswerRepository
{
    private readonly AppDbContext _context;

    public AnswerRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Answer>> GetAllAsync() =>
        await _context.Answers.ToListAsync();

    public async Task<Answer?> GetByIdAsync(int id) =>
        await _context.Answers.FindAsync(id);

    public async Task AddAsync(Answer answer)
    {
        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Answer answer)
    {
        var existing = await _context.Answers.FindAsync(answer.Id);
        if (existing == null) return false;

        _context.Entry(existing).CurrentValues.SetValues(answer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var answer = await _context.Answers.FindAsync(id);
        if (answer == null) return false;

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();
        return true;
    }
}
