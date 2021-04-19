using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GoldenCity.Models.Tests
{
    [TestFixture]
    public class BanditsTests
    {
        private GameSetting gameSetting;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(2, 4000, true); //без таймера для теста логики
        }
        
        [Test]
        public void CheckAttack()
        {
            gameSetting.ChangeMoney(2000);
            gameSetting.AddBuilding(new Store(0,1));
            gameSetting.AddBuilding(new Store(1, 0));

            for (var i = 0; i < 2; i++)
            {
                gameSetting.AddCitizien(null);
            }

            gameSetting.AddWorker(0, gameSetting.Map[1, 0]);
            gameSetting.AddWorker(1, gameSetting.Map[0, 1]);
            
            Assert.AreEqual(0, gameSetting.Money);
            gameSetting.ChangeMoney(100);
            gameSetting.Attack(null);
            Assert.AreEqual(68, gameSetting.Money); //68, т.к. store+store+livingHouse = 32%
            Assert.AreEqual(false, gameSetting.citiziens.Any());
            Assert.AreEqual(false, gameSetting.workingCitiziens.Any());
        }
    }
}