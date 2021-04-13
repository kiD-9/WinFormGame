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
        private int payTimerInterval; //время ms
        private Timer payTimer;
        private int attackTimer; //время ms
        private int newCitizienTimer; //время ms
        private int citiziensLimit;
        private int newCitizienId;
        private Dictionary<int, (int,int)> citiziens;
        private Dictionary<int, (int,int)> workingCitiziens;


        public GameSetting(int mapSize)
        {
            map = new Building[mapSize, mapSize];
            
            //не могу понять как работает System.Threading.Timers. Но нужно как-то передать gameSetting в другой поток и вызвать PayDay()
            //TODO создать таймеры
            payTimerInterval = 45000;
            payTimer = new Timer(PayDay, null, 0, payTimerInterval); //ms
            
            //Аналогично с PayTimer
            attackTimer = 120000; //ms
            newCitizienTimer = 45000; //ms
            money = 4000;

            map[0, 0] = new LivingHouse(0, 0, this);//TODO
            citiziens = new Dictionary<int, (int, int)>(); //следить за citiziensLimit и добавлять жителей по-нормальному (т.е. через таймер) => они сами добавятся в момент создания
            workingCitiziens = new Dictionary<int, (int, int)>();
        }

        public Building[,] Map => map;
        public int Money => money;
        public int Sheriffs { get; set; } //проверка на >2 в SheriffsHouse
        public int AttackTimer => attackTimer;
        public int NewCitizienTimer => newCitizienTimer;

        public void Start()
        {
            // если в конструкторе создаются таймеры, то здесь не нужно добавлять срабатывание таймеров через System.Threading.Timer?     
        }

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

        public void Attack() //TODO сделать срабатывание при AttackTimer
        {
            var bandits = new Bandits(this);
            bandits.FindBuildingsToRaid(this);
            bandits.Raid(this);
        }

        public bool IsCitizien(int id)
        {
            return citiziens.ContainsKey(id);
        }

        public void AddCitizien() //TODO сделать срабатывание при NewCitizienTimer
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
        
        public void PayDay(object? obj) //TODO сделать срабатывание при PayTimer 
        {
            ChangeMoney(incomeMoney);
        }
        
        public void ChangeAttackTimer(int deltaT) //TODO переделать, когда будет таймер
        {
            attackTimer += deltaT; //ms
        }
        
        public void ChangeHappiness(int happiness) //TODO переделать, когда будет таймер
        {
            newCitizienTimer -= happiness * 500; //ms
        }
    }
}