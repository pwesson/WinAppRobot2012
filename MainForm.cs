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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

using Util.EventMessages;

namespace RoboticBoat
{
    public partial class MainForm : Form
    {
        //Initialise the different Control Objects

        GPS objGPS = new GPS();
        Compass objCompass = new Compass();
        Servos objServos = new Servos();
        Weather objWeather = new Weather();
        SonicSensor objSonicSonar = new SonicSensor();
        Battery objBattery = new Battery();
        Cpu objCpu = new Cpu();

        private String ProcessLine; 
        private StreamWriter myFile = new StreamWriter("./data/" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "-ALL.csv", true);
        
        public MainForm()
        {
            InitializeComponent();

            //Set the SerialPorts List
            foreach (string port in SerialPort.GetPortNames())
            {
                cboCompass.Items.Add(port);
                cboCompass.Text = port;

                cboGPS.Items.Add(port);
                cboGPS.Text = port;

                cboServos.Items.Add(port);
                cboServos.Text = port;

                cboWeather.Items.Add(port);
                cboWeather.Text = port;

                cboSonicSensor.Items.Add(port);
                cboSonicSensor.Text = port;
            }

            GlobalEventMessages obj1 = new GlobalEventMessages();
            GlobalEventMessages.TheEvent += new GlobalEventHandler(ShowOnScreen);

            toolStripStatusLabel1.Text = Environment.MachineName;
            toolStripStatusLabel2.Text = Environment.UserName;
            toolStripStatusLabel3.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        public void ShowOnScreen(object o, GlobalEventArgs e)
        {
            try
            {
                if (e.sCont == "btnGPS")     { btnGPS.Invoke(new MethodInvoker(delegate { btnGPS.Text = e.sMsg; })); }
                if (e.sCont == "btnCompass") { btnCompass.Invoke(new MethodInvoker(delegate { btnCompass.Text = e.sMsg; })); }
                if (e.sCont == "button3")    { button3.Invoke(new MethodInvoker(delegate { button3.Text = e.sMsg; })); }
                if (e.sCont == "btnWeather") { btnWeather.Invoke(new MethodInvoker(delegate { btnWeather.Text = e.sMsg; })); }

                if (e.sCont == "textBox1") { textBox1.Invoke(new MethodInvoker(delegate { textBox1.Text = e.sMsg; })); }
                if (e.sCont == "textBox2") { textBox2.Invoke(new MethodInvoker(delegate { textBox2.Text = e.sMsg; })); }
                if (e.sCont == "textBox3") { textBox3.Invoke(new MethodInvoker(delegate { textBox3.Text = e.sMsg; })); }

                if (e.sCont == "serializerFirmware") { serializerFirmware.Invoke(new MethodInvoker(delegate { serializerFirmware.Text = e.sMsg; })); }
                if (e.sCont == "serializerDotNet")   { serializerDotNet.Invoke(new MethodInvoker(delegate { serializerDotNet.Text = e.sMsg; })); }
                if (e.sCont == "serializerVoltage")  { serializerVoltage.Invoke(new MethodInvoker(delegate { serializerVoltage.Text = e.sMsg; })); }
                
                // GPS
                if (e.sCont == "lblNS")        { lblNS.Invoke(new MethodInvoker(delegate { lblNS.Text = e.sMsg; })); }
                if (e.sCont == "lblEW")        { lblEW.Invoke(new MethodInvoker(delegate { lblEW.Text = e.sMsg; })); }
                if (e.sCont == "lblLatitude")  { lblLatitude.Invoke(new MethodInvoker(delegate { lblLatitude.Text = e.sMsg; })); }
                if (e.sCont == "lblLongitute") { lblLongitute.Invoke(new MethodInvoker(delegate { lblLongitute.Text = e.sMsg; })); }
                if (e.sCont == "lblSatilites") { lblSatilites.Invoke(new MethodInvoker(delegate { lblSatilites.Text = e.sMsg; })); }
                if (e.sCont == "lblFix")       { lblFix.Invoke(new MethodInvoker(delegate { lblFix.Text = e.sMsg; })); }

                // Electronic Compass
                if (e.sCont == "lblDHeading")  { lblDHeading.Invoke(new MethodInvoker(delegate { lblDHeading.Text = e.sMsg; })); }
                if (e.sCont == "lblDPitch")    { lblDPitch.Invoke(new MethodInvoker(delegate { lblDPitch.Text = e.sMsg; })); }
                if (e.sCont == "lblDRoll")     { lblDRoll.Invoke(new MethodInvoker(delegate { lblDRoll.Text = e.sMsg; })); }

                //Battery
                if (e.sCont == "lblBatteryStatus") { lblBatteryStatus.Invoke(new MethodInvoker(delegate { lblBatteryStatus.Text = e.sMsg; }));}
                if (e.sCont == "lblBatteryPower") { lblBatteryPower.Invoke(new MethodInvoker(delegate { lblBatteryPower.Text = e.sMsg; })); }
                if (e.sCont == "lblBatteryLifePercent") { lblBatteryLifePercent.Invoke(new MethodInvoker(delegate { lblBatteryLifePercent.Text = e.sMsg; })); }
                if (e.sCont == "lblBatteryLifeRemaining") { lblBatteryLifeRemaining.Invoke(new MethodInvoker(delegate { lblBatteryLifeRemaining.Text = e.sMsg; })); }
                if (e.sCont == "lblCPU") { lblCPU.Invoke(new MethodInvoker(delegate { lblCPU.Text = e.sMsg; })); }
                if (e.sCont == "lblRAM") { lblRAM.Invoke(new MethodInvoker(delegate { lblRAM.Text = e.sMsg; })); }

                // Weather Station
                if (e.sCont == "lblWindDirection") { lblWindDirection.Invoke(new MethodInvoker(delegate { lblWindDirection.Text = e.sMsg; })); }
                if (e.sCont == "lblWindSpeed") { lblWindSpeed.Invoke(new MethodInvoker(delegate { lblWindSpeed.Text = e.sMsg; })); }
                if (e.sCont == "lblTemperature") { lblTemperature.Invoke(new MethodInvoker(delegate { lblTemperature.Text = e.sMsg; })); }
                if (e.sCont == "lblRelativeLight") { lblRelativeLight.Invoke(new MethodInvoker(delegate { lblRelativeLight.Text = e.sMsg; })); }
                if (e.sCont == "lblPressure") { lblPressure.Invoke(new MethodInvoker(delegate { lblPressure.Text = e.sMsg; })); }
                if (e.sCont == "lblHumidity") { lblHumidity.Invoke(new MethodInvoker(delegate { lblHumidity.Text = e.sMsg; })); }
                if (e.sCont == "lblDewpoint") { lblDewpoint.Invoke(new MethodInvoker(delegate { lblDewpoint.Text = e.sMsg; })); }
                if (e.sCont == "lblRainfall") { lblRainfall.Invoke(new MethodInvoker(delegate { lblRainfall.Text = e.sMsg; })); }
                if (e.sCont == "lblWeatherBattery") { lblWeatherBattery.Invoke(new MethodInvoker(delegate { lblWeatherBattery.Text = e.sMsg; })); }

                // Enable / Disable
                if (e.sCont == "cboGPS") { cboGPS.Invoke(new MethodInvoker(delegate { cboGPS.Enabled = Convert.ToBoolean(e.sMsg); })); }
                if (e.sCont == "cboCompass") { cboCompass.Invoke(new MethodInvoker(delegate { cboCompass.Enabled = Convert.ToBoolean(e.sMsg); })); }
                if (e.sCont == "cboServos") { cboServos.Invoke(new MethodInvoker(delegate { cboServos.Enabled = Convert.ToBoolean(e.sMsg); })); }
                if (e.sCont == "cboWeather") { cboWeather.Invoke(new MethodInvoker(delegate { cboWeather.Enabled = Convert.ToBoolean(e.sMsg); })); }

                // Update Servo Controls
                if (e.sCont == "comboBoxRudder") { comboBoxRudder.Invoke(new MethodInvoker(delegate { comboBoxRudder.Text = e.sMsg; })); }
                if (e.sCont == "comboBoxWinch") { comboBoxWinch.Invoke(new MethodInvoker(delegate { comboBoxWinch.Text = e.sMsg; })); }
                if (e.sCont == "comboBoxMotor") { comboBoxMotor.Invoke(new MethodInvoker(delegate { comboBoxMotor.Text = e.sMsg; })); }

                // Mission Update
                if (e.sCont == "lblWayPoint") { lblWayPoint.Invoke(new MethodInvoker(delegate { lblWayPoint.Text = e.sMsg; })); }
                if (e.sCont == "lblBearing") { lblBearing.Invoke(new MethodInvoker(delegate { lblBearing.Text = e.sMsg; })); }
                if (e.sCont == "lblDistance") { lblDistance.Invoke(new MethodInvoker(delegate { lblDistance.Text = e.sMsg; })); }
                if (e.sCont == "lblChange") { lblChange.Invoke(new MethodInvoker(delegate { lblChange.Text = e.sMsg; })); }
                if (e.sCont == "lblWayPointLat") { lblWayPointLat.Invoke(new MethodInvoker(delegate { lblWayPointLat.Text = e.sMsg; })); }
                if (e.sCont == "lblWayPointLon") { lblWayPointLon.Invoke(new MethodInvoker(delegate { lblWayPointLon.Text = e.sMsg; })); }
                if (e.sCont == "lblAligned") { lblAligned.Invoke(new MethodInvoker(delegate { lblAligned.Text = e.sMsg; })); }
                
                if (e.sCont == "lblSonicSensor") { lblSonicSensor.Invoke(new MethodInvoker(delegate { lblSonicSensor.Text = e.sMsg; })); }


                //lblSonicSensor

                // StatusBar
                if (e.sCont == "toolStripStatusLabel4") { toolStripStatusLabel4.Text = e.sMsg; };

                Application.DoEvents();
            }
            catch
            {
            }
        }

        //==========================================================================

        private void button1_Click(object sender, EventArgs e)
        {
            objGPS.OpenPort(cboGPS.Text);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            objCompass.OpenPort(cboCompass.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            objServos.OpenPort(cboServos.Text);
        }

        private void btnWeather_Click(object sender, EventArgs e)
        {
            objWeather.OpenPort(cboWeather.Text);
        }


        //==========================================================================

        private void comboBoxMotor_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Motor has been changed

                ComboBox comboBox = (ComboBox)sender;
                int pos = Convert.ToInt32(comboBox.Text);
                objServos.SetServoPosition(4, pos);
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorRoutine(ex);
            }

        }
        
        private void comboBoxRudder_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Rudder has been changed
                
                ComboBox comboBox = (ComboBox)sender;
                int pos = Convert.ToInt32(comboBox.Text);
                objServos.SetServoPosition(5, pos);
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorRoutine(ex);
            }
        }

        private void comboBoxWinch_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Winch has been changed

                ComboBox comboBox = (ComboBox)sender;
                int pos = Convert.ToInt32(comboBox.Text);
                objServos.SetServoPosition(6, pos);
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorRoutine(ex);
            }
        }

        //==========================================================================

        private void button6_Click(object sender, EventArgs e)
        {
            Chart3 obj = new Chart3();
            obj.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            objBattery.StatusUpdate();

            objCpu.Update();

            toolStripStatusLabel4.Text = DateTime.Now.ToString("HH:mm:ss");

            ProcessLine = DateTime.Now.ToString("yyyy-MM-dd");
            ProcessLine += "," + DateTime.Now.ToString("HH:mm:ss");

            ProcessLine += "," + "GPS->";
            ProcessLine += "," + lblLatitude.Text;
            ProcessLine += "," + lblLongitute.Text;
            ProcessLine += "," + lblSatilites.Text;

            ProcessLine += "," + "Compass->";
            ProcessLine += "," + lblDHeading.Text;
            ProcessLine += "," + lblDRoll.Text;
            ProcessLine += "," + lblDPitch.Text;
            
            ProcessLine += "," + "Weather->";
            ProcessLine += "," + lblWindDirection.Text;
            ProcessLine += "," + lblWindSpeed.Text;
            ProcessLine += "," + lblPressure.Text;
            ProcessLine += "," + lblTemperature.Text;
            ProcessLine += "," + lblRelativeLight.Text;
            ProcessLine += "," + lblHumidity.Text;
            ProcessLine += "," + lblDewpoint.Text;
            ProcessLine += "," + lblRainfall.Text;
            ProcessLine += "," + lblWeatherBattery.Text;
            
            ProcessLine += "," + "Mission->";
            ProcessLine += "," + lblWayPoint.Text;
            ProcessLine += "," + lblBearing.Text;
            ProcessLine += "," + lblDistance.Text;
            ProcessLine += "," + lblChange.Text;
            ProcessLine += "," + lblWayPointLat.Text;
            ProcessLine += "," + lblWayPointLon.Text;
            ProcessLine += "," + lblAligned.Text;
            
            ProcessLine += "," + "Battery->";
            ProcessLine += ",\"" + lblBatteryStatus.Text;
            ProcessLine += "\"," + lblBatteryPower.Text;
            ProcessLine += "," + lblBatteryLifeRemaining.Text;
            ProcessLine += "," + lblBatteryLifePercent.Text;
            ProcessLine += "," + lblCPU.Text;
            ProcessLine += "," + lblRAM.Text;
            
            ProcessLine += "," + "Servos->";
            ProcessLine += "," + serializerVoltage.Text;
            ProcessLine += "," + comboBoxRudder.Text;
            ProcessLine += "," + comboBoxWinch.Text;
            ProcessLine += "," + comboBoxMotor.Text;
            ProcessLine += Environment.NewLine;

            myFile.Write(ProcessLine);
            myFile.Flush();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Speech.Say("Do you want to close the program");
            
            if (MessageBox.Show("Do you want to close?", "Robotic Boat", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                // Cancel the Closing event from closing the form.

                e.Cancel = true;
            }

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close the application
            MainForm.ActiveForm.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //Refresh
            cboCompass.Items.Clear();
            cboGPS.Items.Clear();
            cboServos.Items.Clear();
            cboWeather.Items.Clear();

            //Set the SerialPorts List
            foreach (string port in SerialPort.GetPortNames())
            {
                cboCompass.Items.Add(port);
                cboCompass.Text = port;

                cboGPS.Items.Add(port);
                cboGPS.Text = port;

                cboServos.Items.Add(port);
                cboServos.Text = port;

                cboWeather.Items.Add(port);
                cboWeather.Text = port;
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // Set up the COMS
            try
            {
                if (Environment.MachineName == "MT-PC")
                {
                    cboGPS.Text = "COM09";
                    objGPS.OpenPort(cboGPS.Text);
                    cboCompass.Text = "COM23";
                    objCompass.OpenPort(cboCompass.Text);
                    cboWeather.Text = "COM19";
                    objWeather.OpenPort(cboWeather.Text);
                    cboServos.Text = "COM14";
                    objServos.OpenPort(cboServos.Text);
                }

                if (Environment.MachineName == "VPC-F11C5E")
                {
                    cboGPS.Text = "COM20";
                    objGPS.OpenPort(cboGPS.Text);
                    cboCompass.Text = "COM27";
                    objCompass.OpenPort(cboCompass.Text);
                    cboWeather.Text = "COM24";
                    objWeather.OpenPort(cboWeather.Text);
                    cboServos.Text = "COM28";
                    objServos.OpenPort(cboServos.Text);
                    //cboSonicSensor.Text = "COM19";
                    //objSonicSonar.OpenPort(cboSonicSensor.Text);
                }

                if (Environment.MachineName == "SONY-VPCF22C5E")
                {
                    //cboGPS.Text = "COM16";
                    //objGPS.OpenPort(cboGPS.Text);
                    //cboCompass.Text = "COM9";
                    //objCompass.OpenPort(cboCompass.Text);
                    cboServos.Text = "COM17";
                    objServos.OpenPort(cboServos.Text);
                }
            }
            catch
            {
            }

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //Calibrate the motor. Just after the ESC has been pressed

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "50"));
            MessageBox.Show("Max speed forward");

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "-50"));
            MessageBox.Show("Max speed backwards");

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "0"));           
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //Test the system

            System.Threading.Thread.Sleep(2000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", "50"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", "-50"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", "0"));
            System.Threading.Thread.Sleep(1000);

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxWinch", "20"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxWinch", "-20"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxWinch", "0"));

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "20"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "0"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "-20"));
            System.Threading.Thread.Sleep(1000);
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "0"));
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            // Stop the Motor
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "0"));           
        }

        private void btnSonicSonar_Click(object sender, EventArgs e)
        {
            objSonicSonar.OpenPort(cboSonicSensor.Text);
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


    }
}
