using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class BanditsGuideControl : UserControl
    {
        public BanditsGuideControl(MainForm mainForm)
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
                Text = "Бандиты нападают каждый промежуток времени, указанный в верхней панели игрового поля (изначально 15 секунд), но промежуток можно увеличить" +
                       "(читайте о \"Jail\" в \"How building works?\").\n\nВсего они атакуют здания в количестве от половины ширины карты до всей ширины" +
                       " карты, в зависимости от количества шерифов (читайте о \"Sheriffs House\" в \"How building works?\"), но только с работниками внутри. Кроме того," +
                       " они избавляются от этих работников.\n\n Бандиты забирают из всего бюджета города \"%\" денег, который является суммой всех Budget Weakness " +
                       "атакованных зданий (читайте о зданиях в \"Building Parameters\")."
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