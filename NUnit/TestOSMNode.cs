using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Xml;
using OSMDataPrimitives.PostgreSQL;
using OSMDataPrimitives.BSON;
using System.Collections.Specialized;

namespace NUnit
{
	[TestFixture]
	public class TestOSMNode
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
		public void TestOSMNodeClone()
		{
			var node = this.GetDefaultOSMNode();
			var nodeClone = (OSMNode)node.Clone();
			nodeClone.Changeset += 1;
			nodeClone.Version += 1;
			nodeClone.Latitude += 2.341;
			nodeClone.Longitude -= 1.754325;
			nodeClone.UserId = 2;
			nodeClone.UserName = "bar";
			nodeClone.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			nodeClone.Tags["name"] = "hello";
			nodeClone.Tags["ref"] = "world";

			Assert.AreEqual(7, node.Changeset);
			Assert.AreEqual(8, nodeClone.Changeset);
			Assert.AreEqual(3, node.Version);
			Assert.AreEqual(4, nodeClone.Version);
			Assert.AreEqual(52.123456, node.Latitude);
			Assert.AreEqual(54.464456, nodeClone.Latitude);
			Assert.AreEqual(12.654321, node.Longitude);
			Assert.AreEqual(10.899996, nodeClone.Longitude);
			Assert.AreEqual(5, node.UserId);
			Assert.AreEqual(2, nodeClone.UserId);
			Assert.AreEqual("foo", node.UserName);
			Assert.AreEqual("bar", nodeClone.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), node.Timestamp);
			Assert.AreEqual(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc), nodeClone.Timestamp);
			Assert.AreEqual("bar", node.Tags["name"]);
			Assert.AreEqual("hello", nodeClone.Tags["name"]);
			Assert.AreEqual("baz", node.Tags["ref"]);
			Assert.AreEqual("world", nodeClone.Tags["ref"]);
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
		public void TestXmlElementToOSMNode()
		{
			var node = this.GetDefaultOSMNode();
			var xmlNode = node.ToXml();
			var convertedNode = xmlNode.ToOSMElement();

			Assert.AreEqual(2, convertedNode.Id);
			Assert.AreEqual(7, convertedNode.Changeset);
			Assert.AreEqual(3, convertedNode.Version);
			Assert.AreEqual(52.123456, ((OSMNode)convertedNode).Latitude);
			Assert.AreEqual(12.654321, ((OSMNode)convertedNode).Longitude);
			Assert.AreEqual(5, convertedNode.UserId);
			Assert.AreEqual("foo", convertedNode.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), convertedNode.Timestamp);
			Assert.AreEqual("bar", convertedNode.Tags["name"]);
			Assert.AreEqual("baz", convertedNode.Tags["ref"]);
		}

		[Test]
		public void TestXmlStringToOSMNode()
		{
			var node = this.GetDefaultOSMNode();
			var xmlString = node.ToXmlString();
			var convertedNode = xmlString.ToOSMElement();

			Assert.AreEqual(2, convertedNode.Id);
			Assert.AreEqual(7, convertedNode.Changeset);
			Assert.AreEqual(3, convertedNode.Version);
			Assert.AreEqual(52.123456, ((OSMNode)convertedNode).Latitude);
			Assert.AreEqual(12.654321, ((OSMNode)convertedNode).Longitude);
			Assert.AreEqual(5, convertedNode.UserId);
			Assert.AreEqual("foo", convertedNode.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), convertedNode.Timestamp);
			Assert.AreEqual("bar", convertedNode.Tags["name"]);
			Assert.AreEqual("baz", convertedNode.Tags["ref"]);
		}

		[Test]
		public void TestOSMNodeToPostgreSQLInsertString()
		{
			var node = this.GetDefaultOSMNode();
			NameValueCollection sqlParameters;
			var sqlInsert = node.ToPostgreSQLInsert(out sqlParameters);
			var expectedSql = "INSERT INTO nodes (osm_id, lat, lon, tags) ";
			expectedSql += "VALUES(@osm_id::bigint, @lat::double precision, @lon::double precision, @tags::hstore)";
			Assert.AreEqual(expectedSql, sqlInsert);
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"lat", "52.123456"},
				{"lon", "12.654321"},
				{"tags", "\"name\"=>\"bar\", \"ref\"=>\"baz\""}
			};
			Assert.AreEqual(expectedSqlParameters.Count, sqlParameters.Count);
			foreach(string key in expectedSqlParameters) {
				Assert.NotNull(sqlParameters[key]);
				Assert.AreEqual(expectedSqlParameters[key], sqlParameters[key]);
			}
		}

		[Test]
		public void TestOSMNodeToPostgreSQLSelectString()
		{
			var node = this.GetDefaultOSMNode();
			var sqlSelect = node.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, lat, lon, tags::text FROM nodes WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, sqlSelect);

			sqlSelect = node.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, lat, lon, tags::text FROM nodes WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, sqlSelect);

			node.OverrideId(5);
			sqlSelect = node.ToPostgreSQLSelect();
			expectedSql = "SELECT osm_id, lat, lon, tags::text FROM nodes WHERE osm_id = 5";
			Assert.AreEqual(expectedSql, sqlSelect);
		}

		[Test]
		public void TestOSMNodeToPostgreSQLDeleteString()
		{
			var node = this.GetDefaultOSMNode();
			var expectedSql = "DELETE FROM nodes WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, node.ToPostgreSQLDelete());
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
