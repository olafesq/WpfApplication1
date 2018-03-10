using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{

        MySerial mySerial;        
        FlowDocument mcFlowDoc = new FlowDocument();
        App myApp = ((App)Application.Current);
        Paragraph para = new Paragraph();
        string inputData = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            
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
            string portName = comboBox.SelectedValue.ToString();

            if ((string)button.Content == "Connect")
            {                
                if (portName == "KvaserCAN")
                {
                    myApp.initCan(); //intention is to start it in different thread then UI
                    if (myApp.getCanOK()) this.button.Content = "Disconnect";
                    else intoTerminal("Ei õnnestunud");
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
                if(portName == "KvaserCAN") myApp.deinitCan();
                else mySerial.myComPort.Close();
               
                intoTerminal("Disconnected!");
                this.button.Content = "Connect";

            }     
        }

        public void intoTerminal(string text)
        {
            // Assign the value of the plot to the RichTextBox.
            para.Inlines.Add(text + "\n");
            mcFlowDoc.Blocks.Add(para);
            richTextBox.Document = mcFlowDoc;
            richTextBox.CaretPosition = richTextBox.Document.ContentEnd;            
            richTextBox.ScrollToEnd();
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

        private void bProgram_Click(object sender, RoutedEventArgs e)
        {
            mySerial.FWSend();
        }
    }
}
