using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class SettingsControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public SettingsControl(MainForm mainForm)
        {
            DoubleBuffered = true;
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;
            BackgroundImage = mainForm.Bitmaps["Background.png"];
            
            var mapSizeFiveButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Map Size = 5"
            };
            mapSizeFiveButton.Click += (s, e) => mainForm.ChangeGameSize(5);
            
            var mapSizeSixButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(mapSizeFiveButton.Location.X, mapSizeFiveButton.Location.Y + mapSizeFiveButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Map Size = 6"
            };
            mapSizeSixButton.Click += (s, e) => mainForm.ChangeGameSize(6);
            
            var mapSizeSevenButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(mapSizeSixButton.Location.X, mapSizeSixButton.Location.Y + mapSizeSixButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Map Size = 7"
            };
            mapSizeSevenButton.Click += (s, e) => mainForm.ChangeGameSize(7);
            
            var mapSizeEightButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(mapSizeSevenButton.Location.X, mapSizeSevenButton.Location.Y + mapSizeSevenButton.Size.Height),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Map Size = 8"
            };
            mapSizeEightButton.Click += (s, e) => mainForm.ChangeGameSize(8);
            
            var menuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Quit to menu"
            };
            menuButton.Click += (s, e) => mainForm.ShowMenuControl();

            Controls.AddRange(new Control[] {mapSizeFiveButton, mapSizeSixButton, mapSizeSevenButton, mapSizeEightButton, menuButton});
        }
    }
}