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
	public class TestOsmRelation
	{
		private static OsmRelation GetDefaultOSMRelation()
		{
			var relation = new OsmRelation(2) {
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			relation.Tags.Add("name", "this country");
			relation.Tags.Add("ref", "DE");
			relation.Members.Add(new OsmMember(MemberType.Way, 123, "inner"));
			relation.Members.Add(new OsmMember(MemberType.Way, 234, "outer"));
			relation.Members.Add(new OsmMember(MemberType.Node, 345, ""));
			relation.Members.Add(new OsmMember(MemberType.Relation, 456, ""));

			return relation;
		}

		[Test]
		public void TestOSMRelationClone()
		{
			var relation = GetDefaultOSMRelation();
			var relationClone = (OsmRelation)relation.Clone();
			relationClone.Changeset += 1;
			relationClone.Version += 1;
			relationClone.UserId = 2;
			relationClone.UserName = "bar";
			relationClone.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			relationClone.Tags["name"] = "that country";
			relationClone.Tags["ref"] = "GB";
			relationClone.Members.Clear();
			relationClone.Members.Add(new OsmMember(MemberType.Node, 321, "inner"));
			relationClone.Members.Add(new OsmMember(MemberType.Way, 432, "inner"));
			relationClone.Members.Add(new OsmMember(MemberType.Relation, 98765, "outer"));

			Assert.That(relation.Changeset, Is.EqualTo(7));
			Assert.That(relationClone.Changeset, Is.EqualTo(8));
			Assert.That(relation.Version, Is.EqualTo(3));
			Assert.That(relationClone.Version, Is.EqualTo(4));
			Assert.That(relation.UserId, Is.EqualTo(5));
			Assert.That(relationClone.UserId, Is.EqualTo(2));
			Assert.That(relation.UserName, Is.EqualTo("foo"));
			Assert.That(relationClone.UserName, Is.EqualTo("bar"));
			Assert.That(relation.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(relationClone.Timestamp, Is.EqualTo(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc)));
			Assert.That(relation.Tags["name"], Is.EqualTo("this country"));
			Assert.That(relationClone.Tags["name"], Is.EqualTo("that country"));
			Assert.That(relation.Tags["ref"], Is.EqualTo("DE"));
			Assert.That(relationClone.Tags["ref"], Is.EqualTo("GB"));
			Assert.That(relation.Members.Count, Is.EqualTo(4));
			Assert.That(relationClone.Members.Count, Is.EqualTo(3));

			Assert.That(relation.Members[0], Is.EqualTo(new OsmMember(MemberType.Way, 123, "inner")));
			Assert.That(relation.Members[1], Is.EqualTo(new OsmMember(MemberType.Way, 234, "outer")));
			Assert.That(relation.Members[2], Is.EqualTo(new OsmMember(MemberType.Node, 345, "")));

			Assert.That(relationClone.Members[0], Is.EqualTo(new OsmMember(MemberType.Node, 321, "inner")));
			Assert.That(relationClone.Members[1], Is.EqualTo(new OsmMember(MemberType.Way, 432, "inner")));
			Assert.That(relationClone.Members[2], Is.EqualTo(new OsmMember(MemberType.Relation, 98765, "outer")));
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
			Assert.That(relation.ToXmlString(), Is.EqualTo(expectedXmlString));
		}

		[Test]
		public void TestXmlElementToOSMRelation()
		{
			var relation = GetDefaultOSMRelation();
			var xmlRelation = relation.ToXml();
			var convertedRelation = (OsmRelation)xmlRelation.ToOSMElement();

			Assert.That(convertedRelation.Id, Is.EqualTo(2));
			Assert.That(convertedRelation.Changeset, Is.EqualTo(7));
			Assert.That(convertedRelation.Version, Is.EqualTo(3));
			Assert.That(convertedRelation.UserId, Is.EqualTo(5));
			Assert.That(convertedRelation.UserName, Is.EqualTo("foo"));
			Assert.That(convertedRelation.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(convertedRelation.Tags["name"], Is.EqualTo("this country"));
			Assert.That(convertedRelation.Tags["ref"], Is.EqualTo("DE"));
			Assert.That(convertedRelation.Members.Count, Is.EqualTo(4));
			Assert.That(convertedRelation.Members[0], Is.EqualTo(new OsmMember(MemberType.Way, 123, "inner")));
			Assert.That(convertedRelation.Members[1], Is.EqualTo(new OsmMember(MemberType.Way, 234, "outer")));
			Assert.That(convertedRelation.Members[2], Is.EqualTo(new OsmMember(MemberType.Node, 345, "")));
			Assert.That(convertedRelation.Members[3], Is.EqualTo(new OsmMember(MemberType.Relation, 456, "")));
		}

		[Test]
		public void TestXmlStringToOSMRelation()
		{
			var relation = GetDefaultOSMRelation();
			var xmlString = relation.ToXmlString();
			var convertedRelation = (OsmRelation)xmlString.ToOSMElement();

			Assert.That(convertedRelation.Id, Is.EqualTo(2));
			Assert.That(convertedRelation.Changeset, Is.EqualTo(7));
			Assert.That(convertedRelation.Version, Is.EqualTo(3));
			Assert.That(convertedRelation.UserId, Is.EqualTo(5));
			Assert.That(convertedRelation.UserName, Is.EqualTo("foo"));
			Assert.That(convertedRelation.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(convertedRelation.Tags["name"], Is.EqualTo("this country"));
			Assert.That(convertedRelation.Tags["ref"], Is.EqualTo("DE"));
			Assert.That(convertedRelation.Members.Count, Is.EqualTo(4));
			Assert.That(convertedRelation.Members[0], Is.EqualTo(new OsmMember(MemberType.Way, 123, "inner")));
			Assert.That(convertedRelation.Members[1], Is.EqualTo(new OsmMember(MemberType.Way, 234, "outer")));
			Assert.That(convertedRelation.Members[2], Is.EqualTo(new OsmMember(MemberType.Node, 345, "")));
			Assert.That(convertedRelation.Members[3], Is.EqualTo(new OsmMember(MemberType.Relation, 456, "")));
		}

		[Test]
		public void TestOSMRelationToPostgreSQLInsertString()
		{
			var relation = GetDefaultOSMRelation();
			var sqlInsert = relation.ToPostgreSQLInsert(out NameValueCollection sqlParameters);
			var expectedSql = "INSERT INTO relations (osm_id, tags, members) ";
			expectedSql += "VALUES(@osm_id::bigint, @tags::hstore, ARRAY[@member_1::hstore, @member_2::hstore, @member_3::hstore, @member_4::hstore])";
			Assert.That(sqlInsert, Is.EqualTo(expectedSql));
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"tags", "\"name\"=>\"this country\", \"ref\"=>\"DE\""},
				{"member_1", "\"type\"=>\"way\",\"ref\"=>\"123\",\"role\"=>\"inner\""},
				{"member_2", "\"type\"=>\"way\",\"ref\"=>\"234\",\"role\"=>\"outer\""},
				{"member_3", "\"type\"=>\"node\",\"ref\"=>\"345\",\"role\"=>\"\""},
				{"member_4", "\"type\"=>\"relation\",\"ref\"=>\"456\",\"role\"=>\"\""}
			};
			Assert.That(sqlParameters.Count, Is.EqualTo(expectedSqlParameters.Count));
			foreach (string key in expectedSqlParameters) {
				Assert.That(sqlParameters[key], !Is.Null);
				Assert.That(sqlParameters[key], Is.EqualTo(expectedSqlParameters[key]));
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
			Assert.That(sqlInsert, Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMRelationToPostgreSQLSelectString()
		{
			var relation = GetDefaultOSMRelation();
			var sqlSelect = relation.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, tags::text, members::text FROM relations WHERE osm_id = 2";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));

			sqlSelect = relation.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, tags::text, members::text FROM relations WHERE osm_id = 2";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));

			relation.OverrideId(7);
			sqlSelect = relation.ToPostgreSQLSelect();
			expectedSql = "SELECT osm_id, tags::text, members::text FROM relations WHERE osm_id = 7";
			Assert.That(sqlSelect, Is.EqualTo(expectedSql));
		}

		[Test]
		public void TestOSMRelationToPostgreSQLDeleteString()
		{
			var relation = GetDefaultOSMRelation();
			var expectedSql = "DELETE FROM relations WHERE osm_id = 2";
			Assert.That(relation.ToPostgreSQLDelete(), Is.EqualTo(expectedSql));
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

			Assert.That(idElement.Value.AsInt64, Is.EqualTo(2));
			Assert.That(versionElement.Value.AsInt64, Is.EqualTo(3));
			Assert.That(uidElement.Value.AsInt64, Is.EqualTo(5));
			Assert.That(userElement.Value.AsString, Is.EqualTo("foo"));
			Assert.That(changesetElement.Value.AsInt64, Is.EqualTo(7));
			Assert.That(timestampElement.Value.AsBsonDateTime, Is.EqualTo(new MongoDB.Bson.BsonDateTime(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc))));
			Assert.That(tagsElement.Value.AsBsonDocument.ElementCount, Is.EqualTo(2));
			Assert.That(membersElement.Value.AsBsonArray.Count, Is.EqualTo(4));

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.That(tagName.Value.AsString, Is.EqualTo("this country"));
			Assert.That(tagRef.Value.AsString, Is.EqualTo("DE"));

			var membersBsonArray = membersElement.Value.AsBsonArray;
			var member1Doc = membersBsonArray[0].AsBsonDocument;
			var member2Doc = membersBsonArray[1].AsBsonDocument;
			var member3Doc = membersBsonArray[2].AsBsonDocument;
			var member4Doc = membersBsonArray[3].AsBsonDocument;
			Assert.That(member1Doc.GetElement("type").Value.AsString, Is.EqualTo("way"));
			Assert.That(member1Doc.GetElement("ref").Value.AsInt64, Is.EqualTo(123));
			Assert.That(member1Doc.GetElement("role").Value.AsString, Is.EqualTo("inner"));
			Assert.That(member2Doc.GetElement("type").Value.AsString, Is.EqualTo("way"));
			Assert.That(member2Doc.GetElement("ref").Value.AsInt64, Is.EqualTo(234));
			Assert.That(member2Doc.GetElement("role").Value.AsString, Is.EqualTo("outer"));
			Assert.That(member3Doc.GetElement("type").Value.AsString, Is.EqualTo("node"));
			Assert.That(member3Doc.GetElement("ref").Value.AsInt64, Is.EqualTo(345));
			Assert.That(member3Doc.GetElement("role").Value.AsString, Is.Empty);
			Assert.That(member4Doc.GetElement("type").Value.AsString, Is.EqualTo("relation"));
			Assert.That(member4Doc.GetElement("ref").Value.AsInt64, Is.EqualTo(456));
			Assert.That(member4Doc.GetElement("role").Value.AsString, Is.Empty);
		}

		[Test]
		public void TestOSMRelationParseBsonDocument()
		{
			var relation = GetDefaultOSMRelation();
			var bsonDoc = relation.ToBson();

			var parsedRelation = new OsmRelation(0);
			parsedRelation.ParseBsonDocument(bsonDoc);

			var relation2 = new OsmRelation(2) {
				UserId = 5,
				UserName = "foo",
				Version = 3,
				Changeset = 7,
				Timestamp = new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)
			};
			relation2.Tags.Add("name", "this country");
			relation2.Tags.Add("ref", "DE");
			relation2.Members.Add(new OsmMember(MemberType.Way, 123, "inner"));
			relation2.Members.Add(new OsmMember(MemberType.Way, 234, "outer"));

			Assert.That(parsedRelation.Id, Is.EqualTo(relation.Id));
			Assert.That(parsedRelation.UserId, Is.EqualTo(relation.UserId));
			Assert.That(parsedRelation.UserName, Is.EqualTo(relation.UserName));
			Assert.That(parsedRelation.Version, Is.EqualTo(relation.Version));
			Assert.That(parsedRelation.Changeset, Is.EqualTo(relation.Changeset));
			Assert.That(parsedRelation.Timestamp, Is.EqualTo(relation.Timestamp));
			Assert.That(parsedRelation.Tags.Count, Is.EqualTo(relation.Tags.Count));
			Assert.That(parsedRelation.Members.Count, Is.EqualTo(relation.Members.Count));

			foreach (KeyValuePair<string, string> tag in relation.Tags) {
				Assert.That(parsedRelation.Tags[tag.Key], Is.EqualTo(tag.Value));
			}

			for (var i = 0; i < relation.Members.Count; i++) {
				Assert.That(parsedRelation.Members[i].Type, Is.EqualTo(relation.Members[i].Type));
				Assert.That(parsedRelation.Members[i].Ref, Is.EqualTo(relation.Members[i].Ref));
				Assert.That(parsedRelation.Members[i].Role, Is.EqualTo(relation.Members[i].Role));
			}
		}

		[Test]
		public void TestOSMRelationParseEmptyBsonDocument()
		{
			var relation = GetDefaultOSMRelation();
			var bsonDoc = new MongoDB.Bson.BsonDocument();

			relation.ParseBsonDocument(bsonDoc);

			Assert.That(relation.Id, Is.Zero);
			Assert.That(relation.UserId, Is.Zero);
			Assert.That(relation.UserName, Is.Empty);
			Assert.That(relation.Version, Is.Zero);
			Assert.That(relation.Changeset, Is.Zero);
			Assert.That(relation.Timestamp, Is.EqualTo(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
			Assert.That(relation.Tags.Count, Is.Zero);
			Assert.That(relation.Members.Count, Is.Zero);
		}
	}
}
