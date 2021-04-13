namespace GoldenCity.Models
{
    public class Jail : Building
    {
        public Jail(int x, int y) : base(x, y)
        {
            Happiness = -10;
            BudgetWeakness = 0;
            IncomeMoney = 0;
            Cost = 4000;
        }
    }
}