namespace GoldenCity.Models
{
    public class SheriffsHouse : Building
    {
        public SheriffsHouse(int x, int y) : base(x, y)
        {
            Happiness = -7;
            BudgetWeakness = 0;
            IncomeMoney = 0;
            Cost = 5000;
        }
    }
}