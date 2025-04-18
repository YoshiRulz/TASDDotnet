namespace Net.TASBot.TASDDotnet;

using System.Runtime.CompilerServices;

[TestClass]
public sealed class RawPacketTests {
	public static TASDRawHeader V1_Keys3o
		=> new(version: 1, globalKeyLength: 3);

	internal static void AssertEqual(
		TASDRawPacket left,
		TASDRawPacket right,
		[CallerArgumentExpression(nameof(left))] string? leftExpr = default,
		[CallerArgumentExpression(nameof(right))] string? rightExpr = default
	) {
		Assert.IsTrue(left == right, $"expected {leftExpr} == {rightExpr} to be true but it was false (they are unequal)");
		Assert.IsFalse(left != right, $"expected {leftExpr} != {rightExpr} to be false but it was true (they are unequal)");
	}

	private static void AssertNotEqual(
		TASDRawPacket left,
		TASDRawPacket right,
		[CallerArgumentExpression(nameof(left))] string? leftExpr = default,
		[CallerArgumentExpression(nameof(right))] string? rightExpr = default
	) {
		Assert.IsTrue(left != right, $"expected {leftExpr} != {rightExpr} to be true but it was false (they are equal)");
		Assert.IsFalse(left == right, $"expected {leftExpr} == {rightExpr} to be false but it was true (they are equal)");
	}

	/// <remarks>
	/// knowing that bbuf (=Span) is a <c>ref T</c> field and a length,
	/// it makes sense that <c>a.Slice(0, 0) != bbuf.Empty</c>, but I don't like it >:(
	/// </remarks>
	private static void AssertEqualOrBothEmpty(
		bbuf left,
		bbuf right,
		[CallerArgumentExpression(nameof(left))] string? leftExpr = default,
		[CallerArgumentExpression(nameof(right))] string? rightExpr = default
	) => Assert.IsTrue(left == right || (left.Length is 0 && right.Length is 0), $"expected {leftExpr} == {rightExpr} to be true but it was false (they are unequal)");

	private static Exception? CombinedParse(
		bbuf buf,
		TASDRawHeader header,
		out TASDRawPacket fromSafe,
		out int endOffset
	) {
		TASDRawPacket fromThrowing;
		try {
			fromThrowing = TASDRawPacket.Parse(buf, header, out endOffset);
		} catch (Exception e) {
			// clobber endOffset, not point checking it on failure, should be default of 0
			Assert.IsFalse(TASDRawPacket.TryParse(buf, header, out fromSafe, out endOffset), "Parse threw, but TryParse returned true");
			return e;
		}
		Assert.IsTrue(TASDRawPacket.TryParse(buf, header, out fromSafe, out var endOffset1), "Parse returned successfully, but TryParse returned false");
		Assert.AreEqual(endOffset, endOffset1, "Parse and TryParse both succeeded but returned different end offsets");
		AssertEqual(fromThrowing, fromSafe, "Parse and TryParse both succeeded but returned different values");
		return null;
	}

	[TestMethod]
	public void TestEquality() {
		bbuf buf = stackalloc u8[] { 0xFF, 0x01, 0x01, 0x01, 0x41, 0xFF, 0x01, 0x01, 0x01, 0x41 };
		TASDRawPacket a = new(buf[0..2], buf[4..5]);
		AssertEqual(a, a);
		TASDRawPacket b = new(buf[5..7], buf[9..10]);
		AssertEqual(a, b);
		AssertEqual(b, a);
		TASDRawPacket c = new(buf[5..7], buf[8..9]);
		AssertNotEqual(a, c);
		AssertNotEqual(c, a);
		TASDRawPacket d = new(buf[5..6], buf[9..10]);
		AssertNotEqual(a, d);
		AssertNotEqual(d, a);
	}

	[DataRow(new u8[0], false, TASDRawPacket.ERR_MSG_TOO_SHORT_PKHDR, DisplayName = $"{nameof(TestGarbageInputs)} A")]
	[DataRow(new u8[] { 0xFF }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_PKHDR, DisplayName = $"{nameof(TestGarbageInputs)} B")]
	[DataRow(new u8[] { 0xFF, 0x01 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_PKHDR, DisplayName = $"{nameof(TestGarbageInputs)} C")]
	[DataRow(new u8[] { 0xFF, 0x01, 0x01 }, true, TASDRawPacket.ERR_MSG_TOO_SHORT_PKHDR, DisplayName = $"{nameof(TestGarbageInputs)} D")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x01 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_LEN, DisplayName = $"{nameof(TestGarbageInputs)} E")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x02, 0x00 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_LEN, DisplayName = $"{nameof(TestGarbageInputs)} F")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x03, 0x00 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_LEN, DisplayName = $"{nameof(TestGarbageInputs)} G")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x04, 0x00 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_LEN, DisplayName = $"{nameof(TestGarbageInputs)} H")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x04, 0x9F, 0xFF, 0xFF, 0xFF }, false, TASDRawPacket.ERR_MSG_LEN_TOO_HIGH, DisplayName = $"{nameof(TestGarbageInputs)} I")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x05, 0x00 }, false, TASDRawPacket.ERR_MSG_LEN_TOO_HIGH, DisplayName = $"{nameof(TestGarbageInputs)} J")]
	[DataRow(new u8[] { 0xFF, 0x01, 0x01,  0x01 }, true, TASDRawPacket.ERR_MSG_TOO_SHORT_LEN, DisplayName = $"{nameof(TestGarbageInputs)} K")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x01, 0x01 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_PAYLOAD, DisplayName = $"{nameof(TestGarbageInputs)} L")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x02, 0x00, 0x01 }, false, TASDRawPacket.ERR_MSG_TOO_SHORT_PAYLOAD, DisplayName = $"{nameof(TestGarbageInputs)} M")]
	[TestMethod]
	public void TestGarbageInputs(u8[] buf, bool useLongKeyHeader, string ex)
		=> RawHeaderTests.AssertMessageContains(
			ex,
			CombinedParse(
				buf,
				useLongKeyHeader ? V1_Keys3o : TASDRawHeader.V1_Keys2o,
				out _,
				out _
			)
		);

	[DataRow(new u8[] { 0xFF, 0xFF,  0x01, 0x00 }, false, 0, 2, 4, 4, DisplayName = $"{nameof(TestParsing)} A")]
	[DataRow(new u8[] { 0xFF, 0xFF,  0x00 }, false, 0, 2, 3, 3, DisplayName = $"{nameof(TestParsing)} B")] // technically out-of-spec, but easy enough to support
	[DataRow(new u8[] { 0xFF, 0xFE,  0x01, 0x01,  0x01 }, false, 0, 2, 4, 5, DisplayName = $"{nameof(TestParsing)} C")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x02, 0x00, 0x00 }, false, 0, 2, 5, 5, DisplayName = $"{nameof(TestParsing)} D")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x03, 0x00, 0x00, 0x00 }, false, 0, 2, 6, 6, DisplayName = $"{nameof(TestParsing)} E")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x04, 0x00, 0x00, 0x00, 0x00 }, false, 0, 2, 7, 7, DisplayName = $"{nameof(TestParsing)} F")]
	[DataRow(new u8[] { 0xFF, 0x01,  0x02, 0x00, 0x01,  0x41 }, false, 0, 2, 5, 6, DisplayName = $"{nameof(TestParsing)} G")]
	[DataRow(new u8[] { 0xFF, 0x01, 0x02,  0x00 }, true, 0, 3, 4, 4, DisplayName = $"{nameof(TestParsing)} H")] // this is a 2-octet key packet interpreted as 3-octet, and ends up with PEXP = 0
	[DataRow(new u8[] { 0xFF, 0x01, 0x02,  0x00, 0x01 }, true, 0, 3, 4, 4, DisplayName = $"{nameof(TestParsing)} I")] // ditto, but with 1 octet of the next packet in the buffer
	[TestMethod]
	public void TestParsing(
		u8[] raw,
		bool useLongKeyHeader,
		int exKeyStart,
		int exKeyEndExcl,
		int exPayloadStart,
		int exPayloadEndExcl // == endOffset
	) {
		bbuf buf = raw;
		var acExc = CombinedParse(
			buf,
			useLongKeyHeader ? V1_Keys3o : TASDRawHeader.V1_Keys2o,
			out var parsed,
			out var endOffset
		);
		Assert.IsNull(acExc, $"failed to parse:\n{acExc}");
		Assert.AreEqual(exPayloadEndExcl, endOffset, "offset (length) doesn't match expected");
		var (acKey, acPayload) = parsed;
		Assert.AreEqual(exKeyEndExcl - exKeyStart, acKey.Length, "key wrong length?");
		AssertEqualOrBothEmpty(acKey, buf[exKeyStart..exKeyEndExcl]);
		Assert.AreEqual(exPayloadEndExcl - exPayloadStart, acPayload.Length, "payload wrong length");
		AssertEqualOrBothEmpty(acPayload, buf[exPayloadStart..exPayloadEndExcl]);
	}

	[TestMethod]
	public void TestToString() {
		Assert.AreEqual(
			"TASDRawPacket(key: 0xFF01, payload: 0x48_65_6C_6C_6F_2C_20_77_6F_72_6C_64_21)",
			new TASDRawPacket(stackalloc u8[] { 0xFF, 0x01 }, "Hello, world!"u8).ToString()
		);
		Assert.AreEqual(
			"TASDRawPacket(key: 0xFFFF, payload: empty)",
			new TASDRawPacket(stackalloc u8[] { 0xFF, 0xFF }, bbuf.Empty).ToString()
		);
		Assert.AreEqual(
			"TASDRawPacket(key: 0x00FFFE, payload: 0x01)",
			new TASDRawPacket(stackalloc u8[] { 0x00, 0xFF, 0xFE }, stackalloc u8[] { 0x01 }).ToString()
		);
	}

	[TestMethod]
	public void TestWriteTo() {
		rwbbuf dst = stackalloc u8[5];
		Assert.ThrowsException<IndexOutOfRangeException>(() => { // from Span indexer
			_ = TASDRawPacket.WriteTo(rwbbuf.Empty, key: stackalloc u8[] { 0xFF, 0x01 }, payload: stackalloc u8[] { 0x41 });
		}, "expecting static WriteTo to fail when destination too small");
		bbuf ex = stackalloc u8[] { 0xFF, 0x01, 0x01, 0x01, 0x41 };
		bbuf key = stackalloc u8[] { 0xFF, 0x01 };
		bbuf payload = stackalloc u8[] { 0x41 };
		RawHeaderTests.AssertSeqEqual(ex: ex, ac: TASDRawPacket.WriteTo(dst, key: key, payload: payload), "static WriteTo succeeded but wrote wrong values?");

		dst.Clear();
		Assert.IsFalse(TASDRawPacket.TryWriteTo(rwbbuf.Empty, key: key, payload: payload), "expecting static TryWriteTo to fail when destination too small");
		Assert.IsTrue(TASDRawPacket.TryWriteTo(dst, key: key, payload: payload), "expecting static TryWriteTo to succeed");
		RawHeaderTests.AssertSeqEqual(ex: ex, ac: dst, "static TryWriteTo succeeded but wrote wrong values?");

		dst.Clear();
		_ = Assert.ThrowsException<IndexOutOfRangeException>(() => { // from Span indexer
			new TASDRawPacket(key: stackalloc u8[] { 0xFF, 0x01 }, payload: stackalloc u8[] { 0x41 })
				.WriteTo(rwbbuf.Empty);
		}, "expecting instance WriteTo to fail when destination too small");
		TASDRawPacket src = new(key: key, payload: payload);
		src.WriteTo(dst);
		RawHeaderTests.AssertSeqEqual(ex: ex, ac: dst, "instance WriteTo succeeded but wrote wrong values?");

		dst.Clear();
		Assert.IsFalse(src.TryWriteTo(rwbbuf.Empty), "expecting instance TryWriteTo to fail when destination too small");
		Assert.IsTrue(src.TryWriteTo(dst), "expecting instance TryWriteTo to succeed");
		RawHeaderTests.AssertSeqEqual(ex: ex, ac: dst, "instance TryWriteTo succeeded but wrote wrong values?");

		// UGH need to try packets of every PEXP in order to get 100% coverage
	}
}
