using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Car_Racing_Game_MOO_ICT
{
    public partial class InputNameForm : Form
    {
        public string PlayerName { get; private set; }

        public InputNameForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PlayerName = txtName.Text.Trim();
            if (string.IsNullOrEmpty(PlayerName))
            {
                MessageBox.Show("Please enter your name!");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
