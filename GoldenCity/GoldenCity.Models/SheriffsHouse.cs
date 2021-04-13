using System;

namespace GoldenCity.Models
{
    public class SheriffsHouse : Building
    {
        private static int _sheriffsCount;
        
        public SheriffsHouse(int x, int y) : base(x, y)
        {
            Happiness = -7;
            BudgetWeakness = 0;
            IncomeMoney = 0;
            Cost = 5000;
        }

        public static int SheriffsCount => _sheriffsCount;

        public override void AddWorker(int id)
        {
            if (id < 0)
                return; //cant add worker
            if (_sheriffsCount == 2)
                return; //cant be more than 2 sheriffs
            
            //////////////////throw new Exception("Can't be more than 2 sheriffs");

            _sheriffsCount++;
            base.AddWorker(id);
        
        }
        
        public override void RemoveWorker()
        {
            if (WorkerId >= 0)
                _sheriffsCount--;

            base.RemoveWorker();

        }
        
        public override void DeleteBuilding()
        {
            RemoveWorker();
        }
    }
}