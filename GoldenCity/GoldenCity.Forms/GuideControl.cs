using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class GuideControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public GuideControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;
            
            var gameGuideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height / 5),
                Text = "How to play?"
            };
            gameGuideButton.Click += GameGuideButtonHandleClick;
            
            var buildingGuideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(gameGuideButton.Location.X, gameGuideButton.Location.Y + gameGuideButton.Size.Height),
                Text = "How building works?"
            };
            buildingGuideButton.Click += BuildingGuideButtonHandleClick;
            
            var banditsGuideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(buildingGuideButton.Location.X, buildingGuideButton.Location.Y + buildingGuideButton.Size.Height),
                Text = "How bandits work?"
            };
            banditsGuideButton.Click += BanditsGuideButtonHandleClick;
            
            var menuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                Text = "Quit to menu"
            };
            menuButton.Click += MenuButtonHandleClick;

            Controls.AddRange(new Control[] {gameGuideButton, buildingGuideButton, banditsGuideButton, menuButton});
        }

        private void GameGuideButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowGameGuideControl();
        }

        private void BuildingGuideButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowBuildingGuideControl();
        }
        
        private void BanditsGuideButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowBanditsGuideControl();
        }

        private void MenuButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowMenuControl();
        }
    }
}