﻿using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using RTCV.CorruptCore;
using static RTCV.UI.UI_Extensions;
using RTCV.NetCore.StaticTools;

namespace RTCV.UI
{
	public partial class RTC_VmdPool_Form : ComponentForm, IAutoColorize
	{
		public new void HandleMouseDown(object s, MouseEventArgs e) => base.HandleMouseDown(s, e);
		public new void HandleFormClosing(object s, FormClosingEventArgs e) => base.HandleFormClosing(s, e);
		
		public RTC_VmdPool_Form()
		{
			InitializeComponent();
		}

		private void btnUnloadVMD_Click(object sender, EventArgs e)
		{
			if (lbLoadedVmdList.SelectedIndex == -1)
				return;

			foreach (var item in lbLoadedVmdList.SelectedItems)
			{
				string VmdName = item.ToString();

				foreach (BlastUnit bu in StepActions.GetRawBlastLayer().Layer)
				{
					bu.RasterizeVMDs();
				}

				MemoryDomains.RemoveVMD(VmdName);
			}
			RefreshVMDs();
		}

		public void RefreshVMDs()
		{
			lbLoadedVmdList.Items.Clear();
			lbLoadedVmdList.Items.AddRange(MemoryDomains.VmdPool.Values.Select(it => it.ToString()).ToArray());

			lbRealDomainValue.Text = "#####";
			lbVmdSizeValue.Text = "#####";
		}

		private static void RenameVMD(VirtualMemoryDomain VMD) => RenameVMD(VMD.ToString());

		private static void RenameVMD(string vmdName)
		{
			if (!MemoryDomains.VmdPool.ContainsKey(vmdName))
				return;

			string name = "";
			string value = "";
			if (UI_Extensions.GetInputBox("BlastLayer to VMD", "Enter the new VMD name:", ref value) == DialogResult.OK)
			{
				name = value.Trim();
			}
			else
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(name))
				name = CorruptCore.CorruptCore.GetRandomKey();

			VirtualMemoryDomain VMD = (VirtualMemoryDomain)MemoryDomains.VmdPool[vmdName];

			MemoryDomains.RemoveVMD(VMD);
			VMD.Name = name;
			VMD.Proto.VmdName = name;
			MemoryDomains.AddVMD(VMD);
		}

		private void RTC_VmdPool_Form_Load(object sender, EventArgs e)
		{
		}

		private void lbLoadedVmdList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lbLoadedVmdList.SelectedItem == null)
				return;

			string vmdName = lbLoadedVmdList.SelectedItem.ToString();
			MemoryInterface mi = MemoryDomains.VmdPool[vmdName];

			lbVmdSizeValue.Text = mi.Size.ToString() + " (0x" + mi.Size.ToString("X") + ")";

			if ((mi as VirtualMemoryDomain)?.PointerDomains.Distinct().Count() > 1)
				lbRealDomainValue.Text = "Hybrid";
			else
				lbRealDomainValue.Text = (mi as VirtualMemoryDomain)?.PointerDomains.FirstOrDefault();
		}

		private void btnSaveVmd_Click(object sender, EventArgs e)
		{
			if (lbLoadedVmdList.SelectedIndex == -1)
				return;

			string vmdName = lbLoadedVmdList.SelectedItem.ToString();
			VirtualMemoryDomain vmd = (VirtualMemoryDomain)MemoryDomains.VmdPool[vmdName];

			SaveFileDialog saveFileDialog1 = new SaveFileDialog
			{
				DefaultExt = "json",
				Title = "Save VMD to File",
				Filter = "JSON VMD file|*.json",
				RestoreDirectory = true
			};

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string filename = saveFileDialog1.FileName;

				//creater stockpile.xml to temp folder from stockpile object
				using (FileStream fs = File.Open(filename, FileMode.Create))
				{
					JsonHelper.Serialize(vmd.Proto, fs);
				}
			}
			else
				return;
		}

		private void btnLoadVmd_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "json",
				Multiselect = true,
				Title = "Open VMD File",
				Filter = "VMD files|*.json;*.xml",
				RestoreDirectory = true
			};
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				bool notified = false;
				//string Filename = ofd.FileName.ToString();
				foreach (string filename in ofd.FileNames)
				{
					try
					{
						if(Path.GetExtension(filename).ToUpper() == ".XML")
						{
							if (!notified)
							{
								MessageBox.Show("Legacy XML VMD detected. We're going to drop support for these at some point.\nConverting to JSON and saving to the original folder.");
								notified = true;
							}

							using (FileStream fs = File.Open(filename, FileMode.Open))
							{
								XmlSerializer xs = new XmlSerializer(typeof(VmdPrototype));
								VmdPrototype proto = (VmdPrototype)xs.Deserialize(fs);

								var jsonFilename =  Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".json");
								using(FileStream _fs = File.Open(jsonFilename, FileMode.Create))
								{
									JsonHelper.Serialize(proto, _fs, Newtonsoft.Json.Formatting.Indented);
								}
															   
								MemoryDomains.AddVMD(proto);
							}
						}
						else
						{
							using (FileStream fs = File.Open(filename, FileMode.Open))
							{
								VmdPrototype proto = JsonHelper.Deserialize<VmdPrototype>(fs);

								MemoryDomains.AddVMD(proto);
							}
						}
					}
					catch(Exception ex)
					{
						throw new NetCore.CustomException($"The VMD file {filename} could not be loaded." + ex.Message, ex.StackTrace);
					}
				}

				RefreshVMDs();
			}
			else
				return;
		}

		private void btnRenameVMD_Click(object sender, EventArgs e)
		{
			if (lbLoadedVmdList.SelectedIndex == -1)
				return;

			string vmdName = lbLoadedVmdList.SelectedItem.ToString();

			RenameVMD(vmdName);

			RefreshVMDs();
		}

	}
}
