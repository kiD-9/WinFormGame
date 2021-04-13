namespace GoldenCity.Models
{
    public class Jail : Building
    {
        private const int AttackTimerIncreaser = 10000; //ms
        
        public Jail(int x, int y) : base(x, y)
        {
            Happiness = -10;
            BudgetWeakness = 0;
            IncomeMoney = 0;
            Cost = 4000;
        }
        
        public override void AddWorker(int id, GameSetting gameSetting)
        {
            if (gameSetting.CanBecomeWorker(id))
                gameSetting.ChangeAttackTimer(AttackTimerIncreaser);
            
            base.AddWorker(id, gameSetting);
        }

        public override void RemoveWorker(GameSetting gameSetting)
        {
            if (gameSetting.IsWorker(WorkerId))
                gameSetting.ChangeAttackTimer(-AttackTimerIncreaser);
            
            base.RemoveWorker(gameSetting);
            
        }

        public override void Delete(GameSetting gameSetting)
        {
            RemoveWorker(gameSetting);
            gameSetting.ChangeHappiness(-Happiness);
        }
    }
}