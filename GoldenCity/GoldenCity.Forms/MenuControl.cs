using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class MenuControl : UserControl
    {
        public MenuControl(MainForm mainForm)
        {
            DoubleBuffered = true;
            InitializeComponent();
            ClientSize = mainForm.ClientSize;
            BackgroundImage = mainForm.Bitmaps["Background.png"];
            
            var startGameButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Start game"
            };
            startGameButton.Click += (s, e) => mainForm.ShowGameControl();

            var guideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(startGameButton.Location.X, startGameButton.Location.Y + startGameButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Game guide"
            };
            guideButton.Click += (s, e) => mainForm.ShowGuideControl();
            
            var buildingsParametersButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(guideButton.Location.X, guideButton.Location.Y + guideButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Buildings parameters"
            };
            buildingsParametersButton.Click += (s, e) => mainForm.ShowBuildingParametersControl();
            
            var settingsButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(buildingsParametersButton.Location.X, buildingsParametersButton.Location.Y + buildingsParametersButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Game settings"
            };
            settingsButton.Click += (s, e) => mainForm.ShowSettingsControl();

            Controls.AddRange(new Control[] {startGameButton, guideButton, buildingsParametersButton, settingsButton});
        }
    }
}