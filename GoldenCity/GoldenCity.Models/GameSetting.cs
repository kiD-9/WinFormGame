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
        private int attackTimerIntervalIncreaser;
        private Timer attackTimer;
        private int newCitizienTimerInterval; //время ms
        private Timer newCitizienTimer;
        private int citiziensLimit;
        private int newCitizienId;
        public readonly Dictionary<int, (int,int)> citiziens; //public для возможности тестирования
        public readonly Dictionary<int, (int,int)> workingCitiziens;
        private int jailWorkersCount;

        private bool withoutTimers;

        public GameSetting(int mapSize, int startMoney = 500, bool withoutTimers = false) //withoutTimer = true, чтобы протестировать логику модели без таймеров
        {
            this.withoutTimers = withoutTimers;

            map = new Building[mapSize, mapSize];

            attackTimerInterval = 10000; //ms
            attackTimerIntervalIncreaser = 1000;
            newCitizienTimerInterval = 5000; //ms
            money = startMoney + 500; //вычтется 500 из money на следующем шаге

            AddBuilding(new LivingHouse(0, 0)); //вычтется 500 из money
            citiziens = new Dictionary<int, (int, int)>();
            workingCitiziens = new Dictionary<int, (int, int)>();

            if (!withoutTimers)
                StartTimers();
        }

        public Building[,] Map => map;
        public int Money => money;
        private int AttackTimerInterval => attackTimerInterval + jailWorkersCount * attackTimerIntervalIncreaser; //ms
        public int SheriffsCount { get; private set; }

        public void AddBuilding(Building building)
        {
            if (map[building.Y, building.X] != null || (money - building.Cost < 0))
                throw new Exception("Can't build");

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
                    for (var i = 0; i < LivingHouse.LivingPlaces; i++)
                    {
                        DeleteCitizien(livingHouse[i]);
                    }
                    livingHouse.DeleteBuilding();
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
                    jailWorkersCount++;
                    jail.AddWorker(id);
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    if (SheriffsCount == 2)
                        return; //cant be more than 2 sheriffs
                    SheriffsCount++;
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
                    jailWorkersCount--;
                    jail.RemoveWorker();
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    SheriffsCount--;
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
        
        public void PayDay(object obj) //должно вызываться из payTimer
        {
            ChangeMoney(incomeMoney);
        }

        public void ChangeHappiness(int happiness)
        {
            newCitizienTimerInterval -= happiness * 500; //ms
        }
        
        public void Attack(object obj)
        {
            var bandits = new Bandits(this);
            bandits.FindBuildingsToRaid();
            bandits.Raid();
            
            if (!withoutTimers)
                attackTimer.Change(AttackTimerInterval, AttackTimerInterval); //по идее должно сработать через attackTimerInterval
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
            
            if (!withoutTimers)
                newCitizienTimer.Change(newCitizienTimerInterval, newCitizienTimerInterval); //по идее должно сработать через newCitizienTimerInterval
        }
        
        private void StartTimers()
        {
            payTimer = new Timer(PayDay, null, PayTimerInterval, PayTimerInterval);
            attackTimer = new Timer(Attack, null, attackTimerInterval, attackTimerInterval);
            newCitizienTimer = new Timer(AddCitizien, null, 0, newCitizienTimerInterval);
        }
        
        private bool CanBecomeWorker(int id)
        {
            return IsCitizien(id) && !IsWorker(id);
        }

        private bool IsWorker(int id)
        {
            return IsCitizien(id) && workingCitiziens.ContainsKey(id);
        }
    }
}