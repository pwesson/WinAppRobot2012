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

namespace RoboticBoat
{
    static class Navigation
    {
        public static double Distance(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            //==================================
            // Distance
            //==================================

            double R = 6371 * 1000;  //meters

            double dLat = DegreeToRadian(Lat2 - Lat1);
            double dLon = DegreeToRadian(Lon2 - Lon1);
            Lat1 = DegreeToRadian(Lat1);
            Lat2 = DegreeToRadian(Lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(Lat1) * Math.Cos(Lat2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        public static double Bearing(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            //==================================
            // Bearing
            //==================================

            double dLat = DegreeToRadian(Lat2 - Lat1);
            double dLon = DegreeToRadian(Lon2 - Lon1);
            Lat1 = DegreeToRadian(Lat1);
            Lat2 = DegreeToRadian(Lat2);
            Lon1 = DegreeToRadian(Lon1);
            Lon2 = DegreeToRadian(Lon2);

            double y = Math.Sin(dLon) * Math.Cos(Lat2);

            double x = Math.Cos(Lat1) * Math.Sin(Lat2) -
                        Math.Sin(Lat1) * Math.Cos(Lat2) * Math.Cos(dLon);

            double z = RadianToDegree(Math.Atan2(y, x));

            return (z + 360) % 360;
        }

        public static double AngleDifference(double head, double bear)
        {
            double r1 = head - bear;
            double r2 = bear - head;

            if (r1 < 0) { r1 += 360; }
            if (r2 < 0) { r2 += 360; }

            if (r1 < r2)
            {
                return r1;
            }
            else
            {
                return -r2;
            }
        }

        //public static int RudderPosition(double degree)
        //{
            // Negative sign as the rudder moves in the opposite direction
            // to the change of boat direction.

        //    int rudder = -Convert.ToInt32(degree);

        //    if (rudder > 30) { rudder = 50; }
        //    if (rudder < -30) { rudder = -50; }

        //    return rudder;
        //}

        //========== Private below ==============================

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
