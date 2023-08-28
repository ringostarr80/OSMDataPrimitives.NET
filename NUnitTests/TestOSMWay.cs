﻿using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Xml;
using OSMDataPrimitives.PostgreSQL;
using OSMDataPrimitives.BSON;
using System.Collections.Specialized;

namespace NUnit
{
	[TestFixture]
	public class TestOSMWay
	{
		private static OSMWay GetDefaultOSMWay()
		{
			var way = new OSMWay(2) {
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
			var wayClone = (OSMWay)way.Clone();
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

			Assert.AreEqual(7, way.Changeset);
			Assert.AreEqual(8, wayClone.Changeset);
			Assert.AreEqual(3, way.Version);
			Assert.AreEqual(4, wayClone.Version);
			Assert.AreEqual(5, way.UserId);
			Assert.AreEqual(2, wayClone.UserId);
			Assert.AreEqual("foo", way.UserName);
			Assert.AreEqual("bar", wayClone.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), way.Timestamp);
			Assert.AreEqual(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc), wayClone.Timestamp);
			Assert.AreEqual("this road", way.Tags["name"]);
			Assert.AreEqual("that street", wayClone.Tags["name"]);
			Assert.AreEqual("A1", way.Tags["ref"]);
			Assert.AreEqual("B2", wayClone.Tags["ref"]);

			Assert.AreEqual(5, way.NodeRefs[0]);
			Assert.AreEqual(9, way.NodeRefs[1]);
			Assert.AreEqual(12, way.NodeRefs[2]);
			Assert.AreEqual(543, way.NodeRefs[3]);
			Assert.AreEqual(43, way.NodeRefs[4]);
			Assert.AreEqual(1234151, way.NodeRefs[5]);

			Assert.AreEqual(15, wayClone.NodeRefs[0]);
			Assert.AreEqual(19, wayClone.NodeRefs[1]);
			Assert.AreEqual(112, wayClone.NodeRefs[2]);
			Assert.AreEqual(1543, wayClone.NodeRefs[3]);
			Assert.AreEqual(143, wayClone.NodeRefs[4]);
			Assert.AreEqual(11234151, wayClone.NodeRefs[5]);
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
			Assert.AreEqual(expectedXmlString, way.ToXmlString());
		}

		[Test]
		public void TestXmlElementToOSMWay()
		{
			var way = GetDefaultOSMWay();
			var xmlWay = way.ToXml();
			var convertedWay = (OSMWay)xmlWay.ToOSMElement();

			Assert.AreEqual(2, convertedWay.Id);
			Assert.AreEqual(7, convertedWay.Changeset);
			Assert.AreEqual(3, convertedWay.Version);
			Assert.AreEqual(5, convertedWay.UserId);
			Assert.AreEqual("foo", convertedWay.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), convertedWay.Timestamp);
			Assert.AreEqual("this road", convertedWay.Tags["name"]);
			Assert.AreEqual("A1", convertedWay.Tags["ref"]);
			Assert.AreEqual(6, convertedWay.NodeRefs.Count);
			Assert.AreEqual(5, convertedWay.NodeRefs[0]);
			Assert.AreEqual(9, convertedWay.NodeRefs[1]);
			Assert.AreEqual(12, convertedWay.NodeRefs[2]);
			Assert.AreEqual(543, convertedWay.NodeRefs[3]);
			Assert.AreEqual(43, convertedWay.NodeRefs[4]);
			Assert.AreEqual(1234151, convertedWay.NodeRefs[5]);
		}

		[Test]
		public void TestXmlStringToOSMWay()
		{
			var way = GetDefaultOSMWay();
			var xmlString = way.ToXmlString();
			var convertedWay = (OSMWay)xmlString.ToOSMElement();

			Assert.AreEqual(2, convertedWay.Id);
			Assert.AreEqual(7, convertedWay.Changeset);
			Assert.AreEqual(3, convertedWay.Version);
			Assert.AreEqual(5, convertedWay.UserId);
			Assert.AreEqual("foo", convertedWay.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), convertedWay.Timestamp);
			Assert.AreEqual("this road", convertedWay.Tags["name"]);
			Assert.AreEqual("A1", convertedWay.Tags["ref"]);
			Assert.AreEqual(6, convertedWay.NodeRefs.Count);
			Assert.AreEqual(5, convertedWay.NodeRefs[0]);
			Assert.AreEqual(9, convertedWay.NodeRefs[1]);
			Assert.AreEqual(12, convertedWay.NodeRefs[2]);
			Assert.AreEqual(543, convertedWay.NodeRefs[3]);
			Assert.AreEqual(43, convertedWay.NodeRefs[4]);
			Assert.AreEqual(1234151, convertedWay.NodeRefs[5]);
		}

		[Test]
		public void TestOSMWayToPostgreSQLInsertString()
		{
			var way = GetDefaultOSMWay();
			var sqlInsert = way.ToPostgreSQLInsert(out NameValueCollection sqlParameters);
			var expectedSql = "INSERT INTO ways (osm_id, tags, node_refs) ";
			expectedSql += "VALUES(@osm_id::bigint, @tags::hstore, @node_refs::bigint[])";
			Assert.AreEqual(expectedSql, sqlInsert);
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"tags", "\"name\"=>\"this road\", \"ref\"=>\"A1\""},
				{"node_refs", "{5, 9, 12, 543, 43, 1234151}"}
			};
			Assert.AreEqual(expectedSqlParameters.Count, sqlParameters.Count);
			foreach (string key in expectedSqlParameters) {
				Assert.NotNull(sqlParameters[key]);
				Assert.AreEqual(expectedSqlParameters[key], sqlParameters[key]);
			}
		}

		[Test]
		public void TestOSMWayToPostgreSQLSelectString()
		{
			var way = GetDefaultOSMWay();
			var sqlSelect = way.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, tags::text, node_refs::text FROM ways WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, sqlSelect);

			sqlSelect = way.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, tags::text, node_refs::text FROM ways WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, sqlSelect);

			way.OverrideId(6);
			sqlSelect = way.ToPostgreSQLSelect();
			expectedSql = "SELECT osm_id, tags::text, node_refs::text FROM ways WHERE osm_id = 6";
			Assert.AreEqual(expectedSql, sqlSelect);
		}

		[Test]
		public void TestOSMWayToPostgreSQLDeleteString()
		{
			var way = GetDefaultOSMWay();
			var expectedSql = "DELETE FROM ways WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, way.ToPostgreSQLDelete());
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

			Assert.AreEqual(2, idElement.Value.AsInt64);
			Assert.AreEqual(3, versionElement.Value.AsInt64);
			Assert.AreEqual(5, uidElement.Value.AsInt64);
			Assert.AreEqual("foo", userElement.Value.AsString);
			Assert.AreEqual(7, changesetElement.Value.AsInt64);
			Assert.AreEqual(new MongoDB.Bson.BsonDateTime(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)), timestampElement.Value.AsBsonDateTime);
			Assert.AreEqual(2, tagsElement.Value.AsBsonDocument.ElementCount);
			Assert.AreEqual(6, nodeRefsElement.Value.AsBsonArray.Count);

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.AreEqual("this road", tagName.Value.AsString);
			Assert.AreEqual("A1", tagRef.Value.AsString);

			var nodeRefsBsonArray = nodeRefsElement.Value.AsBsonArray;
			Assert.AreEqual(5, nodeRefsBsonArray[0].AsInt64);
			Assert.AreEqual(9, nodeRefsBsonArray[1].AsInt64);
			Assert.AreEqual(12, nodeRefsBsonArray[2].AsInt64);
			Assert.AreEqual(543, nodeRefsBsonArray[3].AsInt64);
			Assert.AreEqual(43, nodeRefsBsonArray[4].AsInt64);
			Assert.AreEqual(1234151, nodeRefsBsonArray[5].AsInt64);
		}

		[Test]
		public void TestOSMWayParseBsonDocument()
		{
			var way = GetDefaultOSMWay();
			var bsonDoc = way.ToBson();

			var parsedWay = new OSMWay(0);
			parsedWay.ParseBsonDocument(bsonDoc);

			Assert.AreEqual(way.Id, parsedWay.Id);
			Assert.AreEqual(way.UserId, parsedWay.UserId);
			Assert.AreEqual(way.UserName, parsedWay.UserName);
			Assert.AreEqual(way.Version, parsedWay.Version);
			Assert.AreEqual(way.Changeset, parsedWay.Changeset);
			Assert.AreEqual(way.Timestamp, parsedWay.Timestamp);
			Assert.AreEqual(way.Tags.Count, parsedWay.Tags.Count);
			Assert.AreEqual(way.NodeRefs.Count, parsedWay.NodeRefs.Count);

			foreach (string key in way.Tags) {
				Assert.AreEqual(way.Tags[key], parsedWay.Tags[key]);
			}

			for (var i = 0; i < way.NodeRefs.Count; i++) {
				Assert.AreEqual(way.NodeRefs[i], parsedWay.NodeRefs[i]);
			}
		}

		[Test]
		public void TestOSMWayParseEmptyBsonDocument()
		{
			var way = GetDefaultOSMWay();
			var bsonDoc = new MongoDB.Bson.BsonDocument();

			way.ParseBsonDocument(bsonDoc);

			Assert.AreEqual(0, way.Id);
			Assert.AreEqual(0, way.UserId);
			Assert.AreEqual(string.Empty, way.UserName);
			Assert.AreEqual(0, way.Version);
			Assert.AreEqual(0, way.Changeset);
			Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), way.Timestamp);
			Assert.AreEqual(0, way.Tags.Count);
			Assert.AreEqual(0, way.NodeRefs.Count);
		}
	}
}
