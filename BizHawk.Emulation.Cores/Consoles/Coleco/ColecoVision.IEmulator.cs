﻿using BizHawk.Emulation.Common;
using BizHawk.Common.NumberExtensions;
using System;

namespace BizHawk.Emulation.Cores.ColecoVision
{
	public partial class ColecoVision : IEmulator
	{
		public IEmulatorServiceProvider ServiceProvider { get; }

		public ControllerDefinition ControllerDefinition => ControllerDeck.Definition;

		public void FrameAdvance(IController controller, bool render, bool renderSound)
		{
			_controller = controller;

			// NOTE: Need to research differences between reset and power cycle
			if (_controller.IsPressed("Power"))
			{
				HardReset();
			}

			if (_controller.IsPressed("Reset"))
			{
				SoftReset();
			}

			_frame++;

			_isLag = true;
			if (_tracer.Enabled)
			{
				_cpu.TraceCallback = s => _tracer.Put(s);
			}
			else
			{
				_cpu.TraceCallback = null;
			}
			byte tempRet1 = ControllerDeck.ReadPort1(controller, true, false);
			byte tempRet2 = ControllerDeck.ReadPort2(controller, true, false);

			bool intPending = false;

			// the return values represent the controller's current state, but the sampling rate is not high enough
			// to catch all changes in wheel orientation
			// so we use the wheel variable and interpolate between frames

			// first determine how many degrees the wheels changed, and how many regions have been traversed
			float change1 = (float)(((ControllerDeck.wheel1 - ControllerDeck.temp_wheel1) % 180) / 1.25);
			float change2 = (float)(((ControllerDeck.wheel2 - ControllerDeck.temp_wheel2) % 180) / 1.25);

			// special cases
			if ((ControllerDeck.temp_wheel1 > 270) && (ControllerDeck.wheel1 < 90))
			{
				change1 = (float)((ControllerDeck.wheel1 + (360 - ControllerDeck.temp_wheel1)) / 1.25);
			}

			if ((ControllerDeck.wheel1 > 270) && (ControllerDeck.temp_wheel1 < 90))
			{
				change1 = -(float)((ControllerDeck.temp_wheel1 + (360 - ControllerDeck.wheel1)) / 1.25);
			}

			if ((ControllerDeck.temp_wheel2 > 270) && (ControllerDeck.wheel2 < 90))
			{
				change2 = (float)((ControllerDeck.wheel2 + (360 - ControllerDeck.temp_wheel2)) / 1.25);
			}

			if ((ControllerDeck.wheel2 > 270) && (ControllerDeck.temp_wheel2 < 90))
			{
				change2 = -(float)((ControllerDeck.temp_wheel2 + (360 - ControllerDeck.wheel2)) / 1.25);
			}

			int changes_1 = change1 > 0 ? (int)Math.Floor(change1) : (int)Math.Ceiling(change1);
			int changes_2 = change2 > 0 ? (int)Math.Floor(change2) : (int)Math.Ceiling(change2);

			for (int scanLine = 0; scanLine < 262; scanLine++)
			{
				_vdp.RenderScanline(scanLine);

				if (scanLine == 192)
				{
					_vdp.InterruptPending = true;

					if (_vdp.EnableInterrupts)
						_cpu.NonMaskableInterrupt = true;
				}

				for (int i = 0; i < 228; i++)
				{
					PSG.generate_sound(1);
					if (use_SGM) { SGM_sound.generate_sound(1); }			
					_cpu.ExecuteOne();

					// pick out sound samples from the sound devies twice per scanline
					if ((i==76) || (i==152))
					{
						PSG.Sample();
						if (use_SGM) { SGM_sound.Sample(); }
					}
				}

				// starting from scanline 20, changes to the wheel are added once per scanline (up to 144)
				if (scanLine > 20)
				{
					if (changes_1 != 0)
					{
						if (changes_1 > 0)
						{
							ControllerDeck.temp_wheel1 = (float)((ControllerDeck.temp_wheel1 + 1.25) % 360);
							changes_1--;
						}
						else
						{
							ControllerDeck.temp_wheel1 = (float)((ControllerDeck.temp_wheel1 - 1.25) % 360);
							changes_1++;
						}
					}

					if (changes_2 != 0)
					{
						if (changes_2 > 0)
						{
							ControllerDeck.temp_wheel2 = (float)((ControllerDeck.temp_wheel2 + 1.25) % 360);
							changes_2--;
						}
						else
						{
							ControllerDeck.temp_wheel2 = (float)((ControllerDeck.temp_wheel2 - 1.25) % 360);
							changes_2++;
						}
					}
				}

				tempRet1 = ControllerDeck.ReadPort1(controller, true, true);
				tempRet2 = ControllerDeck.ReadPort2(controller, true, true);

				intPending = (!tempRet1.Bit(4) && temp_1_prev) | (!tempRet2.Bit(4) && temp_2_prev);

				_cpu.FlagI = false;
				if (intPending)
				{
					if (_vdp.EnableInterrupts)
					{
						_cpu.FlagI = true;
						intPending = false;
					}
				}

				temp_1_prev = tempRet1.Bit(4);
				temp_2_prev = tempRet2.Bit(4);
			}

			ControllerDeck.temp_wheel1 = ControllerDeck.wheel1;
			ControllerDeck.temp_wheel2 = ControllerDeck.wheel2;

			if (_isLag)
			{
				_lagCount++;
			}
		}

		public bool use_SGM = false;
		public bool is_MC = false;
		public int MC_bank = 0;
		public bool enable_SGM_high = false;
		public bool enable_SGM_low = false;
		public byte port_0x53, port_0x7F;


		public int Frame => _frame;

		public string SystemId => "Coleco";

		public bool DeterministicEmulation => true;

		public void ResetCounters()
		{
			_frame = 0;
			_lagCount = 0;
			_isLag = false;
		}

		public CoreComm CoreComm { get; }

		public void Dispose()
		{
		}
	}
}