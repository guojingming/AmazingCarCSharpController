using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AmazingCarRemoteController;
using System.Threading;

/*
Suppose you have a long flowerbed in which some of the plots are planted and some are not. However, flowers cannot be planted in adjacent plots - they would compete for water and both would die.

Given a flowerbed (represented as an array containing 0 and 1, where 0 means empty and 1 means not empty), and a number n, return if n new flowers can be planted in it without violating the no-adjacent-flowers rule.
  
 */

namespace WheeledCarTestController {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private MySerialPort serialPort;

        byte[] command = new byte[16];
        byte commandId = 0;

        private void button1_Click(object sender, EventArgs e) {
            serialPort = new MySerialPort();
            serialPort.OpenSerialPort(textBox3.Text, 115200);

            Thread thread = new Thread(func);
            thread.Start();

            Thread thread1 = new Thread(func1);
            thread1.Start();

        }

        private float speed = 0;
        private float direction = 0;

        private void func1() {
            char[] bbb = new char[100];
            while (true) {
                Array.Clear(bbb, 0, 100);
                serialPort.Read(bbb, 100);
                for (int i = 0; i < 100; i++) {
                    Console.Write("{0} ", (int)bbb[i]);
                    if (bbb[i] == 0x55) {
                        Console.WriteLine();
                    }
                }
            }
        }

        private void func() {
            byte[] buffer = new byte[4];
            char[] bbb = new char[24];
            while (true) {
          
                int speedCmdValue = (int)(speed * 100);
                intToByte(speedCmdValue, buffer);
                command[0] = 0xAA;
                command[1] = commandId;
                commandId++;
                command[2] = 0x01;
                command[3] = 0x00;
                command[4] = buffer[2];
                command[5] = buffer[3];
                command[6] = 0x00;
                command[7] = 0x00;

                command[14] = 0x00;
 
                int directionSpeedValue = (int)(direction * 3.1415926 * 100 / 18);
                intToByte(directionSpeedValue, buffer);
                command[8] = buffer[2];
                command[9] = buffer[3];
                command[10] = 0x00;
                command[11] = 0x00;
                command[12] = 0x00;
                command[13] = 0x00;
                for (int i = 0; i < 14; i++) {
                    command[14] += command[i];
                }
                command[15] = 0x55;

                //for (int i = 0; i < 16; i++) {
                //    Console.Write("{0} ", command[i]);
                //}
                //Console.WriteLine();
                
                serialPort.Write(command);
                //serialPort.Read(bbb,24);
                //for (int i = 0; i < 24; i++) {
                //    Console.Write("{0} ", (int)bbb[i]);
                //    if (bbb[i] == 0x55) {
                //        Console.WriteLine();
                //    }
                //}
            }
        }


        private void button2_Click(object sender, EventArgs e) {
            String speedStr = textBox1.Text;
            speed = float.Parse(speedStr);
            String directionStr = textBox2.Text;
            direction = float.Parse(textBox2.Text);
            //Thread t1 = new Thread(func1);
            //t1.Start();
        }

    

        //int 转 4字节 BYTE[],
        private void intToByte(long i, byte[] abyte) {
	        abyte[3] = (byte)(0xff & i);
	        abyte[2] = (byte)((0xff00 & i) >> 8);
	        abyte[1] = (byte)((0xff0000 & i) >> 16);
	        abyte[0] = (byte)((0xff000000 & i) >> 24);
        }

        //4字节 BYTE[] 转 int 
        private long bytesToInt(byte[] bytes) {
            long addr = bytes[3] & 0xFF;
            addr |= ((bytes[2] << 8) & (long)0xFF00);
            addr |= ((bytes[1] << 16) & (long)0xFF0000);
            addr |= ((bytes[0] << 24) & (long)0xFF000000);
	        //long int result = (x[0] << 24) + (x[1] << 16) + (x[2] << 8) + x[3];   
	        return addr;
        }
    }
}
