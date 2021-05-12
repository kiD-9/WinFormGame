using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GoldenCity.Models.Tests
{
    [TestFixture]
    public class BuildingTests
    {
        private GameSetting gameSetting;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(2, 4000); //без таймера для теста логики
        }
        
        [Test]
        public void ExceptionWhenNegativeWorkerIdGiven()
        {
            var building = new Building(0, 0);
            var ex = Assert.Throws<Exception>(() => building.AddWorker(-5));
            Assert.That(ex.Message, Is.EqualTo("Can't be worker with this id"));
        }
        
        [Test]
        public void CheckBuilding()
        {
            var building = new Building(0, 0);
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
    }
}