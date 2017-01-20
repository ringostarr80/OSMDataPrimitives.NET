using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Xml;

namespace NUnit
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void TestOSMNodeToXmlString()
		{
			var node = new OSMNode(2) {
				Latitude = 52.123456,
				Longitude = 12.654321,
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			node.Tags.Add("name", "bar");
			node.Tags.Add("ref", "baz");

			var expectedXmlString = "<node id=\"2\" lat=\"52.123456\" lon=\"12.654321\" version=\"3\" uid=\"5\" user=\"foo\" changeset=\"7\" timestamp=\"2017-01-20T12:03:43Z\">";
			expectedXmlString += "<tag k=\"name\" v=\"bar\" />";
			expectedXmlString += "<tag k=\"ref\" v=\"baz\" />";
			expectedXmlString += "</node>";
			Assert.AreEqual(expectedXmlString, node.ToXmlString());
		}
	}
}
