using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GoldenCity.Models.Tests
{
    [TestFixture]
    public class GameSettingTests
    {
        private GameSetting gameSetting;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(2, 4000, true); //без таймера для теста логики
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
            Assert.That(ex.Message, Is.EqualTo("Can't build"));
            Assert.AreEqual(4000, gameSetting.Money);
        }
        
        [Test]
        public void CantBuildIfNotEnoughMoney()
        {
            var ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new SheriffsHouse(0, 1)));
            Assert.That(ex.Message, Is.EqualTo("Can't build"));
            Assert.AreEqual(4000, gameSetting.Money);
        }

        [Test]
        public void NoCitiziensWhenCreated()
        {
            Assert.AreEqual(new Dictionary<int, (int, int)>(), gameSetting.citiziens);
        }

        [Test]
        public void AddCitiziens()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizien(null);
            }

            Assert.AreEqual(false, (gameSetting.Map[0, 0] as LivingHouse).HavePlace);
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual((0, 0), gameSetting.citiziens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizien(i));
            }
        }

        [Test]
        public void CheckCitizienLimitExceeded()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizien(null);
            }

            var ex = Assert.Throws<Exception>(() => gameSetting.AddCitizien(null));
            Assert.That(ex.Message, Is.EqualTo("Citiziens limit exceeded"));
        }
        
        [Test]
        public void AddLivingHouse_AddMoreCitiziens()
        {
            gameSetting.AddBuilding(new LivingHouse(0, 1));
            Assert.AreEqual(3500, gameSetting.Money);

            for (var i = 0; i < 2 * LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizien(null);
            }

            Assert.AreEqual(false, (gameSetting.Map[0, 0] as LivingHouse).HavePlace);
            Assert.AreEqual(false, (gameSetting.Map[1, 0] as LivingHouse).HavePlace);
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual((0, 0), gameSetting.citiziens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizien(i));
            }

            for (var i = LivingHouse.LivingPlaces; i < 2 * LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual((0, 1), gameSetting.citiziens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizien(i));
            }
        }

        [Test]
        public void DeleteLivingHouseAfterAddingLivers()
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizien(null);
            }

            gameSetting.DeleteBuilding(0, 0);
            Assert.AreEqual(null, gameSetting.Map[0, 0]);
            Assert.AreEqual(false, gameSetting.citiziens.Any());
        }

        [Test]
        public void CheckAddSheriffsHouse()
        {
            gameSetting.ChangeMoney(5000);
            gameSetting.AddCitizien(null);
            gameSetting.AddCitizien(null);
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
                gameSetting.AddCitizien(null);
            }
            var store = new Store(0, 1);
            gameSetting.AddBuilding(store);
            gameSetting.AddWorker(store);
            gameSetting.PayDay(null);
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
                gameSetting.AddCitizien(null);
            }

            gameSetting.AddWorker(gameSetting.Map[1, 0]);
            gameSetting.AddWorker(gameSetting.Map[0, 1]);
            gameSetting.AddWorker(gameSetting.Map[1, 1]);
            Assert.AreEqual(1, gameSetting.SheriffsCount);
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.PayDay(null);
            Assert.AreEqual(3000, gameSetting.Money);
            gameSetting.Attack(null);
            Assert.AreEqual(2100, gameSetting.Money);
            Assert.AreEqual(1, gameSetting.workingCitiziens.Count);
            Assert.AreEqual(2, gameSetting.Map[1, 1].WorkerId);
            Assert.AreEqual(2, gameSetting.citiziens.Count);
            for (var i = 2; i < 4; i++)
            {
                Assert.AreEqual(i, (gameSetting.Map[0, 0] as LivingHouse)[i]);
            }
        }
    }
}