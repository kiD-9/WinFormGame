using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class MenuControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public MenuControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;
            
            var startGameButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height / 5),
                Text = "Start game"
            };
            startGameButton.Click += StartGameButtonHandleClick;

            var guideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(startGameButton.Location.X, startGameButton.Location.Y + startGameButton.Size.Height),
                Text = "Game guide"
            };
            guideButton.Click += GuideButtonHandleClick;
            
            var buildingsParametersButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(guideButton.Location.X, guideButton.Location.Y + guideButton.Size.Height),
                Text = "Buildings parameters"
            };
            buildingsParametersButton.Click += BuildingsParametersButtonHandleClick;

            Controls.AddRange(new Control[] {startGameButton, guideButton, buildingsParametersButton});
        }

        private void StartGameButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowGameControl();
        }

        private void GuideButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowGuideControl();
        }

        private void BuildingsParametersButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowBuildingParametersControl();
        }
    }
}