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
            doLogin();
        }

        private void txtIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) doLogin();
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) doLogin();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            tmrFokus.Enabled = true;
            parent.Enabled = false;
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Enabled = true;
            Environment.Exit(0);
        }

        private void doLogin()
        {
            if (txtIp.Text != "" && txtName.Text != "")
            {
                if (txtName.Text.Length <= 10)
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
                else
                {
                    MessageBox.Show("Der Name darf nicht länger als 10 Zeichen sein");
                }
            }
        }

        private void tmrFokus_Tick(object sender, EventArgs e)
        {
            tmrFokus.Enabled = false;
            this.Focus();
        }
    }
}
