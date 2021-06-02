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

            var gameName = new Label
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, startGameButton.Location.Y - mainForm.ButtonSize.Height),
                BackColor = Color.Transparent,
                Text = "GOLDEN CITY",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 24, FontStyle.Bold)
            };

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
            
            var exitButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Exit"
            };
            exitButton.Click += (s, e) => mainForm.Close();

            Controls.AddRange(new Control[]
                {gameName, startGameButton, guideButton, buildingsParametersButton, settingsButton, exitButton});
        }
    }
}