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
        public int IncomeMoney { get; protected set; }
        public int Cost { get; protected set; }

        public void AddWorker(int id)
        {
            if (id < 0)
                throw new Exception("Can't be worker with this id");
                
            WorkerId = id;
        }

        public void RemoveWorker()
        {
            WorkerId = -1;
        }

        public virtual void DeleteBuilding()
        {
            RemoveWorker();
        }
    }
}