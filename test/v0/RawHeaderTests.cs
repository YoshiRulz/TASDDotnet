namespace IO.TASD.V0;

using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[TestClass]
public sealed class RawHeaderTests {
	internal static void AssertEqual(
		TASDRawHeader left,
		TASDRawHeader right,
		[CallerArgumentExpression(nameof(left))] string? leftExpr = default,
		[CallerArgumentExpression(nameof(right))] string? rightExpr = default
	) => Assert.IsTrue(left == right, $"expected {leftExpr} == {rightExpr} to be true but it was false (they are unequal)");

	internal static void AssertMessageContains(string ex, Exception? exc)
		=> StringAssert.Contains(
			value: exc?.Message ?? "(no exception thrown)",
			substring: ex
		);

	private static void AssertNotEqual(
		TASDRawHeader left,
		TASDRawHeader right,
		[CallerArgumentExpression(nameof(left))] string? leftExpr = default,
		[CallerArgumentExpression(nameof(right))] string? rightExpr = default
	) => Assert.IsTrue(left != right, $"expected {leftExpr} != {rightExpr} to be true but it was false (they are equal)");

	internal static void AssertSeqEqual(bbuf ex, bbuf ac, string message)
		=> Assert.IsTrue(ex.FastSequenceEqual(ac), message);

	private static Exception? CombinedParse(bbuf buf, out TASDRawHeader fromSafe) {
		TASDRawHeader fromThrowing;
		try {
			fromThrowing = TASDRawHeader.Parse(buf);
		} catch (Exception e) {
			Assert.IsFalse(TASDRawHeader.TryParse(buf, out fromSafe), "Parse threw, but TryParse returned true");
			return e;
		}
		Assert.IsTrue(TASDRawHeader.TryParse(buf, out fromSafe), "Parse returned successfully, but TryParse returned false");
		AssertEqual(fromSafe, fromThrowing, "Parse and TryParse both succeeded but returned different values");
		return null;
	}

	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x00, 0x01, 0x02 }, 1, 2, DisplayName = $"{nameof(TestCreation)} A")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x19, 0xF3, 0xB7 }, 6643, 183, DisplayName = $"{nameof(TestCreation)} B")]
	[TestMethod]
	public void TestCreation(u8[] ex, int version, int globalKeyLength) {
		TASDRawHeader a = unchecked(new(version: (u16) version, globalKeyLength: (u8) globalKeyLength));
		Assert.AreEqual(version, a.Version);
		Assert.AreEqual(globalKeyLength, a.GlobalKeyLength);
		Assert.IsTrue(a._buf.SequenceEqual(ex));
	}

	[TestMethod]
	public void TestEquality() {
		static void HostOrderToBE(ref u64 n)
			=> n = unchecked((u64) IPAddress.HostToNetworkOrder((s64) n));
		var a = TASDRawHeader.V1_Keys2o;
		AssertEqual(a, a);

		var raw = 0x4544153400010200UL; // note wrong magic bytes! their value is fixed by the spec and the public API (not the one used here) validates them, so there's no need to use them in the comparisons
		HostOrderToBE(ref raw);
		TASDRawHeader b = new(MemoryMarshal.Cast<u64, u8>(MemoryMarshal.CreateReadOnlySpan(ref raw, 1)));
		AssertEqual(a, b);
		AssertEqual(b, a);

		// mutate underlying buffer of b
		raw = 0x5441534400010300UL;
		HostOrderToBE(ref raw);
		AssertNotEqual(a, b);
		AssertNotEqual(b, a);
	}

	[DataRow(new u8[0], TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES, DisplayName = $"{nameof(TestGarbageInputs)} A")]
	[DataRow(new u8[] { 0 }, TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES, DisplayName = $"{nameof(TestGarbageInputs)} B")]
	[DataRow(new u8[] { 0, 0 }, TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES, DisplayName = $"{nameof(TestGarbageInputs)} C")]
	[DataRow(new u8[] { 0, 0, 0 }, TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES, DisplayName = $"{nameof(TestGarbageInputs)} D")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x42 }, TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES, DisplayName = $"{nameof(TestGarbageInputs)} E")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44 }, TASDRawHeader.ERR_MSG_TOO_SHORT, DisplayName = $"{nameof(TestGarbageInputs)} F")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x00 }, TASDRawHeader.ERR_MSG_TOO_SHORT, DisplayName = $"{nameof(TestGarbageInputs)} G")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x00, 0x01 }, TASDRawHeader.ERR_MSG_TOO_SHORT, DisplayName = $"{nameof(TestGarbageInputs)} H")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x42, 0x00, 0x01, 0x02 }, TASDRawHeader.ERR_MSG_MISSING_MAGIC_BYTES, DisplayName = $"{nameof(TestGarbageInputs)} I")]
	[TestMethod]
	public void TestGarbageInputs(u8[] buf, string ex)
		=> AssertMessageContains(ex, CombinedParse(buf, out _));

	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x00, 0x01, 0x02 }, 1, 2, DisplayName = $"{nameof(TestParsing)} A")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x00, 0x01, 0x03 }, 1, 3, DisplayName = $"{nameof(TestParsing)} B")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x00, 0x02, 0x02 }, 2, 2, DisplayName = $"{nameof(TestParsing)} C")]
	[DataRow(new u8[] { 0x54, 0x41, 0x53, 0x44, 0x19, 0xF3, 0xB7 }, 6643, 183, DisplayName = $"{nameof(TestParsing)} D")]
	[TestMethod]
	public void TestParsing(u8[] buf, int exVersion, int exGlobalKeyLength) {
		var acExc = CombinedParse(buf, out var parsed);
		Assert.IsNull(acExc, $"failed to parse:\n{acExc}");
		Assert.AreEqual(exVersion, parsed.Version, "version doesn't match expected");
		Assert.AreEqual(exGlobalKeyLength, parsed.GlobalKeyLength, "global key length doesn't match expected");
	}

	[TestMethod]
	public void TestToString() {
		Assert.AreEqual(
			"TASDRawHeader(version: 1, globalKeyLength: 2)",
			TASDRawHeader.V1_Keys2o.ToString()
		);
		Assert.AreEqual(
			"TASDRawHeader(version: 6643, globalKeyLength: 183)",
			new TASDRawHeader(version: 6643, globalKeyLength: 183).ToString()
		);
	}

	[TestMethod]
	public void TestWriteTo() {
		rwbbuf dst = stackalloc u8[7];
		_ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => { // from BinaryPrimitives.WriteUInt32BigEndian
			_ = TASDRawHeader.WriteTo(rwbbuf.Empty, version: 1, globalKeyLength: 2);
		}, "expecting static WriteTo to fail when destination too small");
		bbuf ex = stackalloc u8[] { 0x54, 0x41, 0x53, 0x44, 0x00, 0x01, 0x02 };
		AssertSeqEqual(ex: ex, ac: TASDRawHeader.WriteTo(dst, version: 1, globalKeyLength: 2), "static WriteTo succeeded but wrote wrong values?");

		dst.Clear();
		Assert.IsFalse(TASDRawHeader.TryWriteTo(rwbbuf.Empty, version: 1, globalKeyLength: 2), "expecting static TryWriteTo to fail when destination too small");
		Assert.IsTrue(TASDRawHeader.TryWriteTo(dst, version: 1, globalKeyLength: 2), "expecting static TryWriteTo to succeed");
		AssertSeqEqual(ex: ex, ac: dst, "static TryWriteTo succeeded but wrote wrong values?");

		dst.Clear();
		_ = Assert.ThrowsException<ArgumentException>(() => { // from bbuf.CopyTo(rwbbuf)
			TASDRawHeader.V1_Keys2o.WriteTo(rwbbuf.Empty);
		}, "expecting instance WriteTo to fail when destination too small");
		TASDRawHeader.V1_Keys2o.WriteTo(dst);
		AssertSeqEqual(ex: ex, ac: dst, "instance WriteTo succeeded but wrote wrong values?");

		dst.Clear();
		Assert.IsFalse(TASDRawHeader.V1_Keys2o.TryWriteTo(rwbbuf.Empty), "expecting instance TryWriteTo to fail when destination too small");
		Assert.IsTrue(TASDRawHeader.V1_Keys2o.TryWriteTo(dst), "expecting instance TryWriteTo to succeed");
		AssertSeqEqual(ex: ex, ac: dst, "instance TryWriteTo succeeded but wrote wrong values?");
	}
}
