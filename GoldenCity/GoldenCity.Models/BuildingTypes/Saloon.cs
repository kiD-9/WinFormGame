namespace GoldenCity.Models
{
    public class Saloon : Building
    {
        public Saloon(int x, int y) : base(x, y)
        {
            Happiness = 5;
            BudgetWeakness = 8;
            IncomeMoney = 1000;
            Cost = 1500;
        }
    }
}