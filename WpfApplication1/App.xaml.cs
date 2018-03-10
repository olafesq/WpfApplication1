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

        public void initCan() {
            kcan = new KvaserCAN(this);
        }

        public void deinitCan() {
            kcan.deinitCan();
        }

        public bool getCanOK()
        {
            if(kcan.status==0) return true;
            return false;
        }

        public void send2Terminal(string tekst)
        {
            App.Current.Dispatcher.Invoke(() => //UI has separate thread
            {
                MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                wnd.intoTerminal(tekst);
            });
        } 


    }
}
