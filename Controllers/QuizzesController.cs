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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

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
        await _quizRepository.AddAsync(quiz);

        // Redirect to add first question
        return RedirectToAction("AddQuestion", new { quizId = quiz.Id });
    }
    [HttpGet]
    public IActionResult AddQuestion(int quizId)
    {
        var model = new CreateQuestionViewModel { QuizId = quizId };
        return View(model);
    }
    public async Task<IActionResult> Details(int id)
    {
        var quiz = await GetQuizOrNotFound(id);
        if (quiz == null) return NotFound();
        return View(quiz);
    }
    [HttpPost]
    public async Task<IActionResult> AddQuestion(CreateQuestionViewModel model, string action)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[QuizController] Invalid question model: {@Model}", model);
            return View(model);
        }

        var quiz = await GetQuizOrNotFound(model.QuizId);
        if (quiz == null) return NotFound();

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
        await _quizRepository.UpdateAsync(quiz);

        _logger.LogInformation("[QuizController] Added question '{Question}' to quiz {QuizId}", question.Text, quiz.Id);

        return action == "AddAnother"
            ? RedirectToAction("AddQuestion", new { quizId = quiz.Id })
            : RedirectToAction("Index");
    }


    // GET: /Quizzes/Take?id=5
    public async Task<IActionResult> Take(int id, int questionIndex = 0)
    {
        var quiz = await GetQuizOrNotFound(id);
        if (quiz == null) return NotFound();

        var question = quiz.Questions.ElementAtOrDefault(questionIndex);
        if (question == null)
            return RedirectToAction("Result", new { quizId = id });

        var model = new TakeQuizViewModel
        {
            QuizId = quiz.Id,
            QuestionIndex = questionIndex,
            Question = question,
            TotalQuestions = quiz.Questions.Count
        };

        return View(model);
    }

    // POST: /Quizzes/Take
    [HttpPost]
    public async Task<IActionResult> Take(TakeQuizInputModel input)
    {
        // Get the quiz or return NotFound
        var quiz = await GetQuizOrNotFound(input.QuizId);
        if (quiz == null) return NotFound();

        // Get the current question
        var question = quiz.Questions.ElementAtOrDefault(input.QuestionIndex);
        if (question == null)
        {
            _logger.LogWarning("[QuizController] Take POST: Question index out of range for quiz {QuizId}: {Index}",
                input.QuizId, input.QuestionIndex);
            return RedirectToAction("Result", new { quizId = input.QuizId });
        }

        // Find the selected answer in that question
        var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == input.SelectedAnswerId);
        if (selectedAnswer == null)
        {
            _logger.LogWarning("[QuizController] Take POST: Selected answer not found for question {QuestionId}", question.Id);
            return RedirectToAction("Take", new { id = input.QuizId, questionIndex = input.QuestionIndex });
        }

        // Check if the answer is correct
        bool correct = selectedAnswer.IsCorrect;

        // Update score in TempData
        TempData[$"Score_{input.QuizId}"] = (TempData[$"Score_{input.QuizId}"] as int? ?? 0) + (correct ? 1 : 0);

        _logger.LogInformation("[QuizController] Quiz {QuizId}: Question {QuestionIndex} answered. Correct: {Correct}",
            input.QuizId, input.QuestionIndex, correct);

        // Move to next question
        int nextIndex = input.QuestionIndex + 1;
        return RedirectToAction("Take", new { id = input.QuizId, questionIndex = nextIndex });
    }

    // GET: /Quizzes/Result?quizId=5
    public async Task<IActionResult> Result(int quizId)
    {
        int score = TempData[$"Score_{quizId}"] as int? ?? 0;
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null) return NotFound();

        var model = new QuizResultViewModel
        {
            Quiz = quiz,
            Score = score,
            Total = quiz.Questions?.Count ?? 0
        };

        return View(model);
    }
    private async Task<Quiz?> GetQuizOrNotFound(int quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
        {
            _logger.LogError("[QuizController] Quiz not found: {QuizId}", quizId);
        }
        return quiz;
    }

}

