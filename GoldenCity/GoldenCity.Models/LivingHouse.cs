using System;
using System.Linq;

namespace GoldenCity.Models
{
    public class LivingHouse : Building
    {
        public const int LivingPlaces = 4;
        private readonly int[] livers;

        public LivingHouse(int x, int y) : base(x, y)
        {
            Happiness = 1;
            BudgetWeakness = 2;
            IncomeMoney = 0;
            Cost = 500;
            livers = new int[LivingPlaces] {-1, -1, -1, -1};
            HavePlace = true;
        }
        
        public bool HavePlace { get; private set; }

        public int this[int index] => livers[index];

        public void AddLiver(int liverId)
        {
            if (!HavePlace)
                throw new Exception("No space at living house");
            
            for (var i = 0; i < LivingPlaces; i++)
            {
                if (livers[i] < 0)
                {
                    livers[i] = liverId;
                    break;
                }
            }

            HavePlace = livers.Any(l => l < 0);
        }
        
        public void DeleteLiver(int liverId)
        {
            for (var i = 0; i < LivingPlaces; i++)
            {
                if (livers[i] != liverId)
                    continue;
                livers[i] = -1;
                HavePlace = true;
                return;
            }
            throw new Exception("No liver with this id");
        }

        public override void DeleteBuilding()
        {
            base.DeleteBuilding();
            for (var i = 0; i < LivingPlaces; i++)
            {
                livers[i] = -1;
            }
        }
    }
}