using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class BuildingsParametersControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public BuildingsParametersControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;

            var label = new Label
            {
                Size = new Size(ClientSize.Width, 3 * ClientSize.Height / 4),
                Location = new Point(0, ClientSize.Height / 32),
                Text = "\n\n" //todo
            };
            label.Show();
            Controls.Add(label);
            
            var menuButton = new Button
            {
                Size = mainForm.ButtonSize,
                Location = new Point(ClientSize.Width / 4, ClientSize.Height - ClientSize.Height / 5),
                Text = "Quit to menu"
            };
            menuButton.Click += MenuButtonHandleClick;
            Controls.Add(menuButton);
        }

        private void MenuButtonHandleClick(object sender, EventArgs e)
        {
            mainForm.ShowMenuControl();
        }
    }
}