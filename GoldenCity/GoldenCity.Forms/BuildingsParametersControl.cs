using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class BuildingsParametersControl : UserControl
    {
        public BuildingsParametersControl(MainForm mainForm)
        {
            DoubleBuffered = true;
            InitializeComponent();
            ClientSize = mainForm.ClientSize;
            BackgroundImage = mainForm.Bitmaps["Background.png"];

            var label = new Label
            {
                Size = new Size(ClientSize.Width, 3 * ClientSize.Height / 4),
                Location = new Point(0, ClientSize.Height / 32),
                BackColor = Color.Chocolate,
                Text = "JAIL: Cost = 4000 $, Budget Weakness = 0 %, Income Money = 0, \nHappiness = +5 sec\n\n" +
                       "LIVING HOUSE: Cost = 500 $, Budget Weakness = 2 %, Income Money = 0 $, \nHappiness = -0.5 sec\n\n" +
                       "RAILROAD STATION: Cost = 3000 $, Budget Weakness = 6 %, Income Money = 500 $, \nHappiness = 5 sec\n\n" +
                       "SALOON: Cost = 1500 $, Budget Weakness = 8 %, Income Money = 1000 $, \nHappiness = -2.5 sec\n\n" +
                       "SHERIFFS HOUSE: Cost = 5000 $, Budget Weakness = 0 %, Income Money = 0 $, \nHappiness = +3.5 sec\n\n" +
                       "STORE: Cost = 3000 $, Budget Weakness = 10 %, Income Money = 1500 $, \nHappiness = -1 sec\n\n" +
                       "SHERIFFS HOUSE: Cost = 5000 $, Budget Weakness = 0 %, Income Money = 0 $, \nHappiness = +3.5 sec\n\n" +
                       "TOWN HALL: Cost = 150.000 $, Budget Weakness = 0 %, Income Money = 0 $, \nHappiness = 0 sec\n\n"
            };
            label.Show();
            Controls.Add(label);
            
            var menuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Quit to menu"
            };
            menuButton.Click += (s, e) => mainForm.ShowMenuControl();
            Controls.Add(menuButton);
        }
    }
}