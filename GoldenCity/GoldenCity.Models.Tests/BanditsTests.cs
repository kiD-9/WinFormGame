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
            gameSetting = new GameSetting(2, 4000); //без таймера для теста логики
        }
        
        [Test]
        public void CheckAttack()
        {
            gameSetting.ChangeMoney(2000);
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
            Assert.AreEqual(68, gameSetting.Money); //68, т.к. store+store+livingHouse = 32%
            Assert.AreEqual(false, gameSetting.citizens.Any());
            Assert.AreEqual(false, gameSetting.workingCitizens.Any());
        }
    }
}