// Windows Application - RoboticBoat 
// Copyright (C) 2012 https://www.roboticboat.uk
// 89141ce8-8ac1-4705-bb40-a2b6e173a952
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
// These Terms shall be governed and construed in accordance with the laws of 
// England and Wales, without regard to its conflict of law provisions.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;
using System.Threading;

using Util.EventMessages;

namespace RoboticBoat
{
    class GPS
    {
        public static SerialPort comportGPS = new SerialPort();
        public enum LogMsgType { Incoming, Outgoing, Normal, Warning, Error };

        private String ProcessLine;
        //private StreamWriter myFile = new StreamWriter("./data/" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "-GPS.txt", true);
        private String sGPGGA = "";

        public GPS()
        {
            // When data is recieved through the port, call this method
            comportGPS.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        public void OpenPort(String PortName)
        {
            // If the port is open, close it.
            if (comportGPS.IsOpen)
            {
                try
                {
                    //Close in another thread
                    Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                    CloseDown.Start();

                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnGPS", "Connect"));
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboGPS", "true"));
                }
                catch
                {
                }
            }
            else
            {
                // Set the port's settings

                comportGPS.BaudRate = 4800;  // BaudRate
                comportGPS.DataBits = 8;     // DataBits
                comportGPS.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One"); // StopBits
                comportGPS.Parity = (Parity)Enum.Parse(typeof(Parity), "None");      // Parity
                comportGPS.PortName = PortName; 

                try
                {
                    // Open the port
                    comportGPS.Open();

                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnGPS", "Disconnect"));
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboGPS", "false"));    
                }
                catch
                {
                }
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // If the com port has been closed, do nothing
                if (!comportGPS.IsOpen) return;

                string data = comportGPS.ReadExisting();
                ProcessLine = ProcessLine + data;

                int i = ProcessLine.IndexOf("$GPGGA");
                if (i == -1) return;

                int j = ProcessLine.IndexOf("\r\n", i);
                int k = ProcessLine.Length;

                if (j > i)
                {
                    //Can process the line
                    String sSee = ProcessLine.Substring(i, j - i);

                    sGPGGA = sSee;

                    if (sSee.Substring(0, 6) == "$GPGGA")
                    {
                        string[] sInfo = sSee.Split(new char[] { ',', '\n' });

                        // Display the text to the user in the terminal
                        ScreenUpdate(LogMsgType.Incoming, sInfo[4].ToString());
                    }

                    //Move along the string
                    //String sTmp = ProcessLine.Substring(j, k - j);
                    ProcessLine = "";
                }
            }
            catch (Exception ex)
            {
                String Message = ex.Message.ToString();
            }
        }


        private void ScreenUpdate(LogMsgType msgtype, string msg)
        {
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("textBox2", sGPGGA.ToString()));

            //Split the message up

            String[] CmdParts;
            CmdParts = sGPGGA.Split(',');

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblLatitude", DecimalPosition(CmdParts[2]).ToString("0.00000")));

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblNS", CmdParts[3].ToString()));

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblLongitute", DecimalPosition(CmdParts[4]).ToString("0.00000")));

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblEW", CmdParts[5].ToString()));

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblFix", CmdParts[6].ToString()));
            
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblSatilites", CmdParts[7].ToString()));
            
        }

        private static void CloseSerialOnExit()
        {
            try
            {
                //close the serial port
                comportGPS.Close();
            }
            catch
            {
            }
        }

        private double DecimalPosition(String Lat)
        {
            int n = Lat.Length;

            double degree = Convert.ToDouble(Lat.Substring(0, n - 7));

            double minutes = Convert.ToDouble(Lat.Substring(n-7,7));

            return degree + minutes/60;
        }

    }
}
