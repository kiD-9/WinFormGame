using System.Linq;

namespace GoldenCity.Models
{
    public class Bandits
    {
        private Building[] buildingsToRaid;
        private GameSetting gameSetting;

        public Bandits(GameSetting gameSetting)
        {
            buildingsToRaid = new Building[3 - gameSetting.SheriffsCount];
            this.gameSetting = gameSetting;
        }

        public void FindBuildingsToRaid() //реализовать в отдельный поток?
        {
            var currentMinBudgetWeakness = -1;
            foreach (var building in gameSetting.Map)
            {
                if (building == null || building.BudgetWeakness < currentMinBudgetWeakness)
                    continue;
                AddBuildingToRaid(building);
                currentMinBudgetWeakness = buildingsToRaid[0].BudgetWeakness;
            }
        }

        public void Raid()
        {
           var budgetToRob = buildingsToRaid.Where(b => b != null).Sum(b => b.BudgetWeakness) 
               * gameSetting.Money / 100;
           gameSetting.ChangeMoney(-budgetToRob);
           foreach (var building in buildingsToRaid.Where(b => b != null))
           {
               gameSetting.DeleteCitizien(building.WorkerId);
           }
        }

        private void AddBuildingToRaid(Building building)
        {
            buildingsToRaid[0] = building;
            
            for (var i = 1; i < buildingsToRaid.Length; i++)
            {
                if (buildingsToRaid[i] == null || buildingsToRaid[i - 1].BudgetWeakness >= buildingsToRaid[i].BudgetWeakness)
                {
                    var tmp = buildingsToRaid[i];
                    buildingsToRaid[i] = buildingsToRaid[i - 1];
                    if (tmp != null)
                        buildingsToRaid[i - 1] = tmp;
                }
            }
        }
    }
}