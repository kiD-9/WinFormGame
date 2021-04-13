namespace GoldenCity.Models
{
    public class Jail : Building
    {
        private static int _jailWorkersCount; //ms
        private const int TimerIncreaser = 10000; //ms
        
        public Jail(int x, int y) : base(x, y)
        {
            Happiness = -10;
            BudgetWeakness = 0;
            IncomeMoney = 0;
            Cost = 4000;
        }

        public static int AttackTimerIncreaser => _jailWorkersCount * TimerIncreaser;

        public override void AddWorker(int id)
        {
            if (id < 0)
                return; //cant add worker
            
            _jailWorkersCount++;
            base.AddWorker(id);
        }
        
        public override void RemoveWorker()
        {
            if (WorkerId >= 0)
                _jailWorkersCount--;
            
            base.RemoveWorker();
            
        }

        public override void DeleteBuilding()
        {
            RemoveWorker();
        }
    }
}