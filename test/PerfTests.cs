namespace Net.TASBot.TASDDotnet;

[TestClass]
public sealed class PerfTests {
	public const int BUTTON_COUNT = 32;

	public static u8[] SampleData
		=> Data.GetRawFromEmbeddedResource("4616M.tasd");

	public static void DoCalc(bbuf buf, Span<int> pressedCounts, Span<int> heldCounts) {
		_ = TASDRawPacketEnumeratorSafe.TryCreate(buf, out _, out var iter);
		var iter1 = iter.OfKey(TASDPacketKey.INPUT_CHUNK);
		Span<bool> wasHeld = stackalloc bool[BUTTON_COUNT];
		var isB = 16;
		while (iter1.MoveNext()) {
			var rawInputs = iter1.Current.Payload[1];
			for (var i = 7; i >= 0; i--) {
				var offset = isB + 7 - i; // this seems like reversing and then reversing back, but it was ~400 ms faster (when running this whole thing 1000x, so really not a huge problem) //TODO you know what might be faster than both? doing this in whatever arbitrary order, and then reordering at the end
				var isHeldN = (rawInputs >> i) & 1;
				heldCounts[offset] += isHeldN;
				var isHeld = isHeldN is 1;
				if (isHeld && !wasHeld[offset]) pressedCounts[offset]++;
				wasHeld[offset] = isHeld;
			}
			rawInputs = iter1.Current.Payload[0];
			for (var i = 7; i >= 0; i--) {
				var offset = isB + 15 - i;
				var isHeldN = (rawInputs >> i) & 1;
				heldCounts[offset] += isHeldN;
				var isHeld = isHeldN is 1;
				if (isHeld && !wasHeld[offset]) pressedCounts[offset]++;
				wasHeld[offset] = isHeld;
			}
			isB = 16 - isB;
		}
	}

	[TestMethod]
	public void TestPerfTestCorrectness() {
		Span<int> pressedCounts = stackalloc int[BUTTON_COUNT];
		Span<int> heldCounts = stackalloc int[BUTTON_COUNT];
		DoCalc(SampleData, pressedCounts: pressedCounts, heldCounts: heldCounts);
		// should have someone else run this with a different impl., or manually count even
		// maybe pick a more interesting sample file first
		Assert.IsTrue(pressedCounts.SequenceEqual(stackalloc[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 411, 418, 47, 357, 1, 54, 259, 298, 0, 0, 0, 0, 0, 0, 0, 1 }));
		Assert.IsTrue(heldCounts.SequenceEqual(stackalloc[] { 33200, 33200, 33200, 33200, 33200, 33200, 33200, 33200, 0, 0, 0, 0, 0, 0, 33200, 0, 21353, 32782, 33154, 32379, 33200, 33123, 26263, 12552, 0, 0, 0, 0, 0, 0, 0, 33200 }));
	}
}
