namespace IO.TASD;

using System.Runtime.CompilerServices;
using System.Text;

using static System.Globalization.CultureInfo;

internal static class Extensions {
	public static StringBuilder AppendOctetsAsHex(this StringBuilder sb, bbuf buf, string? separator = null) {
		if (2 <= buf.Length) {
			sb.AppendFormat(InvariantCulture, "0x{0:X2}", buf[0]);
			var fmtStr = separator + "{0:X2}";
			for (var i = 1; i < buf.Length; i++) sb.AppendFormat(InvariantCulture, fmtStr, buf[i]);
		} else if (buf.Length is 1) {
			sb.AppendFormat(InvariantCulture, "0x{0:X2}", buf[0]);
		} else {
			sb.Append("empty");
		}
		return sb;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static u8 As01U8(this bool b)
		=> unchecked(b ? (u8) 1 : (u8) 0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool BoolFrom01(this u8 b)
		=> b is 1;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool FastSequenceEqual(this bbuf left, bbuf right)
		=> left == right || left.SequenceEqual(right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static u32 ReadU24BE(this bbuf buf, int offsetLessOne)
		=> buf.ReadU32BE(offsetLessOne) & 0xFFFFFFU;
}
