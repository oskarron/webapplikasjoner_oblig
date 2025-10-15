using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oblig.DAL;
using oblig.Models;
using oblig.ViewModels;

namespace oblig.Controllers;

public class QuizzesController : Controller
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;

    public QuizzesController(
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
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
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null) return NotFound();
        return View(quiz);
    }
    [HttpPost]
    public async Task<IActionResult> AddQuestion(CreateQuestionViewModel model, string action)
    {
        if (!ModelState.IsValid) return View(model);

        var question = new Question { Text = model.QuestionText };
        for (int i = 0; i < 4; i++)
        {
            question.Answers.Add(new Answer
            {
                Text = model.Answers[i],
                IsCorrect = i == model.CorrectAnswerIndex
            });
        }

        var quiz = await _quizRepository.GetByIdAsync(model.QuizId);
        if (quiz == null)
        {
            // The quiz doesn't exist, return 404
            return NotFound("Quiz not found");
        }

        quiz.Questions.Add(question);
        await _quizRepository.UpdateAsync(quiz);

        if (action == "AddAnother")
            return RedirectToAction("AddQuestion", new { quizId = model.QuizId });
        else
            return RedirectToAction("Index"); // finish quiz
    }


    // GET: /Quizzes/Take?id=5
    public async Task<IActionResult> Take(int id, int questionIndex = 0)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
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
        var answer = await _answerRepository.GetByIdAsync(input.SelectedAnswerId);
        bool correct = answer?.IsCorrect ?? false;

        // Update score in TempData
        TempData[$"Score_{input.QuizId}"] = (TempData[$"Score_{input.QuizId}"] as int? ?? 0) + (correct ? 1 : 0);

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
}
