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
using System.Reflection;
using System.Timers;

using Util.EventMessages;

namespace RoboticBoat
{    
    class Rudder
    {
        private int RudderPosition = 0;

        private double old_heading = 0;
        private double old_bearing = 0;
        
        private double change_heading = 0;
        private double change_bearing = 0;

        private double heading360 = 0;
        
        private StreamWriter myFile = new StreamWriter("./data/" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "-RUD.txt", true);
        
        public Rudder()
        {

        }

        public int NewRudderPosition(double heading, double bearing, int Change)
        {
            try
            {
                change_heading = Navigation.AngleDifference(heading, old_heading);
                change_bearing = Navigation.AngleDifference(bearing, old_bearing);

                heading360 = heading360 + change_heading;

                myFile.Write(DateTime.Now.ToString("HH:mm:ss") + ", " + RudderPosition + ", " + heading.ToString() + "," + change_heading.ToString() + "," + heading360.ToString() + Environment.NewLine);
                myFile.Flush();

                RudderPosition = Change;

                //Store the current Heading & Bearing
                old_heading = heading;
                old_bearing = bearing;

                //Constraint rudder angle
                if (RudderPosition >= 50) { RudderPosition = 50; }
                if (RudderPosition <= -50) { RudderPosition = -50; }

                return RudderPosition;
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorRoutine(ex);
                return 0;
            }
        }
    }


}
