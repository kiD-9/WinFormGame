using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class BuildingGuideControl : UserControl
    {
        public BuildingGuideControl(MainForm mainForm)
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
                Text = "У каждого здания есть параметры Happiness, Budget Weakness, Income Money, Cost и у некоторых специфические параметры.\n\n" +
                       "Happiness: может быть положительный или отрицательный (либо \"+\" сколько-то секунд, либо \"-\" сколько-то секунд). Влияет на частоту появления новых жителей." +
                       "\n\nBudget Weakness: выражается в \"%\" и влияет на то, сколько денег из бюджета бандиты заберут при атаке.\n\n" +
                       "Income Money: определяет сколько денег будет давать здание каждый Pay day, когда в него будет добавлен работник из числа жителей.\n\n" +
                       "Cost: цена постройки здания.\n\n" +
                       "У Living House есть специфический параметр - жители. В каждом Living House может жить 4 жителя. Именно число зданий этого типа влияет на лимит жителей.\n\n" +
                       "Sheriffs House определяет число зданий, на которые нападут бандиты. Зданий этого типа может быть сколь угодно, но всего шерифов (работник этого здания) " +
                       "может быть в 2 раза меньше, чем ширина карты.\n\n" +
                       "Jail определяет промежуток между атаками бандитов. Каждый работник тюрьмы добавляет 3 секунды в этот промежуток.\n\n"
            };
            label.Show();
            Controls.Add(label);
            
            var guideMenuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                BackColor = Color.Chocolate,
                FlatStyle = FlatStyle.Flat,
                Text = "Quit to Guide menu"
            };
            guideMenuButton.Click += (s, e) => mainForm.ShowGuideControl();
            Controls.Add(guideMenuButton);
        }
    }
}