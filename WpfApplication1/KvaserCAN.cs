using System;
using canlibCLSNET;

namespace WpfApplication1
{
    class KvaserCAN
    {
        App myApp;
        
        public int hcan = -1;
        int bitr = Canlib.canBITRATE_100K;
        int channel = 0;
               

        public KvaserCAN(App myApp) //public constructor
        {
            this.myApp = myApp;

            Canlib.canInitializeLibrary();

            
            hcan = Canlib.canOpenChannel(channel, Canlib.canOPEN_REQUIRE_INIT_ACCESS);
            if (hcan <0 )
            {
                string error;
                Canlib.canGetErrorText((Canlib.canStatus)hcan, out error);
                myApp.send2Terminal(error);
            }
            else
            {                
                Canlib.canSetBusParams(hcan, bitr, 0, 0, 0, 0, 0);
                Canlib.canBusOn(hcan);
                myApp.send2Terminal("Can liin avatud");
                
                DumpMessageLoop(hcan);                          
            }
            

        }

        private void DumpMessageLoop(int handle)
        {
            Canlib.canStatus status;
            bool finished = false;

            //These variables hold the incoming message
            byte[] data = new byte[8];
            int id;
            int dlc;
            int flags;
            long time;

            myApp.send2Terminal("Channel opened. Press Disconnect to close. ");
            myApp.send2Terminal("ID  DLC DATA                      Timestamp");

            while (!finished)
            {
                //Wait for up to 100 ms for a message on the channel
                status = Canlib.canReadWait(handle, out id, data, out dlc, out flags, out time, 100);

                //If a message was received, display i
                if (status == Canlib.canStatus.canOK)
                {
                    DumpMessage(id, data, dlc, flags, time);
                }

                //Call DisplayError and exit in case an actual error occurs
                else if (status != Canlib.canStatus.canERR_NOMSG)
                {
                    CheckStatus(status, "canRead/canReadWait");
                    finished = true;
                }

                //Breaks the loop if the user presses the Disconnect key
                if (!myApp.checkRunCan())
                {
                        finished = true;                    
                }
            }
        }


        //Prints an incoming message to the screen
        private void DumpMessage(int id, byte[] data, int dlc, int flags, long time)
        {
            if ((flags & Canlib.canMSG_ERROR_FRAME) != 0)
            {
                //Console.WriteLine("Error Frame received ****");
                myApp.send2Terminal("Error Frame received ****");
            }
            else
            {
                myApp.send2Terminal("{0}  {1}  {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}    {10}"+
                                                 id+ dlc+ data[0]+ data[1]+ data[2]+ data[3]+ data[4]+
                                                 data[5]+ data[6]+ data[7]+ time);
            }
        }

        //This method prints an error if something goes wrong
        private void CheckStatus(Canlib.canStatus status, string method)
        {
            if (status < 0)
            {
                myApp.send2Terminal(method + " failed: " + status.ToString());
            }
        }

        public void deinitCan()
        {
            if (hcan >=0 )
            {
                Canlib.canBusOff(hcan);
                Canlib.canClose(hcan);
            }
            hcan = -1;            
        }
    }

    
    
}
