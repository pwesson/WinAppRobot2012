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
    // Types of output formats
    public enum tOutputFormat
    {
        cmdOutFormatUnknow = 0,
        cmdOutFormatC = 1,
        cmdOutFormatOHPR = 2,
        cmdOutFormatHCHDT = 4,
        cmdOutFormatPlain = 8,
        cmdOutFormatUser = 16,
    }

    class Compass
    {
        //private StreamWriter myFile2 = new StreamWriter("./data/" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "-CMP.txt", true);

        // Know if we're quering compasss
        bool CompassQuering = false;

        // The main control for communicating through the RS-232 port
        private SerialPort comportCompass = new SerialPort();

        // Create a buffer so we can store all the incoming data in case the Compass
        // is faster than the PC current proccess
        private string sPrevBuffer = "";

        // Store Compass information from <esc>& command
        private tCmpData theCmpData = new tCmpData();


        public Compass()
        {
            // Initialize the data we have from the compass
            ResetCmpData();

            // Reset the labels in the screen with "none" data
            //ResetLabels();

            // Assign the function to be called when we receive data
            // The function is created by us
            // When data is recieved through the Serial Port, call this method (PortHandler)
            comportCompass.DataReceived += new SerialDataReceivedEventHandler(PortHandler);

        }

        public void OpenPort(String PortName)
        {
            // Check current port status
            if (comportCompass.IsOpen == false)
            {
                // Set up the port with current configuration (call the function we made)
                ApplyPortCfg(PortName);

                // Now, try to open it
                try
                {
                    // Reset compass data
                    theCmpData.Valid = false;
                 
                    // Open the Serial Port
                    comportCompass.Open();
         
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnCompass", "Disconnect"));
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboCompass", "false"));
                }
                catch
                {
                }
            }
            else
            {
                //Close in another thread
                Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                CloseDown.Start();

                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("btnCompass", "Connect"));
                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboCompass", "true"));
            }
        }


        // Struct used to store compass fields data from the <esc>& command
        public struct tCmpData
        {
            public bool Valid;             // Did we recieve the information from <esc>& command?

            public string FwVer;
            public string FwDate;
            public string SerialNumber;
            public tOutputFormat OutFormat;
            public long Fields;
            public int Deguass;
            public float Deviation;
            public float Declination;
            public bool SoftIron;
            public int BaudRate;
            public int OutRate;
            public int PSI;
            public int AvgSamples;
            public int ADDecimation;
            public int Mounting;
            public float TempOffset;
            public int AccRange;
            public bool QuietMode;
            public string TestDate;
            public float DepthOffset;
            public float AccXScale;
            public float AccYScale;
            public float AccZScale;
            public float AccXOffset;
            public float AccYOffset;
            public float AccZOffset;
            public float MagXScale;
            public float MagYScale;
            public float MagZScale;
            public float MagXOffset;
            public float MagYOffset;
            public float MagZOffset;
            public float MagXBridge;
            public float MagYBridge;
            public float MagZBridge;
            public float TotalGauss;
            public bool IsKFilter;
            public float KFilter;
            public float A;
            public float B;
            public float C;
            public float E;
            public float GyroPitchQGyro;
            public float GyroPitchQAngle;
            public float GyroPitchRAngle;
            public float GyroRollQGyro;
            public float GyroRollQAngle;
            public float GyroRollRAngle;
        }

        private void ResetCmpData()
        {
            theCmpData.A = 0;
            theCmpData.AccRange = 0;
            theCmpData.AccXOffset = 0;
            theCmpData.AccXScale = 0;
            theCmpData.AccYOffset = 0;
            theCmpData.AccYScale = 0;
            theCmpData.AccZOffset = 0;
            theCmpData.AccZScale = 0;
            theCmpData.ADDecimation = 0;
            theCmpData.AvgSamples = 0;
            theCmpData.B = 0;
            theCmpData.BaudRate = 0;
            theCmpData.C = 0;
            theCmpData.Declination = 0;
            theCmpData.Deguass = 0;
            theCmpData.DepthOffset = 0;
            theCmpData.Deviation = 0;
            theCmpData.E = 0;
            theCmpData.Fields = 0;
            theCmpData.FwDate = "";
            theCmpData.FwVer = "";
            theCmpData.GyroPitchQAngle = 0;
            theCmpData.GyroPitchQGyro = 0;
            theCmpData.GyroPitchRAngle = 0;
            theCmpData.GyroRollQAngle = 0;
            theCmpData.GyroRollQGyro = 0;
            theCmpData.GyroRollRAngle = 0;
            theCmpData.IsKFilter = false;
            theCmpData.KFilter = 0;
            theCmpData.MagXBridge = 0;
            theCmpData.MagXOffset = 0;
            theCmpData.MagXScale = 0;
            theCmpData.MagYBridge = 0;
            theCmpData.MagYOffset = 0;
            theCmpData.MagYScale = 0;
            theCmpData.MagZBridge = 0;
            theCmpData.MagZOffset = 0;
            theCmpData.MagZScale = 0;
            theCmpData.Mounting = 0;
            theCmpData.OutFormat = tOutputFormat.cmdOutFormatUnknow;
            theCmpData.OutRate = 0;
            theCmpData.PSI = 0;
            theCmpData.QuietMode = false;
            theCmpData.SerialNumber = "";
            theCmpData.SoftIron = false;
            theCmpData.TempOffset = 0;
            theCmpData.TestDate = "";
            theCmpData.TotalGauss = 0;
            theCmpData.Valid = false;   //Changed to true
        }

        private void PortHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // We must get the data in the Serial Port Buffer and then parse them
            // So create a new variable where we can store it
            string InData = "";

            // Call the readExisting method from the Serial port to read incoming data
            InData = comportCompass.ReadExisting();

            // Here we clean up all the data existing in our software buffers
            // The idea is to get all the sentences from the start $ until the CrLF
            // then, we pass the sentence to the NMEA parser.
            // Note that each time the data is received from the serial port it can be
            // an incomplete NMEA sentence, so here we join all the data and wait to complete it
            // Also we can get two NMEA sentence in one event, so we split them here
            sPrevBuffer += InData; // Join all the previous data with the one we received

            //Debug.WriteLine(InData);

            // Handle the new information on the original UI thread
            ParseSerialPort(sender, e);
        }

        private void ParseSerialPort(object s, EventArgs e)
        {
            string theNMEA = ""; // Used to store the NMEA sentence
            int CrLfPos = -1;   // Used to know the position of the end of the NMEA

            // Check if we have a crlf string (end of the NMEA)
            CrLfPos = sPrevBuffer.IndexOf("\r");

            // Split the NMEA sentence by each crlf
            while (CrLfPos > -1)
            {
                if (CrLfPos > 0)
                {
                    // Get the NMEA (start on 1 as we need to remove the \n too)
                    theNMEA = sPrevBuffer.Substring(1, CrLfPos - 1);

                    // Now that we get the clean NMEA command we must parse it,
                    // so we call the function that do that
                    if (theNMEA != "") ParseNMEACmd(theNMEA);
                }

                // Remove the command from our buffer as we processed it (check if it's the latest one)
                if (sPrevBuffer.Length > CrLfPos + 1)
                    sPrevBuffer = sPrevBuffer.Substring(CrLfPos + 1);
                else
                    sPrevBuffer = "";

                // Find end of next command (if any)
                CrLfPos = sPrevBuffer.IndexOf("\r");
            }
        }

        private void ParseNMEACmd(string theCmd)
        {

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("textBox3", theCmd.ToString()));

            // Try to parse the data
            try
            {
                // Check if we're not quering the compass
                if (CompassQuering == false)
                {
                    // Check if it's a data sentence or a message sentence:
                    // If data, then it starts with $ or a number, else with any other character
                    // We check that to know if we should proccess it as a message or as data
                    if ((theCmd.Substring(0, 1) != "$") && (char.IsNumber(theCmd, 1) == false))
                    {
                        // Add this message to the messages viewer (reverse order)
                        //   txtMsgs.Text = theCmd + "\r\n" + txtMsgs.Text;
                        // Check if we have too many characters on there
                        //   if (txtMsgs.Text.Length > 1000) txtMsgs.Text = "";
                    }
                    else
                    {
                        string[] CmdParts;  // To split command by comma
                        int i, Val;         // Use for indexes

                        // Check if we can decode (depends on the output format)
                        //if (theCmpData.Valid == false)
                        //{
                        // We don't know the output format of the compass,  
                        // so ask data to the compass with the <esc>& format
                        // in order to decode it...
                        //QueryCompassData();
                        //}
                        //else
                        if (1 == 1)
                        {
                            // Check output format
                            switch (theCmpData.OutFormat)
                            {
                                case tOutputFormat.cmdOutFormatC:
                                case tOutputFormat.cmdOutFormatUnknow:

                                    string Data = "", CmdName = "";
                                    // Proccess NMEA Sentence...

                                    //Save this "data" to an existing text file /////////////
                                    //myFile2.Write(DateTime.Now.ToString("HH:mm:ss") + "," + theCmd + Environment.NewLine);
                                    //myFile2.Flush();

                                    // Example of Compass NMEA sentence:
                                    // $C216.2P26.7R11.0T25.1*27
                                    // First we remove checksum and initial $
                                    theCmd = theCmd.Substring(1, theCmd.Length - 4);

                                    // Add commas to the NMEA sentence
                                    theCmd = AddNMEACommas(theCmd);


                                    // Now we should have something like:
                                    // C,216.2,P,26.7,R,11.0,T,25.1
                                    // So split the commands by ","
                                    CmdParts = theCmd.Split(',');

                                    // Finally we go trough each command
                                    // We have the array like Command, Data, Command, Data, etc
                                    // so that's why we increment by 2
                                    for (i = 0; i < CmdParts.Length; i += 2)
                                    {
                                        // Save the Name and Data of this parameter
                                        CmdName = CmdParts[i];
                                        Data = CmdParts[i + 1];

                                        // Select the parameter name and do action
                                        switch (CmdName)
                                        {
                                            case "C":   // Compass Heading
                                                // Refresh data in the control
                                                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblDHeading", Data));
                                                break;
                                            case "P":   // Pitch Angle
                                                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblDPitch", Data));
                                                break;
                                            case "R":   // Roll Angle
                                                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblDRoll", Data));
                                                break;
                                            case "T":   // Temperature of the compass board
                                                //    lblDTemp.Text = Data;
                                                break;
                                            case "D":   // Depth
                                                //    lblDDepth.Text = Data;
                                                break;
                                            case "M":   // Magnetic Scalar
                                                //    lblDMag.Text = Data;
                                                break;
                                            case "Mx":  // Magnetic X
                                                //    lblDMagX.Text = Data;
                                                break;
                                            case "My":  // Magnetic Y
                                                //    lblDMagY.Text = Data;
                                                break;
                                            case "Mz":  // Magnetic Z
                                                //    lblDMagZ.Text = Data;
                                                break;
                                            case "A":   // Acceleration Scalar
                                                //    lblDAcc.Text = Data;
                                                break;
                                            case "Ax":  // Acceleration X
                                                //    lblDAccX.Text = Data;
                                                break;
                                            case "Ay":  // Acceleration Y
                                                //    lblDAccY.Text = Data;
                                                break;
                                            case "Az":  // Acceleration Z
                                                //    lblDAccZ.Text = Data;
                                                break;
                                            case "L":   // Life Count
                                                //    lblDLifeCount.Text = Data;
                                                break;
                                            case "G":   // Gyro Scalar
                                                //    lblDGyro.Text = Data;
                                                break;
                                            case "Gx":  // Gyro X
                                                //    lblDGyroX.Text = Data;
                                                break;
                                            case "Gy":  // Gyro Y
                                                //    lblDGyroY.Text = Data;
                                                break;
                                            case "S":   // SPI Errors
                                                //    lblDSPIErr.Text = Data;
                                                break;
                                        }

                                    }

                                    break;

                                case tOutputFormat.cmdOutFormatOHPR:
                                case tOutputFormat.cmdOutFormatPlain:

                                    // If it's OHPR output format, just remove header and trailer...
                                    if (theCmpData.OutFormat == tOutputFormat.cmdOutFormatOHPR)
                                        theCmd = theCmd.Substring(6, theCmd.Length - 8); // 6 for the OHPR, 2 for the TX
                                    // ...and treath it as Format Plain 

                                    // Used for indexes
                                    i = 0; Val = 1;

                                    // Split the commands by comma
                                    CmdParts = theCmd.Split(',');

                                    do
                                    {
                                        // Check if this field is visible (with the Fields parameter)
                                        if ((theCmpData.Fields & Val) == Val)
                                        {

                                            // Check the field we have to update
                                            switch (Val)
                                            {
                                                case 1:   // Compass Heading
                                                    //lblDHeading.Text = CmdParts[i] + "°";
                                                    break;
                                                case 2:   // Pitch angle
                                                    //lblDPitch.Text = CmdParts[i];
                                                    break;
                                                case 4:   // Roll angle
                                                    //lblDRoll.Text = CmdParts[i];
                                                    break;
                                                case 8:   // Temperature of the compass board
                                                    //    lblDTemp.Text = CmdParts[i];
                                                    break;
                                                case 16:   // Depth
                                                    //    lblDDepth.Text = CmdParts[i];
                                                    break;
                                                case 32:   // Magnetic Scalar
                                                    //    lblDMag.Text = CmdParts[i];
                                                    break;
                                                case 64:  // Magnetic X, Y, Z (because the comes all together)
                                                    //    lblDMagX.Text = CmdParts[i];
                                                    // Get next command
                                                    i++;
                                                    //   lblDMagY.Text = CmdParts[i];
                                                    i++;
                                                    //   lblDMagZ.Text = CmdParts[i];
                                                    break;
                                                case 128:   // Acceleration Scalar
                                                    //   lblDAcc.Text = CmdParts[i];
                                                    break;
                                                case 256:  // Acceleration X, Y, Z
                                                    //    lblDAccX.Text = CmdParts[i];
                                                    i++;
                                                    //    lblDAccY.Text = CmdParts[i];
                                                    i++;
                                                    //    lblDAccZ.Text = CmdParts[i];
                                                    break;
                                                case 512: // Gyro Scalar
                                                    //    lblDGyro.Text = CmdParts[i];
                                                    break;

                                                case 1024: // Gyro X Y
                                                    //    lblDGyroX.Text = CmdParts[i];
                                                    i++;
                                                    //    lblDGyroY.Text = CmdParts[i];
                                                    break;
                                                case 2048: // Life count
                                                    //    lblDLifeCount.Text = CmdParts[i];
                                                    break;

                                                case 4096: // SPI Errors (Means SPI can't be used from an interrupt)
                                                    //    lblDSPIErr.Text = CmdParts[i];
                                                    break;

                                            }

                                            // Increment data
                                            i = i + 1;
                                        }

                                        // Increment current field
                                        Val = Val * 2;
                                    }
                                    while ((Val < 8192) && (i <= CmdParts.GetUpperBound(0)));

                                    break;

                                case tOutputFormat.cmdOutFormatHCHDT:

                                    // This format only outputs the Heading so decode it
                                    // Split by comma
                                    CmdParts = theCmd.Split(',');

                                    // Check data integrity
                                    if (CmdParts.GetUpperBound(0) >= 1)
                                    {
                                        //    lblDHeading.Text = CmdParts[1];
                                    }

                                    break;
                            }
                        }

                    }
                }
                else
                {
                    // Parse compass query data
                    ParseCompassInfo(theCmd);
                }
            }
            catch
            {
            }
        }

        private string AddNMEACommas(string theInput)
        {
            int i = 0; bool PrevAscii = true;
            char Ascii = ' ';

            // We want to add comma between number and letter to separaet fields
            // so then we can split it. So run trough each character and check if 
            // it's number or char. 
            while (i < theInput.Length)
            {
                // Get the ascii
                Ascii = theInput[i];
                // Check if it's a number and there were a character before
                if (!char.IsLetter(Ascii) && (PrevAscii == true))
                {
                    // Insert the comma
                    theInput = theInput.Insert(i, ",");
                    i++; // Incremenet position (so we don't get the comma)
                    PrevAscii = false; // Unflag ascii
                }
                else
                {
                    // Check if it's a character
                    if (char.IsLetter(Ascii) && (PrevAscii == false))
                    {
                        // Insert comma, increment position, and flag the chracter
                        theInput = theInput.Insert(i, ",");
                        i++;
                        PrevAscii = true;
                    }
                }
                // Increment char position
                i++;
            }

            // Return result
            return theInput;
        }


        private void ParseCompassInfo(string theData)
        {

            // Check if it's valid string
            if (theData.IndexOf("=") > -1)
            {
                //Debug.WriteLine("Heere!");
                // Split line by Equal Char
                string[] CmdParts = theData.Split('=');

                // Check integrity
                if (CmdParts.GetUpperBound(0) == 1)
                {
                    // Get field
                    CmdParts[0] = CmdParts[0].Trim().ToUpper();
                    // Check Field
                    switch (CmdParts[0])
                    {
                        case "FW_VERSION": theCmpData.FwVer = CmdParts[1].Trim(); theCmpData.Valid = true; break;
                        case "FW_DATE": theCmpData.FwDate = CmdParts[1].Trim(); break;
                        case "SERIAL_NUMBER": theCmpData.SerialNumber = CmdParts[1].Trim(); break;
                        case "TEST_DATE": theCmpData.TestDate = CmdParts[1].Trim(); break;
                        case "OUTPUT_FORMAT": theCmpData.OutFormat = (tOutputFormat)Enum.Parse(typeof(tOutputFormat), CmdParts[1].Trim(), true); break;
                        case "DISPLAY_FIELDS": theCmpData.Fields = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "HW_MOUNTING_POS": theCmpData.Mounting = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "BAUD_RATE": theCmpData.BaudRate = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "SET-RESET_RATE": theCmpData.Deguass = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "OUTPUT_RATE": theCmpData.OutRate = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "AVERAGING": theCmpData.AvgSamples = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "AD_UPDATE_RATE": theCmpData.ADDecimation = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "DEVIATION": theCmpData.Deviation = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "DECLINATION": theCmpData.Declination = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "TEMP_OFFSET": theCmpData.TempOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "DEPTH_OFFSET": theCmpData.DepthOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAX_PSI": theCmpData.PSI = Convert.ToInt16(CmdParts[1].Trim()); break;
                        case "ACC_X_OFFSET": theCmpData.AccXOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "ACC_Y_OFFSET": theCmpData.AccYOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "ACC_Z_OFFSET": theCmpData.AccZOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "ACC_X_SCALE": theCmpData.AccXScale = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "ACC_Y_SCALE": theCmpData.AccYScale = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "ACC_Z_SCALE": theCmpData.AccZScale = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_X_OFFSET": theCmpData.MagXOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_Y_OFFSET": theCmpData.MagYOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_Z_OFFSET": theCmpData.MagZOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_X_SCALE": theCmpData.MagXScale = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_Y_SCALE": theCmpData.MagYScale = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_Z_SCALE": theCmpData.MagZScale = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_X_BRIDGE_OFFSET": theCmpData.MagXOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_Y_BRIDGE_OFFSET": theCmpData.MagYOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "MAG_Z_BRIDGE_OFFSET": theCmpData.MagZOffset = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "TOTAL_GAUSS": theCmpData.TotalGauss = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "KFILTER": theCmpData.KFilter = Convert.ToSingle(CmdParts[1].Trim()); break; // IsKFilter = True
                        case "GYR_ROLL_QGYRO": theCmpData.GyroPitchQGyro = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "GYR_ROLL_QANGLE": theCmpData.GyroRollQAngle = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "GYR_ROLL_RANGLE": theCmpData.GyroRollRAngle = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "GYR_PITCH_QGYRO": theCmpData.GyroPitchQGyro = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "GYR_PITCH_QANGLE": theCmpData.GyroPitchQAngle = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "GYR_PITCH_RANGLE": theCmpData.GyroPitchRAngle = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "A": theCmpData.A = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "B": theCmpData.B = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "C": theCmpData.C = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "E": theCmpData.E = Convert.ToSingle(CmdParts[1].Trim()); break;
                        case "SOFTIRONCORRECTION":
                            if (CmdParts[1].Trim() == "0")
                                theCmpData.SoftIron = false;
                            else
                                theCmpData.SoftIron = true;
                            break;
                    }

                }
            }

        }

        private void QueryCompassData()
        {
            comportCompass.Write(Convert.ToString(Convert.ToChar(27)));      // <esc> (we use ascii code)
            theCmpData.Valid = false;        // Flag current data from compass is invalid
            CompassQuering = true;           // Flag we're quering 
            Delay(500);                     // Wait for 500 ms
            comportCompass.Write("&");                     // A
            Delay(500);             // Wait for 500 ms
            comportCompass.Write(" "); // Send command value
            // Unflag we're querying
            CompassQuering = false;

            // Fill boxes with current configuration
            ParseCurSettings();
        }

        void Delay(int ms)
        {
            int time = Environment.TickCount;

            while (true)
            {
                System.Windows.Forms.Application.DoEvents();
                if (Environment.TickCount - time >= ms) return;
            }
        }

        void ParseCurSettings()
        {
            // Check if the information is valid
            if (theCmpData.Valid)
            {
                // Field checkboxes (do an AND mask with Fields value)
                //chkX01Azimuth.Checked = ((theCmpData.Fields & 1) == 1);
                //chkX02Pitch.Checked = ((theCmpData.Fields & 2) == 2);
                //chkX03Roll.Checked = ((theCmpData.Fields & 4) == 4);
                //chkX04Temp.Checked = ((theCmpData.Fields & 8) == 8);
                //chkX05Depth.Checked = ((theCmpData.Fields & 16) == 16);
                //chkX06Mag.Checked = ((theCmpData.Fields & 32) == 32);
                //chkX07MagVect.Checked = ((theCmpData.Fields & 64) == 64);
                //chkX08Acc.Checked = ((theCmpData.Fields & 128) == 128);
                //chkX09AccVect.Checked = ((theCmpData.Fields & 256) == 256);
                //chkX10Gyro.Checked = ((theCmpData.Fields & 512) == 512);
                //chkX11GyroVect.Checked = ((theCmpData.Fields & 1024) == 1024);
                //chkX12Life.Checked = ((theCmpData.Fields & 2048) == 2048);
                //chkX13SPI.Checked = ((theCmpData.Fields & 4096) == 4096);

                // Fill others
                //udAvgSamples.Value = theCmpData.AvgSamples;
                //udDeguass.Value = theCmpData.Deguass;
                //udOutFormat.Value = (int)theCmpData.OutFormat;
                //udOutRate.Value = theCmpData.OutRate;

            }
        }

        private void ApplyPortCfg(String PortName)
        {
            // Check if the port is properly closed
            if (comportCompass.IsOpen) comportCompass.Close();

            // Now, apply the configuration (readed from the file: Settings.settings see project explorer)
            try
            {
                comportCompass.PortName = PortName; //cboCompass.Text;  
                comportCompass.Parity = (Parity)System.IO.Ports.Parity.None; //Settings.Default.Parity;
                comportCompass.StopBits = (StopBits)System.IO.Ports.StopBits.One; //Settings.Default.StopBits;
                comportCompass.DataBits = 8; // Settings.Default.DataBits;
                comportCompass.BaudRate = 19200; // Settings.Default.BaudRate;
            }
            catch
            {
            }
        }


        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        private void CloseSerialOnExit()
        {
            try
            {
                //close the serial port
                comportCompass.Close();
            }
            catch
            {
            }
        }

    }
}
