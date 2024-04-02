using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AudioChatRoom
{
    public partial class LoginForm : Form
    {      
        public LoginForm( )
        {
            InitializeComponent();          
        }

        private string currentUserID;
        public string CurrentUserID
        {
            get { return currentUserID; }            
        }

        public string RoomID
        {
            get
            {
                return this.textBox1.Text;
            }
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            string userID = this.textBox_id.Text.Trim();
            if (userID.Length > 21)
            {
                MessageBox.Show("ID长度必须小于21.");
                return;
            }

            this.currentUserID = userID;
            this.DialogResult = DialogResult.OK;           
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
        }
        
       
    }
}
