using System;
using System.Text;
using System.IO.Ports;

namespace IBISutilities
{
    class SerialManager
    {
        #region Manager Variables
        //property variables
        private string _baudRate = string.Empty;
        private string _parity = string.Empty;
        private string _stopBits = string.Empty;
        private string _dataBits = string.Empty;
        private string _portName = string.Empty;
        private bool _emulate7e1 = false;
        //global manager variables
        //private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();
        private IBISutils utils = new IBISutils();
        #endregion

        #region Manager Properties
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public bool Emulate7e1
        {
            get { return _emulate7e1; }
            set { _emulate7e1 = value; }
        }
        #endregion

        #region Manager Constructors
        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baud">Desired BaudRate</param>
        /// <param name="par">Desired Parity</param>
        /// <param name="sBits">Desired StopBits</param>
        /// <param name="dBits">Desired DataBits</param>
        /// <param name="name">Desired PortName</param>
        public SerialManager(string baud, string par, string sBits, string dBits, bool emu7e1, string name)
        {
            _baudRate = baud;
            _parity = par;
            _stopBits = sBits;
            _dataBits = dBits;
            _emulate7e1 = emu7e1;
            _portName = name;
            //now add an event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to nothing
        /// </summary>
        public SerialManager()
        {
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
            _emulate7e1 = false;
            _portName = "COM1";
            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        #endregion

        #region WriteIbisData
        public void WriteIbisData(string msg)
        {
            string ibisSCMsg;
            byte[] ibisSndMsg;
            char ibisParity;
            //first make sure the port is open
            //if its not open then open it
            try
            {
                if (!(comPort.IsOpen == true)) comPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // replace special characters in string 
            ibisSCMsg = utils.ReplaceIbisSC(msg);
            // calculate parity Byte
            ibisParity = utils.GetIbisParity(ibisSCMsg + '\r');
            //display the data to the user
            Console.WriteLine("out> " + ibisSCMsg + "<CR>" + "<" + Convert.ToString(ibisParity, 16) + ">");
            if (_emulate7e1 == true)
            {
                // emulate even parity
                ibisSndMsg = utils.EmulateEvenParity(ibisSCMsg + '\r' + ibisParity);
            }
            else
            {
                ibisSndMsg = System.Text.Encoding.ASCII.GetBytes(ibisSCMsg + '\r' + ibisParity);
            }
            //send the message to the port
            //comPort.Write(ibisSndMsg, 0, ibisSndMsg.Length);
            try
            {
                for (int i = 0; i < ibisSndMsg.Length; i++)
                {
                    comPort.Write(ibisSndMsg, i, 1);
                    System.Threading.Thread.Sleep(20);
                }
                //display the data to the user
                //Console.WriteLine("out> " + ByteToHex(ibisSndMsg) );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion


        #region HexToByte
        /// <summary>
        /// method to convert hex string into a byte array
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] HexToByte(string msg)
        {
            //remove any spaces from the string
            msg = msg.Replace(" ", "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[msg.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < msg.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// method to convert a byte array into a hex string
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        private string ByteToHex(byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            //return the converted value
            return builder.ToString().ToUpper();
        }
        #endregion


        #region OpenPort
        public bool OpenPort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (comPort.IsOpen == true) comPort.Close();

                //set the properties of our SerialPort Object
                comPort.BaudRate = int.Parse(_baudRate);    //BaudRate
                comPort.DataBits = int.Parse(_dataBits);    //DataBits
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity
                comPort.PortName = _portName;   //PortName
                //now open the port
                comPort.Open();
                //display message
                Console.WriteLine("Port " + comPort.PortName + " opened at " + DateTime.Now );
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region ClosePort
        public bool ClosePort()
        {
            try
            {
                if (comPort.IsOpen == true) comPort.Close();
                //display message
                Console.WriteLine("Port " + comPort.PortName + " closed at " + DateTime.Now );
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //retrieve number of bytes in the buffer
            int bytes = comPort.BytesToRead;
            //create a byte array to hold the awaiting data
            byte[] comBuffer = new byte[bytes];
            //read the data and store it
            comPort.Read(comBuffer, 0, bytes);
            //display the data to the user
            Console.WriteLine("In  < " + ByteToHex(comBuffer) + "\n");
        }
        #endregion
    }
}
