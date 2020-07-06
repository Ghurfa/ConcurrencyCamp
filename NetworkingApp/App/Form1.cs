using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            byte[] data = Encoding.UTF8.GetBytes(text);

            TcpClient client = new TcpClient();
            await client.ConnectAsync("192.168.1.172", 9999);

            var stream = client.GetStream();
            await stream.WriteAsync(data, 0, data.Length);
            data = new byte[50];
            await stream.ReadAsync(data, 0, 50);

            listBox1.Items.Add(Encoding.UTF8.GetString(data));
        }
    }
}
