using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;


namespace HighFlyers
{
    class RS232
    {
        private readonly SerialPort port; 
        private byte[] receiving_data; // array to store received data
        private StopBits stop_bits = StopBits.One;      // default value; if needed - add some code in the contructor
        private Parity parity = Parity.None; // default value; if needed - add some code in the contructor
        public delegate void DataReceivedEventHandler(byte[] rcvd); 
        public event DataReceivedEventHandler DataReceived;
        public bool StopReceiving = false; // for checking if receiving data is stopped
        readonly object lockingObj = new object();

        
        // RS232 constructor
		public RS232(string portName, string baud_rate, string data_bits)
        {
            port = new SerialPort
            {
                PortName = portName,
                BaudRate = int.Parse(baud_rate),
                DataBits = int.Parse(data_bits),
                StopBits = stop_bits,
                Parity = parity
            };

            port.DataReceived += DataReceivedHandler;
        }
            

        public RS232(string portName)
        {
            port = new SerialPort(portName);
            port.DataReceived += DataReceivedHandler;
        }

        
		// Event - some data came
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (port.IsOpen)
            {
                int count = port.BytesToRead;
                if (count > 0)
                    receiving_data = new byte[count];


                if (!StopReceiving && port.IsOpen && (count > 0))
                {
                    port.Read(receiving_data, 0, count);
                    DataReceived(receiving_data);
                }
            }
        }

		// sending data
        public int SendData(byte[] sending_data)
        {
            lock (lockingObj)
            {
                if (port.IsOpen)
                {
                    port.Write(sending_data, 0, sending_data.Length);
                    return port.BytesToWrite;
                }
                else
                    return 0;
            }
        }

		
        public static string[] ListOfAvailablePorts
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }
    
		
        public void OpenPort()
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
        }


        public void ClosePort()
        {
            if (port.IsOpen)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                System.Threading.Thread closeThread = new System.Threading.Thread(CloseTh);
                closeThread.Start();
            }
        }

        private void CloseTh()
        {
            port.Close();
        }


        public bool IsOpen
        {
            get { return (port != null) && port.IsOpen; }
        }

    }
}
