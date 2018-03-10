using System.IO.Ports;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml    /// 
    /// </summary>
    public partial class App : Application
    {

        KvaserCAN kcan;
        MainWindow wnd;

        public void initCan() {
            kcan = new KvaserCAN(this);
        }

        public bool checkRunCan() {
                return wnd.runCan;          
        }

        public bool getCanOK()
        {
            if(kcan.hcan>=0) return true;
            return false;
        }

        public void send2Terminal(string tekst)
        {
            App.Current.Dispatcher.Invoke(() => //UI has separate thread
            {                
                wnd.intoTerminal(tekst);
            });
        } 

        public void setMainW()
        {
            wnd = (MainWindow)Application.Current.MainWindow;
        }


    }
}
