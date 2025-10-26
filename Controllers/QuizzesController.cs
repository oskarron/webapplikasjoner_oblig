using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oblig.DAL;
using oblig.Models;
using oblig.ViewModels;

namespace oblig.Controllers;

public class QuizzesController : Controller
{
    private readonly IQuizRepository _quizRepository;
    private readonly ILogger<QuizzesController> _logger;

    public QuizzesController(
        IQuizRepository quizRepository,
        ILogger<QuizzesController> logger)
    {
        _quizRepository = quizRepository;
        _logger = logger;
    }

    // GET: /Quizzes
    public async Task<IActionResult> Index()
    {
        var quizzes = await _quizRepository.GetAllAsync();
        return View(quizzes);
    }

    // GET: /Quizzes/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Quizzes/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateQuizViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var quiz = new Quiz
        {
            Title = model.Title,
            Description = model.Description,
            Questions = new List<Question>()
        };

        bool returnOk = await _quizRepository.CreateAsync(quiz);
        if (!returnOk
        )
        {
            _logger.LogError("[QuizController] Failed to create quiz: {Title}", model.Title);
            ModelState.AddModelError("", "Failed to create quiz. Please try again.");
            return View(model);
        }

        return RedirectToAction("AddQuestion", new { quizId = quiz.Id });
    }

    // GET: /Quizzes/AddQuestion
    [HttpGet]
    public IActionResult AddQuestion(int quizId)
    {
        var model = new CreateQuestionViewModel { QuizId = quizId };
        return View(model);
    }

    // POST: /Quizzes/AddQuestion
    [HttpPost]
    public async Task<IActionResult> AddQuestion(CreateQuestionViewModel model, string action)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[QuizController] Invalid question model: {@Model}", model);
            return View(model);
        }

        var quiz = await _quizRepository.GetByIdAsync(model.QuizId);
        if (quiz == null)
        {
            _logger.LogError("[QuizzesController] Quiz not found when adding question  {QuizId:0000}", model.QuizId);
            return BadRequest("Item not found for the QuizId");
        }

        for (int i = 0; i < model.Answers.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(model.Answers[i]))
            {
                ModelState.AddModelError($"Answers[{i}]", "This answer cannot be empty.");
            }
        }


        var question = new Question
        {
            Text = model.QuestionText,
            Answers = model.Answers.Select((text, i) => new Answer
            {
                Text = text,
                IsCorrect = i == model.CorrectAnswerIndex
            }).ToList()
        };

        quiz.Questions.Add(question);
        bool returnOk
         = await _quizRepository.UpdateAsync(quiz);
        if (!returnOk
        )
        {
            _logger.LogError("[QuizController] Failed to add question '{Question}' to quiz {QuizId}", question.Text, quiz.Id);
            ModelState.AddModelError("", "Failed to save question. Please try again.");
            return View(model);
        }

        _logger.LogInformation("[QuizController] Added question '{Question}' to quiz {QuizId}", question.Text, quiz.Id);

        return action == "AddAnother"
            ? RedirectToAction("AddQuestion", new { quizId = quiz.Id })
            : RedirectToAction("Index");
    }

    // GET: /Quizzes/Take
    public async Task<IActionResult> Take(int quizId, int questionIndex = 0, int currentScore = 0)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
        {
            _logger.LogError("[QuizzesController] Quiz not found for QuizId {QuizId:0000}", quizId);
            return BadRequest("Quiz not found");
        }

        if (questionIndex >= quiz.Questions.Count)
        {
            _logger.LogInformation("[QuizzesController] All questions answered for QuizId {QuizId}", quizId);
            return RedirectToAction("Result", new { quizId, score = currentScore });
        }

        var model = new TakeQuizViewModel
        {
            QuizId = quizId,
            QuestionIndex = questionIndex,
            Question = quiz.Questions.ElementAt(questionIndex),
            TotalQuestions = quiz.Questions.Count,
            CurrentScore = currentScore
        };

        return View(model);
    }

    // POST: /Quizzes/Take
    [HttpPost]
    public async Task<IActionResult> Take(TakeQuizViewModel input, int selectedAnswerId)
    {
        var quiz = await _quizRepository.GetByIdAsync(input.QuizId);
        if (quiz == null)
        {
            _logger.LogError("[QuizzesController] Quiz not found for QuizId {QuizId:0000}", input.QuizId);
            return BadRequest("Quiz not found");
        }

        var question = quiz.Questions.ElementAtOrDefault(input.QuestionIndex);
        if (question == null)
        {
            _logger.LogWarning("[QuizzesController] Question index {QuestionIndex} out of range for QuizId {QuizId:0000}", input.QuestionIndex, input.QuizId);
            return RedirectToAction("Result", new { quizId = input.QuizId, score = input.CurrentScore });
        }

        var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
        if (selectedAnswer == null)
        {
            _logger.LogWarning("[QuizzesController] Selected answer Id {AnswerId} not found for QuestionId {QuestionId}", selectedAnswerId, question.Id);
            return RedirectToAction("Take", new { quizId = input.QuizId, questionIndex = input.QuestionIndex, currentScore = input.CurrentScore });
        }

        int newScore = input.CurrentScore + (selectedAnswer.IsCorrect ? 1 : 0);
        int nextIndex = input.QuestionIndex + 1;

        return RedirectToAction("Take", new { quizId = input.QuizId, questionIndex = nextIndex, currentScore = newScore });
    }

    // GET: /Quizzes/Result
    public async Task<IActionResult> Result(int quizId)
    {
        // Get the quiz
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
        {
            _logger.LogError(
                "[QuizzesController] Quiz not found when trying to view results for QuizId {QuizId:0000}",
                quizId
            );
            return NotFound("Quiz not found.");
        }

        // Retrieve the score from TempData
        int score = TempData[$"Score_{quizId}"] as int? ?? 0;

        _logger.LogInformation(
            "[QuizzesController] Displaying results for QuizId {QuizId}. Score: {Score}/{TotalQuestions}",
            quizId, score, quiz.Questions?.Count ?? 0
        );

        var model = new QuizResultViewModel
        {
            Quiz = quiz,
            Score = score,
            Total = quiz.Questions?.Count ?? 0
        };

        return View(model);
    }

    // GET: /Quizzes/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null)
        {
            _logger.LogError("[QuizzesController] Quiz not found for the QuizId {QuizId:0000}", id);
            return BadRequest("Quiz not found for the QuizId");
        }
        return View(quiz);
    }

    // POST: /Quizzes/DeleteConfirmed/5
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool returnOk = await _quizRepository.DeleteAsync(id);
        if (!returnOk)
        {
            _logger.LogError("[QuizzesController] Quiz deletion failed for the QuizId {QuizId:0000}", id);
            return BadRequest("Quiz deletion failed");
        }
        return RedirectToAction(nameof(Index));
    }


}
