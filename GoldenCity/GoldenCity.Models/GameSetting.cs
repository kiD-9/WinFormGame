using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GoldenCity.Models
{
    public class GameSetting
    {
        public const int PayTimerInterval = 4500; // ms //TODO
        private readonly Dictionary<int, (int,int)> citizens;
        private readonly Dictionary<int, (int,int)> workingCitizens;
        private int incomeMoney;
        private readonly int attackTimerInterval; // ms
        private readonly int attackTimerIntervalIncrease; //ms
        private int newCitizenId;
        private int jailWorkersCount;

        public GameSetting(int mapSize, int startMoney = 500, bool isTest = false)
        {
            Map = new Building[mapSize, mapSize];
            MapSize = mapSize;

            attackTimerInterval = 15000; //ms //TODO balance timers
            attackTimerIntervalIncrease = 2500; //ms
            NewCitizenTimerInterval = 5000; //ms
            Money = startMoney + 500; // -500 money on next step

            AddBuilding(new LivingHouse(0, 0)); // -500 money
            citizens = new Dictionary<int, (int, int)>();
            workingCitizens = new Dictionary<int, (int, int)>();
            BuildingsToRaid = new List<Building>();

            if (!isTest)
                for (var i = 0; i < 4; i++)
                    AddCitizen();
        }

        public Building[,] Map { get; }
        public int MapSize { get; }
        public List<Building> BuildingsToRaid { get; private set; }
        public int Money { get; private set; }
        public int NewCitizenTimerInterval { get; private set; }
        public int AttackTimerInterval => attackTimerInterval + jailWorkersCount * attackTimerIntervalIncrease; //ms
        public int SheriffsCount { get; private set; }
        public int CitizensLimit { get; private set; }
        public int CitizensCount => citizens.Count;
        public bool IsGameFinished { get; private set; }

        public void AddBuilding(Building building)
        {
            if (Map[building.Y, building.X] != null)
                throw new Exception("No space to build");
            if (Money - building.Cost < 0)
                throw new Exception("Not enough money to build");

            if (building is TownHall)
            {
                if (CitizensCount < MapSize * 8)
                    throw new Exception("To build town hall you need more than 40 livers");
                IsGameFinished = true;
            }
            
            Map[building.Y, building.X] = building;
            ChangeHappiness(building.Happiness);
            ChangeMoney(-building.Cost);
            
            if (building is LivingHouse)
                ChangeCitizensLimit(LivingHouse.LivingPlaces);
        }

        public void DeleteBuilding(int x, int y)
        {
            var building = Map[y, x];
            RetireWorker(building.WorkerId);
            ChangeHappiness(-building.Happiness);
            
            switch (building)
            {
                case LivingHouse livingHouse:
                    ChangeCitizensLimit(-LivingHouse.LivingPlaces);
                    for (var i = 0; i < LivingHouse.LivingPlaces; i++)
                    {
                        if (livingHouse[i] >= 0)
                            DeleteCitizen(livingHouse[i]);
                    }
                    livingHouse.DeleteBuilding();
                    break;
                
                default:
                    building.DeleteBuilding();
                    break;
            }

            Map[y, x] = null;
        }

        public void DeleteCitizen(int id)
        {
            if (!IsCitizen(id))
                throw new Exception("Citizen doesn't exist");
            
            if (IsWorker(id))
                RetireWorker(id);

            var livingHouse = Map[citizens[id].Item2, citizens[id].Item1] as LivingHouse;
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
            
            switch (building)
            {
                case Jail jail:
                    jail.AddWorker(id);
                    jailWorkersCount++;
                    break;
                
                case SheriffsHouse sheriffsHouse:
                    if (SheriffsCount == MapSize / 2)
                        throw new Exception($"Can't be more than {MapSize - SheriffsCount} sheriffs");
                    sheriffsHouse.AddWorker(id);
                    SheriffsCount++;
                    break;
                
                default:
                    building.AddWorker(id);
                    break;
            }
            
            ChangeIncomeMoney(building.IncomeMoney);
            workingCitizens[id] = (building.X, building.Y);
        }

        public void RetireWorker(int id)
        {
            if (!IsWorker(id))
                return; //isn't worker

            var workingPlace = Map[workingCitizens[id].Item2, workingCitizens[id].Item1];
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
        
        public void Attack()
        {
            
            var bandits = new Bandits(this);
            bandits.FindBuildingsToRaid();
            BuildingsToRaid = bandits.BuildingsToRaid.ToList();
            bandits.Raid();
            // var bandits = new Bandits(this);
            // bandits.FindPathForRaid();
            // BuildingsToRaid = bandits.BuildingsToRaid;
            // bandits.Raid();
        }
        
        public void AddCitizen()
        {
            if (citizens.Count >= CitizensLimit)
                throw new Exception("Citizens limit exceeded");
            
            foreach (var building in Map)
            {
                if (building is LivingHouse house && house.HavePlace)
                {
                    citizens[newCitizenId] = (building.X, building.Y);
                    house.AddLiver(newCitizenId);
                    break;
                }
            }

            newCitizenId++;
        }
        
        public void ChangeMoney(int deltaM)
        {
            Money += deltaM;
            if (Money < 0)
                Money = 0;
        }

        public void PayDay() 
            => ChangeMoney(incomeMoney);

        public bool IsInsideMap(Point point)
            => point.X < MapSize && point.X >= 0 && point.Y < MapSize && point.Y >= 0;

        public bool IsEmpty(Point point)
            => Map[point.Y, point.X] == null;

        private void ChangeHappiness(int happiness)
        {
            if (NewCitizenTimerInterval - happiness * 500 >= 2500)
                NewCitizenTimerInterval -= happiness * 500; //ms
            else
                NewCitizenTimerInterval = 2500;
        }

        private void ChangeCitizensLimit(int deltaL) 
            => CitizensLimit += deltaL;

        private void ChangeIncomeMoney(int deltaM) 
            => incomeMoney += deltaM;

        private bool CanBecomeWorker(int id) 
            => IsCitizen(id) && !IsWorker(id);

        private bool IsWorker(int id) 
            => IsCitizen(id) && workingCitizens.ContainsKey(id);
    }
}