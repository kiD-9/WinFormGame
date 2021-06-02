using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace GoldenCity.Models.Tests
{
    [TestFixture]
    public class BanditsTests
    {
        private GameSetting gameSetting;
        private Bandits bandits;
        private Dictionary<int, (int, int)> gameCitizens;
        private Dictionary<int, (int, int)> gameWorkingCitizens;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(3, 6000, true);
            bandits = new Bandits(gameSetting);
            var fieldInfo = typeof(GameSetting).GetField("citizens", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                throw new NullReferenceException();
            gameCitizens = (Dictionary<int, (int, int)>)fieldInfo.GetValue(gameSetting);
            fieldInfo = typeof(GameSetting).GetField("workingCitizens", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                throw new NullReferenceException();
            gameWorkingCitizens = (Dictionary<int, (int, int)>) fieldInfo.GetValue(gameSetting);
        }

        [Test]
        public void Check_AddBuildingToRaid()
        {
            var methodInfo =
                typeof(Bandits).GetMethod("AddBuildingToRaid", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
                throw new NullReferenceException();
            var railroadStation = new RailroadStation(0, 1);
            railroadStation.AddWorker(0);
            methodInfo.Invoke(bandits, new object[] {railroadStation});
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 3], null);
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 2], null);
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 1], railroadStation);
            var store = new Store(0, 0);
            store.AddWorker(1);
            methodInfo.Invoke(bandits, new object[] {store});
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 3], null);
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 2], railroadStation);
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 1], store);

            var saloon = new Saloon(1, 0);
            saloon.AddWorker(2);
            methodInfo.Invoke(bandits, new object[] {saloon});
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 3], railroadStation);
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 2], saloon);
            Assert.AreEqual(bandits.BuildingsToRaid[gameSetting.MapSize - 1], store);
        }

        [Test]
        public void Simple_CheckAttack()
        {
            gameSetting.AddBuilding(new Store(0,1));
            gameSetting.AddBuilding(new Store(1, 0));

            for (var i = 0; i < 2; i++)
            {
                gameSetting.AddCitizen();
            }

            gameSetting.AddWorker(gameSetting.Map[1, 0]);
            gameSetting.AddWorker(gameSetting.Map[0, 1]);
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.ChangeMoney(100);
            gameSetting.Attack();
            Assert.AreEqual(80, gameSetting.Money); //80, т.к. store + store = 20 (livingHouse.WorkerId < 0)
            Assert.AreEqual(false, gameCitizens.Any());
            Assert.AreEqual(false, gameWorkingCitizens.Any());
        }
        
        [Test]
        public void CheckAttack()
        {
            gameSetting.ChangeMoney(1500);
            gameSetting.AddBuilding(new Saloon(2, 0));
            gameSetting.AddBuilding(new Store(0,1));
            gameSetting.AddBuilding(new Store(1, 0));

            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            gameSetting.AddWorker(gameSetting.Map[0, 2]);
            gameSetting.AddWorker(gameSetting.Map[0, 1]);
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.ChangeMoney(100);
            gameSetting.Attack();
            Assert.AreEqual(82, gameSetting.Money); //82, т.к. store + saloon = 18 (livingHouse.WorkerId < 0, store(1,0).WorkerId < 0)
            Assert.AreEqual(2, gameCitizens.Count);
            Assert.AreEqual(false, gameWorkingCitizens.Any());
        }
        
        [Test]
        public void CheckAttackOnBigMap()
        {
            
            gameSetting = new GameSetting(10, 2500, true);
            bandits = new Bandits(gameSetting);
            var fieldInfo = typeof(GameSetting).GetField("citizens", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                throw new NullReferenceException();
            gameCitizens = (Dictionary<int, (int, int)>)fieldInfo.GetValue(gameSetting);
            fieldInfo = typeof(GameSetting).GetField("workingCitizens", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                throw new NullReferenceException();
            gameWorkingCitizens = (Dictionary<int, (int, int)>) fieldInfo.GetValue(gameSetting);
            if (gameCitizens == null || gameWorkingCitizens == null)
                throw new NullReferenceException();
            
            
            gameSetting.DeleteBuilding(0, 0);
            for (var i = 0; i < 5; i++)
            {
                gameSetting.AddBuilding(new LivingHouse(i, 0));
                for (var j = 0; j < LivingHouse.LivingPlaces; j++)
                {
                    gameSetting.AddCitizen();
                }
            }
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.ChangeMoney(30000);
            for (var i = 0; i < 20; i++)
            {
                var saloon = new Saloon(i % gameSetting.MapSize, i / gameSetting.MapSize + 1);
                gameSetting.AddBuilding(saloon);
                gameSetting.AddWorker(saloon);
            }
            Assert.AreEqual(0, gameSetting.Money);
            Assert.AreEqual(true, gameCitizens.Count == gameWorkingCitizens.Count);
            
            gameSetting.ChangeMoney(100);
            gameSetting.Attack();
            Assert.AreEqual(20, gameSetting.Money);
            Assert.AreEqual(10, gameCitizens.Count);
            Assert.AreEqual(10, gameWorkingCitizens.Count);

            gameSetting.ChangeMoney(-20);
            gameSetting.ChangeMoney(25000);
            for (var i = 0; i < 5; i++)
            {
                gameSetting.AddCitizen();
                var sheriffsHouse = new SheriffsHouse(i, 3);
                gameSetting.AddBuilding(sheriffsHouse);
                gameSetting.AddWorker(sheriffsHouse);
            }

            Assert.AreEqual(0, gameSetting.Money);
            Assert.AreEqual(5, gameSetting.SheriffsCount);
            gameSetting.ChangeMoney(100);
            gameSetting.Attack();
            Assert.AreEqual(60, gameSetting.Money);
            Assert.AreEqual(10, gameCitizens.Count);
            Assert.AreEqual(10, gameWorkingCitizens.Count);
        }
    }
}