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
using System.Net;
using System.IO; 
using System.Xml;
using System.Diagnostics;

namespace RoboticBoat
{
	public class ErrorLog
	{
		protected static string strLogFilePath	= string.Empty;
		
		private static StreamWriter sw = null;
	
		public static string LogFilePath
		{
			set
			{
				strLogFilePath	= value;	
			}
			get
			{
				return strLogFilePath;
			}
		}
		
		public ErrorLog()
		{
		
		}

		public static bool ErrorRoutine(Exception objException)
		{
			try
			{					
				//Create the txt file if it does not exist
					
				if (File.Exists("./data/ErrorLog.txt") == false)
				{
					FileStream fs = new FileStream("./data/ErrorLog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
					fs.Close();
				}

				WriteErrorLog("./data/ErrorLog.txt", objException);
			
			    return true;
			}
			catch(Exception)
			{
				return false;
			}
		}


		private static bool WriteErrorLog(string strPathName,Exception  objException)
		{
			bool bReturn		= false;
			string strException	= string.Empty; 
			try
			{
                sw = new StreamWriter(strPathName,true);
				
                sw.WriteLine("Source		: " + objException.Source.ToString().Trim());
                sw.WriteLine("Method		: " + objException.TargetSite.Name.ToString());
                sw.WriteLine(" ");
                sw.WriteLine("Date		: " + DateTime.Now.ToShortDateString());
				sw.WriteLine("Time		: " + DateTime.Now.ToLongTimeString());
                sw.WriteLine(" ");
				sw.WriteLine("Error		: " +  objException.Message.ToString().Trim());
                sw.WriteLine(" ");
                sw.WriteLine("Stack Trace	: " + objException.StackTrace.ToString().Trim());  
				sw.WriteLine("----------------------------------------------------------------------------");

				sw.Flush();
				sw.Close();
				bReturn	= true;
			}
			catch(Exception)
			{
				bReturn	= false;
			}
			return bReturn;
		}
	}


}
