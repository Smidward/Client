using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public IPAddress ipAddr;
        public IPEndPoint ipEndPoint;
        public Thread ReciveThread;
        public string host;
        bool check1 = true, check2 = true;
        public Socket messager;

        public Form1()
        {
            InitializeComponent();      
            textBox2.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            button2.Enabled = true;
            textBox1.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = true;
            ipAddr = IPAddress.Parse("192.168.1.64");
            ipEndPoint = new IPEndPoint(ipAddr, 11000);
            messager = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ReciveThread = new Thread(new ThreadStart(recive));
            IPEndPoint iP = (IPEndPoint)messager.RemoteEndPoint;
            messager.Connect(ipEndPoint);
            string str = textBox1.Text.Trim() + "\n";
            messager.Send(Encoding.UTF8.GetBytes(str));
            if (!ReciveThread.IsAlive)
            {
                ReciveThread = new Thread(new ThreadStart(recive));
            }
            check1 = false;
            check2 = true;
            ReciveThread.Start();
        }

        public void recive()
        {
            while (true)
            {
                byte[] reciver = new byte[1024];
                int size = messager.Receive(reciver);
                string txt = Encoding.UTF8.GetString(reciver, 0, size);
                richTextBox1.BeginInvoke(new Action(() => { richTextBox1.AppendText(txt); }));
            }
        }

         private void button2_Click(object sender, EventArgs e)
        {
            string txt = textBox2.Text + "\n";
            messager.Send(Encoding.UTF8.GetBytes(txt));
            textBox2.Text = "";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            check2 = false;
            Disconnect();
        }
        void Disconnect()
        {
            if (!check1 && check2)
            {
                messager.Shutdown(SocketShutdown.Both);
                messager.Close();
            }
            if (!check1)
            ReciveThread.Abort();
            textBox2.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Enabled = true;
            button1.Enabled = true;
        }
    }
}
