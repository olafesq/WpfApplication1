using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FSFWTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{

        MySerial mySerial;        
        FlowDocument mcFlowDoc = new FlowDocument();
        //App myApp = ((App)Application.Current);
        Paragraph para = new Paragraph();

        KvaserCAN kvaser;
        //MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public static readonly object _locker = new object();

        string inputData = string.Empty;
        string portName;
        public volatile bool runCan = false;

        public MainWindow()
        {
            InitializeComponent();

            //myApp.setMainW();

            string[] ArrayComPortsNames = null;
            int index = -1;
            string ComPortName = null;
            

            ArrayComPortsNames = SerialPort.GetPortNames();

            comboBox.Items.Add("KvaserCAN");            

            do
            {
                index += 1;
                comboBox.Items.Add(ArrayComPortsNames[index]);
            } while (!((ArrayComPortsNames[index] == ComPortName) ||
                                (index == ArrayComPortsNames.GetUpperBound(0))));

            //Array.Sort(ArrayComPortsNames);

            //want to get first out
            //if (index == ArrayComPortsNames.GetUpperBound(0))
            //{
            //    ComPortName = ArrayComPortsNames[0];
            //}
            //comboBox.Text = ArrayComPortsNames[0];
            comboBox.Text = comboBox.Items.GetItemAt(0).ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            

            if ((string)button.Content == "Connect")
            {
                portName = comboBox.SelectedValue.ToString();
                if (portName == "KvaserCAN")
                {
                    kvaser = new KvaserCAN(this);
                    
                    startCanMsgDump();
                                   
                    this.button.Content = "Disconnect";
                }
                else
                {
                    mySerial = new MySerial(portName);
                    if (mySerial.myComPort.IsOpen) this.button.Content = "Disconnect";
                    else intoTerminal("Ei õnnestunud");
                }
                                  
                richTextBox.Focus();               
            }
            else
            {
                try
                {
                    if (portName == "KvaserCAN")
                    {
                        lock(_locker) runCan = false;
                        kvaser.deinitCan();                   
                    }
                    else mySerial.myComPort.Close();
                    intoTerminal(portName +" Disconnected!");
                    button.Content = "Connect";
                }
                catch (NullReferenceException)
                {
                    intoTerminal(portName + " pole ühendatud");
                }
            }     
        }

        public void intoTerminal(string text)
        {
            // Assign the value of the plot to the RichTextBox.
            para.Inlines.Add(text + "\n");
            mcFlowDoc.Blocks.Add(para);
            richTextBox.Document = mcFlowDoc;
            richTextBox.CaretPosition = richTextBox.Document.ContentEnd;            
            //richTextBox.ScrollToEnd();
        }

        private void Data2Send(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                //intoTerminal("data2send called!" + inputData);
                mySerial.DataSend(inputData);
                inputData = string.Empty;
            }
            else if (e.Key >= System.Windows.Input.Key.D0 && e.Key <= System.Windows.Input.Key.D9)
            {
                // Number keys pressed so need to so special processing
                //intoTerminal("data2send called!" + e.Key);
                mySerial.DataSend(e.Key.ToString().Substring(1));
            }
            else
            {
                inputData += e.Key;
            }
        }

        void startCanMsgDump()
        {
            lock (_locker) runCan = true;
            Thread t1 = new Thread(() => this.kvaser.DumpMessageLoop());
            t1.Start();
        }

         private void bProgram_Click(object sender, RoutedEventArgs e)
        {
            if (runCan)
            {
                //lock (_locker) runCan = false; //stops msgDump

                Thread fwth = new Thread(() =>
                {
                    KvaserCAN kvaserB = new KvaserCAN(this);
                    kvaserB.FWSend();
                });
                fwth.Start();
                           
            }
            else mySerial.FWSend();
        }

        private void bEraseF_Click(object sender, RoutedEventArgs e)
        {
            if (runCan)
            {
                lock (_locker) runCan = false; //stops msgDump

                Thread fwth = new Thread(kvaser.EraseFl);
                fwth.Start();

            }
        }

        private void bReadF_Click(object sender, RoutedEventArgs e)
        {
            if (runCan)
            {
                lock (_locker) runCan = false; //stops msgDump

                Thread fwth = new Thread(kvaser.ReadFl);
                fwth.Start();

            }
        }
    }
}
