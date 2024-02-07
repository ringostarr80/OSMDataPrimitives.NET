using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Xml;
using OSMDataPrimitives.PostgreSQL;
using OSMDataPrimitives.BSON;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace NUnit
{
	[TestFixture]
	public class TestOSMNode
	{
		private static OsmNode GetDefaultOSMNode()
		{
			var node = new OsmNode(2) {
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
			var node = GetDefaultOSMNode();
			var nodeClone = (OsmNode)node.Clone();
			nodeClone.Changeset += 1;
			nodeClone.Version += 1;
			nodeClone.Latitude += 2.341;
			nodeClone.Longitude -= 1.754325;
			nodeClone.UserId = 2;
			nodeClone.UserName = "bar";
			nodeClone.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			nodeClone.Tags["name"] = "hello";
			nodeClone.Tags["ref"] = "world";

			Assert.That(node.Changeset, Is.EqualTo(7));
			Assert.That(nodeClone.Changeset, Is.EqualTo(8));
			Assert.That(node.Version, Is.EqualTo(3));
			Assert.That(nodeClone.Version, Is.EqualTo(4));
			Assert.That(node.Latitude, Is.EqualTo(52.123456));
			Assert.That(nodeClone.Latitude, Is.EqualTo(54.464456));
			Assert.That(node.Longitude, Is.EqualTo(12.654321));
			Assert.That(nodeClone.Longitude, Is.EqualTo(10.899996));
			Assert.That(node.UserId, Is.EqualTo(5));
			Assert.That(nodeClone.UserId, Is.EqualTo(2));
			Assert.That(node.UserName, Is.EqualTo("foo"));
			Assert.That(nodeClone.UserName, Is.EqualTo("bar"));
			Assert.That(node.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(nodeClone.Timestamp, Is.EqualTo(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc)));
			Assert.That(node.Tags["name"], Is.EqualTo("bar"));
			Assert.That(nodeClone.Tags["name"], Is.EqualTo("hello"));
			Assert.That(node.Tags["ref"], Is.EqualTo("baz"));
			Assert.That(nodeClone.Tags["ref"], Is.EqualTo("world"));
		}

		[Test]
		public void TestOSMNodeToXmlString()
		{
			var node = GetDefaultOSMNode();

			var expectedXmlString = "<node id=\"2\" lat=\"52.123456\" lon=\"12.654321\" version=\"3\" uid=\"5\" user=\"foo\" changeset=\"7\" timestamp=\"2017-01-20T12:03:43Z\">";
			expectedXmlString += "<tag k=\"name\" v=\"bar\" />";
			expectedXmlString += "<tag k=\"ref\" v=\"baz\" />";
			expectedXmlString += "</node>";
			Assert.That(node.ToXmlString(), Is.EqualTo(expectedXmlString));
		}

		[Test]
		public void TestXmlElementToOSMNode()
		{
			var node = GetDefaultOSMNode();
			var xmlNode = node.ToXml();
			var convertedNode = xmlNode.ToOSMElement();

			Assert.That(convertedNode.Id, Is.EqualTo(2));
			Assert.That(convertedNode.Changeset, Is.EqualTo(7));
			Assert.That(convertedNode.Version, Is.EqualTo(3));
			Assert.That(((OsmNode)convertedNode).Latitude, Is.EqualTo(52.123456));
			Assert.That(((OsmNode)convertedNode).Longitude, Is.EqualTo(12.654321));
			Assert.That(convertedNode.UserId, Is.EqualTo(5));
			Assert.That(convertedNode.UserName, Is.EqualTo("foo"));
			Assert.That(convertedNode.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(convertedNode.Tags["name"], Is.EqualTo("bar"));
			Assert.That(convertedNode.Tags["ref"], Is.EqualTo("baz"));
		}

		[Test]
		public void TestXmlStringToOSMNode()
		{
			var node = GetDefaultOSMNode();
			var xmlString = node.ToXmlString();
			var convertedNode = xmlString.ToOSMElement();

			Assert.That(convertedNode.Id, Is.EqualTo(2));
			Assert.That(convertedNode.Changeset, Is.EqualTo(7));
			Assert.That(convertedNode.Version, Is.EqualTo(3));
			Assert.That(((OsmNode)convertedNode).Latitude, Is.EqualTo(52.123456));
			Assert.That(((OsmNode)convertedNode).Longitude, Is.EqualTo(12.654321));
			Assert.That(convertedNode.UserId, Is.EqualTo(5));
			Assert.That(convertedNode.UserName, Is.EqualTo("foo"));
			Assert.That(convertedNode.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(convertedNode.Tags["name"], Is.EqualTo("bar"));
			Assert.That(convertedNode.Tags["ref"], Is.EqualTo("baz"));
		}

		[Test]
		public void TestOSMNodeToPostgreSQLInsertString()
		{
			var node = GetDefaultOSMNode();
			var sqlInsert = node.ToPostgreSQLInsert(out NameValueCollection sqlParameters, true);
			var expectedSql = "INSERT INTO nodes (osm_id, version, changeset, uid, user, timestamp, lat, lon, tags) ";
			expectedSql += "VALUES(@osm_id::bigint, @version::bigint, @changeset::bigint, @uid::bigint, @user, TIMESTAMP @timestamp, @lat::double precision, @lon::double precision, @tags::hstore)";
			Assert.That(sqlInsert, Is.EqualTo(expectedSql));
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"version", "3"},
				{"changeset", "7"},
				{"uid", "5"},
				{"user", "foo"},
				{"timestamp", "2017-01-20 12:03:43"},
				{"lat", "52.123456"},
				{"lon", "12.654321"},
				{"tags", "\"name\"=>\"bar\", \"ref\"=>\"baz\""}
			};
			Assert.That(sqlParameters.Count, Is.EqualTo(expectedSqlParameters.Count));
			foreach (string key in expectedSqlParameters) {
				Assert.That(sqlParameters[key], !Is.Null);
				Assert.That(sqlParameters[key], Is.EqualTo(expectedSqlParameters[key]));
			}
		}

		[Test]
		public void TestOSMNodeToPostgreSQLInsertWithoutTagsString()
		{
			var node = GetDefaultOSMNode();
			node.Tags.Clear();
			var sqlInsert = node.ToPostgreSQLInsert(out NameValueCollection sqlParameters, true);
			var expectedSql = "INSERT INTO nodes (osm_id, version, changeset, uid, user, timestamp, lat, lon, tags) ";
			expectedSql += "VALUES(@osm_id::bigint, @version::bigint, @changeset::bigint, @uid::bigint, @user, TIMESTAMP @timestamp, @lat::double precision, @lon::double precision, @tags::hstore)";
			Assert.That(sqlInsert, Is.EqualTo(expectedSql));
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"version", "3"},
				{"changeset", "7"},
				{"uid", "5"},
				{"user", "foo"},
				{"timestamp", "2017-01-20 12:03:43"},
				{"lat", "52.123456"},
				{"lon", "12.654321"},
				{"tags", ""}
			};
			Assert.That(sqlParameters.Count, Is.EqualTo(expectedSqlParameters.Count));
			foreach (string key in expectedSqlParameters) {
				Assert.That(sqlParameters[key], !Is.Null);
				Assert.That(sqlParameters[key], Is.EqualTo(expectedSqlParameters[key]));
			}
		}

		[Test]
		public void TestOSMNodeToPostgreSQLSelectString()
		{
			var node = GetDefaultOSMNode();
			var sqlSelect = node.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, lat, lon, tags::text FROM nodes WHERE osm_id = 2";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));

			sqlSelect = node.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, lat, lon, tags::text FROM nodes WHERE osm_id = 2";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));

			node.OverrideId(5);
			sqlSelect = node.ToPostgreSQLSelect();
			expectedSql = "SELECT osm_id, lat, lon, tags::text FROM nodes WHERE osm_id = 5";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMNodeToPostgreSQLDeleteString()
		{
			var node = GetDefaultOSMNode();
			var expectedSql = "DELETE FROM nodes WHERE osm_id = 2";
			Assert.That(node.ToPostgreSQLDelete(), Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMNodeToPostgreSQLDeleteWithCustomTableString()
		{
			var node = GetDefaultOSMNode();
			var expectedSql = "DELETE FROM foo_nodes WHERE osm_id = 2";
			Assert.That(node.ToPostgreSQLDelete("foo_nodes"), Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMNodeToBson()
		{
			var node = GetDefaultOSMNode();

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

			Assert.That(idElement.Value.AsInt64, Is.EqualTo(2));
			Assert.That(latElement.Value.AsDouble, Is.EqualTo(52.123456));
			Assert.That(lonElement.Value.AsDouble, Is.EqualTo(12.654321));
			Assert.That(versionElement.Value.AsInt64, Is.EqualTo(3));
			Assert.That(uidElement.Value.AsInt64, Is.EqualTo(5));
			Assert.That(userElement.Value.AsString, Is.EqualTo("foo"));
			Assert.That(changesetElement.Value.AsInt64, Is.EqualTo(7));
			Assert.That(timestampElement.Value.AsBsonDateTime, Is.EqualTo(new MongoDB.Bson.BsonDateTime(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc))));
			Assert.That(tagsElement.Value.AsBsonDocument.ElementCount, Is.EqualTo(2));

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.That(tagName.Value.AsString, Is.EqualTo("bar"));
			Assert.That(tagRef.Value.AsString, Is.EqualTo("baz"));
		}

		[Test]
		public void TestOSMNodeParseBsonDocument()
		{
			var node = GetDefaultOSMNode();
			var bsonDoc = node.ToBson();

			var parsedNode = new OsmNode(0);
			parsedNode.ParseBsonDocument(bsonDoc);

			Assert.That(parsedNode.Id, Is.EqualTo(node.Id));
			Assert.That(parsedNode.Latitude, Is.EqualTo(node.Latitude));
			Assert.That(parsedNode.Longitude, Is.EqualTo(node.Longitude));
			Assert.That(parsedNode.UserId, Is.EqualTo(node.UserId));
			Assert.That(parsedNode.UserName, Is.EqualTo(node.UserName));
			Assert.That(parsedNode.Version, Is.EqualTo(node.Version));
			Assert.That(parsedNode.Changeset, Is.EqualTo(node.Changeset));
			Assert.That(parsedNode.Timestamp, Is.EqualTo(node.Timestamp));
			Assert.That(parsedNode.Tags.Count, Is.EqualTo(node.Tags.Count));

			foreach (KeyValuePair<string, string> tag in node.Tags) {
				Assert.That(parsedNode.Tags[tag.Key], Is.EqualTo(tag.Value));
			}
		}

		[Test]
		public void TestOSMNodeParseEmptyBsonDocument()
		{
			var node = GetDefaultOSMNode();
			var bsonDoc = new MongoDB.Bson.BsonDocument();

			node.ParseBsonDocument(bsonDoc);

			Assert.That(node.Id, Is.Zero);
			Assert.That(node.Latitude, Is.Zero);
			Assert.That(node.Longitude, Is.Zero);
			Assert.That(node.UserId, Is.Zero);
			Assert.That(node.UserName, Is.Empty);
			Assert.That(node.Version, Is.Zero);
			Assert.That(node.Changeset, Is.Zero);
			Assert.That(node.Timestamp, Is.EqualTo(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
			Assert.That(node.Tags.Count, Is.Zero);
		}
	}
}
