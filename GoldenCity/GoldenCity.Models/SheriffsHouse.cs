using System;

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

        public override void AddWorker(int id, GameSetting gameSetting)
        {
            if (gameSetting.Sheriffs == 2)
                throw new Exception("Can't be more than 2 sheriffs");
            
            if (gameSetting.CanBecomeWorker(id))
                gameSetting.Sheriffs++;
            
            base.AddWorker(id, gameSetting);

        }

        public override void RemoveWorker(GameSetting gameSetting)
        {
            if (gameSetting.IsWorker(WorkerId))
                gameSetting.Sheriffs--;
            
            base.RemoveWorker(gameSetting);

        }

        public override void Delete(GameSetting gameSetting)
        {
            RemoveWorker(gameSetting);
            gameSetting.ChangeHappiness(-Happiness);
        }
    }
}