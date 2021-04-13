using System;

namespace GoldenCity.Models
{
    public class Building
    {
        public readonly int X;
        public readonly int Y;

        public Building(int x, int y) 
        {
            WorkerId = -1;
            X = x;
            Y = y;
        }

        public int Happiness { get; protected set; }
        public int WorkerId { get; private set; }
        public int BudgetWeakness { get; protected set; }
        protected int IncomeMoney { get; set; }
        public int Cost { get; protected set; }

        public virtual void AddWorker(int id, GameSetting gameSetting)
        {
            //должен добавлять жителя как работника в это здание (если работник где-то работает, то нельзя), добавить жителя в трудящихся и давать прибыль
            
            if (!gameSetting.CanBecomeWorker(WorkerId))
                throw new Exception("Can't become worker");
            
            WorkerId = id;
            gameSetting.AddWorker(id, X, Y);
            gameSetting.ChangeIncomeMoney(IncomeMoney);
        }

        public virtual void RemoveWorker(GameSetting gameSetting)
        {
            //должен уволить работника из этого здания, убрать из трудящихся и уменьшить прибыль
            
            if (!gameSetting.IsWorker(WorkerId))
                return;

            WorkerId = -1;
            gameSetting.RetireWorker(WorkerId);
            gameSetting.ChangeIncomeMoney(-IncomeMoney);
        }

        public virtual void Delete(GameSetting gameSetting)
        {
            //удаляет всё, что хранит здание: увольняет работника + уменьшает прибыль, забирает счастье
            
            RemoveWorker(gameSetting);
            gameSetting.ChangeHappiness(-Happiness);
        }
    }
}