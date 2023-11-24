namespace Net.TASBot.TASDDotnet;

using static TASDPacketKey;

[TestClass]
public sealed class RawPacketEnumTests {
	internal static readonly u8[] SampleFile = {
		0x54, 0x41, 0x53, 0x44, 0x00, 0x01, 0x02,
		0xFF, 0xFF, 0x01, 0x00,
		0xFF, 0xFE, 0x01, 0x01, 0x01,
		0xFF, 0x01, 0x01, 0x0D, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x2C, 0x20, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x21,
	};

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
