﻿using System;
using System.Windows.Forms;

namespace RTC
{
	public static class RTC_FreezeEngine
	{
		//The freeze engine is very similar to the Hellgenie and shares common functions with it. See RTC_HellgenieEngine.cs for cheat-related methods.

		public static BlastUnit GenerateUnit(string domain, long address, int precision)
		{
			try
			{
				if (domain == null)
					return null;
				MemoryDomainProxy mdp = RTC_MemoryDomains.GetProxy(domain, address);

				long safeAddress = address - (address % precision);

				return new BlastUnit(BackupSource.PREEXECUTE, domain, safeAddress, precision, mdp.BigEndian, 0, -1);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Something went wrong in the RTC Freeze Engine. \n" +
					"This is an RTC error, so you should probably send this to the RTC devs.\n" +
					"If you know the steps to reproduce this error it would be greatly appreciated.\n\n" +
								ex.ToString());
				return null;
			}
		}
	}
}
