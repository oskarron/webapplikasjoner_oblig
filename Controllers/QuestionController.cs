using Microsoft.AspNetCore.Mvc;
using oblig.DAL;
using oblig.Models;

namespace oblig.Controllers;

public class QuestionsController : Controller
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionsController(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<IActionResult> Index()
    {
        var questions = await _questionRepository.GetAllAsync();
        return View(questions);
    }

    public async Task<IActionResult> Details(int id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound();
        return View(question);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Question question)
    {
        if (ModelState.IsValid)
        {
            await _questionRepository.AddAsync(question);
            return RedirectToAction(nameof(Index));
        }
        return View(question);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound();
        return View(question);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Question question)
    {
        if (ModelState.IsValid)
        {
            await _questionRepository.UpdateAsync(question);
            return RedirectToAction(nameof(Index));
        }
        return View(question);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
            return NotFound();
        return View(question);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _questionRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
