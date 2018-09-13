using System;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace FSFWTool
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
            myComPort.ReadTimeout = 1000;
            myComPort.WriteTimeout = 1000;

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
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                wnd.intoTerminal(recieved_data);
            });
        }

        public void DataSend(string data)
        {
            if (data.Equals("1")) myComPort.Write(data);
            else myComPort.WriteLine(data);
        }

        public void FWSend()
        {
            const int NACK = 19;
            const int ACK = 17;

            MessageBox.Show("Starting to program new FW!");
            string fileName = "D:/Documents/TTY/FormulaStudent/CanBootloader/stm32disco_application/Debug/stm32diso_application.bin";
            byte[] buffer = File.ReadAllBytes(fileName);
            int offset = 0;
            int count = buffer.Length; //The number of bytes to write.
            Console.WriteLine("byte array length " + count);
                        
            int i = 0;
            byte[] buffer2 = new byte[1];
            int stop = 0;
            
            while (i < count && stop==0)
            {                
                buffer2[0] = buffer[i];
                myComPort.Write(buffer2, offset, 1);

                int timeoutNack = 0;
                int response = 0;
                while (response!=ACK) //continue only if respone is ACK
                {
                    stop = 1;

                    try
                    {
                        response = myComPort.ReadByte(); 

                        if (response == NACK || timeoutNack == 100)
                        {
                            MessageBox.Show("Failed programming, no ACK or timeout");
                            break;
                        }
                        else if (response == ACK) stop = 0;
                            //nack = 19, ack = 17        
                            //System.Threading.Thread.Sleep(5); 
                    }
                    catch (Exception ex)
                    {
                        timeoutNack++;                        
                    }
                    timeoutNack++;
                }
                i++;
            }
            MessageBox.Show("Finished programming");
        }
    }
}
