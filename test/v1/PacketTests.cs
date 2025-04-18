namespace IO.TASD.V1;

[TestClass]
public sealed class PacketTests {
	[TestMethod]
	public void TestStronglyTypedPackets() {
		var file = TASDFile.ParseHeaderAndAllPackets(Data.GetRawFromEmbeddedResource("sample.tasd"));
		Assert.AreEqual(25, file.AllPackets.Count);
	}
}
