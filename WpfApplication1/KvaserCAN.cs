using System;
using canlibCLSNET;
using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace FSFWTool
{
    class KvaserCAN
    {
        int hcan = -1;
        int bitr = Canlib.canBITRATE_500K;
        int channel = 0;
        int flashTalkId = 887;
        int flashProgID = 888;
        const int NACK = 19;
        const int ACK = 17;

        List<int> ids = new List<int>();
        MainWindow wnd;
       

        public KvaserCAN(MainWindow wnd) //public constructor
        {           
            this.wnd = wnd;
            
            Canlib.canInitializeLibrary();
                        
            hcan = Canlib.canOpenChannel(channel, Canlib.canOPEN_REQUIRE_INIT_ACCESS);
            if (hcan <0 )
            {
                string error;
                Canlib.canGetErrorText((Canlib.canStatus)hcan, out error);
                   send2Terminal(error);
            }
            else
            {
                Canlib.canSetBusParams(hcan, bitr, 0, 0, 0, 0, 0); //parameters set by dafault based on bitr
                Canlib.canBusOn(hcan);
                   send2Terminal("Can liin avatud");
                
                //DumpMessageLoop(hcan);                          
            }
     
        }

        public void DumpMessageLoop()
        {
            Canlib.canStatus status;
            bool finished = false;

            //These variables hold the incoming message
            byte[] data = new byte[8];
            int id;
            int dlc;
            int flags;
            long time;
            
            send2Terminal("Channel opened. Press Disconnect to close. ");
            send2Terminal("ID  DLC DATA                      Timestamp");

            while (!finished)
            {
                //Wait for up to 100 ms for a message on the channel
                status = Canlib.canReadWait(hcan, out id, data, out dlc, out flags, out time, 100);
                

                //If a message was received, display i
                if (status == Canlib.canStatus.canOK)
                {
                    DumpMessage(id, data, dlc, flags, time);
                }

                //Call DisplayError and exit in case an actual error occurs
                else if (status != Canlib.canStatus.canERR_NOMSG)
                {
                    CheckStatus(status, "canRead/canReadWait");
                    lock (MainWindow._locker) wnd.runCan = false;
                    deinitCan();                                  
                }

                //Breaks the loop if the user presses the Disconnect key                
                lock (MainWindow._locker) finished = !wnd.runCan;

            }
            //deinitCan();
        }

        internal int FWwrite(byte[] buffer2, int id)
        {
            //int id = 888;
            Canlib.canStatus status;
            status = Canlib.canWrite(hcan, id, buffer2, 8, Canlib.canMSG_STD);
            if (status != Canlib.canStatus.canOK) send2Terminal("mingi error sõnumi saatmisel");
            
            //Get ACK response
            byte[] data = new byte[8];
            int dlc;
            int flags;
            long time;
                   
            while(id!=889) Canlib.canReadWait(hcan, out id, data, out dlc, out flags, out time, 100);

            if (data[0] == ACK) return ACK;
            else return NACK;

        }

        public void FWSend() //Start reprogramming flash!
        {
            int response = 0;

            byte[] initFl = { 1, 2, 3, 4, 5, 6, 7, 8 }; //Ask flash to initialized
            response = FWwrite(initFl, flashTalkId);
            if (response == NACK)
            {
                MessageBox.Show("Failed to initialize flash.");
                return;
            } 

            MessageBox.Show("Starting to program new FW!");
            string fileName = "D:/Documents/TTY/FormulaStudent/CanBootloader/stm32disco_application/Debug/stm32diso_application.bin";
            byte[] buffer = File.ReadAllBytes(fileName);
            int count = buffer.Length; //The number of bytes to write.
            Console.WriteLine("byte array length " + count);

            int i = 0;
            byte[] buffer2 = new byte[8]; //can msg max length is 8 bytes
            int stop = 0;
            int id = 888;


            while (i < buffer.Length-8)
            {
                //buffer2[0] = buffer[i];
                Array.Copy(buffer, i, buffer2, 0, 8);
                //myComPort.Write(buffer2, offset, 1);
                response = FWwrite(buffer2, flashProgID);

                if (response != ACK) //continue only if respone is ACK
                {
                    MessageBox.Show("Failed programming, no ACK or timeout");
                    break;
                }
                i = i + 4; //STM seems to write to flash only WORD length
                send2Terminal("*");
            }
            MessageBox.Show("Finished programming");

        }

        //Prints an incoming message to the screen
        private void DumpMessage(int id, byte[] data, int dlc, int flags, long time)
        {
            if ((flags & Canlib.canMSG_ERROR_FRAME) != 0)
            {
                //Console.WriteLine("Error Frame received ****");
                   send2Terminal("Error Frame received ****");
            }
            else
            {
                   send2Terminal("{0}  {1}  {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}    {10}"+
                                                 id+ dlc+ data[0]+ data[1]+ data[2]+ data[3]+ data[4]+
                                                 data[5]+ data[6]+ data[7]+ time);
                whoisThere(id);
            }
        }

        //This method prints an error if something goes wrong
        private void CheckStatus(Canlib.canStatus status, string method)
        {
            if (status < 0)
            {
                   send2Terminal(method + " failed: " + status.ToString());
            }
        }

        void whoisThere(int id)
        {
            //ids.SetValue(id, 0);
            if (ids.IndexOf(id)==-1) ids.Add(id);
            
               send2Terminal("Device ID: " + id);
        }

        void send2Terminal(string tekst)
        {
            Application.Current.Dispatcher.Invoke(() => //UI has separate thread
            {
                wnd.intoTerminal(tekst);
            });
        }

        public void EraseFl()
        {
            byte[] erase = { 1, 2, 3, 4, 4, 3, 2, 1 };
            //int id = 887;
            FWwrite(erase, flashTalkId);
        }

        public void ReadFl()
        {
            byte[] read = { 1,1,2,2,3,3,4,4 };
            //int id = 887;
            FWwrite(read, flashTalkId);
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
