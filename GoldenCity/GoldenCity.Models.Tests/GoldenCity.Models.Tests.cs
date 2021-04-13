using System;
using System.Collections.Generic;
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
            gameSetting = new GameSetting(2, true);
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
        }
        
        [Test]
        public void CantBuildIfNotEnoughMoney()
        {
            var ex = Assert.Throws<Exception>(() => gameSetting.AddBuilding(new SheriffsHouse(0, 1)));
            Assert.That(ex.Message, Is.EqualTo("Can't build"));
        }

        [Test]
        public void NoCitiziens()
        {
            Assert.AreEqual(new Dictionary<int, (int, int)>(), gameSetting.citiziens);
        }

        [Test]
        public void AddCitiziens()
        {
            for (var i = 0; i < 4; i++)
            {
                gameSetting.AddCitizien(null);
            }
            
            Assert.AreEqual(false, (gameSetting.Map[0, 0] as LivingHouse).HavePlace);
            for (var i = 0; i < 4; i++)
            {
                Assert.AreEqual((0, 0), gameSetting.citiziens[i]);
                Assert.AreEqual(true, gameSetting.IsCitizien(i));
            }
        }

        [Test]
        public void CitizienLimitExceeded()
        {
            for (var i = 0; i < 4; i++)
            {
                gameSetting.AddCitizien(null);
            }
            
            var ex = Assert.Throws<Exception>(() => gameSetting.AddCitizien(null));
            Assert.That(ex.Message, Is.EqualTo("Citiziens limit exceeded"));
        }
    }
}