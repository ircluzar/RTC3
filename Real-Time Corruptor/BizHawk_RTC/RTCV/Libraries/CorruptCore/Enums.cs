﻿	namespace RTCV.CorruptCore
{
	public enum BlastRadius
	{
		SPREAD,
		CHUNK,
		BURST,
		NORMALIZED,
		PROPORTIONAL,
		EVEN,
		NONE
	}

	public enum BlastUnitSource
	{
		VALUE,
		STORE
	}
	public enum StoreTime
	{
		IMMEDIATE,  //Frame 0 for the blastunit. Right when it's applied. Used for Distortion
		PREEXECUTE, //For when you want it to happen right before the first step
	}
	public enum LimiterTime
	{
		NONE,       //For when something will never happen
		GENERATE,  //Generate
		PREEXECUTE, //For when you want it to happen right before the first step
		EXECUTE     //For when you want it to happen every step
	}
	public enum StoreType
	{
		ONCE, 
		CONTINUOUS
	}

	public enum StoreLimiterSource
	{
		ADDRESS,
		SOURCEADDRESS,
		BOTH
	}

	public enum CustomValueSource
	{
		RANDOM,
		VALUELIST,
		RANGE
	}
	public enum CustomStoreAddress
	{
		SAME,
		RANDOM
	}

	public enum NightmareAlgo
	{
		RANDOM,
		RANDOMTILT,
		TILT
	}
	public enum NightmareType
	{
		SET,
		ADD,
		SUBTRACT
	}

	public enum CorruptionEngine
	{
		NIGHTMARE,
		HELLGENIE,
		DISTORTION,
		FREEZE,
		PIPE,
		VECTOR,
		BLASTGENERATORENGINE,
		CUSTOM,
		NONE
	}
	public enum BGValueModes
	{
		SET,
		ADD,
		SUBTRACT,
		RANDOM,
		RANDOM_RANGE,
		SHIFT_LEFT,
		SHIFT_RIGHT,
		REPLACE_X_WITH_Y,
		BITWISE_AND,
		BITWISE_OR,
		BITWISE_XOR,
		BITWISE_COMPLEMENT,
		BITWISE_SHIFT_LEFT,
		BITWISE_SHIFT_RIGHT,
		BITWISE_ROTATE_LEFT,
		BITWISE_ROTATE_RIGHT
	}

	public enum BGStoreModes
	{
		CHAINED,
		SOURCE_SET,
		SOURCE_RANDOM,
		DEST_RANDOM,
		FREEZE,
	}

	public enum ProblematicItemTypes
	{
		PROCESS,
		ASSEMBLY
	}
	public enum StashKeySavestateLocation
	{
		SKS,
		SSK,
		MP,
		SESSION,
		DEFAULTVALUE
	}

	public enum RTCSPEC
	{
		CORE_SELECTEDENGINE,
		CORE_ALLOWCROSSCORECORRUPTION,
		
		CORE_CURRENTPRECISION,

		CORE_INTENSITY,
		CORE_ERRORDELAY,
		CORE_RADIUS,

		STEP_CLEARSTEPACTIONSONREWIND,


		STEP_MAXINFINITEBLASTUNITS,
		STEP_LOCKEXECUTION,
		STEP_RUNBEFORE,

		CORE_EXTRACTBLASTLAYER,
		CORE_AUTOCORRUPT,

		CORE_KILLSWITCHINTERVAL,
		CORE_BIZHAWKOSDDISABLED,
		CORE_SHOWCONSOLE,
		CORE_DONTCLEANSAVESTATESONQUIT,

		CORE_REROLLADDRESS,
		CORE_REROLLSOURCEADDRESS,
		CORE_REROLLDOMAIN,
		CORE_REROLLSOURCEDOMAIN,
		CORE_REROLLIGNOREORIGINALSOURCE,
		CORE_REROLLFOLLOWENGINESETTINGS,

		NIGHTMARE_ALGO,
		NIGHTMARE_MINVALUE8BIT,
		NIGHTMARE_MAXVALUE8BIT,
		NIGHTMARE_MAXVALUE16BIT,
		NIGHTMARE_MINVALUE16BIT,
		NIGHTMARE_MAXVALUE32BIT,
		NIGHTMARE_MINVALUE32BIT,

		HELLGENIE_MINVALUE8BIT,
		HELLGENIE_MAXVALUE8BIT,
		HELLGENIE_MINVALUE16BIT,
		HELLGENIE_MAXVALUE16BIT,
		HELLGENIE_MINVALUE32BIT,
		HELLGENIE_MAXVALUE32BIT,


		DISTORTION_DELAY,

		CUSTOM_NAME,
		CUSTOM_PATH,

		CUSTOM_DELAY,
		CUSTOM_LIFETIME,
		CUSTOM_LIMITERLISTHASH,
		CUSTOM_LIMITERTIME,
		CUSTOM_LIMITERINVERTED,
		CUSTOM_LOOP,
		CUSTOM_MINVALUE8BIT,
		CUSTOM_MINVALUE16BIT,
		CUSTOM_MINVALUE32BIT,
		CUSTOM_MAXVALUE8BIT,
		CUSTOM_MAXVALUE16BIT,
		CUSTOM_MAXVALUE32BIT,
		CUSTOM_SOURCE,
		CUSTOM_STOREADDRESS,
		CUSTOM_STORETIME,
		CUSTOM_STORETYPE,
		CUSTOM_STORELIMITERMODE,
		CUSTOM_TILTVALUE,
		CUSTOM_VALUELISTHASH,
		CUSTOM_VALUESOURCE,


		FILTERING_HASH2LIMITERDICO,
		FILTERING_HASH2VALUEDICO,


		VECTOR_LIMITERLISTHASH,
		VECTOR_VALUELISTHASH,

		RENDER_AT_LOAD,
		RENDER_ISRENDERING,
		RENDER_RENDERTYPE,

		STOCKPILE_CURRENTSAVESTATEKEY,
		STOCKPILE_BACKUPEDSTATE,
		STOCKPILE_StashAfterOperation,
		STOCKPILE_StashHistory,
		STOCKPILE_SavestateStashkeyDico,
		STOCKPILE_RenderAtLoad

	}

	public enum VSPEC
	{
		CORE_LASTLOADERROM,

		STEP_RUNBEFORE,

		SYSTEM,
		GAMENAME,
		SYSTEMPREFIX,
		SYSTEMCORE,
		SYNCSETTINGS,
		OPENROMFILENAME,

		SYNCOBJECT,

		MEMORYDOMAINS_INTERFACES,
		MEMORYDOMAINS_BLACKLISTEDDOMAINS

	}

	public enum UISPEC
	{
	}
}