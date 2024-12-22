using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xiangqi.Gamescreen
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.Text = "About Xiangqi";
            this.labelProductName.Text = "Author: ";
            this.labelVersion.Text = "Version: 1.0";
            this.labelCopyright.Text = "2024";
            this.labelCompanyName.Text = "City";
            this.textBoxDescription.Text = "About Xiangqi";

            CenterToScreen();
        }

        private void AboutOkBtnClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void labelVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
