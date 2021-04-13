namespace GoldenCity.Models
{
    public class RailroadStation : Building
    {
        public RailroadStation(int x, int y) : base(x, y)
        {
            Happiness = 10;
            BudgetWeakness = 10;
            IncomeMoney = 500;
            Cost = 3000;
        }
    }
}