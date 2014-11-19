using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomberCowClient
{
    public partial class frmLogin : Form
    {
        private frmMain parent;

        public string PlayerName;

        public frmLogin(frmMain setparent)
        {
            InitializeComponent();

            //get parent form
            parent = setparent;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtIp.Text != "" && txtName.Text != "")
            {
                btnLogin.Enabled = false;
                parent.PlayerName = txtName.Text;
                parent.ServerIP = txtIp.Text;

                //Call connect function
                parent.connect();
                parent.Enabled = true;

                //Hide me
                this.Visible = false;
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            parent.Enabled = false;
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Enabled = true;
            Environment.Exit(0);
        }
    }
}
