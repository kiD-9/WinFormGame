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
        public readonly Dictionary<int, (int,int)> citiziens; //public для возможности тестирования
        public readonly Dictionary<int, (int,int)> workingCitiziens;

        private bool isTestNow;


        public GameSetting(int mapSize)
        {
            map = new Building[mapSize, mapSize];
            
            attackTimerInterval = 120000; //ms
            newCitizienTimerInterval = 45000; //ms
            money = 4000;
            
            payTimer = new Timer(PayDay, null, PayTimerInterval, PayTimerInterval);
            attackTimer = new Timer(Attack, null, attackTimerInterval, attackTimerInterval);
            newCitizienTimer = new Timer(AddCitizien, null, 0, newCitizienTimerInterval);

            map[0, 0] = new LivingHouse(0, 0);
            citiziens = new Dictionary<int, (int, int)>(); //первый житель должен сам добавиться при создании, т.к. в таймере 0
            workingCitiziens = new Dictionary<int, (int, int)>();
        }
        
        public GameSetting(int mapSize, bool isTest) //без таймеров, чтобы протестировать логику модели
        {
            isTestNow = true;
            
            map = new Building[mapSize, mapSize];
            
            attackTimerInterval = 10000; //ms
            newCitizienTimerInterval = 5000; //ms
            money = 4000;

            //payTimer = new Timer(PayDay, null, PayTimerInterval, PayTimerInterval);
            //attackTimer = new Timer(Attack, null, attackTimerInterval, attackTimerInterval);
            //newCitizienTimer = new Timer(AddCitizien, null, 0, newCitizienTimerInterval);

            map[0, 0] = new LivingHouse(0, 0);
            citiziens = new Dictionary<int, (int, int)>();
            workingCitiziens = new Dictionary<int, (int, int)>();
        }

        public Building[,] Map => map;
        public int Money => money;
        public int AttackTimerInterval => attackTimerInterval;
        public int NewCitizienTimerInterval => newCitizienTimerInterval;
        public int Sheriffs { get; set; } //проверка на >2 в SheriffsHouse

        public void StartTimers()
        {
            
        }

        public void AddBuilding(Building building)
        {
            if (map[building.Y, building.X] != null || (money - building.Cost < 0))
                throw new Exception("Can't build");
            
            // if (building is SheriffsHouse && Sheriffs == 2) //реализовано в классе SheriffsHouse
            //     return;

            map[building.Y, building.X] = building;
            ChangeHappiness(building.Happiness);
            ChangeMoney(-building.Cost);
            
            if (building is LivingHouse)
            {
                ChangeCitiziensLimit(LivingHouse.LivingPlaces);
            }
        }

        public void DeleteBuilding(int x, int y)
        {
            var building = map[y, x];
            RetireWorker(building.WorkerId);
            ChangeHappiness(-building.Happiness);
            
            switch (building)
            {
                case LivingHouse livingHouse:
                    ChangeCitiziensLimit(-LivingHouse.LivingPlaces);
                    livingHouse.DeleteBuilding();
                    break;
                
                case Jail jail:
                    jail.DeleteBuilding();
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    sheriffsHouse.DeleteBuilding();
                    break;
                
                default:
                    building.DeleteBuilding();
                    break;
            }
            
            map[y, x] = null;
        }

        public void DeleteCitizien(int id)
        {
            if (!IsCitizien(id))
                return; //not citizien
            
            if (IsWorker(id))
                RetireWorker(id);

            var livingHouse = map[citiziens[id].Item2, citiziens[id].Item1] as LivingHouse;
            if (livingHouse == null)
                throw new Exception("Living house doesn't exist");
            
            livingHouse.DeleteLiver(id);
            citiziens.Remove(id);
        }

        public bool CanBecomeWorker(int id)
        {
            return IsCitizien(id) && !IsWorker(id);
        }

        public bool IsWorker(int id)
        {
            return IsCitizien(id) && workingCitiziens.ContainsKey(id);
        }

        public bool IsCitizien(int id)
        {
            return citiziens.ContainsKey(id);
        }

        public void AddWorker(int id, Building building)
        {
            if (!CanBecomeWorker(id))
                return; //cant become worker
            
            ////////////////throw new Exception("Can't become worker");
            
            ChangeIncomeMoney(building.IncomeMoney);
            workingCitiziens[id] = (building.X, building.Y);
            
            switch (building)
            {
                case Jail jail:
                    jail.AddWorker(id);
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    sheriffsHouse.AddWorker(id);
                    break;
                
                default:
                    building.AddWorker(id);
                    break;
            }
        }

        public void RetireWorker(int id)
        {
            if (!IsWorker(id))
                return; //isnt worker
            
            /////////////////throw new Exception("Isn't worker");

            var workingPlace = map[workingCitiziens[id].Item2, workingCitiziens[id].Item1];
            ChangeIncomeMoney(-workingPlace.IncomeMoney);
            workingCitiziens.Remove(id);
            
            switch (workingPlace)
            {
                case Jail jail:
                    jail.RemoveWorker();
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    sheriffsHouse.RemoveWorker();
                    break;
                
                default:
                    workingPlace.RemoveWorker();
                    break;
            }
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
        
        public void Attack(object obj)
        {
            var bandits = new Bandits(this);
            bandits.FindBuildingsToRaid(this);
            bandits.Raid(this);
            
            if (!isTestNow)
                attackTimer.Change(attackTimerInterval, attackTimerInterval); //по идее должно сработать через attackTimerInterval
        }
        
        public void AddCitizien(object obj)
        {
            if (citiziens.Count >= citiziensLimit)
                throw new Exception("Citiziens limit exceeded");
            
            foreach (var building in map)
            {
                if (building is LivingHouse house && house.HavePlace)
                {
                    citiziens[newCitizienId] = (building.X, building.Y);
                    house.AddLiver(newCitizienId);
                    break;
                }
            }

            newCitizienId++;
            
            if (!isTestNow)
                newCitizienTimer.Change(newCitizienTimerInterval, newCitizienTimerInterval); //по идее должно сработать через newCitizienTimerInterval
        }
    }
}