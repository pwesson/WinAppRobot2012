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
    class Weather
    {
        private String ProcessLine;

        public static SerialPort comportWeather = new SerialPort();

        //private StreamWriter myFile = new StreamWriter("./data/" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "-WTH.txt", true);

        public Weather()
        {
            // When data is recieved through the port, call this method
            comportWeather.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        public void OpenPort(String PortName)
        {
            // If the port is open, close it.
            if (comportWeather.IsOpen)
            {
                try
                {
                    //Close in another thread
                    Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                    CloseDown.Start();

                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnWeather", "Connect"));
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboWeather", "true"));
                }
                catch
                {
                }
            }
            else
            {
                // Set the port's settings

                comportWeather.BaudRate = 9600;  // BaudRate
                comportWeather.DataBits = 8;     // DataBits
                comportWeather.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One"); // StopBits
                comportWeather.Parity = (Parity)Enum.Parse(typeof(Parity), "None");      // Parity
                comportWeather.PortName = PortName;

                try
                {
                    // Open the port
                    comportWeather.Open();

                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnWeather", "Disconnect"));
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboWeather", "false"));
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
                if (!comportWeather.IsOpen) return;

                // Read all the data waiting in the buffer
                string data = comportWeather.ReadExisting();

                //Save this "data" to an existing text file
                //myFile.Write(data);
                //myFile.Flush();

                ProcessLine = ProcessLine + data;

                int i = ProcessLine.IndexOf('$');
                int j = 0;
                int k = 0;
                if (i != -1)
                {
                    j = ProcessLine.IndexOf("\r\n", i);
                    k = ProcessLine.Length;
                }

                if (j > i && i > -1)
                {
                    //Can process the line
                    String sSee = ProcessLine.Substring(i, j - i);

                    if (sSee.Substring(0, 1) == "$")
                    {
                        //myFile.Write(DateTime.Now.ToString("HH:mm:ss") + "," + sSee + Environment.NewLine);
                        //myFile.Flush();

                        string[] sInfo = sSee.Split(new char[] { ',', '\n' });

                        // Display the text to the user in the terminal
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblTemperature", ConvertFahrenheitToCelsius(Convert.ToDouble(sInfo[1])).ToString("0.0")));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblHumidity", sInfo[2]));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblDewpoint", ConvertFahrenheitToCelsius(Convert.ToDouble(sInfo[3])).ToString("0.0")));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblPressure", mmhg2millibars(inch2cm(Convert.ToDouble(sInfo[4]) * 10)).ToString("0.0")));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblRelativeLight", sInfo[5]));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWindSpeed", mph2kmh(Convert.ToDouble(sInfo[6])).ToString("0.0")));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWindDirection", sInfo[7]));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblRainfall", inch2cm(Convert.ToDouble(sInfo[8])).ToString("0.0")));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWeatherBattery", sInfo[9]));
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("textBox1", sSee));
                    }

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

        public static double ConvertCelsiusToFahrenheit(double c)
        {
            return ((9.0 / 5.0) * c) + 32;
        }

        public static double ConvertFahrenheitToCelsius(double f)
        {
            return (5.0 / 9.0) * (f - 32);
        }

        public static double inch2cm(double f)
        {
            return (2.54 * f);
        }

        public static double cm2inch(double f)
        {
            return (f / 2.54);
        }

        public static double mph2kmh(double f)
        {
            return (1.60934 * f);
        }

        public static double kmh2mph(double f)
        {
            return (f / 1.60934);
        }

        //1 mmhg = 1.33322368 millibars

        public static double mmhg2millibars(double f)
        {
            return (1.33322368 * f);
        }

        private static void CloseSerialOnExit()
        {
            try
            {
                //close the serial port
                comportWeather.Close();
            }
            catch
            {
            }
        }
    

    }
}
