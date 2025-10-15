using Microsoft.EntityFrameworkCore;
using oblig.Models;

namespace oblig.DAL;

public static class DbInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database and tables exist
        context.Database.EnsureCreated();

        // Seed Quizzes
        if (!context.Quizzes.Any())
        {
            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Title = "General Knowledge",
                    Description = "Test your general knowledge!",
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "What is the capital of France?",
                            Answers = new List<Answer>
                            {
                                new Answer { Text = "Paris", IsCorrect = true },
                                new Answer { Text = "Berlin", IsCorrect = false },
                                new Answer { Text = "London", IsCorrect = false },
                                new Answer { Text = "Madrid", IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Text = "Which planet is known as the Red Planet?",
                            Answers = new List<Answer>
                            {
                                new Answer { Text = "Mars", IsCorrect = true },
                                new Answer { Text = "Jupiter", IsCorrect = false },
                                new Answer { Text = "Venus", IsCorrect = false },
                                new Answer { Text = "Saturn", IsCorrect = false }
                            }
                        }
                    }
                }
            };

            context.Quizzes.AddRange(quizzes);
            context.SaveChanges();
        }
    }
}
