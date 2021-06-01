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
        private Dictionary<int, (int, int)> gameCitizens;
        private Dictionary<int, (int, int)> gameWorkingCitizens;
        
        [SetUp]
        public void Setup()
        {
            gameSetting = new GameSetting(2, 6000, true);
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
        public void CheckAttack()
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
    }
}