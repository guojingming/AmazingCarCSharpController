using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmazingCarRemoteController {
    public partial class Form3 : Form {

        private MySerialPort dataTranser;


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

        public Form3(MySerialPort port) {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;
            dataTranser = port;
        }

        private void Form3_Load(object sender, EventArgs e) {
            comboBox1.Items.Add("/dev/ttyUSB0");
            comboBox1.Items.Add("/dev/ttyUSB1");
            comboBox1.Items.Add("/dev/ttyUSB2");
            comboBox1.Items.Add("/dev/ttyUSB3");
            comboBox1.Text = "/dev/ttyUSB2";
            comboBox2.Items.Add("/dev/ttyUSB0");
            comboBox2.Items.Add("/dev/ttyUSB1");
            comboBox2.Items.Add("/dev/ttyUSB2");
            comboBox2.Items.Add("/dev/ttyUSB3");
            comboBox2.Text = "/dev/ttyUSB0";

            Thread thread1 = new Thread(getDataTranserData);
            thread1.Start();

        }

        //打开算法节点
        private void button1_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[5], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }
        
        //关闭算法节点
        private void button2_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[4], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        //打开GNSS节点
        private void button4_Click(object sender, EventArgs e) {
            //获取用户输入的设备号
            int deviceNum = 0;
            try {
                deviceNum = Int32.Parse(comboBox1.Text.Substring(comboBox1.Text.Length - 1, 1));
            } catch {
                System.Console.WriteLine("请输入合法设备号");
                return;
            }
            try {
                //替换设备号
                String cmd = ControllerCmd[1];
                cmd = cmd.Replace("0", deviceNum + "");
                dataTranser.WriteString(cmd, true);
            } catch {
                System.Console.WriteLine("请先打开串口");
                return;
            }
        }

        //关闭GNSS节点
        private void button3_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[0], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        //打开地图节点
        private void button9_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[3], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }


        //关闭地图节点
        private void button8_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[2], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        //打开车控节点
        private void button6_Click(object sender, EventArgs e) {
            //获取用户输入的设备号
            int deviceNum = 0;
            try {
                deviceNum = Int32.Parse(comboBox2.Text.Substring(comboBox2.Text.Length - 1, 1));
            } catch {
                System.Console.WriteLine("请输入合法设备号");
                return;
            }
            try {
                //替换设备号
                String cmd = ControllerCmd[7];
                cmd = cmd.Replace("0", deviceNum + "");
                dataTranser.WriteString(cmd, true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        //关闭车控节点
        private void button5_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[6], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        //强制停车
        private void button7_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[8], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button10_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[11], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private void button11_Click(object sender, EventArgs e) {
            try {
                dataTranser.WriteString(ControllerCmd[9], true);
            } catch {
                System.Console.WriteLine("请先打开串口");
            }
        }

        private delegate void SetTextCallbackWithContent(RichTextBox textBox, String tips);
        private delegate void AppendTextCallbackWithContentAndLimit(RichTextBox textBox, String tips, int maxCharsCount);
        // richTextBox1
        private void setRichTextBox(RichTextBox textBox, String tips) {
            if (textBox.InvokeRequired) {
                SetTextCallbackWithContent d = new SetTextCallbackWithContent(setRichTextBox);
                try {
                    textBox.Invoke(d, textBox, tips);
                } catch {
                    return;
                }
            } else {
                textBox.Text = tips;
            }
        }

        private void appendRichTextBox(RichTextBox textBox, String tips, int maxCharsCount) {
            if (textBox.InvokeRequired) {
                AppendTextCallbackWithContentAndLimit d = new AppendTextCallbackWithContentAndLimit(appendRichTextBox);
                try {
                    textBox.Invoke(d, textBox, tips);
                } catch {
                    return;
                }
            } else {
                textBox.AppendText(tips + "\n");
                if (textBox.Text.Length >= maxCharsCount) {
                    textBox.Text = "";
                }
                textBox.ScrollToCaret();
            }
        }


        char[] data_buffer;

        private void getDataTranserData() {

            data_buffer = new char[1000];
            bool startRecvCmdflag = false;
            int cmdByteCount = 0;
            String cmd = "";
            int cmdCount = 0;
      
            while (!this.IsDisposed) {
                //Array.Clear(result, 0, 4);
                //负责把#......................#####%这一字符串拼好，检查是否是50字节，拼好后传给其他handle函数
                int size = dataTranser.Read(data_buffer, 1000);
                String str = new String(data_buffer, 0, size);
                //for (int i = 0; i < size; i++) {
                //    Console.Write(data_buffer[i]);
                //}
                //Console.WriteLine();
                for (int i = 0; i < str.Length; i++) {
                    if (!startRecvCmdflag) {
                        if (str[i] == '#') {
                            startRecvCmdflag = true;
                            cmdByteCount = 1;
                            cmd = "#";
                        }
                    } else {
                        cmd += str[i];
                        cmdByteCount++;
                        if (str[i] == '%') {
                            //判断
                            if (cmdByteCount == 50) {
                                //Console.WriteLine(cmd);
                                if (cmdCount >= 999999) {
                                    cmdCount = 0;
                                }
                                //传给处理函数
                                if (cmd.StartsWith("#UISTATE")) {
                                    float[] carState = handleCarLocationState(cmd);
                                    setRichTextBox(richTextBox3, "X:" + carState[0] + "\nY:" + carState[1] + "\nAngle:" + carState[2] + "\nState:" + carState[3] + "\nCmdCount:" + cmdCount);
                                    Form2.setMapResult(carState);
                                    cmdCount++;
                                }
                                if (cmd.StartsWith("#ALGORITHMSTATE")) {
                                    float[] algorithmState = handleAlgorithmState(cmd);
                                    setRichTextBox(richTextBox1, "Left:" + algorithmState[0] + "\nRight:" + algorithmState[1] + "\nStopFlag:" + algorithmState[2] + "\nCmdCount:" + cmdCount);
                                    cmdCount++;
                                }
                                if (cmd.StartsWith("#CONTROLLERSTATE")) {
                                    float[] controllerState = handleControllerState(cmd);
                                    setRichTextBox(richTextBox1, "Speed:" + controllerState[0] + "\nDirection:" + controllerState[1] + "\nCmdCount:" + cmdCount);
                                    cmdCount++;
                                }
                                if (cmd.StartsWith("#GPRSSTATE")) {
                                    float[] gprsState = handleGprsState(cmd);
                                    setRichTextBox(richTextBox1, "Lon:" + gprsState[0] + "\nLat:" + gprsState[1] + "\nOriAngle:" + gprsState[2] + "\nState:" + gprsState[3] + "\nCmdCount:" + cmdCount);
                                    cmdCount++;
                                }
                            } else {
                                //System.Console.WriteLine("Wrong CMD: " + cmd);
                            }
                            //重置参数
                            startRecvCmdflag = false;
                            cmdByteCount = 0;
                            cmd = "";
                        }
                    }
                }
                //System.Console.WriteLine(str);
            }
        }

        private float[] handleGprsState(String str) {
            float[] gprsState = new float[4];
            int _baifenhao_index = str.IndexOf("%");
            if (_baifenhao_index == -1) {
                Console.WriteLine("Wrong Str Rear: " + str);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }
            str = str.Substring(0, _baifenhao_index);
            if (str.Length != 49) {
                Console.WriteLine("Wrong Str Length: " + str);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }

            int _N_index = str.IndexOf("_N");
            int _T_index = str.IndexOf("_T");
            int _A_index = str.IndexOf("_A");
            int _S_index = str.IndexOf("_S");

            if (_N_index == -1) {
                Console.WriteLine("Wrong Index: _N " + _N_index);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }
            if (_T_index == -1) {
                Console.WriteLine("Wrong Index: _T " + _T_index);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }
            if (_A_index == -1) {
                Console.WriteLine("Wrong Index: _A " + _A_index);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }
            if (_S_index == -1) {
                Console.WriteLine("Wrong Index: _S " + _S_index);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }

            String _N_str = null;
            String _T_str = null;
            String _A_str = null;
            String _S_str = null;
            try {
                _N_str = str.Substring(_N_index + 2, _T_index - _N_index - 2);
                _T_str = str.Substring(_T_index + 2, _A_index - _T_index - 2);
                _A_str = str.Substring(_A_index + 2, _S_index - _A_index - 2);
                _S_str = str.Substring(_S_index + 2, str.IndexOf("$") - _S_index - 2);
            } catch {
                Console.WriteLine("Wrong Str Format: " + str);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }
            try {
                gprsState[0] = float.Parse(_N_str);
                gprsState[1] = float.Parse(_T_str);
                gprsState[0] = float.Parse(_A_str);
                gprsState[1] = float.Parse(_S_str);
            } catch {
                Console.WriteLine("Wrong Number Str: " + _N_str + " " + _T_str + " " + _A_str + " " + _S_str);
                Array.Clear(gprsState, 0, gprsState.Length);
                return gprsState;
            }
            //System.Console.WriteLine("Result: {0} {1} {2}", algorithmState[0], algorithmState[1], algorithmState[2]);
            return gprsState;
        }

        private float[] handleControllerState(String str) {
            float[] controllerState = new float[3];
            int _baifenhao_index = str.IndexOf("%");
            if (_baifenhao_index == -1) {
                Console.WriteLine("Wrong Str Rear: " + str);
                Array.Clear(controllerState, 0, controllerState.Length);
                return controllerState;
            }
            str = str.Substring(0, _baifenhao_index);
            if (str.Length != 49) {
                Console.WriteLine("Wrong Str Length: " + str);
                Array.Clear(controllerState, 0, controllerState.Length);
                return controllerState;
            }

            int _S_index = str.IndexOf("_S");
            int _D_index = str.IndexOf("_D");
            if (_S_index == -1) {
                Console.WriteLine("Wrong Index: _S " + _S_index);
                Array.Clear(controllerState, 0, controllerState.Length);
                return controllerState;
            }
            if (_D_index == -1) {
                Console.WriteLine("Wrong Index: _D " + _D_index);
                Array.Clear(controllerState, 0, controllerState.Length);
                return controllerState;
            }
            String _S_str = null;
            String _D_str = null;
            try {
                _S_str = str.Substring(_S_index + 2, _D_index - _S_index - 2);
                _D_str = str.Substring(_D_index + 2, str.IndexOf("$") - _D_index - 2);
            } catch {
                Console.WriteLine("Wrong Str Format: " + str);
                Array.Clear(controllerState, 0, controllerState.Length);
                return controllerState;
            }
            try {
                controllerState[0] = float.Parse(_S_str);
                controllerState[1] = float.Parse(_D_str);
            } catch {
                Console.WriteLine("Wrong Number Str: " + _S_str + " " + _D_str);
                Array.Clear(controllerState, 0, controllerState.Length);
                return controllerState;
            }
            //System.Console.WriteLine("Result: {0} {1} {2}", algorithmState[0], algorithmState[1], algorithmState[2]);
            return controllerState;
        }

        private float[] handleAlgorithmState(String str) {
            float[] algorithmState = new float[3];
            int _baifenhao_index = str.IndexOf("%");
            if (_baifenhao_index == -1) {
                Console.WriteLine("Wrong Str Rear: " + str);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }
            str = str.Substring(0, _baifenhao_index);
            if (str.Length != 49) {
                Console.WriteLine("Wrong Str Length: " + str);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }

            int _L_index = str.IndexOf("_L");
            int _R_index = str.IndexOf("_R");
            int _S_index = str.IndexOf("_S");
            if (_L_index == -1) {
                Console.WriteLine("Wrong Index: _L " + _L_index);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }
            if (_R_index == -1) {
                Console.WriteLine("Wrong Index: _R " + _R_index);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }
            if (_S_index == -1) {
                Console.WriteLine("Wrong Index: _S " + _S_index);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }
            String _L_str = null;
            String _R_str = null;
            String _S_str = null;
            try {
                _L_str = str.Substring(_L_index + 2, _R_index - _L_index - 2);
                _R_str = str.Substring(_R_index + 2, _S_index - _R_index - 2);
                _S_str = str.Substring(_S_index + 2, str.IndexOf("$") - _S_index - 2);
            } catch {
                Console.WriteLine("Wrong Str Format: " + str);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }
            try {
                algorithmState[0] = float.Parse(_L_str);
                algorithmState[1] = float.Parse(_R_str);
                algorithmState[2] = float.Parse(_S_str);
            } catch {
                Console.WriteLine("Wrong Number Str: " + _L_str + " " + _R_str + " " + _S_str);
                Array.Clear(algorithmState, 0, algorithmState.Length);
                return algorithmState;
            }
            //System.Console.WriteLine("Result: {0} {1} {2}", algorithmState[0], algorithmState[1], algorithmState[2]);
            return algorithmState;
        }

        float[] carLocationState = new float[4];
        private float[] handleCarLocationState(String str) {
            
            int _baifenhao_index = str.IndexOf("%");
            if (_baifenhao_index == -1) {
                Console.WriteLine("Wrong Str Rear: " + str);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            str = str.Substring(0, _baifenhao_index);
            if (str.Length != 49) {
                Console.WriteLine("Wrong Str Length: " + str);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }

            int _X_index = str.IndexOf("_X");
            int _Y_index = str.IndexOf("_Y");
            int _A_index = str.IndexOf("_A");
            int _S_index = str.IndexOf("_S");
            if (_X_index == -1) {
                Console.WriteLine("Wrong Index: _X " + _X_index);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            if (_Y_index == -1) {
                Console.WriteLine("Wrong Index: _Y " + _Y_index);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            if (_A_index == -1) {
                Console.WriteLine("Wrong Index: _A " + _A_index);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            if (_S_index == -1) {
                Console.WriteLine("Wrong Index: _S " + _S_index);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            String _X_str = null;
            String _Y_str = null;
            String _A_str = null;
            String _S_str = null;
            try {
                _X_str = str.Substring(_X_index + 2, _Y_index - _X_index - 2);
                _Y_str = str.Substring(_Y_index + 2, _A_index - _Y_index - 2);
                _A_str = str.Substring(_A_index + 2, _S_index - _A_index - 2);
                _S_str = str.Substring(_S_index + 2, str.IndexOf("$") - _S_index - 2);
            } catch {
                Console.WriteLine("Wrong Str Format: " + str);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            try {
                carLocationState[0] = float.Parse(_X_str);
                carLocationState[1] = float.Parse(_Y_str);
                carLocationState[2] = float.Parse(_A_str);
                carLocationState[3] = float.Parse(_S_str);
            } catch {
                Console.WriteLine("Wrong Number Str: " + _X_str + " " + _Y_str + " " + _A_str + " " + _S_str);
                Array.Clear(carLocationState, 0, 4);
                return carLocationState;
            }
            //System.Console.WriteLine("Result: {0} {1} {2} {3}", carLocationState[0], carLocationState[1], carLocationState[2], carLocationState[3]);
            return carLocationState;
        }

       
    }
}
