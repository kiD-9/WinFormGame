using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class FinishedControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public FinishedControl(MainForm mainForm)
        {
            DoubleBuffered = true;
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;
            BackgroundImage = mainForm.Bitmaps["Background.png"];
            
            var button = new Button
            {
                Size = new Size(ClientSize.Width / 3, ClientSize.Height / 3),
                Location = new Point(ClientSize.Width / 3, ClientSize.Height / 3),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "CONGRATS!!! \n\n YOU WON! \n\n\n Press to quit to menu"
            };
            button.Click += (s, e) => mainForm.ShowMenuControl();
            Controls.Add(button);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(mainForm.Bitmaps["TownHall.png"],
                new Point((ClientSize.Width - MainForm.BitmapSize) / 2, ClientSize.Height / 8));
            e.Graphics.DrawImage(mainForm.Bitmaps["Bandit.png"],
                new Point(ClientSize.Width / 20, ClientSize.Height / 4));
            e.Graphics.DrawImage(mainForm.Bitmaps["Bandit.png"], 
                new Point(15 * ClientSize.Width / 20, ClientSize.Height / 4));
            e.Graphics.DrawImage(mainForm.Bitmaps["Bandit.png"],
                new Point(ClientSize.Width / 20, 3 * ClientSize.Height / 4));
            e.Graphics.DrawImage(mainForm.Bitmaps["Bandit.png"],
                new Point(15 * ClientSize.Width / 20, 3 * ClientSize.Height / 4));
        }
    }
}