using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class BanditsGuideControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public BanditsGuideControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;

            var label = new Label //todo
            {
                Size = new Size(ClientSize.Width, 3 * ClientSize.Height / 4),
                Location = new Point(0, ClientSize.Height / 32),
                Text = "АААААААААААААААААААААААААААААААААААААААА\n\n" //todo TODO TODO TODO
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