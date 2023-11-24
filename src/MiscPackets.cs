namespace Net.TASBot.TASDDotnet;

using System.Collections.Generic;

using static TASDPacketKey;

using u8List = System.Collections.Generic.IReadOnlyList<u8>;

public interface ITASDPacket {
	TASDPacketKey Key { get; }
}

public readonly struct TASDExperimentalPacket: ITASDPacket {
	public readonly TASDPacketKey Key
		=> EXPERIMENTAL;

	private readonly u8 _value;

	/// <remarks>Called <c>Experimental</c> in the spec.</remarks>
	public readonly bool IsExperimental
		=> _value.BoolFrom01();

	public TASDExperimentalPacket(bool isExperimental)
		=> _value = isExperimental.As01U8();
}

public record class TASDFile(IReadOnlyList<ITASDPacket> AllPackets) {
	/// <remarks>TODO rename inner and add public wrapper which checks key length</remarks>
	internal static ITASDPacket Parse1(TASDRawPacket raw) {
		var key = unchecked((TASDPacketKey) raw.Key.ReadU16BE());
		return key switch {
			// general
//			CONSOLE_TYPE => new TASDConsoleTypePacket(),
			CONSOLE_REGION => new TASDConsoleRegionPacket(),
//			GAME_TITLE => new TASDGameTitlePacket(),
//			ROM_NAME => new TASDRomNamePacket(),
//			ATTRIBUTION => new TASDAttributionPacket(),
//			CATEGORY => new TASDCategoryPacket(),
//			EMULATOR_NAME => new TASDEmuNamePacket(),
//			EMULATOR_VERSION => new TASDEmuVersionPacket(),
//			EMULATOR_CORE => new TASDEmuCorePacket(),
			TAS_LAST_MODIFIED => new TASDTASLastModifiedPacket(),
			DUMP_CREATED => new TASDDumpCreatedPacket(),
			DUMP_LAST_MODIFIED => new TASDDumpLastModifiedPacket(),
			TOTAL_FRAMES => new TASDTotalFramesPacket(),
			RERECORDS => new TASDRerecordsPacket(),
//			SOURCE_LINK => new TASDSourceMovieURIPacket(),
//			BLANK_FRAMES => new TASDBlankFrames(),
			VERIFIED => new TASDIsVerifiedPacket(),
//			MEMORY_INIT => new TASDMemoryInitPacket(),
			GAME_IDENTIFIER => new TASDGameIdentifierPacket(),
//			MOVIE_LICENSE => new TASDMovieLicensePacket(),
//			MOVIE_FILE => new TASDMovieFilePacket(),
			PORT_CONTROLLER => new TASDPortControllerPacket(),

#if false
			// NES
			NES_LATCH_FILTER => new NES_LATCH_FILTER(),
			NES_CLOCK_FILTER => new NES_CLOCK_FILTER(),
			NES_OVERREAD => new NES_OVERREAD(),
			NES_GAME_GENIE_CODE => new NES_GAME_GENIE_CODE(),

			// SNES
			SNES_CLOCK_FILTER => new SNES_CLOCK_FILTER(),
			SNES_OVERREAD => new SNES_OVERREAD(),
			SNES_GAME_GENIE_CODE => new SNES_GAME_GENIE_CODE(),
			SNES_LATCH_TRAIN => new SNES_LATCH_TRAIN(),

			// Genesis
			GENESIS_GAME_GENIE_CODE => new GENESIS_GAME_GENIE_CODE(),

			// inputs
			INPUT_CHUNK => new INPUT_CHUNK(),
			INPUT_MOMENT => new INPUT_MOMENT(),
			TRANSITION => new TRANSITION(),
			LAG_FRAME_CHUNK => new LAG_FRAME_CHUNK(),
			MOVIE_TRANSITION => new MOVIE_TRANSITION(),
#endif

			// misc.
//			COMMENT => new TASDCommentPacket(),
			EXPERIMENTAL => new TASDExperimentalPacket(),
			UNSPECIFIED => new TASDUnspecifiedPacket(),
			_ => new TASDNotUnspecifiedPacket(key, raw.Payload.ToArray())
		};
	}

	public static TASDFile ParseHeaderAndAllPackets(bbuf fileBuf) {
		var iter = TASDRawPacketEnumeratorThrowing.Create(fileBuf, out var header);
		if (header.GlobalKeyLength is not sizeof(TASDPacketKey)) throw new ArgumentException(paramName: nameof(fileBuf), message: $"G_KEYLEN in header was {header.GlobalKeyLength}, but this library only works with 2-octet packet keys");
		List<ITASDPacket> packets = new();
		foreach (var rawPacket in iter) packets.Add(Parse1(rawPacket));
		return new(packets);
	}
}

/// <summary>For working with packet types that aren't in version 1 of the spec (which is what this library implements).</summary>
public readonly struct TASDNotUnspecifiedPacket: ITASDPacket {
	/// <remarks>Called <c>Payload</c> in the spec (w.r.t. packet headers).</remarks>
	public readonly u8List Contents;

	public TASDPacketKey Key { get; }

	/// <remarks><paramref name="contents"/> is not copied and WILL reflect any changes when writing out</remarks>
	public TASDNotUnspecifiedPacket(TASDPacketKey key, u8List contents) {
		Contents = contents;
		Key = key;
	}
}

public enum TASDPacketKey: u16 {
	// general
	CONSOLE_TYPE = 0x0001,
	CONSOLE_REGION = 0x0002,
	GAME_TITLE = 0x0003,
	ROM_NAME = 0x0004,
	ATTRIBUTION = 0x0005,
	CATEGORY = 0x0006,
	EMULATOR_NAME = 0x0007,
	EMULATOR_VERSION = 0x0008,
	EMULATOR_CORE = 0x0009,
	TAS_LAST_MODIFIED = 0x000A,
	DUMP_CREATED = 0x000B,
	DUMP_LAST_MODIFIED = 0x000C,
	TOTAL_FRAMES = 0x000D,
	RERECORDS = 0x000E,
	SOURCE_LINK = 0x000F,
	BLANK_FRAMES = 0x0010,
	VERIFIED = 0x0011,
	MEMORY_INIT = 0x0012,
	GAME_IDENTIFIER = 0x0013,
	MOVIE_LICENSE = 0x0014,
	MOVIE_FILE = 0x0015,
	PORT_CONTROLLER = 0x00F0,
//	PORT_OVERREAD = 0x00F1, //TODO this is new, replaces `{NES,SNES}_OVERREAD` I believe

	// NES
	NES_LATCH_FILTER = 0x0101,
	NES_CLOCK_FILTER = 0x0102,
	NES_OVERREAD = 0x0103,
	NES_GAME_GENIE_CODE = 0x0104,

	// SNES
	SNES_CLOCK_FILTER = 0x0202,
	SNES_OVERREAD = 0x0203,
	SNES_GAME_GENIE_CODE = 0x0204,
	SNES_LATCH_TRAIN = 0x0205,

	// Genesis
	GENESIS_GAME_GENIE_CODE = 0x0804,

	// inputs
	INPUT_CHUNK = 0xFE01,
	INPUT_MOMENT = 0xFE02,
	TRANSITION = 0xFE03,
	LAG_FRAME_CHUNK = 0xFE04,
	MOVIE_TRANSITION = 0xFE05,

	// misc.
	COMMENT = 0xFF01,
	EXPERIMENTAL = 0xFFFE,
	UNSPECIFIED = 0xFFFF,
}

/// <seealso cref="TASDNotUnspecifiedPacket"/>
public readonly struct TASDUnspecifiedPacket: ITASDPacket {
	/// <remarks>Called <c>Unspecified Data</c> in the spec.</remarks>
	public readonly u8List Contents;

	public readonly TASDPacketKey Key
		=> UNSPECIFIED;

	/// <remarks><paramref name="contents"/> is not copied and WILL reflect any changes when writing out</remarks>
	public TASDUnspecifiedPacket(u8List contents)
		=> Contents = contents;
}
