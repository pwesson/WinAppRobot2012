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
using System.Diagnostics;   //For EventLog

namespace Util.EventMessages
{
	public delegate void GlobalEventHandler(object o, GlobalEventArgs e);

	public class GlobalEventArgs : EventArgs
	{
		public string sMsg;
        public string sCont;

		public GlobalEventArgs( string sControl, string sStr )
		{
			sCont = sControl;
			sMsg = sStr;
		}
	}

	public class GlobalEventMessages
	{
		public static event GlobalEventHandler TheEvent;

        public GlobalEventMessages(){}

		public static void OnGlobalEvent( GlobalEventArgs e)
		{
			if (TheEvent != null)
			{
				TheEvent(new object(), e);
			}
		}
	}

}
