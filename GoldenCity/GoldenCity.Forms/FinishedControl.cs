using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenCity.Forms
{
    public partial class FinishedControl : UserControl
    {
        private readonly MainForm mainForm;
        
        public FinishedControl(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;
            
            var button = new Button
            {
                Size = new Size(ClientSize.Width / 3, ClientSize.Height / 3),
                Location = new Point(ClientSize.Width / 3, ClientSize.Height / 3),
                Text = "CONGRATS!!! \n\n\n Press to return to Menu"
            };
            button.Click += HandleClick;
            Controls.Add(button);
        }

        private void HandleClick(object sender, EventArgs e)
        {
            mainForm.ShowMenuControl();
        }
    }
}