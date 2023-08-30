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
	public class TestOSMRelation
	{
		private static OSMRelation GetDefaultOSMRelation()
		{
			var relation = new OSMRelation(2) {
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			relation.Tags.Add("name", "this country");
			relation.Tags.Add("ref", "DE");
			relation.Members.Add(new OSMMember(MemberType.Way, 123, "inner"));
			relation.Members.Add(new OSMMember(MemberType.Way, 234, "outer"));
			relation.Members.Add(new OSMMember(MemberType.Node, 345, ""));
			relation.Members.Add(new OSMMember(MemberType.Relation, 456, ""));

			return relation;
		}

		[Test]
		public void TestOSMRelationClone()
		{
			var relation = GetDefaultOSMRelation();
			var relationClone = (OSMRelation)relation.Clone();
			relationClone.Changeset += 1;
			relationClone.Version += 1;
			relationClone.UserId = 2;
			relationClone.UserName = "bar";
			relationClone.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			relationClone.Tags["name"] = "that country";
			relationClone.Tags["ref"] = "GB";
			relationClone.Members.Clear();
			relationClone.Members.Add(new OSMMember(MemberType.Node, 321, "inner"));
			relationClone.Members.Add(new OSMMember(MemberType.Way, 432, "inner"));
			relationClone.Members.Add(new OSMMember(MemberType.Relation, 98765, "outer"));

			Assert.AreEqual(7, relation.Changeset);
			Assert.AreEqual(8, relationClone.Changeset);
			Assert.AreEqual(3, relation.Version);
			Assert.AreEqual(4, relationClone.Version);
			Assert.AreEqual(5, relation.UserId);
			Assert.AreEqual(2, relationClone.UserId);
			Assert.AreEqual("foo", relation.UserName);
			Assert.AreEqual("bar", relationClone.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), relation.Timestamp);
			Assert.AreEqual(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc), relationClone.Timestamp);
			Assert.AreEqual("this country", relation.Tags["name"]);
			Assert.AreEqual("that country", relationClone.Tags["name"]);
			Assert.AreEqual("DE", relation.Tags["ref"]);
			Assert.AreEqual("GB", relationClone.Tags["ref"]);
			Assert.AreEqual(4, relation.Members.Count);
			Assert.AreEqual(3, relationClone.Members.Count);

			Assert.AreEqual(new OSMMember(MemberType.Way, 123, "inner"), relation.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 234, "outer"), relation.Members[1]);
			Assert.AreEqual(new OSMMember(MemberType.Node, 345, ""), relation.Members[2]);

			Assert.AreEqual(new OSMMember(MemberType.Node, 321, "inner"), relationClone.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 432, "inner"), relationClone.Members[1]);
			Assert.AreEqual(new OSMMember(MemberType.Relation, 98765, "outer"), relationClone.Members[2]);
		}

		[Test]
		public void TestOSMRelationToXmlString()
		{
			var relation = GetDefaultOSMRelation();

			var expectedXmlString = "<relation id=\"2\" version=\"3\" uid=\"5\" user=\"foo\" changeset=\"7\" timestamp=\"2017-01-20T12:03:43Z\">";
			expectedXmlString += "<member type=\"way\" ref=\"123\" role=\"inner\" />";
			expectedXmlString += "<member type=\"way\" ref=\"234\" role=\"outer\" />";
			expectedXmlString += "<member type=\"node\" ref=\"345\" role=\"\" />";
			expectedXmlString += "<member type=\"relation\" ref=\"456\" role=\"\" />";
			expectedXmlString += "<tag k=\"name\" v=\"this country\" />";
			expectedXmlString += "<tag k=\"ref\" v=\"DE\" />";
			expectedXmlString += "</relation>";
			Assert.AreEqual(expectedXmlString, relation.ToXmlString());
		}

		[Test]
		public void TestXmlElementToOSMRelation()
		{
			var relation = GetDefaultOSMRelation();
			var xmlRelation = relation.ToXml();
			var convertedRelation = (OSMRelation)xmlRelation.ToOSMElement();

			Assert.AreEqual(2, convertedRelation.Id);
			Assert.AreEqual(7, convertedRelation.Changeset);
			Assert.AreEqual(3, convertedRelation.Version);
			Assert.AreEqual(5, convertedRelation.UserId);
			Assert.AreEqual("foo", convertedRelation.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), convertedRelation.Timestamp);
			Assert.AreEqual("this country", convertedRelation.Tags["name"]);
			Assert.AreEqual("DE", convertedRelation.Tags["ref"]);
			Assert.AreEqual(4, convertedRelation.Members.Count);
			Assert.AreEqual(new OSMMember(MemberType.Way, 123, "inner"), convertedRelation.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 234, "outer"), convertedRelation.Members[1]);
			Assert.AreEqual(new OSMMember(MemberType.Node, 345, ""), convertedRelation.Members[2]);
			Assert.AreEqual(new OSMMember(MemberType.Relation, 456, ""), convertedRelation.Members[3]);
		}

		[Test]
		public void TestXmlStringToOSMRelation()
		{
			var relation = GetDefaultOSMRelation();
			var xmlString = relation.ToXmlString();
			var convertedRelation = (OSMRelation)xmlString.ToOSMElement();

			Assert.AreEqual(2, convertedRelation.Id);
			Assert.AreEqual(7, convertedRelation.Changeset);
			Assert.AreEqual(3, convertedRelation.Version);
			Assert.AreEqual(5, convertedRelation.UserId);
			Assert.AreEqual("foo", convertedRelation.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), convertedRelation.Timestamp);
			Assert.AreEqual("this country", convertedRelation.Tags["name"]);
			Assert.AreEqual("DE", convertedRelation.Tags["ref"]);
			Assert.AreEqual(4, convertedRelation.Members.Count);
			Assert.AreEqual(new OSMMember(MemberType.Way, 123, "inner"), convertedRelation.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 234, "outer"), convertedRelation.Members[1]);
			Assert.AreEqual(new OSMMember(MemberType.Node, 345, ""), convertedRelation.Members[2]);
			Assert.AreEqual(new OSMMember(MemberType.Relation, 456, ""), convertedRelation.Members[3]);
		}

		[Test]
		public void TestOSMRelationToPostgreSQLInsertString()
		{
			var relation = GetDefaultOSMRelation();
			var sqlInsert = relation.ToPostgreSQLInsert(out NameValueCollection sqlParameters);
			var expectedSql = "INSERT INTO relations (osm_id, tags, members) ";
			expectedSql += "VALUES(@osm_id::bigint, @tags::hstore, ARRAY[@member_1::hstore, @member_2::hstore, @member_3::hstore, @member_4::hstore])";
			Assert.AreEqual(expectedSql, sqlInsert);
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"tags", "\"name\"=>\"this country\", \"ref\"=>\"DE\""},
				{"member_1", "\"type\"=>\"way\",\"ref\"=>\"123\",\"role\"=>\"inner\""},
				{"member_2", "\"type\"=>\"way\",\"ref\"=>\"234\",\"role\"=>\"outer\""},
				{"member_3", "\"type\"=>\"node\",\"ref\"=>\"345\",\"role\"=>\"\""},
				{"member_4", "\"type\"=>\"relation\",\"ref\"=>\"456\",\"role\"=>\"\""}
			};
			Assert.AreEqual(expectedSqlParameters.Count, sqlParameters.Count);
			foreach (string key in expectedSqlParameters) {
				Assert.NotNull(sqlParameters[key]);
				Assert.AreEqual(expectedSqlParameters[key], sqlParameters[key]);
			}
		}

		[Test]
		public void TestOSMRelationWithoutMembersToPostgreSQLInsertString()
		{
			var relation = GetDefaultOSMRelation();
			relation.Members.Clear();
            var sqlInsert = relation.ToPostgreSQLInsert(out _);
			var expectedSql = "INSERT INTO relations (osm_id, tags, members) ";
			expectedSql += "VALUES(@osm_id::bigint, @tags::hstore, '{}')";
			Assert.AreEqual(expectedSql, sqlInsert);
		}

		[Test]
		public void TestOSMRelationToPostgreSQLSelectString()
		{
			var relation = GetDefaultOSMRelation();
			var sqlSelect = relation.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, tags::text, members::text FROM relations WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, sqlSelect);

			sqlSelect = relation.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, tags::text, members::text FROM relations WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, sqlSelect);

			relation.OverrideId(7);
			sqlSelect = relation.ToPostgreSQLSelect();
			expectedSql = "SELECT osm_id, tags::text, members::text FROM relations WHERE osm_id = 7";
			Assert.AreEqual(expectedSql, sqlSelect);
		}

		[Test]
		public void TestOSMRelationToPostgreSQLDeleteString()
		{
			var relation = GetDefaultOSMRelation();
			var expectedSql = "DELETE FROM relations WHERE osm_id = 2";
			Assert.AreEqual(expectedSql, relation.ToPostgreSQLDelete());
		}

		[Test]
		public void TestOSMRelationToBson()
		{
			var relation = GetDefaultOSMRelation();

			var bsonDoc = relation.ToBson();
			var idElement = bsonDoc.GetElement("id");
			var versionElement = bsonDoc.GetElement("version");
			var uidElement = bsonDoc.GetElement("uid");
			var userElement = bsonDoc.GetElement("user");
			var changesetElement = bsonDoc.GetElement("changeset");
			var timestampElement = bsonDoc.GetElement("timestamp");
			var tagsElement = bsonDoc.GetElement("tags");
			var membersElement = bsonDoc.GetElement("members");

			Assert.AreEqual(2, idElement.Value.AsInt64);
			Assert.AreEqual(3, versionElement.Value.AsInt64);
			Assert.AreEqual(5, uidElement.Value.AsInt64);
			Assert.AreEqual("foo", userElement.Value.AsString);
			Assert.AreEqual(7, changesetElement.Value.AsInt64);
			Assert.AreEqual(new MongoDB.Bson.BsonDateTime(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)), timestampElement.Value.AsBsonDateTime);
			Assert.AreEqual(2, tagsElement.Value.AsBsonDocument.ElementCount);
			Assert.AreEqual(4, membersElement.Value.AsBsonArray.Count);

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.AreEqual("this country", tagName.Value.AsString);
			Assert.AreEqual("DE", tagRef.Value.AsString);

			var membersBsonArray = membersElement.Value.AsBsonArray;
			var member1Doc = membersBsonArray[0].AsBsonDocument;
			var member2Doc = membersBsonArray[1].AsBsonDocument;
			var member3Doc = membersBsonArray[2].AsBsonDocument;
			var member4Doc = membersBsonArray[3].AsBsonDocument;
			Assert.AreEqual("way", member1Doc.GetElement("type").Value.AsString);
			Assert.AreEqual(123, member1Doc.GetElement("ref").Value.AsInt64);
			Assert.AreEqual("inner", member1Doc.GetElement("role").Value.AsString);
			Assert.AreEqual("way", member2Doc.GetElement("type").Value.AsString);
			Assert.AreEqual(234, member2Doc.GetElement("ref").Value.AsInt64);
			Assert.AreEqual("outer", member2Doc.GetElement("role").Value.AsString);
			Assert.AreEqual("node", member3Doc.GetElement("type").Value.AsString);
			Assert.AreEqual(345, member3Doc.GetElement("ref").Value.AsInt64);
			Assert.AreEqual("", member3Doc.GetElement("role").Value.AsString);
			Assert.AreEqual("relation", member4Doc.GetElement("type").Value.AsString);
			Assert.AreEqual(456, member4Doc.GetElement("ref").Value.AsInt64);
			Assert.AreEqual("", member4Doc.GetElement("role").Value.AsString);
		}

		[Test]
		public void TestOSMRelationParseBsonDocument()
		{
			var relation = GetDefaultOSMRelation();
			var bsonDoc = relation.ToBson();

			var parsedRelation = new OSMRelation(0);
			parsedRelation.ParseBsonDocument(bsonDoc);

			var relation2 = new OSMRelation(2) {
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			relation2.Tags.Add("name", "this country");
			relation2.Tags.Add("ref", "DE");
			relation2.Members.Add(new OSMMember(MemberType.Way, 123, "inner"));
			relation2.Members.Add(new OSMMember(MemberType.Way, 234, "outer"));

			Assert.AreEqual(relation.Id, parsedRelation.Id);
			Assert.AreEqual(relation.UserId, parsedRelation.UserId);
			Assert.AreEqual(relation.UserName, parsedRelation.UserName);
			Assert.AreEqual(relation.Version, parsedRelation.Version);
			Assert.AreEqual(relation.Changeset, parsedRelation.Changeset);
			Assert.AreEqual(relation.Timestamp, parsedRelation.Timestamp);
			Assert.AreEqual(relation.Tags.Count, parsedRelation.Tags.Count);
			Assert.AreEqual(relation.Members.Count, parsedRelation.Members.Count);

			foreach (string key in relation.Tags) {
				Assert.AreEqual(relation.Tags[key], parsedRelation.Tags[key]);
			}

			for (var i = 0; i < relation.Members.Count; i++) {
				Assert.AreEqual(relation.Members[i].Type, parsedRelation.Members[i].Type);
				Assert.AreEqual(relation.Members[i].Ref, parsedRelation.Members[i].Ref);
				Assert.AreEqual(relation.Members[i].Role, parsedRelation.Members[i].Role);
			}
		}

		[Test]
		public void TestOSMRelationParseEmptyBsonDocument()
		{
			var relation = GetDefaultOSMRelation();
			var bsonDoc = new MongoDB.Bson.BsonDocument();

			relation.ParseBsonDocument(bsonDoc);

			Assert.AreEqual(0, relation.Id);
			Assert.AreEqual(0, relation.UserId);
			Assert.AreEqual(string.Empty, relation.UserName);
			Assert.AreEqual(0, relation.Version);
			Assert.AreEqual(0, relation.Changeset);
			Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), relation.Timestamp);
			Assert.AreEqual(0, relation.Tags.Count);
			Assert.AreEqual(0, relation.Members.Count);
		}
	}
}
