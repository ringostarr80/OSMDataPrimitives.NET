using System;
using System.Xml;

using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestSpatialExtension
	{
		[Test]
		public void TestSpatialToOSMSpatialElement()
		{
            var xml = new XmlDocument();
			var xmlNode = xml.CreateElement("node");
			xmlNode.SetAttribute("id", "1");
			xmlNode.SetAttribute("changeset", "2");
			xmlNode.SetAttribute("version", "3");
			xmlNode.SetAttribute("uid", "4");
			xmlNode.SetAttribute("user", "unknown");
			xmlNode.SetAttribute("timestamp", "2023-06-01T12:00:00");
			xmlNode.SetAttribute("lat", "52.1234");
			xmlNode.SetAttribute("lon", "12.4321");
			var xmlTag = xml.CreateElement("tag");
			xmlTag.SetAttribute("k", "building");
			xmlTag.SetAttribute("v", "house");
			xmlNode.AppendChild(xmlTag);
			xml.AppendChild(xmlNode);

			var spatialElement = xmlNode.ToOSMSpatialElement();
			if (spatialElement is OSMNode osmNode) {
				Assert.AreEqual(1, osmNode.Id);
				Assert.AreEqual(2, osmNode.Changeset);
				Assert.AreEqual(3, osmNode.Version);
				Assert.AreEqual(4, osmNode.UserId);
				Assert.AreEqual("unknown", osmNode.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmNode.Timestamp);
				Assert.AreEqual(52.1234, osmNode.Latitude);
				Assert.AreEqual(12.4321, osmNode.Longitude);
				Assert.AreEqual(1, osmNode.Tags.Count);
				Assert.AreEqual("house", osmNode.Tags["building"]);
			}
        }
    }
}
