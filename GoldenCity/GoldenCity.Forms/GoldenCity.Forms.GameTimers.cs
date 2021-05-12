using System;
using System.Windows.Forms;
using GoldenCity.Models;

namespace GoldenCity.Forms
{
    public partial class Form1
    {
        private Timer payTimer = new Timer();
        private Timer attackTimer = new Timer();
        private Timer newCitizenTimer = new Timer();
        
        private void InitializeGameTimers()
        {
            payTimer = new Timer {Interval = GameSetting.PayTimerInterval};
            payTimer.Tick += PayTimerTick;
            payTimer.Start();
            
            attackTimer = new Timer {Interval = gameSetting.AttackTimerInterval};
            attackTimer.Tick += AttackTimerTick;
            attackTimer.Start();
            
            newCitizenTimer = new Timer {Interval = gameSetting.NewCitizenTimerInterval};
            newCitizenTimer.Tick += NewCitizenTimerTick;
            newCitizenTimer.Start();
        }

        private void PayTimerTick(object sender, EventArgs e)
        {
            gameSetting.PayDay();
        }

        private void AttackTimerTick(object sender, EventArgs e)
        {
            gameSetting.Attack();
            attackTimer.Interval = gameSetting.AttackTimerInterval;
        }

        private void NewCitizenTimerTick(object sender, EventArgs e)
        {
            gameSetting.AddCitizen();
            newCitizenTimer.Interval = gameSetting.NewCitizenTimerInterval;
        }
    }
}