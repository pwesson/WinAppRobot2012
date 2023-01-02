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

using System.Diagnostics;
using Util.EventMessages;

namespace RoboticBoat
{
    class Cpu
    {
        private string machineName = System.Environment.MachineName;

        private PerformanceCounter cpuCounter = null;
        private PerformanceCounter ramCounter = null;

        public Cpu()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", machineName);
        }

        public String StatusCPU()
        {
            return String.Format("{0:##0}%", cpuCounter.NextValue());    
        }

        public String StatusRAM()
        {
            ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", String.Empty, machineName);
            return String.Format("{0:##0}%", ramCounter.NextValue());
        }

        public void Update()
        {
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblCPU", StatusCPU()));
            GlobalEventMessages.OnGlobalEvent(new GlobalEventArgs("lblRAM", StatusRAM()));
        }

    }
}
