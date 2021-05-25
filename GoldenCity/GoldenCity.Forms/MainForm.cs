using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class MainForm : Form
    {
        private const int GamePropertiesBarHeight = 40;
        private const int BitmapSize = 120;
        private const int MapSize = 5;
        private readonly Panel mainPanel;
        private readonly MenuControl menuControl;
        private readonly GameControl gameControl;
        private readonly FinishedControl finishedControl;
        
        public MainForm()
        {
            InitializeComponent();
            
            ClientSize = new Size(MapSize * BitmapSize, MapSize * BitmapSize + GamePropertiesBarHeight);
            mainPanel = new Panel {ClientSize = ClientSize};
            Controls.Add(mainPanel);
            menuControl = new MenuControl(this);
            gameControl = new GameControl(this);
            finishedControl = new FinishedControl(this);
            ShowMenuControl();
        }

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
        
        public void ShowFinishedControl()
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(finishedControl);
        }
        
        public static void Application_ThreadException(ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
    }
}