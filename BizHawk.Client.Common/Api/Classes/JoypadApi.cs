﻿using System;
using System.Collections.Generic;

using BizHawk.Client.Common;

namespace BizHawk.Client.Common
{
	public sealed class JoypadApi : IJoypad
	{
		public JoypadApi(Action<string> logCallback)
		{
			LogCallback = logCallback;
		}

		public JoypadApi() : this(Console.WriteLine) {}

		private readonly Action<string> LogCallback;

		public Dictionary<string,dynamic> Get(int? controller = null)
		{
			var buttons = new Dictionary<string, dynamic>();
			var adapter = Global.AutofireStickyXORAdapter;
			foreach (var button in adapter.Source.Definition.BoolButtons)
			{
				if (!controller.HasValue)
				{
					buttons[button] = adapter.IsPressed(button);
				}
				else if (button.Length >= 3 && button.Substring(0, 2) == $"P{controller}")
				{
					buttons[button.Substring(3)] = adapter.IsPressed($"P{controller} {button.Substring(3)}");
				}
			}

			foreach (var button in adapter.Source.Definition.FloatControls)
			{
				if (controller == null)
				{
					buttons[button] = adapter.GetFloat(button);
				}
				else if (button.Length >= 3 && button.Substring(0, 2) == $"P{controller}")
				{
					buttons[button.Substring(3)] = adapter.GetFloat($"P{controller} {button.Substring(3)}");
				}
			}

			return buttons;
		}

		public Dictionary<string, dynamic> GetImmediate()
		{
			var buttons = new Dictionary<string, dynamic>();
			var adapter = Global.ActiveController;
			foreach (var button in adapter.Definition.BoolButtons)
			{
				buttons[button] = adapter.IsPressed(button);
			}

			foreach (var button in adapter.Definition.FloatControls)
			{
				buttons[button] = adapter.GetFloat(button);
			}

			return buttons;
		}

		public void SetFromMnemonicStr(string inputLogEntry)
		{
			try
			{
				var lg = Global.MovieSession.MovieControllerInstance();
				lg.SetControllersAsMnemonic(inputLogEntry);

				foreach (var button in lg.Definition.BoolButtons)
				{
					Global.ButtonOverrideAdaptor.SetButton(button, lg.IsPressed(button));
				}

				foreach (var floatButton in lg.Definition.FloatControls)
				{
					Global.ButtonOverrideAdaptor.SetFloat(floatButton, lg.GetFloat(floatButton));
				}
			}
			catch (Exception)
			{
				LogCallback($"invalid mnemonic string: {inputLogEntry}");
			}
		}

		public void Set(Dictionary<string,bool> buttons, int? controller = null)
		{
			try
			{
				foreach (var button in buttons.Keys)
				{
					var invert = false;
					bool? theValue;
					var theValueStr = buttons[button].ToString();

					if (!string.IsNullOrWhiteSpace(theValueStr))
					{
						if (theValueStr.ToLower() == "false")
						{
							theValue = false;
						}
						else if (theValueStr.ToLower() == "true")
						{
							theValue = true;
						}
						else
						{
							invert = true;
							theValue = null;
						}
					}
					else
					{
						theValue = null;
					}

					var toPress = button;
					if (controller.HasValue)
					{
						toPress = $"P{controller} {button}";
					}

					if (!invert)
					{
						if (theValue.HasValue) // Force
						{
							Global.ButtonOverrideAdaptor.SetButton(toPress, theValue.Value);
							Global.ActiveController.Overrides(Global.ButtonOverrideAdaptor);
						}
						else // Unset
						{
							Global.ButtonOverrideAdaptor.UnSet(toPress);
							Global.ActiveController.Overrides(Global.ButtonOverrideAdaptor);
						}
					}
					else // Inverse
					{
						Global.ButtonOverrideAdaptor.SetInverse(toPress);
						Global.ActiveController.Overrides(Global.ButtonOverrideAdaptor);
					}
				}
			}
			catch
			{
				/*Eat it*/
			}
		}
		public void Set(string button, bool? state = null, int? controller = null)
		{
			try
			{
				var toPress = button;
				if (controller.HasValue)
				{
					toPress = $"P{controller} {button}";
				}
				if (state.HasValue)
					Global.ButtonOverrideAdaptor.SetButton(toPress, state.Value);
				else 
					Global.ButtonOverrideAdaptor.UnSet(toPress);
				Global.ActiveController.Overrides(Global.ButtonOverrideAdaptor);
			}
			catch
			{
				/*Eat it*/
			}
		}
		public void SetAnalog(Dictionary<string,float> controls, object controller = null)
		{
			try
			{
				foreach (var name in controls.Keys)
				{
					var theValueStr = controls[name].ToString();
					float? theValue = null;

					if (!string.IsNullOrWhiteSpace(theValueStr))
					{
						if (float.TryParse(theValueStr, out var f))
						{
							theValue = f;
						}
					}

					Global.StickyXORAdapter.SetFloat(controller == null ? name : $"P{controller} {name}", theValue);
				}
			}
			catch
			{
				/*Eat it*/
			}
		}
		public void SetAnalog(string control, float? value = null, object controller = null)
		{
			try
			{
				Global.StickyXORAdapter.SetFloat(controller == null
					? control
					: $"P{controller} {control}", value);
			}
			catch
			{
				/*Eat it*/
			}
		}
	}
}
