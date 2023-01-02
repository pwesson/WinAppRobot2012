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

//using RoboticsConnection.Serializer;
//using RoboticsConnection.Serializer.Components;
//using RoboticsConnection.Serializer.Controllers;
//using RoboticsConnection.Serializer.Ids;
//using RoboticsConnection.Serializer.Sensors;

using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Reflection;
using System.Timers; 

using Util.EventMessages;

namespace RoboticBoat
{

    class Servos
    {
        private bool ServoControlConnected = false;

        //public Serializer serializer;
        //public AnalogSensor batteryVoltage;
        private static System.Timers.Timer aTimer = new System.Timers.Timer(1000);

        //private ServoMotorController servo1;
        //private ServoMotorController servo2;
        //private ServoMotorController servo3;
        //private ServoMotorController servo4;
        //private ServoMotorController servo5;
        //private ServoMotorController servo6;

        public Servos()
        {
            //serializer = new Serializer();
            //serializer.BaudRate = 19200;
            //serializer.CommunicationStarted += new SerializerEventHandler(serializer_CommunicationStarted);

            //servo1 = new ServoMotorController(serializer);
            //servo1.ServoMotorId = ServoMotorId.ServoMotor1;
            //servo2 = new ServoMotorController(serializer);
            //servo2.ServoMotorId = ServoMotorId.ServoMotor2;
            //servo3 = new ServoMotorController(serializer);
            //servo3.ServoMotorId = ServoMotorId.ServoMotor3;
            //servo4 = new ServoMotorController(serializer);
            //servo4.ServoMotorId = ServoMotorId.ServoMotor4;
            //servo5 = new ServoMotorController(serializer);
            //servo5.ServoMotorId = ServoMotorId.ServoMotor5;
            //servo6 = new ServoMotorController(serializer);
            //servo6.ServoMotorId = ServoMotorId.ServoMotor6;

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true; 

            //batteryVoltage = new AnalogSensor(serializer);
            //batteryVoltage.Pin = AnalogPinId.Pin5;
            //batteryVoltage.ValueChangedThreshold = 1;
            //batteryVoltage.UpdateFrequency = 1000;
            //batteryVoltage.ValueChanged += new SerializerComponentEventHandler(batteryVoltage_ValueChanged);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (ServoControlConnected)
                {
              //      serializer.PumpEvents();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorRoutine(ex);
            }
        } 


        //Set the Servos to the desired positions
        public void SetServoPosition(int ServoNum, int Position)
        {
            //if (ServoNum == 1) { servo1.Position = Position; }
            //if (ServoNum == 2) { servo2.Position = Position; }
            //if (ServoNum == 3) { servo3.Position = Position; }
            //if (ServoNum == 4) { servo4.Position = Position; }
            //if (ServoNum == 5) { servo5.Position = Position; }
            //if (ServoNum == 6) { servo6.Position = Position; }
        }

        // CommunicationStarted Event Handler
        //void serializer_CommunicationStarted(Serializer sender)
        //{
            //ServoControlConnected = true;

            //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("serializerFirmware", "Serializer Firmware Version: " + serializer.GetFirmwareVersion()));
            
            //Assembly a = Assembly.GetAssembly(typeof(RoboticsConnection.Serializer.Serializer));
            //string AssemblyString = a.GetTypes()[0].Assembly.FullName;
            //string version = AssemblyString.Substring(AssemblyString.IndexOf("Version=") + 8, 7);

            //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("serializerDotNet", ".NET Library Version: " + version));
            
            //servo1.Position = 0;
            //servo2.Position = 0;
            //servo3.Position = 0;
            //servo4.Position = 0;
            //servo5.Position = 0;
            //servo6.Position = 0;

        //}

        //void batteryVoltage_ValueChanged(SerializerComponent sender)
        //{
            // Calculate actual battery voltage: 
            //double voltage = batteryVoltage.Value * 0.01459;
            //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("serializerVoltage", String.Format("{0:0.0} volts", voltage)));
        //}

        public void OpenPort(String PortName)
        {
            try
            {
                if (ServoControlConnected == false)
                {
                    //Connect
                    //serializer.PortName = PortName;
                    //serializer.StartCommunication();
                    //ServoControlConnected = true;
                    //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("button3", "Disconnect"));
                    //GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboServos", "false"));
                }
                else
                {
                    //Close in another thread
                    Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                    CloseDown.Start();
                    
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("button3", "Connect"));
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("cboServos", "true"));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorRoutine(ex);
            }
        }


        private void CloseSerialOnExit()
        {
            try
            {
                //close the serial port
                //serializer.ShutDown();
                //ServoControlConnected = false;
            }
            catch
            {
            }
        }
    }

}
