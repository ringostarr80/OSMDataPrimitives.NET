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
	public class TestOSMWay
	{
		private static OsmWay GetDefaultOSMWay()
		{
			var way = new OsmWay(2) {
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			way.NodeRefs.Add(5);
			way.NodeRefs.Add(9);
			way.NodeRefs.Add(12);
			way.NodeRefs.Add(543);
			way.NodeRefs.Add(43);
			way.NodeRefs.Add(1234151);
			way.Tags.Add("name", "this road");
			way.Tags.Add("ref", "A1");

			return way;
		}

		[Test]
		public void TestOSMWayClone()
		{
			var way = GetDefaultOSMWay();
			var wayClone = (OsmWay)way.Clone();
			wayClone.Changeset += 1;
			wayClone.Version += 1;
			wayClone.UserId = 2;
			wayClone.UserName = "bar";
			wayClone.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			wayClone.Tags["name"] = "that street";
			wayClone.Tags["ref"] = "B2";
			wayClone.NodeRefs.Clear();
			wayClone.NodeRefs.Add(15);
			wayClone.NodeRefs.Add(19);
			wayClone.NodeRefs.Add(112);
			wayClone.NodeRefs.Add(1543);
			wayClone.NodeRefs.Add(143);
			wayClone.NodeRefs.Add(11234151);

			Assert.That(way.Changeset, Is.EqualTo(7));
			Assert.That(wayClone.Changeset, Is.EqualTo(8));
			Assert.That(way.Version, Is.EqualTo(3));
			Assert.That(wayClone.Version, Is.EqualTo(4));
			Assert.That(way.UserId, Is.EqualTo(5));
			Assert.That(wayClone.UserId, Is.EqualTo(2));
			Assert.That(way.UserName, Is.EqualTo("foo"));
			Assert.That(wayClone.UserName, Is.EqualTo("bar"));
			Assert.That(way.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(wayClone.Timestamp, Is.EqualTo(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc)));
			Assert.That(way.Tags["name"], Is.EqualTo("this road"));
			Assert.That(wayClone.Tags["name"], Is.EqualTo("that street"));
			Assert.That(way.Tags["ref"], Is.EqualTo("A1"));
			Assert.That(wayClone.Tags["ref"], Is.EqualTo("B2"));

			Assert.That(way.NodeRefs[0], Is.EqualTo(5));
			Assert.That(way.NodeRefs[1], Is.EqualTo(9));
			Assert.That(way.NodeRefs[2], Is.EqualTo(12));
			Assert.That(way.NodeRefs[3], Is.EqualTo(543));
			Assert.That(way.NodeRefs[4], Is.EqualTo(43));
			Assert.That(way.NodeRefs[5], Is.EqualTo(1234151));

			Assert.That(wayClone.NodeRefs[0], Is.EqualTo(15));
			Assert.That(wayClone.NodeRefs[1], Is.EqualTo(19));
			Assert.That(wayClone.NodeRefs[2], Is.EqualTo(112));
			Assert.That(wayClone.NodeRefs[3], Is.EqualTo(1543));
			Assert.That(wayClone.NodeRefs[4], Is.EqualTo(143));
			Assert.That(wayClone.NodeRefs[5], Is.EqualTo(11234151));
		}

		[Test]
		public void TestOSMWayToXmlString()
		{
			var way = GetDefaultOSMWay();

			var expectedXmlString = "<way id=\"2\" version=\"3\" uid=\"5\" user=\"foo\" changeset=\"7\" timestamp=\"2017-01-20T12:03:43Z\">";
			expectedXmlString += "<nd ref=\"5\" />";
			expectedXmlString += "<nd ref=\"9\" />";
			expectedXmlString += "<nd ref=\"12\" />";
			expectedXmlString += "<nd ref=\"543\" />";
			expectedXmlString += "<nd ref=\"43\" />";
			expectedXmlString += "<nd ref=\"1234151\" />";
			expectedXmlString += "<tag k=\"name\" v=\"this road\" />";
			expectedXmlString += "<tag k=\"ref\" v=\"A1\" />";
			expectedXmlString += "</way>";
			Assert.That(way.ToXmlString(), Is.EqualTo(expectedXmlString));
		}

		[Test]
		public void TestXmlElementToOSMWay()
		{
			var way = GetDefaultOSMWay();
			var xmlWay = way.ToXml();
			var convertedWay = (OsmWay)xmlWay.ToOSMElement();

			Assert.That(convertedWay.Id, Is.EqualTo(2));
			Assert.That(convertedWay.Changeset, Is.EqualTo(7));
			Assert.That(convertedWay.Version, Is.EqualTo(3));
			Assert.That(convertedWay.UserId, Is.EqualTo(5));
			Assert.That(convertedWay.UserName, Is.EqualTo("foo"));
			Assert.That(convertedWay.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(convertedWay.Tags["name"], Is.EqualTo("this road"));
			Assert.That(convertedWay.Tags["ref"], Is.EqualTo("A1"));
			Assert.That(convertedWay.NodeRefs.Count, Is.EqualTo(6));
			Assert.That(convertedWay.NodeRefs[0], Is.EqualTo(5));
			Assert.That(convertedWay.NodeRefs[1], Is.EqualTo(9));
			Assert.That(convertedWay.NodeRefs[2], Is.EqualTo(12));
			Assert.That(convertedWay.NodeRefs[3], Is.EqualTo(543));
			Assert.That(convertedWay.NodeRefs[4], Is.EqualTo(43));
			Assert.That(convertedWay.NodeRefs[5], Is.EqualTo(1234151));
		}

		[Test]
		public void TestXmlStringToOSMWay()
		{
			var way = GetDefaultOSMWay();
			var xmlString = way.ToXmlString();
			var convertedWay = (OsmWay)xmlString.ToOSMElement();

			Assert.That(convertedWay.Id, Is.EqualTo(2));
			Assert.That(convertedWay.Changeset, Is.EqualTo(7));
			Assert.That(convertedWay.Version, Is.EqualTo(3));
			Assert.That(convertedWay.UserId, Is.EqualTo(5));
			Assert.That(convertedWay.UserName, Is.EqualTo("foo"));
			Assert.That(convertedWay.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(convertedWay.Tags["name"], Is.EqualTo("this road"));
			Assert.That(convertedWay.Tags["ref"], Is.EqualTo("A1"));
			Assert.That(convertedWay.NodeRefs.Count, Is.EqualTo(6));
			Assert.That(convertedWay.NodeRefs[0], Is.EqualTo(5));
			Assert.That(convertedWay.NodeRefs[1], Is.EqualTo(9));
			Assert.That(convertedWay.NodeRefs[2], Is.EqualTo(12));
			Assert.That(convertedWay.NodeRefs[3], Is.EqualTo(543));
			Assert.That(convertedWay.NodeRefs[4], Is.EqualTo(43));
			Assert.That(convertedWay.NodeRefs[5], Is.EqualTo(1234151));
		}

		[Test]
		public void TestOSMWayToPostgreSQLInsertString()
		{
			var way = GetDefaultOSMWay();
			var sqlInsert = way.ToPostgreSQLInsert(out NameValueCollection sqlParameters);
			var expectedSql = "INSERT INTO ways (osm_id, tags, node_refs) ";
			expectedSql += "VALUES(@osm_id::bigint, @tags::hstore, @node_refs::bigint[])";
			Assert.That(sqlInsert, Is.EqualTo(expectedSql));
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"tags", "\"name\"=>\"this road\", \"ref\"=>\"A1\""},
				{"node_refs", "{5, 9, 12, 543, 43, 1234151}"}
			};
			Assert.That(sqlParameters.Count, Is.EqualTo(expectedSqlParameters.Count));
			foreach (string key in expectedSqlParameters) {
				Assert.That(sqlParameters[key], !Is.Null);
				Assert.That(sqlParameters[key], Is.EqualTo(expectedSqlParameters[key]));
			}
		}

		[Test]
		public void TestOSMWayToPostgreSQLSelectString()
		{
			var way = GetDefaultOSMWay();
			var sqlSelect = way.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, tags::text, node_refs::text FROM ways WHERE osm_id = 2";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));

			sqlSelect = way.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, tags::text, node_refs::text FROM ways WHERE osm_id = 2";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));

			way.OverrideId(6);
			sqlSelect = way.ToPostgreSQLSelect();
			expectedSql = "SELECT osm_id, tags::text, node_refs::text FROM ways WHERE osm_id = 6";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMWayToPostgreSQLDeleteString()
		{
			var way = GetDefaultOSMWay();
			var expectedSql = "DELETE FROM ways WHERE osm_id = 2";
			Assert.That(way.ToPostgreSQLDelete(), Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMWayToBson()
		{
			var way = GetDefaultOSMWay();

			var bsonDoc = way.ToBson();
			var idElement = bsonDoc.GetElement("id");
			var versionElement = bsonDoc.GetElement("version");
			var uidElement = bsonDoc.GetElement("uid");
			var userElement = bsonDoc.GetElement("user");
			var changesetElement = bsonDoc.GetElement("changeset");
			var timestampElement = bsonDoc.GetElement("timestamp");
			var tagsElement = bsonDoc.GetElement("tags");
			var nodeRefsElement = bsonDoc.GetElement("node_refs");

			Assert.That(idElement.Value.AsInt64, Is.EqualTo(2));
			Assert.That(versionElement.Value.AsInt64, Is.EqualTo(3));
			Assert.That(uidElement.Value.AsInt64, Is.EqualTo(5));
			Assert.That(userElement.Value.AsString, Is.EqualTo("foo"));
			Assert.That(changesetElement.Value.AsInt64, Is.EqualTo(7));
			Assert.That(timestampElement.Value.AsBsonDateTime, Is.EqualTo(new MongoDB.Bson.BsonDateTime(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc))));
			Assert.That(tagsElement.Value.AsBsonDocument.ElementCount, Is.EqualTo(2));
			Assert.That(nodeRefsElement.Value.AsBsonArray.Count, Is.EqualTo(6));

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.That(tagName.Value.AsString, Is.EqualTo("this road"));
			Assert.That(tagRef.Value.AsString, Is.EqualTo("A1"));

			var nodeRefsBsonArray = nodeRefsElement.Value.AsBsonArray;
			Assert.That(nodeRefsBsonArray[0].AsInt64, Is.EqualTo(5));
			Assert.That(nodeRefsBsonArray[1].AsInt64, Is.EqualTo(9));
			Assert.That(nodeRefsBsonArray[2].AsInt64, Is.EqualTo(12));
			Assert.That(nodeRefsBsonArray[3].AsInt64, Is.EqualTo(543));
			Assert.That(nodeRefsBsonArray[4].AsInt64, Is.EqualTo(43));
			Assert.That(nodeRefsBsonArray[5].AsInt64, Is.EqualTo(1234151));
		}

		[Test]
		public void TestOSMWayParseBsonDocument()
		{
			var way = GetDefaultOSMWay();
			var bsonDoc = way.ToBson();

			var parsedWay = new OsmWay(0);
			parsedWay.ParseBsonDocument(bsonDoc);

			Assert.That(parsedWay.Id, Is.EqualTo(way.Id));
			Assert.That(parsedWay.UserId, Is.EqualTo(way.UserId));
			Assert.That(parsedWay.UserName, Is.EqualTo(way.UserName));
			Assert.That(parsedWay.Version, Is.EqualTo(way.Version));
			Assert.That(parsedWay.Changeset, Is.EqualTo(way.Changeset));
			Assert.That(parsedWay.Timestamp, Is.EqualTo(way.Timestamp));
			Assert.That(parsedWay.Tags.Count, Is.EqualTo(way.Tags.Count));
			Assert.That(parsedWay.NodeRefs.Count, Is.EqualTo(way.NodeRefs.Count));

			foreach (KeyValuePair<string, string> tag in way.Tags) {
				Assert.That(parsedWay.Tags[tag.Key], Is.EqualTo(tag.Value));
			}

			for (var i = 0; i < way.NodeRefs.Count; i++) {
				Assert.That(parsedWay.NodeRefs[i], Is.EqualTo(way.NodeRefs[i]));
			}
		}

		[Test]
		public void TestOSMWayParseEmptyBsonDocument()
		{
			var way = GetDefaultOSMWay();
			var bsonDoc = new MongoDB.Bson.BsonDocument();

			way.ParseBsonDocument(bsonDoc);

			Assert.That(way.Id, Is.Zero);
			Assert.That(way.UserId, Is.Zero);
			Assert.That(way.UserName, Is.Empty);
			Assert.That(way.Version, Is.Zero);
			Assert.That(way.Changeset, Is.Zero);
			Assert.That(way.Timestamp, Is.EqualTo(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
			Assert.That(way.Tags.Count, Is.Zero);
			Assert.That(way.NodeRefs.Count, Is.Zero);
		}
	}
}
