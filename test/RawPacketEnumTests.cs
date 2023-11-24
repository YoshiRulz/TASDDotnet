namespace Net.TASBot.TASDDotnet;

using static TASDPacketKey;

[TestClass]
public sealed class RawPacketEnumTests {
	internal static readonly u8[] SampleFile = Data.GetRawFromEmbeddedResource("simpler.tasd");

	private static TASDRawPacket SampleFileExPacket0(rwbbuf key)
		=> new(key.WriteU16BE((u16) UNSPECIFIED), bbuf.Empty);

	private static TASDRawPacket SampleFileExPacket1(rwbbuf key, rwbbuf payload)
		=> new(key.WriteU16BE((u16) EXPERIMENTAL), payload.WriteU8(1));

	private static TASDRawPacket SampleFileExPacket2(rwbbuf key)
		=> new(key.WriteU16BE((u16) COMMENT), "Hello, world!"u8);

	[TestMethod]
	public void TestForeach() {
		var i = 0;
		TASDRawPacket last = default;
		foreach (var packet in TASDRawPacketEnumeratorThrowing.Create(SampleFile, out _)) {
			i++;
			last = packet;
		}
		Assert.AreEqual(3, i);
		RawPacketTests.AssertEqual(last, SampleFileExPacket2(stackalloc u8[2]));
	}

	[TestMethod]
	public void TestGarbageInputs() {
		Assert.IsFalse(TASDRawPacketEnumeratorSafe.TryCreate(SampleFile.AsSpan(start: 1), out _, out _), "safe enumerator succeeded unexpectedly on garbage input");
		RawHeaderTests.AssertMessageContains(
			TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES,
			Assert.ThrowsException<ArgumentException>(() => {
				_ = TASDRawPacketEnumeratorThrowing.Create(SampleFile.AsSpan(start: 1), out _);
			}, "throwing enumerator succeeded unexpectedly on garbage input")
		);
	}

	[DataRow(0, "simpler.tasd")]
	[DataRow(1, "sample.tasd")]
	[DataRow(2, "5256M.tasd")]
	[DataRow(3, "4616M.tasd")]
	[TestMethod]
	public void TestOffsetParsing(int exDataIndex, string embedPathFragment) {
		var ex = Data.Expected[exDataIndex];
		var i = 0;
		var packetStream = TASDRawPacketEnumeratorThrowing.Create(
			Data.GetRawFromEmbeddedResource(embedPathFragment),
			out var acHeader
		);
		Assert.AreEqual(sizeof(TASDPacketKey), acHeader.GlobalKeyLength);
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

	[DataRow("simpler.tasd", 0, 0)]
	[DataRow("sample.tasd", 0, 1)]
	[DataRow("5256M.tasd", 11558, 1)]
	[DataRow("4616M.tasd", 66400, 1)]
	[TestMethod]
	public void TestOfKey(string embedPathFragment, s32 exInputPacketCount, s32 exRerecordPacketCount) {
		_ = TASDRawPacketEnumeratorSafe.TryCreate(Data.GetRawFromEmbeddedResource(embedPathFragment), out _, out var iter);
		var i = 0;
		foreach (var _ in iter.OfKey(INPUT_CHUNK)) i++;
		Assert.AreEqual(exInputPacketCount, i);

		_ = TASDRawPacketEnumeratorSafe.TryCreate(Data.GetRawFromEmbeddedResource(embedPathFragment), out _, out iter);
		i = 0;
		foreach (var _ in iter.OfKey(RERECORDS)) i++;
		Assert.AreEqual(exRerecordPacketCount, i);
	}

	[TestMethod]
	public void TestSafeEnumeration() {
		Assert.IsTrue(TASDRawPacketEnumeratorSafe.TryCreate(SampleFile, out var header, out var iter));
		RawHeaderTests.AssertEqual(header, TASDRawHeader.V1_Keys2o);

		Assert.IsTrue(iter.MoveNext());
		rwbbuf keyBuf = stackalloc u8[2];
		RawPacketTests.AssertEqual(iter.Current, SampleFileExPacket0(keyBuf));

		Assert.IsTrue(iter.MoveNext());
		RawPacketTests.AssertEqual(iter.Current, SampleFileExPacket1(keyBuf, stackalloc u8[1]));

		Assert.IsTrue(iter.MoveNext());
		RawPacketTests.AssertEqual(iter.Current, SampleFileExPacket2(keyBuf));

		Assert.IsFalse(iter.MoveNext());
	}

	/// <remarks>code duplication :( but you can't implement interfaces on </c>ref struct</c>s or anything</remarks>
	[TestMethod]
	public void TestThrowingEnumeration() {
		var iter = TASDRawPacketEnumeratorThrowing.Create(SampleFile, out var header);
		RawHeaderTests.AssertEqual(header, TASDRawHeader.V1_Keys2o);

		Assert.IsTrue(iter.MoveNext());
		rwbbuf keyBuf = stackalloc u8[2];
		RawPacketTests.AssertEqual(iter.Current, SampleFileExPacket0(keyBuf));

		Assert.IsTrue(iter.MoveNext());
		RawPacketTests.AssertEqual(iter.Current, SampleFileExPacket1(keyBuf, stackalloc u8[1]));

		Assert.IsTrue(iter.MoveNext());
		RawPacketTests.AssertEqual(iter.Current, SampleFileExPacket2(keyBuf));

		Assert.IsFalse(iter.MoveNext());
	}
}
