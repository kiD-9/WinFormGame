namespace GoldenCity.Models
{
    public class Shop : Building
    {
        public Shop(int x, int y) : base(x, y)
        {
            Happiness = 2;
            BudgetWeakness = 15;
            IncomeMoney = 1500;
            Cost = 3000;
        }
    }
}