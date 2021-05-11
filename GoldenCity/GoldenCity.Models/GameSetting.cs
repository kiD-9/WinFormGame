using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GoldenCity.Models
{
    public class GameSetting
    {
        public readonly Dictionary<int, (int,int)> citizens; //public для возможности тестирования
        public readonly Dictionary<int, (int,int)> workingCitizens;
        private Building[,] map;
        private int money;
        private int incomeMoney;
        private const int PayTimerInterval = 45000; // ms
        private Timer payTimer;
        private int attackTimerInterval; // ms
        private int attackTimerIntervalIncreaser;
        private Timer attackTimer;
        private int newCitizenTimerInterval; // ms
        private Timer newCitizenTimer;
        private int citizensLimit;
        private int newCitizenId;
        private int jailWorkersCount;
        private bool withoutTimers;

        public GameSetting(int mapSize, int startMoney = 500, bool withoutTimers = false) //withoutTimers = true, чтобы протестировать логику модели без таймеров
        {
            this.withoutTimers = withoutTimers;

            map = new Building[mapSize, mapSize];

            attackTimerInterval = 10000; //ms //TODO переделать баланс таймеров
            attackTimerIntervalIncreaser = 1000;
            newCitizenTimerInterval = 5000; //ms
            money = startMoney + 500; //вычтется 500 из money на следующем шаге

            AddBuilding(new LivingHouse(0, 0)); //вычтется 500 из money
            citizens = new Dictionary<int, (int, int)>();
            workingCitizens = new Dictionary<int, (int, int)>();
            // if (!withoutTimers)
            //     Start();
        }

        public Building[,] Map => map;
        public int Money => money;
        public int AttackTimerInterval => attackTimerInterval + jailWorkersCount * attackTimerIntervalIncreaser; //ms
        public int SheriffsCount { get; private set; }
        public int CitizensLimit => citizensLimit;

        public void Start()
        {
            payTimer = new Timer(PayDay, null, PayTimerInterval, PayTimerInterval);
            attackTimer = new Timer(Attack, null, attackTimerInterval, attackTimerInterval);
            newCitizenTimer = new Timer(AddCitizen, null, 0, newCitizenTimerInterval);
        }
        
        public void AddBuilding(Building building)
        {
            if (map[building.Y, building.X] != null)
                throw new Exception("No space to build");
            if (money - building.Cost < 0)
                throw new Exception("Not enough money to build");

            map[building.Y, building.X] = building;
            ChangeHappiness(building.Happiness);
            ChangeMoney(-building.Cost);
            
            if (building is LivingHouse)
            {
                ChangeCitizensLimit(LivingHouse.LivingPlaces);
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
                    ChangeCitizensLimit(-LivingHouse.LivingPlaces);
                    for (var i = 0; i < LivingHouse.LivingPlaces; i++)
                    {
                        DeleteCitizen(livingHouse[i]);
                    }
                    livingHouse.DeleteBuilding();
                    break;
                
                default:
                    building.DeleteBuilding();
                    break;
            }
            
            map[y, x] = null;
        }

        public void DeleteCitizen(int id)
        {
            if (!IsCitizen(id))
                throw new Exception("Citizen doesn't exist");
            
            if (IsWorker(id))
                RetireWorker(id);

            var livingHouse = map[citizens[id].Item2, citizens[id].Item1] as LivingHouse;
            if (livingHouse == null)
                throw new Exception("Living house doesn't exist");
            
            livingHouse.DeleteLiver(id);
            citizens.Remove(id);
        }

        public bool IsCitizen(int id)
        {
            return citizens.ContainsKey(id);
        }

        public void AddWorker(Building building)
        {
            if (workingCitizens.Count == citizens.Count)
                throw new Exception("Not enough citizens to add worker");
            
            var id = citizens.Keys.First(CanBecomeWorker);
            
            ChangeIncomeMoney(building.IncomeMoney);
            workingCitizens[id] = (building.X, building.Y);
            
            switch (building)
            {
                case Jail jail:
                    jailWorkersCount++;
                    jail.AddWorker(id);
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    if (SheriffsCount == 2)
                        throw new Exception("Can't be more than 2 sheriffs");
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

            var workingPlace = map[workingCitizens[id].Item2, workingCitizens[id].Item1];
            ChangeIncomeMoney(-workingPlace.IncomeMoney);
            workingCitizens.Remove(id);
            
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

        public void ChangeCitizensLimit(int deltaL)
        {
            citizensLimit += deltaL;
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
            newCitizenTimerInterval -= happiness * 500; //ms
        }
        
        public void Attack(object obj)
        {
            var bandits = new Bandits(this);
            bandits.FindBuildingsToRaid();
            bandits.Raid();
            
            if (!withoutTimers)
                attackTimer.Change(AttackTimerInterval, AttackTimerInterval); // должно сработать через attackTimerInterval
        }
        
        public void AddCitizen(object obj)
        {
            if (citizens.Count >= citizensLimit)
                throw new Exception("Citizens limit exceeded");
            
            foreach (var building in map)
            {
                if (building is LivingHouse house && house.HavePlace)
                {
                    citizens[newCitizenId] = (building.X, building.Y);
                    house.AddLiver(newCitizenId);
                    break;
                }
            }

            newCitizenId++;
            
            if (!withoutTimers)
                newCitizenTimer.Change(newCitizenTimerInterval, newCitizenTimerInterval); //по идее должно сработать через newCitizenTimerInterval
        }
        
        private bool CanBecomeWorker(int id)
        {
            return IsCitizen(id) && !IsWorker(id);
        }

        private bool IsWorker(int id)
        {
            return IsCitizen(id) && workingCitizens.ContainsKey(id);
        }
    }
}