using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GoldenCity.Models.Tests
{
    [TestFixture]
    public class Tests
    {
        private GameSetting gameSetting;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(2, 4000, true); //без таймера для теста логики
        }

        [Test]
        public void CheckBuilding()
        {
            var building = new Building(0, 0);
            building.AddWorker(-5);
            Assert.AreEqual(-1, building.WorkerId);
            building.AddWorker(20);
            Assert.AreEqual(20, building.WorkerId);
            building.RemoveWorker();
            Assert.AreEqual(-1, building.WorkerId);
            building.DeleteBuilding();
            Assert.AreEqual(-1, building.WorkerId);
        }
        
        [Test]
        public void CheckLivingHouse()
        {
            var livingHouse = new LivingHouse(0, 0);
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual(true, livingHouse.HavePlace);
                livingHouse.AddLiver(i);
            }

            Assert.AreEqual(false, livingHouse.HavePlace);
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                Assert.AreEqual(i, livingHouse[i]);
            }

            livingHouse.DeleteLiver(2);
            Assert.AreEqual(-1, livingHouse[2]);
            livingHouse.AddLiver(5);
            Assert.AreEqual(5, livingHouse[2]);
        }

        


        [Test]
        public void CanBuild()
        {
            var shop = new Shop(0, 1);
            gameSetting.AddBuilding(shop);
            Assert.AreEqual(1000, gameSetting.Money);
            Assert.AreEqual(shop, gameSetting.Map[1, 0]);
        }
        
        [Test]
        public void CantBuildIfNotEmptyPlace()
        {
            var ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new Shop(0, 0)));
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
        public void NoCitiziens()
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
        public void CitizienLimitExceeded()
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
        public void AddSheriffsHouse()
        {
            gameSetting.ChangeMoney(5000);
            gameSetting.AddCitizien(null);
            gameSetting.AddCitizien(null);
            gameSetting.AddBuilding(new SheriffsHouse(0, 1));
            Assert.AreEqual(0, gameSetting.SheriffsCount);
            gameSetting.AddWorker(1, gameSetting.Map[1, 0]);
            Assert.AreEqual(1, gameSetting.Map[1, 0].WorkerId);
            Assert.AreEqual(1, gameSetting.SheriffsCount);
        }

        [Test]
        public void CheckPayday() //TODO изменить incomeMoney и вызвать Payday
        {
            for (var i = 0; i < LivingHouse.LivingPlaces; i++)
            {
                gameSetting.AddCitizien(null);
            }
            var shop = new Shop(0, 1);
            gameSetting.AddBuilding(shop);
            gameSetting.AddWorker(0, shop);
            gameSetting.PayDay(null);
            Assert.AreEqual(2500, gameSetting.Money);
        }

        [Test]
        public void CheckBandits_GameSettingAttack()
        {
            gameSetting.ChangeMoney(2000);
            gameSetting.AddBuilding(new Shop(0,1));
            gameSetting.AddBuilding(new Shop(1, 0));

            for (var i = 0; i < 2; i++)
            {
                gameSetting.AddCitizien(null);
            }

            gameSetting.AddWorker(0, gameSetting.Map[1, 0]);
            gameSetting.AddWorker(1, gameSetting.Map[0, 1]);
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.ChangeMoney(100);
            gameSetting.Attack(null);
            Assert.AreEqual(68, gameSetting.Money); //68, т.к. shop+shop+livingHouse = 32%
            Assert.AreEqual(false, gameSetting.citiziens.Any());
            Assert.AreEqual(false, gameSetting.workingCitiziens.Any());
        }
    }
}