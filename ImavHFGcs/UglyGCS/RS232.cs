using System;
using System.IO.Ports;

namespace UglyGCS
{
    class RS232
    {
        private readonly SerialPort port; 
        private byte[] receivingData; // array to store received data
        private const StopBits CustomStopBits = StopBits.One; // default value; if needed - add some code in the contructor
        private const Parity CustomParity = Parity.None; // default value; if needed - add some code in the contructor
        public delegate void DataReceivedEventHandler(byte[] rcvd); 
        public event DataReceivedEventHandler DataReceived;
        public bool Stop = false; // for checking if receiving data is stopped

        readonly Object lockingObj;

		public RS232(string portName, int baudRate, int dataBits)
        {
            port = new SerialPort
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    DataBits = dataBits,
                    StopBits = CustomStopBits,
                    Parity = CustomParity
                };

		    port.DataReceived += DataReceivedHandler;

            lockingObj = new Object();
        }

        public RS232(string portName)
        {
            port = new SerialPort(portName);
            port.DataReceived += DataReceivedHandler;
            lockingObj = new Object();
        }

		// Event - some data came
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int count = port.BytesToRead;
            if(count > 0)
                receivingData = new byte[count];


            if (!Stop && port.IsOpen && (count > 0))
            {
                port.Read(receivingData, 0, count);
                DataReceived(receivingData);
            }

        }

		// sending data
        public int SendData(byte[] sendingData)
        {
            lock (lockingObj)
            {
                if (port.IsOpen)
                {
                    port.Write(sendingData, 0, sendingData.Length);
                    return port.BytesToWrite;
                }
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
                port.Close();
        }

        public bool IsOpen
        {
            get { return port != null && port.IsOpen; }
        }
    }
}
