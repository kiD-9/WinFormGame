﻿using System.Linq;

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
                if (building.BudgetWeakness >= currentMinBudgetWeakness)
                {
                    AddBuildingToRaid(building);
                    currentMinBudgetWeakness = buildingsToRaid[0].BudgetWeakness;
                }
            }
        }

        public void Raid()
        {
           var budgetToRob = buildingsToRaid.Sum(b => b.BudgetWeakness) * gameSetting.Money / 100;
           gameSetting.ChangeMoney(-budgetToRob);
           foreach (var building in buildingsToRaid)
           {
               gameSetting.DeleteCitizien(building.WorkerId);
           }
        }

        private void AddBuildingToRaid(Building building)
        {
            buildingsToRaid[0] = building;
            
            for (var i = 1; i < buildingsToRaid.Length; i++)
            {
                if (buildingsToRaid[i - 1].BudgetWeakness < buildingsToRaid[i].BudgetWeakness) 
                    break;
                var tmp = buildingsToRaid[i];
                buildingsToRaid[i] = buildingsToRaid[i - 1];
                buildingsToRaid[i - 1] = tmp;
            }
        }
    }
}