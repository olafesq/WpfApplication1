using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using canlibCLSNET;


namespace WpfApplication1
{
    class KvaserCAN
    {
        App myApp;
        public int status=-1;
        int hcan = -1;
        int bitr = Canlib.canBITRATE_100K;
        int channel = 0;

        public KvaserCAN(App myApp) //public constructor
        {
            this.myApp = myApp;

            Canlib.canInitializeLibrary();
            
            status = Canlib.canOpenChannel(channel, Canlib.canOPEN_REQUIRE_INIT_ACCESS);
            if (status <0 )
            {
                string error;
                Canlib.canGetErrorText((Canlib.canStatus)status, out error);
                myApp.send2Terminal(error);
            }
            else
            {
                hcan = status;
                Canlib.canSetBusParams(hcan, bitr, 0, 0, 0, 0, 0);
                myApp.send2Terminal("Can liin avatud");
            }
            

        }

        public void deinitCan()
        {
            if (status >=0 )
            {
                Canlib.canBusOff(hcan);
                Canlib.canClose(hcan);
            }
            status = hcan = -1;
        }
    }
}
