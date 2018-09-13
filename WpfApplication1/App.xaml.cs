using System;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace FSFWTool
{
    /// <summary>
    /// Interaction logic for App.xaml    /// 
    /// </summary>
    public partial class App : Application
    {
    }
}
    //    KvaserCAN kcan;
    //    MainWindow wnd;

    //    public void initCan() {
    //        kcan = new KvaserCAN(this);
    //    }

    //    public bool checkRunCan() {
    //            return wnd.runCan;          
    //    }

    //    public void send2Terminal(string tekst)
    //    {
    //        App.Current.Dispatcher.Invoke(() => //UI has separate thread
    //        {                
    //            wnd.intoTerminal(tekst);
    //        });
    //    } 

    //    public void setMainW()
    //    {
    //        wnd = (MainWindow)Application.Current.MainWindow;
    //    }

    //    internal void FWSend()
    //    {
    //        const int NACK = 19;
    //        const int ACK = 17;

    //        MessageBox.Show("Starting to program new FW!");
    //        string fileName = "D:/Documents/TTY/FormulaStudent/CanBootloader/stm32disco_application/Debug/stm32diso_application.bin";
    //        byte[] buffer = File.ReadAllBytes(fileName);
    //        int count = buffer.Length; //The number of bytes to write.
    //        Console.WriteLine("byte array length " + count);

    //        int i = 0;
    //        byte[] buffer2 = new byte[1];
    //        int stop = 0;
            

    //        while (i < count && stop == 0)
    //        {
    //            int response = 0;
    //            buffer2[0] = buffer[i];
    //            //myComPort.Write(buffer2, offset, 1);
    //            response = kcan.FWwrite(buffer2); 
                
    //            if (response != ACK) //continue only if respone is ACK
    //            {          
    //                MessageBox.Show("Failed programming, no ACK or timeout");
    //                break;
    //            }
    //            i++;
    //        }
    //        MessageBox.Show("Finished programming");
    //    }
    
    //}
//}
