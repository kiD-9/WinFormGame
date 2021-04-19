namespace GoldenCity.Models
{
    public class Store : Building
    {
        public Store(int x, int y) : base(x, y)
        {
            Happiness = 2;
            BudgetWeakness = 15;
            IncomeMoney = 1500;
            Cost = 3000;
        }
    }
}