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

using System.IO;
using Util.EventMessages;

namespace RoboticBoat
{
    public partial class Chart3 : Form
    {
        bool OnMission = false;
        bool OnMotor = false;
        bool OnRudderTest = false;
        int iMission = 0; 
        
        double x0 = 0.148315;
        double y0 = 51.285286;
        double x1 = 0.163872;
        double y1 = 51.290936;

        Point a = new Point();
        Point b = new Point();
        Point c = new Point();
        Point d = new Point();

        Rudder objRudder = new Rudder();

        List<double> pLatitude = new List<double>();
        List<double> pLongitude = new List<double>();
        List<Point> pPoints = new List<Point>();

        public Chart3()
        {
            InitializeComponent();

            GlobalEventMessages obj1 = new GlobalEventMessages();
            GlobalEventMessages.TheEvent += new GlobalEventHandler(ShowOnScreen);
        }

        public void ShowOnScreen(object o, GlobalEventArgs e)
        {
            try
            {
                if (e.sCont == "lblLatitude") { label1.Invoke(new MethodInvoker(delegate { label1.Text = e.sMsg; })); }
                if (e.sCont == "lblLongitute") { label2.Invoke(new MethodInvoker(delegate { label2.Text = e.sMsg; })); }
                if (e.sCont == "lblDHeading") { label4.Invoke(new MethodInvoker(delegate { label4.Text = e.sMsg; })); }

                Application.DoEvents();
            }
            catch
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //Add in the current GPS location if starting

            Point c = System.Windows.Forms.Cursor.Position;
            Point c1 = new Point();

            double y = Convert.ToDouble(label1.Text);
            double x = Convert.ToDouble(label2.Text);

            if (pPoints.Count == 0)
            {

                if (label3.Text == "On chart")
                {
                    pLatitude.Add(y);
                    pLongitude.Add(x);
                    
                    int plotX = Convert.ToInt32((x - x0) / (x1 - x0) * 770);
                    int plotY = Convert.ToInt32(438 - (y - y0) / (y1 - y0) * 438);
                    c1.X = Convert.ToInt32(plotX);
                    c1.Y = Convert.ToInt32(plotY);
                    pPoints.Add(c1);

                    textBox1.Text = "START" + Environment.NewLine;
                    textBox1.Text = textBox1.Text + "[" + y.ToString("0.00000") + ", " + x.ToString("0.00000") + "]" + Environment.NewLine;

                }
            }

            //Want to work out the mouse coordinates

            double bear = 0;
            double dist = 0;
            double head = 0;
            double change = 0;

            //Point c = System.Windows.Forms.Cursor.Position;
            //Point c1 = new Point();

            x = c.X - this.Left - 8;
            y = c.Y - this.Top - 58;
            int count = 0;

            c1.X = Convert.ToInt32(x);
            c1.Y = Convert.ToInt32(y);

            x = x0 + (x1 - x0) * x / 770;
            y = y0 + (y1 - y0) * (438 - y) / 438;

            pLatitude.Add(y);
            pLongitude.Add(x);
            pPoints.Add(c1);

            //Work out the distance from last pt

            count = pLatitude.Count;
            if (count > 1)
            {
                bear = Navigation.Bearing(pLatitude[count - 2], pLongitude[count - 2], pLatitude[count - 1], pLongitude[count - 1]);
                dist = Navigation.Distance(pLatitude[count - 2], pLongitude[count - 2], pLatitude[count - 1], pLongitude[count - 1]);
                
                head = Convert.ToDouble(label4.Text);
                change = Navigation.AngleDifference(head, bear);

                textBox1.Text += "Bearing = " + bear.ToString("0");
                textBox1.Text += ", Distance = " + dist.ToString("0.00");

                String SayMessage = "Bearing " + bear.ToString("0") + " degrees to:";
            }

            textBox1.Text = textBox1.Text + Environment.NewLine;
            textBox1.Text = textBox1.Text + "[" + y.ToString("0.00000") + ", " + x.ToString("0.00000") + "]" + Environment.NewLine;

            pictureBox1.Refresh();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int count = pPoints.Count;

            Graphics G = Graphics.FromImage(pictureBox1.Image);
            var p = new Pen(Color.LightCyan, 2);

            for (int i = 1; i < count; i++)
            {
                G.DrawLine(p, pPoints[i - 1], pPoints[i]);
            }

            if (label3.Text == "On chart")
            {
                double y = Convert.ToDouble(label1.Text);
                double x = Convert.ToDouble(label2.Text);

                int plotX = Convert.ToInt32((x - x0) / (x1 - x0) * 770);
                int plotY = Convert.ToInt32( 438 - (y - y0) / (y1 - y0) * 438);

                a.X = plotX - 4;
                a.Y = plotY + 4;
                b.X = plotX + 4;
                b.Y = plotY + 4;
                c.X = plotX + 4;
                c.Y = plotY - 4;
                d.X = plotX - 4;
                d.Y = plotY - 4;

                G.DrawLine(p, a, c);
                G.DrawLine(p, b, d);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //Call this routine 4 times per second.

            Point c = System.Windows.Forms.Cursor.Position;

            toolStripStatusLabel1.Text = (c.X - this.Left - 8).ToString();
            toolStripStatusLabel2.Text = (c.Y - this.Top - 58).ToString();

            //Draw the crosshairs if GPS location is on the map

            double y = Convert.ToDouble(label1.Text);
            double x = Convert.ToDouble(label2.Text);

            if (x >= x0 && x <= x1 && y >= y0 && y <= y1)
            {
                label3.Text = "On chart";
                pictureBox1.Refresh();
                pictureBox1.Invalidate();
            }
            else
            {
                label3.Text = "Off chart";
            }

            //===========================================================

            if (OnRudderTest)
            {
                double Lat1 = Convert.ToDouble(label1.Text);
                double Lon1 = Convert.ToDouble(label2.Text);
             
                double heading = Convert.ToDouble(label4.Text);
                double bearing = Convert.ToDouble(textBoxBearing.Text);

                int change = Convert.ToInt32(Navigation.AngleDifference(heading, bearing));

                if (OnMotor == false)
                {
                    if (Math.Abs(change) <= 10)
                    {
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", textBoxMotor.Text));
                        OnMotor = true;
                    }
                }

                String sRudder = objRudder.NewRudderPosition(heading, bearing, Convert.ToInt32(textBoxRudder.Text)).ToString();

                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", sRudder));

            }

            //===========================================================

            if (OnMission)
            {
                //Current waypoint is number i

                double Lat1 = Convert.ToDouble(label1.Text);
                double Lon1 = Convert.ToDouble(label2.Text);
                double Lat2 = pLatitude[iMission];
                double Lon2 = pLongitude[iMission];

                double heading = Convert.ToDouble(label4.Text);
                double bearing = Navigation.Bearing(Lat1, Lon1, Lat2, Lon2);

                int change = Convert.ToInt32(Navigation.AngleDifference(heading, bearing));

                if (OnMotor == false)
                {
                    //Should we turn on the Motor?
                    if (Math.Abs(change) <= 10)
                    {
                        GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxMotor", "40"));
                        OnMotor = true;
                    }
                }

                //Given the required change - update the Rudder
                
                String sRudder = objRudder.NewRudderPosition(heading, bearing, change).ToString();

                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", sRudder));

                //Have we reached the way point?

                double distance = Navigation.Distance(Lat1, Lon1, Lat2, Lon2);

                if (distance < 3) // Within two meters
                {
                    iMission = iMission + 1;
                    Speech.Say("Next waypoint");
                    GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblAligned", "false"));
                }

                //Has the Mission finished?

                if (iMission == pLatitude.Count)
                {
                    EndofMission();
                }

                //Update the User

                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWayPoint", iMission.ToString()));
                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblBearing", bearing.ToString("0")));
                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblDistance", distance.ToString("0")));
                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblChange", change.ToString("0")));
                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWayPointLat", Lat2.ToString("0.00000")));
                GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWayPointLon", Lon2.ToString("0.00000")));
            }
        }

        private void EndofMission()
        {
            OnMission = false;
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", "0"));
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
            iMission = 0;
            OnMission = false;
            pLatitude.Clear();
            pLongitude.Clear();
            pPoints.Clear();
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblWayPoint", "0"));
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblBearing", "0"));
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblDistance", "0"));
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblChange", "0"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pLatitude.Add(Convert.ToDouble(label1.Text));
            pLongitude.Add(Convert.ToDouble(label2.Text));
            iMission = 0;
            OnMission = true;
        }

        private void chipsteadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnMission = false;
            x0 = 0.148315;
            y0 = 51.285286;
            x1 = 0.163872;
            y1 = 51.290936;
            textBox1.Text = "";
            pLatitude.Clear();
            pLongitude.Clear();
            pPoints.Clear();
            pictureBox1.Image = new Bitmap("../../Images/Chipstead2.bmp");
            pictureBox1.Refresh();
            pictureBox1.Invalidate();
        }

        private void chipsteadShoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnMission = false;
            x0 = 0.152639;
            y0 = 51.287012;
            x1 = 0.154675;
            y1 = 51.287755;
            textBox1.Text = "";
            pLatitude.Clear();
            pLongitude.Clear();
            pPoints.Clear();
            pictureBox1.Image = new Bitmap("../../Images/ChipsteadShore.bmp");
            pictureBox1.Refresh();
            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Rudder Test GO button

            OnRudderTest = true;

            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("comboBoxRudder", textBoxRudder.Text));

        }


    }
}
