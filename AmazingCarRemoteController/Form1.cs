using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmazingCarRemoteController {
   
    public partial class Form1 : Form {


        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            //Control.CheckForIllegalCrossThreadCalls = false;
            //this.SetBounds(0, 0, 683, 615);
            String[] PortNames = SerialPort.GetPortNames();
            for (int i = 0; i < PortNames.Count(); i++) {
                comboBox2.Items.Add(PortNames[i]);
            }
            if (PortNames.Count() > 0) {
                comboBox2.Text = PortNames[0];
            } else {
                comboBox2.Text = "";
            }
            comboBox1.Items.Add("4800");
            comboBox1.Items.Add("9600");
            comboBox1.Items.Add("19200");
            comboBox1.Items.Add("38400");
            comboBox1.Items.Add("57600");
            comboBox1.Items.Add("115200");
            comboBox1.Text = "115200";
            dataTranser = new MySerialPort();
            button2.Enabled = false;
        }


        private MySerialPort dataTranser;

        private void button1_Click(object sender, EventArgs e) {
            if (dataTranser.OpenSerialPort(comboBox2.Text, Int32.Parse(comboBox1.Text))) {
                button1.Enabled = false;
                button2.Enabled = true;
                Form2.setPortObj(dataTranser);
            }
            
        }

        private Form2 form2;
        private void button10_Click(object sender, EventArgs e) {
            if (form2 == null || !form2.Created) {
                form2 = new Form2();
                form2.Show();
            } else {
                try {
                    form2.Close();
                    form2 = new Form2();
                    form2.Show();
                }catch{
                    System.Console.WriteLine("打开窗口失败");
                }
            }
               
        }
       
        //关闭数传
        private void button2_Click(object sender, EventArgs e) {
            dataTranser.Close();
            button1.Enabled = true;
            button2.Enabled = false;
        }


        private Form3 form3;
        private void button3_Click_1(object sender, EventArgs e) {
            if (form3 == null || !form3.Created) {
                form3 = new Form3(dataTranser);
                form3.Show();
            } else {
                try {
                    form3.Close();
                    form3 = new Form3(dataTranser);
                    form3.Show();
                } catch {
                    System.Console.WriteLine("打开窗口失败");
                }
            }
        }
    }
}
