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
    class SonicSensor
    {
        private String ProcessLine;

        public static SerialPort comportSonicSensor = new SerialPort();

        //private StreamWriter myFile = new StreamWriter("./data/" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "-SON.txt", true);

        public SonicSensor()
        {
            // When data is recieved through the port, call this method
            comportSonicSensor.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        public void OpenPort(String PortName)
        {
            // If the port is open, close it.
            if (comportSonicSensor.IsOpen)
            {
                try
                {
                    //Close in another thread
                    Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                    CloseDown.Start();

                    //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnWeather", "Connect"));
                    //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboWeather", "true"));
                }
                catch
                {
                }
            }
            else
            {
                // Set the port's settings

                comportSonicSensor.BaudRate = 9600;  // BaudRate
                comportSonicSensor.DataBits = 8;     // DataBits
                comportSonicSensor.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One"); // StopBits
                comportSonicSensor.Parity = (Parity)Enum.Parse(typeof(Parity), "None");      // Parity
                comportSonicSensor.PortName = PortName;

                try
                {
                    // Open the port
                    comportSonicSensor.Open();

                    //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnWeather", "Disconnect"));
                    //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboWeather", "false"));
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
                if (!comportSonicSensor.IsOpen) return;

                // Read all the data waiting in the buffer
                string data = comportSonicSensor.ReadExisting();

                //Save this "data" to an existing text file
                //myFile.Write(data);
                //myFile.Flush();

                ProcessLine = ProcessLine + data;

                int i = ProcessLine.IndexOf("\r\n", 0);
                int j = ProcessLine.IndexOf("\r\n", i+1);
                int k = ProcessLine.Length;
                
                if (j > i)
                {
                    //Can process the line
                    String sSee = ProcessLine.Substring(i+2, j - i-2);

                    if (Convert.ToInt32(sSee) < 100)
                    {
                        Speech.Say("Close");
                    }

                    //myFile.Write(DateTime.Now.ToString("yyyyMMdd-HHmmss") + "," + sSee + Environment.NewLine);
                    //myFile.Flush();

                    // Display the text to the user in the terminal
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblSonicSensor", sSee + " cm"));
                 
                    //Move along the string
                    String sTmp = ProcessLine.Substring(j, k - j);
                    ProcessLine = sTmp;
                }
            }
            catch (Exception ex)
            {
                String Message = ex.Message.ToString();
            }
        }

        private static void CloseSerialOnExit()
        {
            try
            {
                //close the serial port
                comportSonicSensor.Close();
            }
            catch
            {
            }
        }
    }
}
