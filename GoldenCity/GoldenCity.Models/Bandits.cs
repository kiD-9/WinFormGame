using System.Linq;

namespace GoldenCity.Models
{
    public class Bandits
    {
        private readonly GameSetting gameSetting;

        public Bandits(GameSetting gameSetting)
        {
            this.gameSetting = gameSetting;
            BuildingsToRaid = new Building[gameSetting.MapSize - gameSetting.SheriffsCount];
        }
        
        public Building[] BuildingsToRaid { get; }

        public void FindBuildingsToRaid()
        {
            foreach (var building in gameSetting.Map)
            {
                if (building == null || building.BudgetWeakness == 0 || building.WorkerId < 0)
                    continue;
                
                if (BuildingsToRaid[0] == null || building.BudgetWeakness >= BuildingsToRaid[0].BudgetWeakness)
                    AddBuildingToRaid(building);
            }
        }
        
        public void Raid()
        {
            var budgetToRob = BuildingsToRaid.Where(b => b != null).Sum(b => b.BudgetWeakness) 
                * gameSetting.Money / 100;
            gameSetting.ChangeMoney(-budgetToRob);
            foreach (var building in BuildingsToRaid.Where(b => b != null))
            {
                gameSetting.DeleteCitizen(building.WorkerId);
            }
        }
        
        private void AddBuildingToRaid(Building building)
        {
            BuildingsToRaid[0] = building;
            
            for (var i = 1; i < BuildingsToRaid.Length; i++)
            {
                if (BuildingsToRaid[i] == null || BuildingsToRaid[i - 1].BudgetWeakness >= BuildingsToRaid[i].BudgetWeakness)
                {
                    var tmp = BuildingsToRaid[i];
                    BuildingsToRaid[i] = BuildingsToRaid[i - 1];
                    BuildingsToRaid[i - 1] = tmp;
                }
            }
        }
    }
}