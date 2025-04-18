namespace IO.TASD.V0;

using u8List = System.Collections.Generic.IReadOnlyList<u8>;

using static TASDPacketKey;

#if false // a u8 and a string
public readonly struct TASDAttributionPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> ATTRIBUTION;

	public TASDAttributionPacket()
		=> ;
}
#endif

public readonly struct TASDBlankFramesPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> BLANK_FRAMES;

	public readonly s16 Value;

	public TASDBlankFramesPacket(s16 value)
		=> Value = value;
}

#if false // just a string
public readonly struct TASDCategoryPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> CATEGORY;

	public TASDCategoryPacket()
		=> ;
}
#endif

public readonly struct TASDConsoleRegionPacket: ITASDPacket {
	public enum WellKnown: u8 {
		NTSC = 0x01,
		PAL = 0x02,
		Unknown = 0xFF,
	}

	public readonly TASDPacketKey Key
		=> CONSOLE_REGION;

	public readonly WellKnown Region;

	public TASDConsoleRegionPacket(WellKnown region) {
//		if (!Enum.IsDefined(typeof(WellKnown), region)) throw new ArgumentException(paramName: nameof(region), message: ERR_MSG_); //TODO good idea?
		Region = region;
	}
}

#if false // a u8 and a string
public readonly struct TASDConsoleTypePacket: ITASDPacket {
	public enum WellKnown: u8 { //TODO notes for spec: should be in order of oldest console verification :)
		Famicom = 0x01,
		NES = 0x01,
		SNES = 0x02,
		SuperFamicom = 0x02,
		N64 = 0x03,
		GameCube = 0x04,
		GB = 0x05,
		GBC = 0x06,
		GBA = 0x07,
		Genesis = 0x08,
		MegaDrive = 0x08,
		Atari2600 = 0x09,
		Custom = 0xFF,
	}

	public readonly string? Custom;

	public readonly bool IsWellKnown
		=> Custom is null;

	public readonly TASDPacketKey Key
		=> CONSOLE_TYPE;

	public readonly WellKnown WellKnown;

	public TASDConsoleTypePacket(string custom) {
		Custom = custom;
		WellKnown = WellKnown.Custom;
	}

	public TASDConsoleTypePacket(WellKnown wellKnown) {
		if (wellKnown is wellKnown.Custom || !Enum.IsDefined(typeof(WellKnown), wellKnown)) throw new ArgumentException(paramName: nameof(wellKnown), message: "TODO this message (well-known XOR custom)");
		Custom = null;
		WellKnown = wellKnown;
	}
}
#endif

public readonly struct TASDDumpCreatedPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> TAS_LAST_MODIFIED;

	public readonly s64 SecondsSinceUnixEpoch;

	public TASDDumpCreatedPacket(DateTimeOffset timestamp): this(timestamp.ToUnixTimeSeconds()) {}

	public TASDDumpCreatedPacket(s64 secondsSinceUnixEpoch)
		=> SecondsSinceUnixEpoch = secondsSinceUnixEpoch;

	public readonly DateTimeOffset AsDateTimeOffset()
		=> DateTimeOffset.FromUnixTimeSeconds(SecondsSinceUnixEpoch);
}

public readonly struct TASDDumpLastModifiedPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> TAS_LAST_MODIFIED;

	public readonly s64 SecondsSinceUnixEpoch;

	public TASDDumpLastModifiedPacket(DateTimeOffset timestamp): this(timestamp.ToUnixTimeSeconds()) {}

	public TASDDumpLastModifiedPacket(s64 secondsSinceUnixEpoch)
		=> SecondsSinceUnixEpoch = secondsSinceUnixEpoch;

	public readonly DateTimeOffset AsDateTimeOffset()
		=> DateTimeOffset.FromUnixTimeSeconds(SecondsSinceUnixEpoch);
}

#if false // just a string
public readonly struct TASDEmuCorePacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> EMULATOR_CORE;

	public TASDEmuCorePacket()
		=> ;
}
#endif

#if false // just a string
public readonly struct TASDEmuNamePacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> EMULATOR_NAME;

	public TASDEmuNamePacket()
		=> ;
}
#endif

#if false // just a string
public readonly struct TASDEmuVersionPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> EMULATOR_VERSION;

	public TASDEmuVersionPacket()
		=> ;
}
#endif

public readonly struct TASDGameIdentifierPacket: ITASDPacket {
	public enum WellKnownEncoding: u8 {
		RawBits = 0x01,
		Base16String = 0x02,
		Base32String = 0x03,
		Base64String = 0x04,
		Other = 0xFF,
	}

	public enum WellKnownKind: u8 {
		CRC8 = 0x01,
		CRC16 = 0x02,
		CRC32 = 0x03,
		MD5 = 0x04,
		SHA1 = 0x05,
		SHA224 = 0x06,
		SHA256 = 0x07,
		SHA384 = 0x08,
		SHA512 = 0x09,
		SHA512Trunc224 = 0x0A,
		SHA512Trunc256 = 0x0B,
		SHA3_224 = 0x0C,
		SHA3_256 = 0x0D,
		SHA3_384 = 0x0E,
		SHA3_512 = 0x0F,
		SHAKE128 = 0x10,
		SHAKE256 = 0x11,
		Other = 0xFF,
	}

	/// <summary>The actual identifier. For checksum algorithms, the 'digest' or 'hash'.</summary>
	public readonly u8List Digest;

	/// <summary>Which human-readable encoding, if any, the digest/identifier has been represented as.</summary>
	/// <remarks>Called <c>Base</c> in the spec.</remarks>
	public readonly WellKnownEncoding Encoding;

	/// <summary>What kind of identifier this is. For checksums, which algorithm and variant to use.</summary>
	/// <remarks>Called <c>Type</c> in the spec.</remarks>
	public readonly WellKnownKind Kind;

	public readonly TASDPacketKey Key
		=> GAME_IDENTIFIER;

	/// <remarks>
	/// <paramref name="contents"/> is not copied and WILL reflect any changes when writing out
	/// (its length is only checked in this ctor too)
	/// </remarks>
	public TASDGameIdentifierPacket(WellKnownKind kind, u8List digest, WellKnownEncoding encoding = WellKnownEncoding.RawBits) {
		var exLength = kind switch { // in bits
			WellKnownKind.CRC8 => 8,
			WellKnownKind.CRC16 => 16,
			WellKnownKind.CRC32 => 32,
			WellKnownKind.MD5 => 128,
			WellKnownKind.SHA1 => 160,
			WellKnownKind.SHA224 => 224,
			WellKnownKind.SHA256 => 256,
			WellKnownKind.SHA384 => 384,
			WellKnownKind.SHA512 => 512,
			WellKnownKind.SHA512Trunc224 => 224,
			WellKnownKind.SHA512Trunc256 => 256,
			WellKnownKind.SHA3_224 => 224,
			WellKnownKind.SHA3_256 => 256,
			WellKnownKind.SHA3_384 => 384,
			WellKnownKind.SHA3_512 => 512,
//			WellKnownKind.SHAKE128 => , // not specific enough (takes digest length as parameter)
//			WellKnownKind.SHAKE256 => , // ditto
			_ => 0
		};
		if (exLength is not 0) {
			switch (encoding) { //TODO check this is rounding correctly, I suspect what I need is ceil (and this is floor)
				case WellKnownEncoding.RawBits:
					exLength = exLength / 8;
					break;
				case WellKnownEncoding.Base16String:
					exLength = exLength / 4;
					break;
				case WellKnownEncoding.Base32String:
					exLength = exLength / 5;
					break;
				case WellKnownEncoding.Base64String:
					exLength = exLength / 6;
					break;
				default:
					exLength = 0;
					break;
			}
			if (exLength is not 0 && digest.Count != exLength) {
				var message = $"incorrect digest length, expected {exLength}";
				if (encoding is not WellKnownEncoding.RawBits) message += $" ({encoding})";
				throw new ArgumentException(paramName: nameof(digest), message: message);
			}
		}

		Digest = digest;
		Encoding = encoding;
		Kind = kind;
	}
}

#if false // just a string
public readonly struct TASDGameTitlePacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> GAME_TITLE;

	public TASDGameTitlePacket()
		=> ;
}
#endif

public readonly struct TASDIsVerifiedPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> VERIFIED;

	private readonly u8 _value;

	public readonly bool Value
		=> _value.BoolFrom01();

	public TASDIsVerifiedPacket(bool value)
		=> _value = value.As01U8();
}

#if false // bunch of stuff, including a string
public readonly struct TASDMemoryInitPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> MEMORY_INIT;

	public TASDMemoryInitPacket()
		=> ;
}
#endif

#if false // u8, string, bbuf
public readonly struct TASDMovieFilePacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> MOVIE_FILE;

	public TASDMovieFilePacket()
		=> ;
}
#endif

#if false // just a string
public readonly struct TASDMovieLicensePacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> MOVIE_LICENSE;

	public TASDMovieLicensePacket()
		=> ;
}
#endif

public readonly struct TASDPortControllerPacket: ITASDPacket {
	/// <remarks>most-significant octet matches <see cref="TASDConsoleTypePacket.WellKnown"/></remarks>
	public enum WellKnown: u16 {
		NESStandardController = 0x0101,
		NESFourScore = 0x0102,
		NESZapper = 0x0103,
		NESPowerPad = 0x0104,
		FamicomFamilyBASICKeyboard = 0x0105,

		SNESStandardController = 0x0201,
		SNESSuperMultitap = 0x0202,
		SNESMouse = 0x0203,
		SNESSuperscope = 0x0204,

		N64StandardController = 0x0301,
		N64StandardControllerRumblePak = 0x0302,
		N64StandardControllerControllerPak = 0x0303,
		N64StandardControllerTransferPak = 0x0304,
		N64Mouse = 0x0305,
		N64VRU = 0x0306,
		N64RandNetKeyboard = 0x0307,
		N64DenshaDeGo = 0x0308,

		GameCubeStandardController = 0x0401,
		GameCubeKeyboard = 0x0402,

		GB = 0x0501,
		GBC = 0x0601,
		GBA = 0x0701,

		Genesis3Button = 0x0801,
		Genesis6Button = 0x0802,

		A2600Joystick = 0x0901,
		A2600Paddle = 0x0902,
		A2600KeyboardController = 0x0903,

		Other = 0xFFFF,
	}

	public readonly TASDPacketKey Key
		=> PORT_CONTROLLER;

	public readonly WellKnown Peripheral;

	public readonly u8 Port;

	public TASDPortControllerPacket(u8 port, WellKnown peripheral) {
		Peripheral = peripheral;
		Port = port;
	}
}

public readonly struct TASDRerecordsPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> TOTAL_FRAMES;

	public readonly u32 Value;

	public TASDRerecordsPacket(u32 value)
		=> Value = value;
}

#if false // just a string
public readonly struct TASDRomNamePacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> ROM_NAME;

	public TASDRomNamePacket()
		=> ;
}
#endif

#if false // just a string
public readonly struct TASDSourceMovieURIPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> SOURCE_LINK;

	public TASDSourceMovieURIPacket()
		=> ;
}
#endif

public readonly struct TASDTASLastModifiedPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> TAS_LAST_MODIFIED;

	public readonly s64 SecondsSinceUnixEpoch;

	public TASDTASLastModifiedPacket(DateTimeOffset timestamp): this(timestamp.ToUnixTimeSeconds()) {}

	public TASDTASLastModifiedPacket(s64 secondsSinceUnixEpoch)
		=> SecondsSinceUnixEpoch = secondsSinceUnixEpoch;

	public readonly DateTimeOffset AsDateTimeOffset()
		=> DateTimeOffset.FromUnixTimeSeconds(SecondsSinceUnixEpoch);
}

public readonly struct TASDTotalFramesPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> TOTAL_FRAMES;

	public readonly u32 Value;

	public TASDTotalFramesPacket(u32 value)
		=> Value = value;
}
