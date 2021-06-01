using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class GameGuideControl : UserControl
    {
        public GameGuideControl(MainForm mainForm)
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
                Text = "Вы создаете свой город в сеттинге Дикого Запада.\n\n" +
                       "Вы можете строить здания, которые влияют на текущее состояние вашего города (подробнее читайте в \"How building works?\"). Раз в определенный " +
                       "промежуток времени в городе появляется новый житель (на это влияет радость жителей от зданий в городе, но промежуток не может быть меньше 2.5 секунд)." +
                       " \n\nРаз в определенный промежуток времени вы получаете доход от ваших зданий (Pay day), в которых работают жители.\n\n" +
                       "Через определенный промежуток времени на город нападают бандиты. На частоту и другие параметры нападений можно влиять (подробнее читайте в \"How bandits work?\")." +
                       "\n\nНа экране во время игры вы можете видеть имеющийся у вас бюджет, количество жителей, лимит для жителей (читайте о \"Living House\" в \"How building works?\") и " +
                       "через сколько нападут бандиты (читайте о \"Jail\" в \"How building works?\").\n\n" +
                       "Победить вы можете, построив Town Hall, для которого необходимо минимум в 8 раз больше жителей, чем размер карты, и 150.000 $"
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