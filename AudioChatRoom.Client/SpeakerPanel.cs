using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OMCS.Passive.MultiChat;
using OMCS.Passive;
using ESBasic;

namespace AudioChatRoom
{
    public partial class SpeakerPanel : UserControl
    {
        private IChatUnit chatUnit;     

        public SpeakerPanel()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);//调整大小时重绘
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);// 双缓冲
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);// 禁止擦除背景.
            this.SetStyle(ControlStyles.UserPaint, true);//自行绘制            
            this.UpdateStyles();
        }

        public string MemberID
        {
            get
            {
                if (this.chatUnit == null)
                {
                    return null;
                }

                return this.chatUnit.MemberID;
            }
        }

        public void Initialize(IChatUnit unit)
        {
            this.chatUnit = unit;
            this.skinLabel_name.Text = unit.MemberID;
                   
            this.chatUnit.MicrophoneConnector.ConnectEnded += new ESBasic.CbGeneric<string,OMCS.Passive.ConnectResult>(MicrophoneConnector_ConnectEnded);
            this.chatUnit.MicrophoneConnector.OwnerOutputChanged += new CbGeneric<string>(MicrophoneConnector_OwnerOutputChanged);
            this.chatUnit.MicrophoneConnector.AudioDataReceived += new CbGeneric<string,byte[]>(MicrophoneConnector_AudioDataReceived);

            //开始连接到目标成员的麦克风设备
            this.chatUnit.MicrophoneConnector.BeginConnect(unit.MemberID);
        }

        public void Initialize(string curUserID)
        {
            this.skinLabel_name.Text = curUserID;
            this.skinLabel_name.ForeColor = Color.Red;
            this.pictureBox_Mic.Visible = false;
            this.decibelDisplayer1.Visible = false;
        }


void MicrophoneConnector_AudioDataReceived(string ownerId, byte[] data)
{
            // 将接收到的字节数组转换为浮点数数组
            float[] audioData = new float[data.Length / 4];
            Buffer.BlockCopy(data, 0, audioData, 0, data.Length);

            // 设置提高音调的倍频
            float pitchMultiplier = 1.1f; // 一个半音的倍频

            // 创建一个新的音频数据数组，用于存储处理后的数据
            float[] processedAudioData = new float[audioData.Length];

            // 遍历音频数据并应用变声效果
            for (int i = 0; i < audioData.Length; i++)
            {
                // 将音频数据乘以一个系数，提高音调
                processedAudioData[i] = audioData[i] / pitchMultiplier;

                // 确保音频数据在合法范围内（-1 到 1 之间）
                if (processedAudioData[i] > 1.0f)
                    processedAudioData[i] = 1.63f*pitchMultiplier;
                else if (processedAudioData[i] < -1.0f)
                    processedAudioData[i] = -(2.35f/pitchMultiplier);
            }

            // 将处理后的音频数据转换回字节数组
            byte[] processedData = new byte[processedAudioData.Length * 4];
            Buffer.BlockCopy(processedAudioData, 0, processedData, 0, processedData.Length);

             this.decibelDisplayer1.DisplayAudioData(processedData);
        }





        void MicrophoneConnector_OwnerOutputChanged(string ownerId)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbGeneric<string>(this.MicrophoneConnector_OwnerOutputChanged), ownerId);
            }
            else
            {
                this.ShowMicState();
            }
        }

        private ConnectResult connectResult;
        void MicrophoneConnector_ConnectEnded(string ownerID, ConnectResult res)
        {            
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbGeneric<string , ConnectResult>(this.MicrophoneConnector_ConnectEnded), ownerID, res);
            }
            else
            {
                this.connectResult = res;
                this.ShowMicState();
            }
        }       

        /// <summary>
        /// 显示麦克风的状态以及提示语
        /// </summary>
        private void ShowMicState()
        {
            if (this.connectResult != OMCS.Passive.ConnectResult.Succeed)
            {
                this.pictureBox_Mic.BackgroundImage = this.imageList1.Images[2];
                this.toolTip1.SetToolTip(this.pictureBox_Mic, this.connectResult.ToString());
            }
            else
            {
                this.decibelDisplayer1.Working = false;
                if (!this.chatUnit.MicrophoneConnector.OwnerOutput)
                {
                    this.pictureBox_Mic.BackgroundImage = this.imageList1.Images[1];
                    this.toolTip1.SetToolTip(this.pictureBox_Mic, "好友禁用了麦克风");
                    return;
                }

                if (this.chatUnit.MicrophoneConnector.Mute)
                {
                    this.pictureBox_Mic.BackgroundImage = this.imageList1.Images[1];
                    this.toolTip1.SetToolTip(this.pictureBox_Mic, "静音");
                }
                else
                {
                    this.pictureBox_Mic.BackgroundImage = this.imageList1.Images[0];
                    this.toolTip1.SetToolTip(this.pictureBox_Mic, "正常");
                    this.decibelDisplayer1.Working = true;
                }
            }

        }

        private void pictureBox_Mic_Click(object sender, EventArgs e)
        {
            if (!this.chatUnit.MicrophoneConnector.OwnerOutput)
            {
                return;
            }

            this.chatUnit.MicrophoneConnector.Mute = !this.chatUnit.MicrophoneConnector.Mute;
            this.ShowMicState();
        }
    }
}
