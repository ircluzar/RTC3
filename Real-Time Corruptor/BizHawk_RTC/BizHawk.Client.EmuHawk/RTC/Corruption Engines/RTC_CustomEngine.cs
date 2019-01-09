﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace RTC
{
	public static class RTC_CustomEngine
	{

		public static long MinValue8Bit
		{
			get => (long)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_MINVALUE8BIT.ToString(), value);
		}
		public static long MaxValue8Bit
		{
			get => (long)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString(), value);
		}

		public static long MinValue16Bit
		{
			get => (long)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_MINVALUE16BIT.ToString(), value);
		}
		public static long MaxValue16Bit
		{
			get => (long)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString(), value);
		}

		public static long MinValue32Bit
		{
			get => (long)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_MINVALUE32BIT.ToString(), value);
		}
		public static long MaxValue32Bit
		{
			get => (long)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString(), value);
		}

		public static BlastUnitSource Source
		{
			get => (BlastUnitSource)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_SOURCE.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_SOURCE.ToString(), value);
		}

		public static StoreType StoreType
		{
			get => (StoreType)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_STORETYPE.ToString(), value);
		}
		public static StoreTime StoreTime
		{
			get => (StoreTime)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_STORETIME.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_STORETIME.ToString(), value);
		}
		public static CustomStoreAddress StoreAddress
		{
			get => (CustomStoreAddress)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_STOREADDRESS.ToString(), value);
		}

		public static int Delay
		{
			get => (int)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_DELAY.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_DELAY.ToString(), value);
		}
		public static int Lifetime
		{
			get => (int)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_LIFETIME.ToString(), value);
		}

		public static BigInteger TiltValue
		{
			get => (BigInteger)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_TILTVALUE.ToString(), value);
		}

		public static LimiterTime LimiterTime
		{
			get => (LimiterTime)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_LIMITERTIME.ToString(), value);
		}
		public static bool LimiterInverted
		{
			get => (bool)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_LIMITERINVERTED.ToString(), value);
		}

		public static bool Loop
		{
			get => (bool)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LOOP.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_LOOP.ToString(), value);
		}

		public static CustomValueSource ValueSource
		{
			get => (CustomValueSource)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_VALUESOURCE.ToString(), value);
		}

		public static string LimiterListHash
		{
			get => (string)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString(), value);
		}

		public static string ValueListHash
		{
			get => (string)RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()];
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_VALUELISTHASH.ToString(), value);
		}

		public static PartialSpec lastLoadedTemplate = null;

		public static BlastUnit GenerateUnit(string domain, long address, int precision)
		{
			try
			{
						
				if (domain == null)
					return null;

				MemoryDomainProxy mdp = RTC_MemoryDomains.GetProxy(domain, address);


				byte[] value = new byte[precision];
				long safeAddress = address - (address % precision);

			
				BlastUnit bu = new BlastUnit();

				switch (Source)
				{
					case BlastUnitSource.VALUE:
					{
						switch (ValueSource)
						{
							case CustomValueSource.VALUELIST:
							{
								value = RTC_Filtering.GetRandomConstant(ValueListHash, precision);
							}
							break;

							case CustomValueSource.RANGE:
							{
								long randomValue = -1;
								switch (precision)
								{
									case 1:
										randomValue = RTC_Core.RND.RandomLong(MinValue8Bit, MaxValue8Bit);
										break;
									case 2:
										randomValue = RTC_Core.RND.RandomLong(MinValue16Bit, MaxValue16Bit);
										break;
									case 4:
										randomValue = RTC_Core.RND.RandomLong(MinValue32Bit, MaxValue32Bit);
										break;
								}

								if (randomValue != -1)
									value = RTC_Extensions.GetByteArrayValue(precision, randomValue, true);
								else
									for (int i = 0; i < precision; i++)
										value[i] = (byte)RTC_Core.RND.Next();
							}
							break;

							case CustomValueSource.RANDOM:
							{
								for (int i = 0; i < precision; i++)
									value[i] = (byte)RTC_Core.RND.Next();
							}
							break;
								
						}
					}
					break;

					case BlastUnitSource.STORE:
					{
						bu.StoreType = StoreType;
						bu.StoreTime = StoreTime;

						switch (StoreAddress)
						{
							case CustomStoreAddress.RANDOM:
							{
								BlastTarget bt = RTC_Core.GetBlastTarget();
								long safeStartAddress = bt.Address - (bt.Address % precision);
								bu.SourceDomain = bt.Domain;
								bu.SourceAddress = safeStartAddress;
							}
							break;
							case CustomStoreAddress.SAME:
							{
								bu.SourceDomain = domain;
								bu.SourceAddress = safeAddress;
							}
							break;
						}
					}
					break;
				}
				//Precision has to be before Value
				bu.Precision = precision;
				bu.Value = value;
				bu.Address = safeAddress;
				bu.Domain = domain;
				bu.Source = Source;
				bu.ExecuteFrame = Delay;
				bu.Lifetime = Lifetime;
				bu.LimiterTime = LimiterTime;
				bu.Loop = Loop;
				bu.InvertLimiter = LimiterInverted;

				//Only set a list if it's used to save on memory
				if (LimiterTime != LimiterTime.NONE)
					bu.LimiterListHash = LimiterListHash;

				//Limiter handling
				if (LimiterTime == LimiterTime.IMMEDIATE)
				{
					if (LimiterInverted)
					{
						//If it's store, we need to use the sourceaddress and sourcedomain
						if (Source == BlastUnitSource.STORE && RTC_Filtering.LimiterPeekBytes(bu.SourceAddress,
								bu.SourceAddress + bu.Precision, bu.SourceDomain, LimiterListHash, mdp))
							return null;
						if (Source == BlastUnitSource.VALUE && RTC_Filtering.LimiterPeekBytes(bu.Address,
															 bu.Address + bu.Precision, bu.Domain, LimiterListHash, mdp))
							return null;
					}
					else
					{
						//If it's store, we need to use the sourceaddress and sourcedomain
						if (Source == BlastUnitSource.STORE && !RTC_Filtering.LimiterPeekBytes(bu.SourceAddress,
								bu.SourceAddress + bu.Precision, bu.SourceDomain, LimiterListHash, mdp))
							return null;
						if (Source == BlastUnitSource.VALUE && !RTC_Filtering.LimiterPeekBytes(bu.Address,
															 bu.Address + bu.Precision, bu.Domain, LimiterListHash, mdp))
							return null;
					}
				}

				return bu;
			}
			catch (Exception ex)
			{
				throw new Exception("Custom Engine GenerateUnit Threw Up" + ex);
			}
		}


		public static bool IsConstant(byte[] bytes, string[] list, bool bigEndian)
		{
			if (list == null)
				return true;
			if (!bigEndian)
				return list.Contains(ByteArrayToString(bytes));
			Array.Reverse(bytes);
			return list.Contains(ByteArrayToString(bytes));
		}

		public static string ByteArrayToString(byte[] bytes)
		{
			return BitConverter.ToString(bytes).Replace("-", "").ToLower();
		}

		public static byte[] GetRandomConstant(string[] list)
		{
			if (list == null)
			{
				byte[] buffer = new byte[4];
				RTC_Core.RND.NextBytes(buffer);
				return buffer;
			}

			return StringToByteArray(list[RTC_Core.RND.Next(list.Length)]);
		}

		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}

		public static void InitTemplate_NightmareEngine(PartialSpec pSpec)
		{

			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = "Nightmare Engine";

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = 0;
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = 1;
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = 1;

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = null;
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = LimiterTime.NONE;
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = null;

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = 0xFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = 0xFFFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = 0xFFFFFFFFL;

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = CustomValueSource.RANDOM;

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = BlastUnitSource.VALUE;

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = CustomStoreAddress.RANDOM;
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = StoreTime.IMMEDIATE;
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = StoreType.ONCE;

			lastLoadedTemplate = (PartialSpec)pSpec.Clone();
		}
		public static void InitTemplate_HellgenieEngine(PartialSpec pSpec)
		{
			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = "Hellgenie Engine";

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = 0;
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = 1;
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = 1;

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = null;
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = LimiterTime.NONE;
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = null;

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = 0xFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = 0xFFFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = 0xFFFFFFFFL;

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = CustomValueSource.RANDOM;

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = BlastUnitSource.VALUE;

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = CustomStoreAddress.RANDOM;
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = StoreTime.IMMEDIATE;
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = StoreType.ONCE;

			lastLoadedTemplate = (PartialSpec)pSpec.Clone();
		}
		public static void InitTemplate_DistortionEngine(PartialSpec pSpec)
		{
			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = "Distortion Engine";

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = 0;
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = 1;
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = 1;

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = null;
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = LimiterTime.NONE;
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = null;

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = 0xFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = 0xFFFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = 0xFFFFFFFFL;

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = CustomValueSource.RANDOM;

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = BlastUnitSource.VALUE;

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = CustomStoreAddress.RANDOM;
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = StoreTime.IMMEDIATE;
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = StoreType.ONCE;

			lastLoadedTemplate = (PartialSpec)pSpec.Clone();
		}
		public static void InitTemplate_FreezeEngine(PartialSpec pSpec)
		{
			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = "Freeze Engine";

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = 0;
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = 1;
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = 1;

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = null;
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = LimiterTime.NONE;
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = null;

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = 0xFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = 0xFFFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = 0xFFFFFFFFL;

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = CustomValueSource.RANDOM;

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = BlastUnitSource.VALUE;

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = CustomStoreAddress.RANDOM;
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = StoreTime.IMMEDIATE;
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = StoreType.ONCE;

			lastLoadedTemplate = (PartialSpec)pSpec.Clone();
		}
		public static void InitTemplate_PipeEngine(PartialSpec pSpec)
		{
			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = "Pipe Engine";

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = 0;
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = 1;
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = 1;

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = null;
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = LimiterTime.NONE;
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = null;

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = 0xFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = 0xFFFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = 0xFFFFFFFFL;

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = CustomValueSource.RANDOM;

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = BlastUnitSource.VALUE;

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = CustomStoreAddress.RANDOM;
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = StoreTime.IMMEDIATE;
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = StoreType.ONCE;

			lastLoadedTemplate = (PartialSpec)pSpec.Clone();
		}
		public static void InitTemplate_VectorEngine(PartialSpec pSpec)
		{
			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = "Vector Engine";

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = 0;
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = 1;
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = 1;

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = null;
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = LimiterTime.NONE;
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = false;

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = null;

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = 0L;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = 0xFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = 0xFFFFL;
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = 0xFFFFFFFFL;

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = CustomValueSource.RANDOM;

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = BlastUnitSource.VALUE;

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = CustomStoreAddress.RANDOM;
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = StoreTime.IMMEDIATE;
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = StoreType.ONCE;

			lastLoadedTemplate = (PartialSpec)pSpec.Clone();
		}

		public static PartialSpec getCurrentConfigSpec()
		{
			PartialSpec pSpec = new PartialSpec(RTC_Unispec.RTCSpec.name);

			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = pSpec[RTCSPEC.CUSTOM_NAME.ToString()];

			pSpec[RTCSPEC.CUSTOM_DELAY.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_DELAY.ToString()];
			pSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIFETIME.ToString()];
			pSpec[RTCSPEC.CUSTOM_LOOP.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LOOP.ToString()];

			pSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_TILTVALUE.ToString()];

			pSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIMITERLISTHASH.ToString()];
			pSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIMITERTIME.ToString()];
			pSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_LIMITERINVERTED.ToString()];

			pSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_VALUELISTHASH.ToString()];

			pSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MINVALUE8BIT.ToString()];
			pSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MINVALUE16BIT.ToString()];
			pSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MINVALUE32BIT.ToString()];
			pSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MAXVALUE8BIT.ToString()];
			pSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MAXVALUE16BIT.ToString()];
			pSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_MAXVALUE32BIT.ToString()];

			pSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_VALUESOURCE.ToString()];

			pSpec[RTCSPEC.CUSTOM_SOURCE.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_SOURCE.ToString()];

			pSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_STOREADDRESS.ToString()];
			pSpec[RTCSPEC.CUSTOM_STORETIME.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_STORETIME.ToString()];
			pSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()] = RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_STORETYPE.ToString()];

			return pSpec;
		}


		public static string CustomPath
		{
			get => RTC_Unispec.RTCSpec[RTCSPEC.CUSTOM_PATH.ToString()].ToString();
			set => RTC_Unispec.RTCSpec.Update(RTCSPEC.CUSTOM_PATH.ToString(), value);
		}

		public static PartialSpec LoadTemplateFile()
		{
			string Filename;

			OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "json",
				Title = "Open Engine Template File",
				Filter = "JSON files|*.json",
				RestoreDirectory = true
			};

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Filename = ofd.FileName;
			}
			else
				return null;

			PartialSpec pSpec;

			try
			{
				using (FileStream fs = File.Open(Filename, FileMode.OpenOrCreate))
				{
					pSpec = JsonHelper.Deserialize<PartialSpec>(fs);
					fs.Close();
				}

			}
			catch (Exception e)
			{
				MessageBox.Show("The Template file could not be loaded" + e);
				return null;
			}

			//Overwrites spec path with loaded path
			pSpec[RTCSPEC.CUSTOM_PATH.ToString()] = Filename;

			//Keeps a backup for Reset Config
			lastLoadedTemplate = (PartialSpec)pSpec.Clone();

			return pSpec;
		}

		public static string SaveTemplateFile(bool SaveAs = false)
		{
			PartialSpec pSpec = getCurrentConfigSpec();

			string path;
			string templateName;

			if (SaveAs || CustomPath == null)
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog
				{
					DefaultExt = "json",
					Title = "Save Engine Template File",
					Filter = "JSON files|*.json",
					RestoreDirectory = true,
				};

				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					path = saveFileDialog1.FileName;
					templateName = Path.GetFileNameWithoutExtension(path);
				}
				else
					return null;
			}
			else
			{
				path = CustomPath;
				templateName = CustomPath;
			}

			pSpec[RTCSPEC.CUSTOM_NAME.ToString()] = templateName;
			pSpec[RTCSPEC.CUSTOM_PATH.ToString()] = path;

			//Create stockpile.xml to temp folder from stockpile object
			using (FileStream fs = File.Open(path, FileMode.OpenOrCreate))
			{
				var jsonSerializerSettings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All,
					Formatting = Formatting.Indented
				};

				var jsonString = JsonConvert.SerializeObject(pSpec, jsonSerializerSettings);
				var byteArray = jsonString.GetBytes();
				fs.Write(byteArray, 0, byteArray.Length);
				//JsonHelper.Serialize(pSpec, fs, Formatting.Indented);
				fs.Close();
			}

			return templateName;
		}
	}
}