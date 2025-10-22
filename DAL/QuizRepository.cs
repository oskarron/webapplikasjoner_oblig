using oblig.Models;
using Microsoft.EntityFrameworkCore;

namespace oblig.DAL;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<QuizRepository> _logger;

    public QuizRepository(AppDbContext db, ILogger<QuizRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Quiz>> GetAllAsync()
    {
        try
        {
            return await _db.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Failed to get all quizzes: {Message}", e.Message);
            return Enumerable.Empty<Quiz>();
        }
    }

    public async Task<Quiz?> GetByIdAsync(int id)
    {
        try
        {
            return await _db.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Failed to get quiz {QuizId}: {Message}", id, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateAsync(Quiz quiz)
    {
        try
        {
            await _db.Quizzes.AddAsync(quiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Failed to create quiz {QuizTitle}: {Message}", quiz.Title, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Quiz quiz)
    {
        try
        {
            _db.Quizzes.Update(quiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Failed to update quiz {QuizId}: {Message}", quiz.Id, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                _logger.LogError("[QuizRepository] Quiz not found for the QuizId {QuizId:0000}", id);
                return false;
            }

            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Quiz deletion failed for the QuizId {QuizId:0000}, error message: {Error}", id, e.Message);
            return false;
        }
    }
}
