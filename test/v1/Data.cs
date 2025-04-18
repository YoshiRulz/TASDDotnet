namespace IO.TASD.V1;

using System.IO;
using System.Reflection;

using static TASDPacketKey;

internal static class Data {
	private const int SIZEOF_BOOL = sizeof(u8);

	private static readonly (TASDPacketKey Key, int PayloadLen) INP = (INPUT_CHUNK, sizeof(u8) + 1);

	private static readonly (TASDPacketKey Key, int PayloadLen) PRT = (PORT_CONTROLLER, sizeof(u8) + sizeof(u16));

	public static readonly (TASDPacketKey Key, int PayloadLen)[][] Expected = {
		/* sample.tasd */ new[] {
			(DUMP_CREATED, sizeof(s64)),
			(CONSOLE_TYPE, sizeof(u8) + 10),
			(CONSOLE_REGION, sizeof(u8)),
			(GAME_TITLE, 10),
			(ROM_NAME, 12),
			(ATTRIBUTION, sizeof(u8) + 6),
			(RERECORDS, sizeof(u32)),
			(VERIFIED, SIZEOF_BOOL),
			(MEMORY_INIT, sizeof(u8) + sizeof(u16) + SIZEOF_BOOL + sizeof(u8) + 42 + 0),
			(MEMORY_INIT, sizeof(u8) + sizeof(u16) + SIZEOF_BOOL + sizeof(u8) + 13 + 0),
			(GAME_IDENTIFIER, sizeof(u8) + sizeof(u8) + sizeof(u8) + 44), // MTU3YmRkYjcxOTI3NTRhNDUzNzJiZTE5Njc5N2YyODQK --> 157bddb7192754a45372be196797f284
			PRT, PRT, PRT, PRT,
			(NES_LATCH_FILTER, sizeof(u16)),
			(GENESIS_GAME_GENIE_CODE, 9),
			(INPUT_CHUNK, sizeof(u8) + 10),
			(INPUT_MOMENT, sizeof(u8) + sizeof(u8) + sizeof(u8) + sizeof(u64) + 1),
			(MOVIE_TRANSITION, sizeof(u32) + sizeof(u8)),
			(TRANSITION, sizeof(u8) + sizeof(u8) + sizeof(u64) + sizeof(u8) + 0),
			(TRANSITION, sizeof(u8) + sizeof(u8) + sizeof(u64) + sizeof(u8) + (sizeof(TASDPacketKey) + sizeof(u8) + sizeof(u8) + 15)),
			(COMMENT, 81), // "Hello developer! If you can see this, it means your software is probably working."
			(EXPERIMENTAL, SIZEOF_BOOL),
			(UNSPECIFIED, 58), // "Wow, you're even parsing unspecified packets, that's cool!"
		},
	};

	private static readonly Assembly Asm = typeof(Data).Assembly;

	public static u8[] GetRawFromEmbeddedResource(string embedPathFragment)
		=> Asm.GetManifestResourceStream($"TASDDotnet.Tests.v1.data.{embedPathFragment}")!.CopyToArrayAndDispose();
}
