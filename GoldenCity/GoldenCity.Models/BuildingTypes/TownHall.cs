namespace GoldenCity.Models
{
    public class TownHall : Building
    {
        public TownHall(int x, int y) : base(x, y)
        {
            Happiness = 0;
            BudgetWeakness = 0;
            IncomeMoney = 0;
            Cost = 150000;
        }
    }
}