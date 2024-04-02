using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CCWin;
using OMCS.Passive;

namespace AudioChatRoom
{
    public partial class MainForm : Form
    {
        private IMultimediaManager multimediaManager;
        private string chatRoomID;
        public MainForm(IMultimediaManager mgr ,string _chatRoomID)
        {
            InitializeComponent();
            this.chatRoomID = _chatRoomID;
            this.multimediaManager = mgr;
            this.Text = this.Text + " - " + chatRoomID;
            this.skinLabel_userID.Text = this.multimediaManager.CurrentUserID;           
           
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.multiAudioChatContainer1.Close();
            this.multimediaManager.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.multiAudioChatContainer1.Initialize(this.multimediaManager, this.chatRoomID);
        }

        private void multiAudioChatContainer1_Load(object sender, EventArgs e)
        {

        }
    }
}
