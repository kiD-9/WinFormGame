using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace GoldenCity.Models.Tests
{
    [TestFixture]
    public class GameSettingTests
    {
        private GameSetting gameSetting;
        private Dictionary<int, (int, int)> gameCitizens;
        private Dictionary<int, (int, int)> gameWorkingCitizens;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(3, 4000, true);
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
        public void CanBuild()
        {
            var store = new Store(0, 1);
            gameSetting.AddBuilding(store);
            Assert.AreEqual(1000, gameSetting.Money);
            Assert.AreEqual(store, gameSetting.Map[1, 0]);
        }
        
        [Test]
        public void CantBuildIfNotEmptyPlace()
        {
            var ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new Store(0, 0)));
            Assert.That(ex.Message, Is.EqualTo("No space to build"));
            Assert.AreEqual(4000, gameSetting.Money);
        }
        
        [Test]
        public void CantBuildIfNotEnoughMoney()
        {
            var ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new SheriffsHouse(0, 1)));
            Assert.That(ex.Message, Is.EqualTo("Not enough money to build"));
            Assert.AreEqual(4000, gameSetting.Money);
        }

        [Test]
        public void NoCitizensWhenCreated()
        {
            Assert.AreEqual(new Dictionary<int, (int, int)>(), gameCitizens);
        }

        [Test]
        public void AddCitizens()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            if (!(gameSetting.Map[0, 0] is LivingHouse livingHouse))
                throw new NullReferenceException();
            Assert.AreEqual(false, livingHouse.HavePlace);
            
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual((0, 0), gameCitizens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizen(i));
            }
        }

        [Test]
        public void CheckCitizenLimitExceeded()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            var ex = Assert.Throws<Exception>(() => gameSetting.AddCitizen());
            Assert.That(ex.Message, Is.EqualTo("Citizens limit exceeded"));
        }
        
        [Test]
        public void AddLivingHouse_AddMoreCitizens()
        {
            gameSetting.AddBuilding(new LivingHouse(0, 1));
            Assert.AreEqual(3500, gameSetting.Money);

            for (var i = 0; i < 2 * LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            if (!(gameSetting.Map[0, 0] is LivingHouse firstLivingHouse))
                throw new NullReferenceException();
            if (!(gameSetting.Map[1, 0] is LivingHouse secondLivingHouse))
                throw new NullReferenceException();
            
            Assert.AreEqual(false, firstLivingHouse.HavePlace);
            Assert.AreEqual(false, secondLivingHouse.HavePlace);
            
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual((0, 0), gameCitizens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizen(i));
            }

            for (var i = LivingHouse.LivingPlaces; i < 2 * LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual((0, 1), gameCitizens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizen(i));
            }
        }

        [Test]
        public void DeleteLivingHouseAfterAddingLivers()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            gameSetting.DeleteBuilding(0, 0);
            Assert.AreEqual(null, gameSetting.Map[0, 0]);
            Assert.AreEqual(false, gameCitizens.Any());
        }

        [Test]
        public void CheckAddSheriffsHouse()
        {
            gameSetting.ChangeMoney(5000);
            gameSetting.AddCitizen();
            gameSetting.AddCitizen();
            gameSetting.AddBuilding(new SheriffsHouse(0, 1));
            Assert.AreEqual(0, gameSetting.SheriffsCount);
            gameSetting.AddWorker(gameSetting.Map[1, 0]);
            Assert.AreEqual(0, gameSetting.Map[1, 0].WorkerId);
            Assert.AreEqual(1, gameSetting.SheriffsCount);
            var bandits = new Bandits(gameSetting);
            Assert.AreEqual(2, bandits.BuildingsToRaid.Length);
        }

        [Test]
        public void CheckAddJail()
        {
            gameSetting.AddCitizen();
            gameSetting.AddCitizen();
            gameSetting.AddBuilding(new Jail(0, 1));
            Assert.AreEqual(15, gameSetting.AttackTimerInterval / 1000);
            gameSetting.AddWorker(gameSetting.Map[1, 0]);
            Assert.AreEqual(0, gameSetting.Map[1, 0].WorkerId);
            Assert.AreEqual(18, gameSetting.AttackTimerInterval / 1000);
        }
        
        [Test]
        public void CheckPayday()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }
            var store = new Store(0, 1);
            gameSetting.AddBuilding(store);
            gameSetting.AddWorker(store);
            gameSetting.PayDay();
            Assert.AreEqual(2500, gameSetting.Money);
        }

        [Test]
        public void CheckAttack()
        {
            gameSetting.ChangeMoney(7000);
            gameSetting.AddBuilding(new Store(0,1));
            gameSetting.AddBuilding(new Store(1, 0));
            gameSetting.AddBuilding(new SheriffsHouse(1, 1));

            for (var i = 0; i < 4; i++)
            {
                gameSetting.AddCitizen();
            }

            gameSetting.AddWorker(gameSetting.Map[1, 0]);
            gameSetting.AddWorker(gameSetting.Map[0, 1]);
            gameSetting.AddWorker(gameSetting.Map[1, 1]);
            Assert.AreEqual(1, gameSetting.SheriffsCount);
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.PayDay();
            Assert.AreEqual(3000, gameSetting.Money);
            gameSetting.Attack();
            Assert.AreEqual(2400, gameSetting.Money); //2400, because (gameSetting.MapSize - gameSetting.SheriffsCount) == 2
            Assert.AreEqual(1, gameWorkingCitizens.Count);
            Assert.AreEqual(2, gameSetting.Map[1, 1].WorkerId);
            Assert.AreEqual(2, gameSetting.CitizensCount);
            
            if (!(gameSetting.Map[0, 0] is LivingHouse livingHouse))
                throw new NullReferenceException();
            for (var i = 2; i < 4; i++)
            {
                Assert.AreEqual(i, livingHouse[i]);
            }
        }

        [Test]
        public void CheckWin()
        {
            gameSetting.DeleteBuilding(0, 0);
            
            var ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new TownHall(0, 1)));
            Assert.That(ex.Message, Is.EqualTo("Not enough money to build"));
            Assert.AreEqual(4000, gameSetting.Money);
            Assert.AreEqual(false, gameSetting.IsGameFinished);
            gameSetting.ChangeMoney(146000);
            Assert.AreEqual(150000, gameSetting.Money);
            
            ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new TownHall(0, 1)));
            Assert.That(ex.Message, Is.EqualTo("To build town hall you need more than 24 livers"));
            Assert.AreEqual(false, gameSetting.IsGameFinished);
            gameSetting.ChangeMoney(3000);
            for (var i = 0; i < 6; i++)
            {
                gameSetting.AddBuilding(new LivingHouse(i % gameSetting.MapSize, i / gameSetting.MapSize));
                for (var j = 0; j < LivingHouse.LivingPlaces; j++)
                    gameSetting.AddCitizen();
            }
            

            Assert.AreEqual(150000, gameSetting.Money);
            gameSetting.AddBuilding(new TownHall(2, 2));
            Assert.AreEqual(0, gameSetting.Money);
            Assert.AreEqual(true, gameSetting.IsGameFinished);
        }
    }
}