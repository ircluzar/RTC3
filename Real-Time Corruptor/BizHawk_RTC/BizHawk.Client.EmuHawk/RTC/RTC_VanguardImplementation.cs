﻿using RTCV.NetCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Nintendo.N64;
using NetCore;
using RTCV;
using RTCV.CorruptCore;
using static RTCV.NetCore.NetcoreCommands;

namespace RTC
{
	public static class VanguardImplementation
	{
		public static RTCV.Vanguard.VanguardConnector connector = null;


		public static void StartClient()
		{
			try { 
			ConsoleEx.WriteLine("Starting Vanguard Client");
			Thread.Sleep(500); //When starting in Multiple Startup Project, the first try will be uncessful since
			//the server takes a bit more time to start then the client.

			var spec = new NetCoreReceiver();
			spec.Attached = RTC_EmuCore.attached;
			spec.MessageReceived += OnMessageReceived;

			connector = new RTCV.Vanguard.VanguardConnector(spec);

			}
			catch (Exception ex)
			{
				if (RTC_EmuCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new RTCV.NetCore.AbortEverythingException();
			}
		}

		public static void RestartClient()
		{
			connector?.Kill();
			connector = null;
			StartClient();
		}

		private static void OnMessageReceived(object sender, NetCoreEventArgs e)
		{
			try
			{
				// This is where you implement interaction.
				// Warning: Any error thrown in here will be caught by NetCore and handled by being displayed in the console.

				var message = e.message;
				var simpleMessage = message as NetCoreSimpleMessage;
				var advancedMessage = message as NetCoreAdvancedMessage;

				ConsoleEx.WriteLine(message.Type);
				switch (message.Type) //Handle received messages here
				{
					case "INFINITELOOP":
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							while (true)
							{
								Thread.Sleep(10);
							}
						});
						break;


					case "KILLCORE":
						//GlobalWin.MainForm.Bad();
						GlobalWin.MainForm.CloseRom();
						break;

					case REMOTE_ALLSPECSSENT:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_EmuCore.LoadDefaultAndShowBizhawkForm();
						});
						break;
					/*
					case "POKEBYTE":
					{
						break;
					}
					case "PEEKBYTE":
					{
						RTC_Hooks.
					}*/
					case SAVESAVESTATE:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							e.setReturnValue(RTC_EmuCore.SaveSavestate_NET(advancedMessage.objectValue as string));
						});
						break;

					case LOADSAVESTATE:
						{
							var cmd = advancedMessage.objectValue as object[];
							var path = cmd[0] as string;
							var location = (StashKeySavestateLocation)cmd[1];
							SyncObjectSingleton.FormExecute((o, ea) =>
							{
								e.setReturnValue(RTC_EmuCore.LoadSavestate_NET(path, location));
							});
							break;
						}

					case REMOTE_LOADROM:
						{
							var fileName = advancedMessage.objectValue as String;
							SyncObjectSingleton.FormExecute((o, ea) =>
							{
								RTC_EmuCore.LoadRom_NET(fileName);
							});

						}
						break;

					case REMOTE_DOMAIN_GETDOMAINS:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							e.setReturnValue(RTC_Hooks.GetInterfaces());
						});
						break;

					case REMOTE_KEY_SETSYNCSETTINGS:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Hooks.BIZHAWK_GETSET_SYNCSETTINGS = (string)advancedMessage.objectValue;
						});
						break;

					case REMOTE_KEY_SETSYSTEMCORE:
						{
							var cmd = advancedMessage.objectValue as object[];
							var systemName = (string)cmd[0];
							var systemCore = (string)cmd[1];
							SyncObjectSingleton.FormExecute((o, ea) =>
							{
								RTC_Hooks.BIZHAWK_SET_SYSTEMCORE(systemName, systemCore);
							});
						}
						break;

					case BIZHAWK_OPEN_HEXEDITOR_ADDRESS:
						{
							var temp = advancedMessage.objectValue as object[];
							string domain = (string)temp[0];
							long address = (long)temp[1];

							MemoryDomainProxy mdp = RTC_MemoryDomains.GetProxy(domain, address);
							long realAddress = RTC_MemoryDomains.GetRealAddress(domain, address);

							SyncObjectSingleton.FormExecute((o, ea) =>
							{
								RTC_Hooks.BIZHAWK_OPEN_HEXEDITOR_ADDRESS(mdp, realAddress);
							});

							break;
						}
					case REMOTE_EVENT_BIZHAWK_MAINFORM_CLOSE:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Hooks.BIZHAWK_MAINFORM_CLOSE();
						});
						break;

					case REMOTE_EVENT_SAVEBIZHAWKCONFIG:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Hooks.BIZHAWK_MAINFORM_SAVECONFIG();
						});
						break;


					case REMOTE_RENDER_START:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Render.StartRender_NET();
						});
						break;

					case REMOTE_RENDER_STOP:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Render.StopRender_NET();
						});
						break;

					case REMOTE_IMPORTKEYBINDS:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Hooks.BIZHAWK_IMPORTCONFIGINI(RTC_Corruptcore.bizhawkDir + Path.DirectorySeparatorChar + "import_config.ini", RTC_Corruptcore.bizhawkDir + Path.DirectorySeparatorChar + "stockpile_config.ini");
						});
						break;

					case REMOTE_MERGECONFIG:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							RTC_Hooks.BIZHAWK_MERGECONFIGINI(RTC_Corruptcore.bizhawkDir + Path.DirectorySeparatorChar + "backup_config.ini", RTC_Corruptcore.bizhawkDir + Path.DirectorySeparatorChar + "stockpile_config.ini");
						});
						break;

					case REMOTE_RESTOREBIZHAWKCONFIG:
						SyncObjectSingleton.FormExecute((o, ea) =>
						{
							Process.Start(RTC_Corruptcore.bizhawkDir + Path.DirectorySeparatorChar + $"RestoreConfigDETACHED.bat");
						});
						break;

					case REMOTE_EVENT_BIZHAWKSTARTED:
						//if (RTC_StockpileManager.BackupedState == null)
						//S.GET<RTC_Core_Form>().AutoCorrupt = false;


						//Todo
						//RTC_NetcoreImplementation.SendCommandToBizhawk(new RTC_Command("REMOTE_PUSHVMDS) { objectValue = RTC_MemoryDomains.VmdPool.Values.Select(it => (it as VirtualMemoryDomain).Proto).ToArray() }, true, true);

						//Thread.Sleep(100);

						//		if (RTC_StockpileManager.BackupedState != null)
						//			S.GET<RTC_MemoryDomains_Form>().RefreshDomainsAndKeepSelected(RTC_StockpileManager.BackupedState.SelectedDomains.ToArray());

						//		if (S.GET<RTC_Core_Form>().cbUseGameProtection.Checked)
						//			RTC_GameProtection.Start();

						break;
					case REMOTE_ISNORMALADVANCE:
						e.setReturnValue(RTC_Hooks.isNormalAdvance);
						break;





				}
			}
			catch (Exception ex)
			{
				if (RTC_EmuCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new RTCV.NetCore.AbortEverythingException();
			}
		}


		public class BizhawkMemoryDomain : IMemoryDomain
		{
			public MemoryDomain MD;
			public string Name => MD.Name;
			private long size => MD.Size;

			public long Size
			{
				get
				{
					//Bizhawk always displays 8MB of ram even if only 4 are in use.
					if (Global.Emulator is N64 && !(Global.Emulator as N64).UsingExpansionSlot && Name == "RDRAM")
						return size / 2;
					return size;
				}
			}

			public int WordSize => MD.WordSize;
			public bool BigEndian => MD.EndianType == MemoryDomain.Endian.Big;

			public BizhawkMemoryDomain(MemoryDomain md)
			{
				MD = md;
			}

			public byte PeekByte(long addr)
			{
				return MD.PeekByte(addr);
			}

			public void PokeByte(long addr, byte val)
			{
				MD.PokeByte(addr, val);
			}

			public override string ToString()
			{
				return MD.Name;
			}

		}
	}
}