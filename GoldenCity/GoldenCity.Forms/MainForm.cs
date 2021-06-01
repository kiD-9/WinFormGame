using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class MainForm : Form
    {
        public const int BitmapSize = 120;
        private readonly Panel mainPanel;
        private readonly MenuControl menuControl;
        private readonly GameControl gameControl;
        private readonly GuideControl guideControl;
        private readonly GameGuideControl gameGuideControl;
        private readonly BuildingGuideControl buildingGuideControl;
        private readonly BuildingsParametersControl buildingsParametersControl;
        private readonly BanditsGuideControl banditsGuideControl;
        private readonly SettingsControl settingsControl;
        private readonly FinishedControl finishedControl;
        
        public MainForm(int mapSize)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Bitmaps = TakeBitmapsFromDirectory(new DirectoryInfo("Resources"));
            MapSize = mapSize;
            ClientSize = new Size(MapSize * BitmapSize, MapSize * BitmapSize + GameControl.GamePropertiesBarHeight);
            ButtonSize = new Size(ClientSize.Width / 2, ClientSize.Height / 12);
            
            mainPanel = new Panel {ClientSize = ClientSize};
            Controls.Add(mainPanel);
            menuControl = new MenuControl(this);
            gameControl = new GameControl(this);
            guideControl = new GuideControl(this);
            gameGuideControl = new GameGuideControl(this);
            buildingGuideControl = new BuildingGuideControl(this);
            buildingsParametersControl = new BuildingsParametersControl(this);
            banditsGuideControl = new BanditsGuideControl(this);
            settingsControl = new SettingsControl(this);
            finishedControl = new FinishedControl(this);
            Invalidate();
            
            ShowMenuControl();
        }

        public int MapSize { get; }
        public Size ButtonSize { get; }
        public Dictionary<string, Bitmap> Bitmaps { get; }

        public void ShowMenuControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(menuControl);
        }
        
        public void ShowGameControl()
        {
            mainPanel.Controls.Clear();
            gameControl.StartGame();
            mainPanel.Controls.Add(gameControl);
        }

        public void ShowGuideControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(guideControl);
        }
        
        public void ShowGameGuideControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(gameGuideControl);
        }
        
        public void ShowBuildingGuideControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(buildingGuideControl);
        }
        
        public void ShowBuildingParametersControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(buildingsParametersControl);
        }
        
        public void ShowBanditsGuideControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(banditsGuideControl);
        }
        
        public void ShowSettingsControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(settingsControl);
        }
        
        public void ShowFinishedControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(finishedControl);
        }

        public void ChangeGameSize(int size)
        {
            var newMainForm = new MainForm(size) {Text = "Golden City"};
            newMainForm.Closed += (s, args) => this.Close();
            this.Hide();
            newMainForm.Show();
        }
        
        public static void Application_ThreadException(ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
        
        private Dictionary<string, Bitmap> TakeBitmapsFromDirectory(DirectoryInfo imagesDirectoryInfo)
        {
            var bitmapsFromDirectory = new Dictionary<string, Bitmap>();
            foreach (var fileInfo in imagesDirectoryInfo.GetFiles("*.png"))
            {
                bitmapsFromDirectory[fileInfo.Name] = (Bitmap) Image.FromFile(fileInfo.FullName);
            }

            return bitmapsFromDirectory;
        }
    }
}