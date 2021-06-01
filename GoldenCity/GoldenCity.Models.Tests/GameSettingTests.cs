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
            gameSetting = new GameSetting(2, 4000, true);
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
        public void NoCitiziensWhenCreated()
        {
            Assert.AreEqual(new Dictionary<int, (int, int)>(), gameCitizens);
        }

        [Test]
        public void AddCitiziens()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            Assert.AreEqual(false, (gameSetting.Map[0, 0] as LivingHouse).HavePlace);
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
        public void AddLivingHouse_AddMoreCitiziens()
        {
            gameSetting.AddBuilding(new LivingHouse(0, 1));
            Assert.AreEqual(3500, gameSetting.Money);

            for (var i = 0; i < 2 * LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizen();
            }

            Assert.AreEqual(false, (gameSetting.Map[0, 0] as LivingHouse).HavePlace);
            Assert.AreEqual(false, (gameSetting.Map[1, 0] as LivingHouse).HavePlace);
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
            Assert.AreEqual(2700, gameSetting.Money); //2700, так как максимум атак бандитов (gameSetting.MapSize - gameSetting.SheriffsCount), т.е. 1
            Assert.AreEqual(2, gameWorkingCitizens.Count);
            Assert.AreEqual(2, gameSetting.Map[1, 1].WorkerId);
            Assert.AreEqual(3, gameSetting.CitizensCount);
            for (var i = 1; i < 4; i++)
            {
                Assert.AreEqual(i, (gameSetting.Map[0, 0] as LivingHouse)[i]);
            }
        }
    }
}