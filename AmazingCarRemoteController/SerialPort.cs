using System;
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
            } catch (Exception e) {
                System.Console.WriteLine("串口打开失败");
                return false;
            }
            
        }

        public void DiscardInBuffer() {
            sp.DiscardInBuffer();
        }

        public int Read(char[] buffer, int size) {
            try {
                return sp.Read(buffer, 0, size);
            } catch {
                System.Console.WriteLine("强制中断数传");
                return 0;
            }
        }

        public int WriteString(String str, bool newLine) {
            try {
                if (newLine) {
                    sp.Write(str + "\n");
                } else {
                    sp.Write(str);
                }
                
            } catch {
                System.Console.WriteLine("强制中断数传");
            }
            return 0;
        }

        public void Close() {
            sp.Close();
        }

        private SerialPort sp;
    }
}
