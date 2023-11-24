namespace Net.TASBot.TASDDotnet;

[TestClass]
public sealed class PacketTests {
	[TestMethod]
	public void TestStronglyTypedPackets() {
		var file = TASDFile.ParseHeaderAndAllPackets(Data.GetRawFromEmbeddedResource("sample.tasd"));
		Assert.AreEqual(21, file.AllPackets.Count);
	}
}
