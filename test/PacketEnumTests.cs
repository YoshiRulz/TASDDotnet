namespace Net.TASBot.TASDDotnet;

using System.IO;
using System.Reflection;

[TestClass]
public sealed class PacketEnumTests {
	private static readonly Assembly Asm = typeof(PacketEnumTests).Assembly;

	private static bbuf GetRawFromEmbeddedResource(string embedPathFragment) {
		using var stream = Asm.GetManifestResourceStream($"TASDDotnet.Tests.data.{embedPathFragment}")!;
		var buf = new u8[(int) stream.Length];
		using MemoryStream ms = new(buf);
		stream.CopyTo(ms);
		return buf;
	}

	[DataRow(0, "sample.tasd")]
	[DataRow(1, "5256M.tasd")]
	[DataRow(2, "4616M.tasd")]
	[TestMethod]
	public void TestAAAAA(int exDataIndex, string embedPathFragment) {
		var ex = Data.Expected[exDataIndex];
		var i = 0;
		var packetStream = TASDRawPacketEnumeratorThrowing.Create(
			GetRawFromEmbeddedResource(embedPathFragment),
			out var acHeader
		);
		Assert.AreEqual(sizeof(TASDPacketKey), acHeader.GlobalKeyLength); // TASDPacketKey enum is u16
		foreach (var packet in packetStream) {
			var acKey = (TASDPacketKey) packet.Key.ReadU16BE();
			Assert.IsTrue(i < ex.Length, "file has extra packets?");
			var (exKey, exPayloadLen) = ex[i];
			Assert.AreEqual(exKey, acKey, $"packet #{i} failed to parse (key was {acKey}, expecting {exKey})");
			var acPayloadLen = packet.Payload.Length;
			Assert.AreEqual(exPayloadLen, acPayloadLen, $"packet #{i} failed to parse (payload was {acPayloadLen} octets long, expecting {exPayloadLen})");
			i++;
		}
		Assert.AreEqual(ex.Length, i, $"packets #{i}..<#{ex.Length} failed to parse");
	}
}
