using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class GuideControl : UserControl
    {
        public GuideControl(MainForm mainForm)
        {
            DoubleBuffered = true;
            InitializeComponent();
            ClientSize = mainForm.ClientSize;
            BackgroundImage = mainForm.Bitmaps["Background.png"];
            
            var gameGuideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "How to play?"
            };
            gameGuideButton.Click += (s, e) => mainForm.ShowGameGuideControl();
            
            var buildingGuideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(gameGuideButton.Location.X, gameGuideButton.Location.Y + gameGuideButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "How building works?"
            };
            buildingGuideButton.Click += (s, e) => mainForm.ShowBuildingGuideControl();
            
            var banditsGuideButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(buildingGuideButton.Location.X, buildingGuideButton.Location.Y + buildingGuideButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "How bandits work?"
            };
            banditsGuideButton.Click += (s, e) => mainForm.ShowBanditsGuideControl();
            
            var menuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Quit to menu"
            };
            menuButton.Click += (s, e) => mainForm.ShowMenuControl();

            Controls.AddRange(new Control[] {gameGuideButton, buildingGuideButton, banditsGuideButton, menuButton});
        }
    }
}