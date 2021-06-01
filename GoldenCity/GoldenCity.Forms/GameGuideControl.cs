using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class GameGuideControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public GameGuideControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;

            var label = new Label
            {
                Size = new Size(ClientSize.Width, 3 * ClientSize.Height / 4),
                Location = new Point(0, ClientSize.Height / 32),
                Text = "Вы создаете свой город в сеттинге Дикого Запада.\n\n" +
                       "Вы можете строить здания, которые влияют на текущее состояние вашего города (подробнее читайте в \"How building works?\"). Раз в определенный промежуток времени в городе появляется новый " +
                       "житель (на это влияет радость жителей от зданий в городе). \n\nРаз в определенный промежуток времени вы получаете доход от ваших зданий (Pay day), в которых работает житель.\n\n" +
                       "Через определенный промежуток времени на город нападают бандиты. На частоту и другие параметры нападений можно влиять (подробнее читайте в \"How bandits work?\").\n\n" +
                       "На экране вы можете видеть имеющийся у вас бюджет, количество жителей, лимит для жителей (читайте о \"Living House\" в \"How building works?\") и через сколько нападут бандиты.\n\n" +
                       "Победить вы можете, построив Town Hall, для которого необходимо минимум 40 жителей и 100.000 $"
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