using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Xml;
using OSMDataPrimitives.BSON;

namespace NUnit
{
	[TestFixture]
	public class Test
	{
		private OSMNode GetDefaultOSMNode()
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

			return node;
		}
		[Test]
		public void TestOSMNodeToXmlString()
		{
			var node = this.GetDefaultOSMNode();

			var expectedXmlString = "<node id=\"2\" lat=\"52.123456\" lon=\"12.654321\" version=\"3\" uid=\"5\" user=\"foo\" changeset=\"7\" timestamp=\"2017-01-20T12:03:43Z\">";
			expectedXmlString += "<tag k=\"name\" v=\"bar\" />";
			expectedXmlString += "<tag k=\"ref\" v=\"baz\" />";
			expectedXmlString += "</node>";
			Assert.AreEqual(expectedXmlString, node.ToXmlString());
		}

		[Test]
		public void TestOSMNodeToBson()
		{
			var node = this.GetDefaultOSMNode();

			var bsonDoc = node.ToBson();
			var idElement = bsonDoc.GetElement("id");
			var locationElement = bsonDoc.GetElement("location");
			var latElement = locationElement.Value.AsBsonDocument.GetElement("lat");
			var lonElement = locationElement.Value.AsBsonDocument.GetElement("lon");
			var versionElement = bsonDoc.GetElement("version");
			var uidElement = bsonDoc.GetElement("uid");
			var userElement = bsonDoc.GetElement("user");
			var changesetElement = bsonDoc.GetElement("changeset");
			var timestampElement = bsonDoc.GetElement("timestamp");
			var tagsElement = bsonDoc.GetElement("tags");

			Assert.AreEqual(2, idElement.Value.AsInt64);
			Assert.AreEqual(52.123456, latElement.Value.AsDouble);
			Assert.AreEqual(12.654321, lonElement.Value.AsDouble);
			Assert.AreEqual(3, versionElement.Value.AsInt64);
			Assert.AreEqual(5, uidElement.Value.AsInt64);
			Assert.AreEqual("foo", userElement.Value.AsString);
			Assert.AreEqual(7, changesetElement.Value.AsInt64);
			Assert.AreEqual(new MongoDB.Bson.BsonDateTime(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)), timestampElement.Value.AsBsonDateTime);
			Assert.AreEqual(2, tagsElement.Value.AsBsonDocument.ElementCount);

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.AreEqual("bar", tagName.Value.AsString);
			Assert.AreEqual("baz", tagRef.Value.AsString);
		}
	}
}
