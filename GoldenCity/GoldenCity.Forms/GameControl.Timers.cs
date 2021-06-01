using System;
using System.Windows.Forms;
using GoldenCity.Models;

namespace GoldenCity.Forms
{
    public partial class GameControl
    {
        private Timer repaintTimer;
        private Timer gamePayTimer;
        private Timer gameAttackTimer;
        private Timer gameNewCitizenTimer;
        private int banditsDrawingTimerInterval;
        
        private void InitializeTimers()
        {
            repaintTimer = new Timer {Interval = 100};
            repaintTimer.Tick += RepaintTimerTick;
            repaintTimer.Start();
            
            gamePayTimer = new Timer {Interval = GameSetting.PayTimerInterval};
            gamePayTimer.Tick += GamePayTimerTick;
            gamePayTimer.Start();
            
            gameAttackTimer = new Timer {Interval = gameSetting.AttackTimerInterval};
            gameAttackTimer.Tick += GameAttackTimerTick;
            gameAttackTimer.Start();
            
            gameNewCitizenTimer = new Timer {Interval = gameSetting.NewCitizenTimerInterval};
            gameNewCitizenTimer.Tick += GameNewCitizenTimerTick;
            gameNewCitizenTimer.Start();

            banditsDrawingTimerInterval = 0;
        }

        private void StopTimers()
        {
            repaintTimer.Stop();
            gamePayTimer.Stop();
            gameAttackTimer.Stop();
            gameNewCitizenTimer.Stop();
        }

        private void RepaintTimerTick(object sender, EventArgs e)
        {
            Invalidate();
        }
        
        private void GamePayTimerTick(object sender, EventArgs e)
        {
            gameSetting.PayDay();
        }

        private void GameAttackTimerTick(object sender, EventArgs e)
        {
            gameSetting.Attack();
            banditsDrawingTimerInterval = 1500;
            gameAttackTimer.Interval = gameSetting.AttackTimerInterval;
        }

        private void GameNewCitizenTimerTick(object sender, EventArgs e)
        {
            gameSetting.AddCitizen();
            gameNewCitizenTimer.Interval = gameSetting.NewCitizenTimerInterval;
        }
    }
}