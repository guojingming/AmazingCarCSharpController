using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmazingCarRemoteController {
    public partial class Form2 : Form {
        private static MySerialPort mySerialPort;
        public Form2() {
            InitializeComponent();
            this.mapPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseWheel);
        }

        public static void setPortObj(MySerialPort port) {
            mySerialPort = port;
        }

        public void mapPanel_MouseWheel(object sender, MouseEventArgs e) {
            if (e.Delta > 0) {
                if (pixelStep >= 0.1f) {
                    pixelStep -= 0.02f;
                }
            } else {
                pixelStep += 0.02f;
            }
            metterPixel = 1 / pixelStep;
        }

        private void Form2_Load(object sender, EventArgs e) {
            mySerialPort.WriteString("#UI_OPEN$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$", true);
            Thread th1 = new Thread(showTrace);
            th1.Start();

            textBox1.Text = "D:/Target1.txt";
        }

        private void button3_Click(object sender, EventArgs e) {
            Thread th2 = new Thread(getData);
            th2.Start();
        }

        
        

        MyPoint[] tracePoints;
        int[] traceColors;
        int tracePointCount;
        MyPoint latestTracePoint;
        int maxTracePointCount;
        MyPoint[] targetPoints;
        int targetPointCount;
        //1个像素等于实际距离的多少米
        float pixelStep;
        float metterPixel;

        Bitmap bmp;
        float[] result;
        char[] data_buffer;
        char[] real_data_buffer;
        private void getData() {
            data_buffer = new char[200];
            real_data_buffer = new char[50];
            while (!this.IsDisposed) {
                //Array.Clear(result, 0, 4);
                int size = mySerialPort.Read(data_buffer, 200);
                String str = new String(data_buffer);
                System.Console.WriteLine(str);
                if (!str.StartsWith("#CARSTATE")) {
                    int head_index = str.IndexOf("#CARSTATE");
                    if (head_index < 0) {
                        continue;
                    }
                    str = str.Substring(head_index);
                }
                int _baifenhao_index = str.IndexOf("%");
                if (_baifenhao_index == -1) {
                    Console.WriteLine("Wrong Str Rear: " + str);
                    continue;
                }
                str = str.Substring(0, _baifenhao_index);
                if (str.Length != 49) {
                    Console.WriteLine("Wrong Str Length: " + str);
                    continue;
                }

                int _X_index = str.IndexOf("_X");
                int _Y_index = str.IndexOf("_Y");
                int _A_index = str.IndexOf("_A");
                int _S_index = str.IndexOf("_S");
                if (_X_index == -1) {
                    Console.WriteLine("Wrong Index: _X " + _X_index);
                    continue;
                }
                if (_Y_index == -1) {
                    Console.WriteLine("Wrong Index: _Y " + _Y_index);
                    continue;
                }
                if (_A_index == -1) {
                    Console.WriteLine("Wrong Index: _A " + _A_index);
                    continue;
                }
                if (_S_index == -1) {
                    Console.WriteLine("Wrong Index: _S " + _S_index);
                    continue;
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
                    continue;
                }

                try {
                    result[0] = float.Parse(_X_str);
                    result[1] = float.Parse(_Y_str);
                    result[2] = float.Parse(_A_str);
                    result[3] = float.Parse(_S_str);
                } catch {
                    Console.WriteLine("Wrong Number Str: " + _X_str + " " + _Y_str + " " + _A_str + " " + _S_str);
                    continue;
                }
                System.Console.WriteLine("Result: {0} {1} {2} {3}", result[0], result[1], result[2], result[3]);
            }
        }

        private void showTrace() {
            result = new float[4];
            for (int i = 0; i < result.Length; i++) {
                result[i] = 0;
            }
            pixelStep = 0.1f;
            metterPixel = 1 / pixelStep;
            bmp = new Bitmap(mapPanel.Width, mapPanel.Height);
            Graphics g = Graphics.FromImage(bmp);
            Graphics rg = mapPanel.CreateGraphics();
            targetPoints = new MyPoint[100];
            for (int i = 0; i < 100; i++) {
                targetPoints[i] = new MyPoint();
            }
            targetPointCount = 0;
            maxTracePointCount = 200;
            tracePoints = new MyPoint[maxTracePointCount];
            for (int i = 0; i < maxTracePointCount; i++) {
                tracePoints[i] = new MyPoint();
            }
            traceColors = new int[maxTracePointCount];
            tracePointCount = 0;
            latestTracePoint = new MyPoint(-50000, -50000);
            while (!this.IsDisposed) {
                drawMap(g, rg, result);
            }
        }

        private void drawMap(Graphics g, Graphics rg, float[] result) {
            Color carColor = Color.Black;
            int state = (int)(result[3] + 0.5);
            switch (state) {
                case 4: carColor = Color.GreenYellow; break;
                case 5: carColor = Color.Red; break;
                case 3: carColor = Color.Red; break;
                case 2: carColor = Color.Red; break;
                case 1: carColor = Color.Red; break;
                default: carColor = Color.Black; break;
            }
            g.Clear(Color.White);
            float render_x = mapPanel.Width / 2 + result[0] / pixelStep;
            float render_y = mapPanel.Height / 2 - result[1] / pixelStep;
            while (result[2] < 0) {
                result[2] += 360;
            }
            //original point
            //##################################
            g.FillEllipse(blb, mapPanel.Width / 2 - 7, mapPanel.Height / 2 - 7, 15, 15);
            //##################################画目标点
            drawTargetPoints(g);
            //##################################画轨迹点
            drawTracePoints(g);
            //##################################画车
            drawTriangle(g, render_x, render_y, result[2] + 90, carColor);
            //##################################画参考线
            drawLines(g);
            //画字
            //原点坐标
            drawText(g, "O(0,0)", mapPanel.Width / 2 - 10, mapPanel.Height / 2 - 23, Color.Red);
            //车坐标
            drawText(g, "Car(" + result[0].ToString("F2") + "," + result[1].ToString("F2") + ")", render_x - 40, render_y - 30, Color.Red);

            //#################################刷新
            rg.DrawImage(bmp, 0, 0);
            MyPoint realPoint = new MyPoint();
            realPoint.X = result[0];
            realPoint.Y = result[1] * -1;
            if (distance(realPoint, latestTracePoint) >= 1.5) {
                tracePointCount += 1;
                if (tracePointCount >= maxTracePointCount) {
                    tracePointCount = 0;
                }
                tracePoints[tracePointCount] = realPoint;
                traceColors[tracePointCount] = state;
                latestTracePoint = realPoint;
            }
        }

        private float distance(MyPoint p1, MyPoint p2) {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void processCarState(char[] buffer, float[] result) {
           
            float[] carStateData = new float[4];
            Array.Clear(carStateData, 0, 4);
            String carStateStr = new String(buffer);
            int _X = carStateStr.IndexOf("_X");
            int _Y = carStateStr.IndexOf("_Y");
            int _A = carStateStr.IndexOf("_A");
            int _S = carStateStr.IndexOf("_S");
            String _X_str = carStateStr.Substring(_X + 2, _Y - _X - 2);
            String _Y_str = carStateStr.Substring(_Y + 2, _A - _Y - 2);
            String _A_str = carStateStr.Substring(_A + 2, _S - _A - 2);
            String _S_str = carStateStr.Substring(_S + 2, carStateStr.IndexOf("$") - _S - 2);

            result[0] = float.Parse(_X_str);
            result[1] = float.Parse(_Y_str);
            result[2] = float.Parse(_A_str);
            result[3] = float.Parse(_S_str);
        }

        Font textFont = new Font("Adobe Gothic Std", 10f, FontStyle.Regular);

        private void drawText(Graphics g, String text, float x, float y, Color color) {
            //选择字体、字号、风格
            if (color == Color.Black) {
                g.DrawString(text, textFont, bb, x, y);
            } else if (color == Color.Red) {
                g.DrawString(text, textFont, rb, x, y);
            } else if (color == Color.Blue) {
                g.DrawString(text, textFont, blb, x, y);
            } else {
                g.DrawString(text, textFont, bb, x, y);
            }
           
        }

        private void drawTriangle(Graphics g, float x, float y, float angle, Color color) {
            double[] ps = new double[6];
            double r1 = 20;
            double r2 = 8;
            double rad_angle = angle / 180 * 3.1415926;

            while (angle < 0) {
                angle += 360;
            }
            ps[0] = r1 * Math.Sin(rad_angle);
            ps[1] = -1 * r1 * Math.Cos(rad_angle);
            ps[2] = r2 * Math.Cos(rad_angle);
            ps[3] = r2 * Math.Sin(rad_angle);
            ps[4] = -1 * ps[2];
            ps[5] = -1 * ps[3];

            Point[] pp = new Point[3];
            for (int i = 0; i < 6; i++) {
                if (i % 2 == 0) {
                    pp[i / 2].X = (int)(ps[i] + x);
                } else {
                    pp[i / 2].Y = (int)(ps[i] + y);
                }
            }

            g.FillPolygon(new SolidBrush(color), pp);
        }

        private Brush bb = new SolidBrush(Color.Black);
        private Brush rb = new SolidBrush(Color.Red);
        private Brush ygb = new SolidBrush(Color.Green);
        private Brush blb = new SolidBrush(Color.Blue);
        private Brush gb = new SolidBrush(Color.GreenYellow);
 

        private void drawTracePoints(Graphics g) {
            
            for (int i = 0; i < tracePointCount; i++) {
                MyPoint p = tracePoints[i];
                int r = 6;
                int x = (int)(mapPanel.Width / 2 + p.X / pixelStep - r / 2);
                int y = (int)(mapPanel.Height / 2 + p.Y / pixelStep - r / 2);
                switch (traceColors[i]) {
                    case 4: g.FillEllipse(gb, x, y, r, r); break;
                    case 5: g.FillEllipse(rb, x, y, r, r); break;
                    case 3: g.FillEllipse(rb, x, y, r, r); break;
                    case 2: g.FillEllipse(rb, x, y, r, r); break;
                    case 1: g.FillEllipse(rb, x, y, r, r); break;
                    default: g.FillEllipse(bb, x, y, r, r); break;       
                }
                
            }
        }

        private void drawTargetPoints(Graphics g) {
            for (int i = 0; i < targetPointCount; i++) {
                MyPoint p = targetPoints[i];
                int x = (int)(mapPanel.Width / 2 + p.X / pixelStep);
                int y = (int)(mapPanel.Height / 2 - p.Y / pixelStep);
                g.FillEllipse(ygb, x - 5, y - 5, 10, 10);
            }
        }

        private void drawLines(Graphics g) {
            Pen pen = new Pen(Color.Black, 1);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            pen.DashPattern = new float[] { 5, 5 };
            String text = "";
            float calibration = 0;
            for (int i = 1; i < 10; i++) {
                //横线
                g.DrawLine(pen, 0, i * mapPanel.Height / 10, mapPanel.Width, i * mapPanel.Height / 10);
                //画字
                calibration = -1 * (i * mapPanel.Width / 10 - mapPanel.Width / 2) * pixelStep;
                text = calibration.ToString("F3");
                drawText(g, text, 0, i * mapPanel.Width / 10, Color.Black);

                //竖线
                g.DrawLine(pen, i * mapPanel.Width / 10, 0, i * mapPanel.Width / 10,mapPanel.Height);
                //画字
                calibration = (i * mapPanel.Height / 10 - mapPanel.Height / 2) * pixelStep;
                text = calibration.ToString("F3");
                drawText(g, text, i * mapPanel.Height / 10, 0, Color.Black);
            }  
        }

        private void mapPanel_MouseDown(object sender, MouseEventArgs e) {
            //Point p = e.Location;
            //p.X -= mapPanel.Width / 2;
            //p.Y -= mapPanel.Height / 2;
            
            //MyPoint mp = new MyPoint(p.X, p.Y);
            //targetPoints[targetPointCount++] = mp; 
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                //替换设备号
                String cmd = "#TAR_COUNT_0$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$";
                cmd = cmd.Replace("0", targetPointCount + "");
                mySerialPort.WriteString(cmd, true);
                for (int i = 0; i < targetPointCount; i++) {
                    cmd = "#TARS_Np1_Xp2_Yp3$";
                    cmd = cmd.Replace("p1", i + "");
                    cmd = cmd.Replace("p2", targetPoints[i].X + "");
                    cmd = cmd.Replace("p3", targetPoints[i].Y + "");
                    while (cmd.Length < 50) {
                        cmd += "$";
                    }
                    mySerialPort.WriteString(cmd, true);
                }
            } catch {
                System.Console.WriteLine("请先打开串口");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            for (int i = 0; i < targetPointCount; i++) {
                targetPoints[i].X = 0;
                targetPoints[i].Y = 0;
            }
            targetPointCount = 0;
            richTextBox1.Text = "";
        }

        private void button4_Click(object sender, EventArgs e) {
            for (int i = 0; i < tracePointCount; i++) {
                tracePoints[i].X = 0;
                tracePoints[i].Y = 0;
            }
            tracePointCount = 0;
        }

        private void button5_Click(object sender, EventArgs e) {

        }

        private void button5_Click_1(object sender, EventArgs e) {
            richTextBox1.Text = "";
            String filePath = textBox1.Text;
            try{
                 if (File.Exists(filePath)) {
                     targetPointCount = 0;
                     char[] separator = { '\r','\n' };
                     String tarText = File.ReadAllText(filePath);
                     string[] tars = tarText.Split(separator);
                     foreach(String tarStr in tars){
                         if (tarStr.Equals("")) {
                             continue;
                         }
                         float x = float.Parse(tarStr.Substring(0, tarStr.IndexOf(' ')));
                         float y = float.Parse(tarStr.Substring(tarStr.IndexOf(' ') + 1));
                         targetPoints[targetPointCount++] = new MyPoint(x, y);
                         richTextBox1.Text += (x + ", " + y + "\n");
                         
                     }
                 }else{
                     MessageBox.Show("文件不存在");
                 }
             }catch (Exception ex){
                 MessageBox.Show(ex.Message);
             }
        }

        
    }

    public class MyPoint {
        public float X;
        public float Y;
        public MyPoint() {
            X = 0;
            Y = 0;
        }
        public MyPoint(float x, float y) {
            X = x;
            Y = y;
        }
    }
}
