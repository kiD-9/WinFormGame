using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class BuildingGuideControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public BuildingGuideControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;

            var label = new Label
            {
                Size = new Size(ClientSize.Width, 3 * ClientSize.Height / 4),
                Location = new Point(0, ClientSize.Height / 32),
                Text = "У каждого здания есть параметры Happiness, Budget Weakness, Income Money, Cost и у некоторых специфические параметры.\n\n" +
                       "Happiness: может быть положительный или отрицательный. Влияет на частоту появления новых жителей.\n\n" +
                       "Budget Weakness: выражается в \"%\" и влияет на то, сколько денег из бюджета бандиты заберут при атаке.\n\n" +
                       "Income Money: определяет сколько денег будет давать здание каждый Pay day, когда в него будет добавлен работник из числа жителей.\n\n" +
                       "Cost: цена постройки здания.\n\n" +
                       "У Living House есть специфический параметр - жители. В каждом Living House может жить 4 жителя. Именно число зданий этого типа влияет на лимит жителей.\n\n" +
                       "Sheriffs House определяет число зданий, на которые нападут бандиты. Зданий этого типа может быть сколь угодно, но всего шерифов (работник этого здания) " +//todo
                       "может быть только 2.\n\n" +
                       "Jail определяет промежуток между атаками бандитов. Каждая работник тюрьмы добавляет 1 секунду в этот промежуток.\n\n" //todo
            };
            label.Show();
            Controls.Add(label);
            
            var guideMenuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                Text = "Quit to Guide menu"
            };
            guideMenuButton.Click += GuideMenuButtonHandleClick;
            Controls.Add(guideMenuButton);
        }

        private void GuideMenuButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowGuideControl();
        }
    }
}