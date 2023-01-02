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

using System.Windows.Forms;
using System.IO;
using Util.EventMessages;
using System.Diagnostics;

namespace RoboticBoat
{
    class Battery
    {
        //private StreamWriter myFile = new StreamWriter("./data/BAT-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".txt", true);
        
        public void StatusUpdate()
        {
            //String Line = "";
            String sMsg = "";

            sMsg = BatteryChargeStatus();
            //Line = DateTime.Now.ToString("yyyyMMdd-HHmm") + "," + sMsg;
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblBatteryStatus", sMsg));

            sMsg = PowerStatus();
            //Line += "," + sMsg;
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblBatteryPower", sMsg));

            sMsg = BatteryLifePercent().ToString();
            //Line += "," + sMsg;
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblBatteryLifePercent", sMsg));

            sMsg = BatteryLifeRemaining().ToString();
            //Line += "," + sMsg + Environment.NewLine;
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblBatteryLifeRemaining", sMsg));

            //myFile.Write(Line);
            //myFile.Flush();
        }

        public String BatteryChargeStatus()
        {
            return SystemInformation.PowerStatus.BatteryChargeStatus.ToString();
        }

        public String PowerStatus()
        {
            switch (SystemInformation.PowerStatus.PowerLineStatus)
            {
                case System.Windows.Forms.PowerLineStatus.Offline: return "Offline";
                case System.Windows.Forms.PowerLineStatus.Online: return "Online";
                case System.Windows.Forms.PowerLineStatus.Unknown: return "Unknown";
            }
            return "";
        }
        
        public int BatteryFullLifetime()
        {
            return SystemInformation.PowerStatus.BatteryFullLifetime;
        }

        public int BatteryLifeRemaining()
        {
            return SystemInformation.PowerStatus.BatteryLifeRemaining / 60;
        }
        
        public float BatteryLifePercent()
        {
            return SystemInformation.PowerStatus.BatteryLifePercent * 100;
        }

        public String Voltage()
        {
            return "";
        }

    }


}
