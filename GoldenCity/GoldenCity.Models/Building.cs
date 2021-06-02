using System;

namespace GoldenCity.Models
{
    public class Building
    {
        public Building(int x, int y) 
        {
            WorkerId = -1;
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
        public int Happiness { get; protected set; }
        public int WorkerId { get; private set; }
        public int BudgetWeakness { get; protected set; }
        public int IncomeMoney { get; protected set; }
        public int Cost { get; protected set; }

        public void AddWorker(int id)
        {
            if (id < 0)
                throw new Exception("Can't be worker with this id");
            if (WorkerId >= 0)
                throw new Exception("Somebody already works here");
                
            WorkerId = id;
        }

        public void RemoveWorker() 
            => WorkerId = -1;

        public virtual void DeleteBuilding()
            => RemoveWorker();
    }
}