using System;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace WpfApplication1
{
    class MySerial
    {
        public SerialPort myComPort;
       
        public MySerial(string myPortName) //constructor
        {
            myComPort = new SerialPort();

            myComPort.PortName = myPortName;
            myComPort.BaudRate = 2400;
            myComPort.Handshake = System.IO.Ports.Handshake.None;
            myComPort.Parity = Parity.None;
            myComPort.DataBits = 8;
            myComPort.StopBits = StopBits.One;
            myComPort.ReadTimeout = -1;
            myComPort.WriteTimeout = -1;

            myComPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            try
            {
                myComPort.Open();            
                wnd.intoTerminal("Connected to "+ myComPort.PortName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening my port: {0}", ex.Message);
                wnd.intoTerminal(myComPort.PortName + " in use!");
                
            }  
            
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            string recieved_data = myComPort.ReadLine();           
            App.Current.Dispatcher.Invoke(() =>
            {
                MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                wnd.intoTerminal(recieved_data);
            });
        }

        public void DataSend(string data)
        {
            myComPort.WriteLine(data);
        }

        public void FWSend()
        {
            MessageBox.Show("Starting to program new FW!");
            string fileName = "D:/Documents/TTY/FormulaStudent/CanBootloader/stm32disco_application/Debug/stm32diso_application.bin";
            byte[] buffer = File.ReadAllBytes(fileName);
            int offset = 0;
            int count = buffer.Length;
            myComPort.Write(buffer, offset, count);

            MessageBox.Show("Finished programming");
        }
    }
}
