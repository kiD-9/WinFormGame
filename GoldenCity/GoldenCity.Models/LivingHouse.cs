using System.Linq;

namespace GoldenCity.Models
{
    public class LivingHouse : Building
    {
        private const int LivingPlaces = 4;
        private readonly int[] livers;

        public LivingHouse(int x, int y, GameSetting gameSetting) : base(x, y)
        {
            Happiness = 1;
            BudgetWeakness = 2;
            IncomeMoney = 0;
            Cost = 500;
            livers = new int[LivingPlaces] {-1, -1, -1, -1};
            HavePlace = true;
            gameSetting.ChangeCitiziensLimit(LivingPlaces);
        }
        
        public bool HavePlace { get; private set; }

        public void AddLiver(int citizienId, GameSetting gameSetting)
        {
            if (!HavePlace || !gameSetting.IsCitizien(citizienId))
                return;
            for (var i = 0; i < LivingPlaces; i++)
            {
                if (livers[i] < 0)
                {
                    livers[i] = citizienId;
                    break;
                }
            }

            HavePlace = livers.Any(l => l < 0);
        }
        
        public void DeleteLiver(int citizienId, GameSetting gameSetting)
        {
            if (!gameSetting.IsCitizien(citizienId))
                return;
            for (var i = 0; i < LivingPlaces; i++)
            {
                if (livers[i] == citizienId)
                {
                    livers[i] = -1;
                    HavePlace = true;
                    break;
                }
            }
        }
        
        public override void Delete(GameSetting gameSetting)
        {
            base.Delete(gameSetting);
            
            gameSetting.ChangeCitiziensLimit(-LivingPlaces);
            foreach (var liver in livers)
            {
                gameSetting.DeleteCitizien(liver);
            }
        }
    }
}