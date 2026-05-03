namespace V_Quiz_Backend.Models
{
    public class CategoryStat
    {
        public List <bool> RecentAnswers { get; set; } = [];
        public int Percent { get; set; }
    }
}
