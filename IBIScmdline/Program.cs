using System;

using IBISutilities;

namespace IBIScmdline
{
    class Program
    {

        static void Main(string[] args)
        {
            SerialManager comm = new SerialManager();
            string comPort;
            string msgStr;
            var myPorts = System.IO.Ports.SerialPort.GetPortNames();
            Boolean comPortFound = false;

            if (args.Length == 2)
            {
                comPort = args[0];
                msgStr = args[1];
                foreach (string item in myPorts)
                {
                    if (item == comPort)
                    {
                        comPortFound = true;
                        break;
                    }
                }
                if (comPortFound)
                {
                    comm.Parity = System.IO.Ports.Parity.None.ToString();
                    comm.StopBits = System.IO.Ports.StopBits.Two.ToString();
                    comm.DataBits = "8";
                    comm.BaudRate = "1200";
                    comm.PortName = comPort;
                    comm.Emulate7e1 = true;
                    Console.Out.Write("open " + comm.PortName + " (" + comm.BaudRate + " Baud, " + comm.DataBits + " Data, " + comm.Parity + " Parity, " + comm.StopBits +
                         " Stop Bits, " + "emulate e72 = " + comm.Emulate7e1 + ")\n");
                    comm.OpenPort();
                    comm.WriteIbisData(msgStr);
                    Console.Out.Write("out> \"" + msgStr + "\"\n");

                }
                else
                {
                    Console.Out.Write("invalid com Port: " + comPort);
                }
            }
            else
            {
                Console.Out.Write("invalid parameters");
            }
        }
    }
}
