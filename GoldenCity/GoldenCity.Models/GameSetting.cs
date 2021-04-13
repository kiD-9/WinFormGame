using System;
using System.Collections.Generic;
using System.Threading;

namespace GoldenCity.Models
{
    public class GameSetting
    {
        private Building[,] map;
        private int money;
        private int incomeMoney;
        private const int PayTimerInterval = 45000; //время ms
        private Timer payTimer;
        private int attackTimerInterval; //время ms
        private Timer attackTimer;
        private int newCitizienTimerInterval; //время ms
        private Timer newCitizienTimer;
        private int citiziensLimit;
        private int newCitizienId;
        private Dictionary<int, (int,int)> citiziens;
        private Dictionary<int, (int,int)> workingCitiziens;


        public GameSetting(int mapSize)
        {
            map = new Building[mapSize, mapSize];
            
            attackTimerInterval = 120000; //ms
            newCitizienTimerInterval = 45000; //ms
            money = 4000;
            
            payTimer = new Timer(PayDay, null, PayTimerInterval, PayTimerInterval);
            attackTimer = new Timer(Attack, null, attackTimerInterval, attackTimerInterval);
            newCitizienTimer = new Timer(AddCitizien, null, 0, newCitizienTimerInterval);

            map[0, 0] = new LivingHouse(0, 0, this);
            citiziens = new Dictionary<int, (int, int)>(); //первый житель должен сам добавиться при создании, т.к. в таймере 0
            workingCitiziens = new Dictionary<int, (int, int)>();
        }

        public Building[,] Map => map;
        public int Money => money;
        public int Sheriffs { get; set; } //проверка на >2 в SheriffsHouse

        public void AddBuilding(int x, int y, Building building)
        {
            if (map[y, x] == null || (money - building.Cost < 0))
                throw new Exception("Can't build");
            
            // if (building is SheriffsHouse && Sheriffs == 2) //реализовано в классе SheriffsHouse
            //     return;
            
            map[y, x] = building;
            ChangeHappiness(building.Happiness);
            ChangeMoney(-building.Cost);
        }

        public void DeleteBuilding(int x, int y)
        {
            map[y, x].Delete(this);
            map[y, x] = null;
        }

        public bool IsCitizien(int id)
        {
            return citiziens.ContainsKey(id);
        }

        public void DeleteCitizien(int citizienId)
        {
            RetireWorker(citizienId);
            if (citiziens.ContainsKey(citizienId))
            {
                if (map[citiziens[citizienId].Item2, citiziens[citizienId].Item1] == null)
                    throw new Exception("Living house doesn't exist");
                (map[citiziens[citizienId].Item2, citiziens[citizienId].Item1] as LivingHouse)
                    .DeleteLiver(citizienId, this);
                citiziens.Remove(citizienId);
            }
        }

        public bool CanBecomeWorker(int workerId)
        {
            return citiziens.ContainsKey(workerId) && !workingCitiziens.ContainsKey(workerId);
        }

        public bool IsWorker(int workerId)
        {
            return citiziens.ContainsKey(workerId) && workingCitiziens.ContainsKey(workerId);
        }

        public void AddWorker(int workerId, int x, int y)
        {
            if (CanBecomeWorker(workerId))
                workingCitiziens[workerId] = (x, y);
        }

        public void RetireWorker(int workerId)
        {
            if (workingCitiziens.ContainsKey(workerId))
                workingCitiziens.Remove(workerId);
        }

        public void ChangeCitiziensLimit(int deltaL)
        {
            citiziensLimit += deltaL;
        }
        
        public void ChangeMoney(int deltaM)
        {
            money += deltaM;
            if (money < 0)
                money = 0;
        }

        public void ChangeIncomeMoney(int deltaM)
        {
            incomeMoney += deltaM;
        }
        
        private void PayDay(object obj)
        {
            ChangeMoney(incomeMoney);
        }
        
        public void ChangeAttackTimer(int deltaT)
        {
            attackTimerInterval += deltaT; //ms
        }
        
        public void ChangeHappiness(int happiness)
        {
            newCitizienTimerInterval -= happiness * 500; //ms
        }
        
        private void Attack(object obj)
        {
            var bandits = new Bandits(this);
            bandits.FindBuildingsToRaid(this);
            bandits.Raid(this);
            attackTimer.Change(attackTimerInterval, attackTimerInterval); //по идее должно сработать через attackTimerInterval
        }
        
        private void AddCitizien(object obj)
        {
            if (citiziens.Count >= citiziensLimit)
                throw new Exception("Citiziens limit exceeded");
            
            foreach (var building in map)
            {
                if (building is LivingHouse house && house.HavePlace)
                {
                    house.AddLiver(newCitizienId, this);
                    citiziens[newCitizienId] = (building.X, building.Y);
                    break;
                }
            }

            newCitizienId++;
            newCitizienTimer.Change(newCitizienTimerInterval, newCitizienTimerInterval); //по идее должно сработать через newCitizienTimerInterval
        }
    }
}