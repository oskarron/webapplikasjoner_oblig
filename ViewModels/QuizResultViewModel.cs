using oblig.Models;

namespace oblig.ViewModels
{
    public class QuizResultViewModel
    {
        public Quiz Quiz { get; set; } = null!;
        public int Score { get; set; }
        public int Total { get; set; }
        public double Percentage => Total > 0 ? ((double)Score / Total) * 100 : 0;
    }
}
