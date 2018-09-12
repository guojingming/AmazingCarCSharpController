using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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


        private delegate void SetTextCallback();
        private delegate void SetTextCallbackWithContent(string str);

        public String[] ControllerCmd = new String[]{
            "#GNSS_CLOSE$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//GNSS关闭
            "#GNSS_OPEN_0$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//GNSS打开 设备号0
            "#UI_CLOSE$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//UI数据传输关闭
            "#UI_OPEN$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//UI数据传输打开
            "#ALGORITHM_CLOSE$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//控制算法关闭
            "#ALGORITHM_OPEN$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//控制算法打开
            "#CONTROLLER_CLOSE$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//车控关闭
            "#CONTROLLER_OPEN_0$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//车控打开 设备号0
            "#CONTROLLER_SHUTDOWN_ON$$$$$$$$$$$$$$$$$$$$$$$$$$$",//急停打开
            "#CONTROLLER_SHUTDOWN_OFF$$$$$$$$$$$$$$$$$$$$$$$$$$",//急停关闭
            "#VLP_CLOSE$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//打开VLP16雷达
            "#VLP_OPEN$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//关闭VLP16雷达

            "#TAR_COUNT_0$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$",//发送目标点数量
            "#TARS_Np1_Xp2_Yp3$"              //发送目标点
        };


        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            threadArray = new Thread[2];
            System.Console.WriteLine(ControllerCmd[0].Length);

            //Control.CheckForIllegalCrossThreadCalls = false;
            this.SetBounds(0, 0, 850, 780);

            comboBox2.Items.Add("COM0");
            comboBox2.Items.Add("COM1");
            comboBox2.Items.Add("COM2");
            comboBox2.Items.Add("COM3");
            comboBox2.Items.Add("COM4");
            comboBox2.Items.Add("COM5");
            comboBox2.Text = "COM4";

            comboBox1.Items.Add("4800");
            comboBox1.Items.Add("9600");
            comboBox1.Items.Add("19200");
            comboBox1.Items.Add("38400");
            comboBox1.Items.Add("57600");
            comboBox1.Items.Add("115200");
            comboBox1.Text = "115200";

            comboBox3.Items.Add("/dev/ttyUSB0");
            comboBox3.Items.Add("/dev/ttyUSB1");
            comboBox3.Items.Add("/dev/ttyUSB2");
            comboBox3.Items.Add("/dev/ttyUSB3");
            comboBox3.Text = "/dev/ttyUSB1";

            comboBox5.Items.Add("/dev/ttyUSB0");
            comboBox5.Items.Add("/dev/ttyUSB1");
            comboBox5.Items.Add("/dev/ttyUSB2");
            comboBox5.Items.Add("/dev/ttyUSB3");
            comboBox5.Text = "/dev/ttyUSB2";

            comboBox4.Items.Add("/dev/ttyUSB0");
            comboBox4.Items.Add("/dev/ttyUSB1");
            comboBox4.Items.Add("/dev/ttyUSB2");
            comboBox4.Items.Add("/dev/ttyUSB3");
            comboBox4.Text = "/dev/ttyUSB2";

            dataTranser = new MySerialPort();

            button2.Enabled = false;
        }


        private void changeRichTextBox1(string str) {
            if (richTextBox1.InvokeRequired) {
                SetTextCallbackWithContent d = new SetTextCallbackWithContent(changeRichTextBox1);
                richTextBox1.Invoke(d, str);
            } else {
                richTextBox1.AppendText(str);
                if (richTextBox1.Text.Length >= 1000) {
                    richTextBox1.Text = "";
                }
                richTextBox1.ScrollToCaret();
            }
           
        }

        private String processCarState(char[] buffer) {
            String carStateStr = new String(buffer);

            int _X = carStateStr.IndexOf("_X");
            int _Y = carStateStr.IndexOf("_Y");
            int _A = carStateStr.IndexOf("_A");
            int _S = carStateStr.IndexOf("_S");

            try {
                String _X_str = carStateStr.Substring(_X + 2, _Y - _X - 2);
                String _Y_str = carStateStr.Substring(_Y + 2, _A - _Y - 2);
                String _A_str = carStateStr.Substring(_A + 2, _S - _A - 2);
                String _S_str = carStateStr.Substring(_S + 2, carStateStr.IndexOf("$") - _S - 2);
                return _X_str + " " + _Y_str + " " + _A_str + " " + _S_str;
            } catch {
                return "";
            }
        }

        //private void recieveData() {
        //    char[] buffer = new char[100];
        //    bool startFlag = false;
        //    char[] data = new char[100];
        //    int current_index = 0;
        //    while (!dataTranserStopFlag) {
        //        Array.Clear(buffer, 0, buffer.Length);
        //        int size = dataTranser.Read(buffer, buffer.Length);

        //        for (int i = 0; i < 100; i++) { 
        //            if (buffer[i] == '#') {
        //                Array.Clear(data, 0, 100);
        //                startFlag = true;
        //                current_index = 0;
        //                data[current_index++] = '#';
        //            } else if (startFlag) {
        //                if (current_index >= 100) { 
        //                    //lost frames;
        //                    Array.Clear(data, 0, 100);
        //                    startFlag = false;
        //                    current_index = 0;
        //                    continue;
        //                }
        //                data[current_index++] = buffer[i];
        //                if (buffer[i] == '$') {
        //                    processCarState(data);
        //                    Array.Clear(data, 0, 100);
        //                    startFlag = false;
        //                    current_index = 0;
        //                    continue;
        //                }
        //            }
        //        }
        //        String str = new String(buffer);
        //        changeRichTextBox1(str);
        //    }
        //}

        private MySerialPort dataTranser;
        private bool dataTranserStopFlag;
        private void button1_Click(object sender, EventArgs e) {
            if (dataTranser.OpenSerialPort(comboBox2.Text, Int32.Parse(comboBox1.Text))) {
                dataTranserStopFlag = false;
                //Thread th = new Thread(recieveData);
                //th.Start();
                button1.Enabled = false;
                button2.Enabled = true;
            }
            
        }

        private Thread[] threadArray;

        private void button10_Click(object sender, EventArgs e) {
            try {
                Thread t1 = (Thread)threadArray[0];
                Thread t2 = (Thread)threadArray[1];
                t1.Abort();
                t2.Abort();
                client.Close();
            } catch { 
            
            }

            if (button10.Text == "停止转发") {
                try {
                    dataTranser.WriteString(ControllerCmd[2], true);
                    changeRichTextBox2(ControllerCmd[2]);
                } catch {
                    System.Console.WriteLine("请先打开串口");
                }
                stopTransFlag = true;
                button10.Text = "开始转发";
            } else if (button10.Text == "开始转发") {
                try {
                    dataTranser.WriteString(ControllerCmd[3], true);
                    changeRichTextBox2(ControllerCmd[3]);
                } catch {
                    System.Console.WriteLine("请先打开串口");
                }

                client = new TcpClient();

                Thread th1 = new Thread(uiTransData);
                
                Thread th2 = new Thread(uiTarData);
                try {
                    th1.Start();
                    threadArray[0] = th1;
                    th2.Start();
                    threadArray[1] = th2;
                }catch{
                    return;
                }
                

                stopTransFlag = false;
                button10.Text = "停止转发";
            }
        }

        public static bool stopTransFlag = true;
        private void changeRichTextBox7(String tips){
            if (richTextBox7.InvokeRequired) {
                SetTextCallbackWithContent d = new SetTextCallbackWithContent(changeRichTextBox7);
                richTextBox7.Invoke(d, tips);
            } else {
                richTextBox7.Text = tips;
            }
        }

        private void changeButton10(String tips) {
            if (button10.InvokeRequired) {
                SetTextCallbackWithContent d = new SetTextCallbackWithContent(changeButton10);
                button10.Invoke(d, tips);
            } else {
                button10.Text = tips;
            }
        }

        //Parse将字符串转换为IP地址类型
        private IPAddress myIP;
        //构造一个TcpClient类对象,TCP客户端
        private TcpClient client;


        //创建网络流,获取数据流
        private NetworkStream stream = null;
        //写数据流对象
        private StreamWriter sw = null;
        //读数据流对象
        private StreamReader sr = null;

        public void uiTransData() {
            myIP = IPAddress.Parse(textBox1.Text);
            //与TCP服务器连接
            try {
                if (!client.Connected) {
                    client.Connect(myIP, 1888);
                    changeRichTextBox7("连接成功！");
                }
            } catch (Exception e) {
                System.Console.WriteLine(e.Message);
                changeRichTextBox7("连接失败！请检查网络设置！");
                changeButton10("开始转发");
                return;
            }

            try {
                stream = client.GetStream();
                sw = new StreamWriter(stream);
            } catch(Exception exception) {
                System.Console.WriteLine(exception.Message);
                changeRichTextBox7("管道创建失败！请重新连接！");
                changeButton10("开始转发");
                client.Close();
                return;
            }
            char[] buffer = new char[200];
            bool startFlag = false;
            char[] data = new char[200];
            int current_index = 0;
            while (!stopTransFlag) {
                
                Array.Clear(buffer, 0, buffer.Length);
                int size = dataTranser.Read(buffer, buffer.Length);

                String str = new String(buffer);
                changeRichTextBox1(str);

                for (int i = 0; i < buffer.Length; i++) {
                    if (buffer[i] == '#') {
                        Array.Clear(data, 0, data.Length);
                        startFlag = true;
                        current_index = 0;
                        data[current_index++] = '#';
                    } else if (startFlag) {
                        if (current_index >= buffer.Length) {
                            //lost frames;
                            Array.Clear(data, 0, data.Length);
                            startFlag = false;
                            current_index = 0;
                            continue;
                        }
                        data[current_index++] = buffer[i];
                        if (buffer[i] == '$') {
                            String result = processCarState(data);
                            if (result != "") {
                                try {
                                    System.Console.WriteLine("TRANS:" + result);

                                    sw.Write("#" + result + "$");
                                    sw.Flush();
                                } catch {
                                    changeRichTextBox7("连接已中断！");
                                    changeButton10("开始转发");
                                    client.Close();
                                    return;
                                }

                            }
                            Array.Clear(data, 0, buffer.Length);
                            startFlag = false;
                            current_index = 0;
                            continue;
                        }
                    }
                    
                }
                
            }
            changeRichTextBox7("连接已关闭！");
            client.Close();
            //CarData data1;
            //data1.x = 20;
            //data1.y = 20;
            //data1.angle = 0;
            //data1.state = 4;
            //char[] buffer = new char[100];
            //while (!stopTransFlag) {
            //    data1.angle += 0.1f;
            //    data1.x = 20 * (float)Math.Cos(data1.angle);
            //    data1.y = 20 * (float)Math.Sin(data1.angle);
            //    String s = data1.x + " " + data1.y + " " + data1.angle + " " + data1.state + "\0";
            //    char[] b = s.ToCharArray();
            //    for (int i = 0; i < b.Length; i++) {
            //        buffer[i] = b[i];
            //    }
            //    for (int i = b.Length; i < 100; i++) {
            //        buffer[i] = '\0';
            //    }
            //    sw.Write(buffer);
            //    sw.Flush();             //刷新流
            //    System.Threading.Thread.Sleep(100);
            //}
            
        }

        private void uiTarData() {
            //myIP = IPAddress.Parse(textBox1.Text);
            ////与TCP服务器连接
            //try {
            //    if (!client.Connected) {
            //        client.Connect(myIP, 1888);
            //    }
            //} catch (Exception e) {
            //    System.Console.WriteLine(e.Message);
            //    changeRichTextBox7("连接失败！请检查网络连接！");
            //    client.Close();
            //    return;
            //}
            Thread.Sleep(2000);
            //读数据流对象
            try {
                sr = new StreamReader(stream);
            } catch {
                return;
            }
            
            bool temp_flag = false;

           

            char[] buffer = new char[1024];
            while (!stopTransFlag) {
                try {
                    Array.Clear(buffer, 0, 1024);
                    sr.Read(buffer, 0, 1024);
                } catch {
                    System.Console.WriteLine("Read TCP 失败");
                    return;
                }
                String str = new String(buffer);
                if (!str.StartsWith("#T")) {
                    System.Console.WriteLine("目标数据头错误");
                    continue;
                }
                try {
                    str = str.Substring(2);
                    str = str.Substring(0, str.LastIndexOf('$') + 1);
                } catch {
                    System.Console.WriteLine("目标数据串错误");
                    System.Console.WriteLine(str);
                    continue;
                }
                
                //add to tars
                ArrayList tars = new ArrayList();
                String[] strs = str.Split('$');
                if (strs.Length == 0) {
                    continue;
                }
                ArrayList realStrs = new ArrayList();
                for (int i = 0; i < strs.Length; i++) {
                    if (strs[i] != "") {
                        realStrs.Add(strs[i]);
                    }
                }
                
                int tarCount = 0;
                try {
                    tarCount = Int32.Parse(((String)realStrs[0]).Substring(1));
                    if (realStrs.Count != tarCount + 1) {
                        continue;
                    } else {
                        for (int i = 1; i < realStrs.Count; i++) {
                            String tempStr = (String)realStrs[i];
                            float x = (float)Math.Round(float.Parse(tempStr.Substring(1, tempStr.IndexOf(',')-1)),3);
                            float y = (float)Math.Round(float.Parse(tempStr.Substring(tempStr.IndexOf(',') + 1)),3);
                            CYPoint point = new CYPoint(x, y);
                            tars.Add(point);
                        }
                    }
                } catch {
                    continue;
                }

                System.Console.WriteLine(str);
                try {
                    //替换设备号
                    String cmd = ControllerCmd[12];
                    cmd = cmd.Replace("0", tars.Count + "");
                    dataTranser.WriteString(cmd, true);
                    for (int i = 0; i < tars.Count; i++) {
                        cmd = ControllerCmd[13];
                        cmd = cmd.Replace("p1", i + "");
                        cmd = cmd.Replace("p2", ((CYPoint)tars[i]).x + "");
                        cmd = cmd.Replace("p3", ((CYPoint)tars[i]).y + "");
                        while (cmd.Length < 50) {
                            cmd += "$";
                        }
                        dataTranser.WriteString(cmd, true);
                    }
                    //changeRichTextBox2(cmd);
                } catch {
                    System.Console.WriteLine("请先打开串口");
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            dataTranserStopFlag = true;
            dataTranser.Close();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void changeRichTextBox2(string str) {
            if (richTextBox2.InvokeRequired) {
                SetTextCallbackWithContent d = new SetTextCallbackWithContent(changeRichTextBox2);
                richTextBox2.Invoke(d);
            } else {
                richTextBox2.AppendText(str + "\n");
                if (richTextBox2.Text.Length >= 1000) {
                    richTextBox2.Text = "";
                }
                richTextBox1.ScrollToCaret();
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            //获取用户输入的设备号
            int deviceNum = 0;
            try {
                deviceNum = Int32.Parse(comboBox3.Text.Substring(comboBox3.Text.Length - 1, 1));
            } catch {
                System.Console.WriteLine("请输入合法设备号");
                return;
            }
            
            try {
                //替换设备号
                String cmd = ControllerCmd[1];
                cmd = cmd.Replace("0", deviceNum + "");
                dataTranser.WriteString(cmd, true);
                changeRichTextBox2(cmd);
            } catch {
                System.Console.WriteLine("请先打开串口");
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[0], true);
                changeRichTextBox2(ControllerCmd[0]);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button12_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[5], true);
                changeRichTextBox2(ControllerCmd[5]);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button13_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[4], true);
                changeRichTextBox2(ControllerCmd[4]);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[11], true);
                changeRichTextBox2(ControllerCmd[11]);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button8_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[10], true);
                changeRichTextBox2(ControllerCmd[10]);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button9_Click(object sender, EventArgs e) {
            //获取用户输入的设备号
            int deviceNum = 0;
            try {
                deviceNum = Int32.Parse(comboBox5.Text.Substring(comboBox5.Text.Length - 1, 1));
            } catch {
                System.Console.WriteLine("请输入合法设备号");
                return;
            }
            try {
                //替换设备号
                String cmd = ControllerCmd[7];
                cmd = cmd.Replace("0", deviceNum + "");
                dataTranser.WriteString(cmd, true);
                changeRichTextBox2(cmd);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button11_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[6], true);
                changeRichTextBox2(ControllerCmd[6]);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

    }

    public class CYPoint {
        public CYPoint(float i_x, float i_y) {
            x = i_x;
            y = i_y;
        }
        public float x;
        public float y;
    }
}
