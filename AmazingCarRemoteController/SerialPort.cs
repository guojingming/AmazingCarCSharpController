﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazingCarRemoteController {
    public class MySerialPort {
        //
        public bool OpenSerialPort(string portName, int baudRate) {
            try {
                sp = new SerialPort();
                sp.DataBits = 8;
                sp.BaudRate = baudRate;
                sp.PortName = portName;
                sp.Open();
                return true;
            } catch {
                System.Console.WriteLine("串口打开失败");
                return false;
            }    
        }

        public bool IsOpen() {
            if (sp == null) {
                return false;
            }
            return sp.IsOpen;
        }

        public void DiscardInBuffer() {
            sp.DiscardInBuffer();
        }

        public void DiscardOutBuffer() {
            sp.DiscardOutBuffer();
        }

        public int Read(char[] buffer, int size) {
            try {
                return sp.Read(buffer, 0, size);
            } catch {
                System.Console.WriteLine("串口读取失败！请检查串口是否已断开！");
                return 0;
            }
        }

        public int Write(byte[] buffer) {
            try {
                sp.Write(buffer, 0, buffer.Length);
            } catch {
                System.Console.WriteLine("串口写入失败！请检查串口是否已断开！");
            }
            return 0;
        }

        public int WriteString(String str, bool newLine) {
            try {
                if (newLine) {
                    sp.Write(str + "\n");
                } else {
                    sp.Write(str);
                }
            } catch {
                System.Console.WriteLine("串口写入失败！请检查串口是否已断开！");
            }
            return 0;
        }

        public void Close() {
            sp.Close();
        }

        private SerialPort sp;
    }
}
