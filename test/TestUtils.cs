namespace IO.TASD;

using System.IO;

public static class TestUtils {
	public static u8[] CopyToArrayAndDispose(this Stream stream) {
		var buf = new u8[(int) stream.Length];
		using MemoryStream ms = new(buf);
		stream.CopyTo(ms);
		stream.Dispose();
		return buf;
	}
}
