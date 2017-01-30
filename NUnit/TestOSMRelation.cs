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
	public class TestOSMRelation
	{
		private OSMRelation GetDefaultOSMRelation()
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

			return relation;
		}

		[Test]
		public void TestOSMRelationClone()
		{
			var relation = this.GetDefaultOSMRelation();
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
			Assert.AreEqual(2, relation.Members.Count);
			Assert.AreEqual(3, relationClone.Members.Count);

			Assert.AreEqual(new OSMMember(MemberType.Way, 123, "inner"), relation.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 234, "outer"), relation.Members[1]);

			Assert.AreEqual(new OSMMember(MemberType.Node, 321, "inner"), relationClone.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 432, "inner"), relationClone.Members[1]);
			Assert.AreEqual(new OSMMember(MemberType.Relation, 98765, "outer"), relationClone.Members[2]);
		}

		[Test]
		public void TestOSMRelationToXmlString()
		{
			var relation = this.GetDefaultOSMRelation();

			var expectedXmlString = "<relation id=\"2\" version=\"3\" uid=\"5\" user=\"foo\" changeset=\"7\" timestamp=\"2017-01-20T12:03:43Z\">";
			expectedXmlString += "<member type=\"way\" ref=\"123\" role=\"inner\" />";
			expectedXmlString += "<member type=\"way\" ref=\"234\" role=\"outer\" />";
			expectedXmlString += "<tag k=\"name\" v=\"this country\" />";
			expectedXmlString += "<tag k=\"ref\" v=\"DE\" />";
			expectedXmlString += "</relation>";
			Assert.AreEqual(expectedXmlString, relation.ToXmlString());
		}

		[Test]
		public void TestXmlElementToOSMRelation()
		{
			var relation = this.GetDefaultOSMRelation();
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
			Assert.AreEqual(2, convertedRelation.Members.Count);
			Assert.AreEqual(new OSMMember(MemberType.Way, 123, "inner"), convertedRelation.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 234, "outer"), convertedRelation.Members[1]);
		}

		[Test]
		public void TestXmlStringToOSMRelation()
		{
			var relation = this.GetDefaultOSMRelation();
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
			Assert.AreEqual(2, convertedRelation.Members.Count);
			Assert.AreEqual(new OSMMember(MemberType.Way, 123, "inner"), convertedRelation.Members[0]);
			Assert.AreEqual(new OSMMember(MemberType.Way, 234, "outer"), convertedRelation.Members[1]);
		}

		[Test]
		public void TestOSMRelationToPostgreSQLInsertString()
		{
			var relation = this.GetDefaultOSMRelation();
			NameValueCollection sqlParameters;
			var sqlInsert = relation.ToPostgreSQLInsert(out sqlParameters);
			var expectedSql = "INSERT INTO relations (osm_id, tags, members) ";
			expectedSql += "VALUES(@osm_id::bigint, @tags::hstore, ARRAY[@member_1::hstore, @member_2::hstore])";
			Assert.AreEqual(expectedSql, sqlInsert);
			var expectedSqlParameters = new NameValueCollection {
				{"osm_id", "2"},
				{"tags", "\"name\"=>\"this country\", \"ref\"=>\"DE\""},
				{"member_1", "\"type\"=>\"way\",\"ref\"=>\"123\",\"role\"=>\"inner\""},
				{"member_2", "\"type\"=>\"way\",\"ref\"=>\"234\",\"role\"=>\"outer\""}
			};
			Assert.AreEqual(expectedSqlParameters.Count, sqlParameters.Count);
			foreach(string key in expectedSqlParameters) {
				Assert.NotNull(sqlParameters[key]);
				Assert.AreEqual(expectedSqlParameters[key], sqlParameters[key]);
			}
		}

		[Test]
		public void TestOSMRelationToPostgreSQLSelectString()
		{
			var relation = this.GetDefaultOSMRelation();
			var sqlSelect = relation.ToPostgreSQLSelect();
			var expectedSql = "SELECT osm_id, tags, members FROM relations";
			Assert.AreEqual(expectedSql, sqlSelect);

			sqlSelect = relation.ToPostgreSQLSelect(inclusiveMetaField: true);
			expectedSql = "SELECT osm_id, version, changeset, uid, user, timestamp, tags, members FROM relations";
			Assert.AreEqual(expectedSql, sqlSelect);

			sqlSelect = relation.ToPostgreSQLSelect(id: 7);
			expectedSql = "SELECT osm_id, tags, members FROM relations WHERE osm_id = 7";
			Assert.AreEqual(expectedSql, sqlSelect);

			sqlSelect = relation.ToPostgreSQLSelect(offset: 30, limit: 300);
			expectedSql = "SELECT osm_id, tags, members FROM relations OFFSET 30 LIMIT 300";
			Assert.AreEqual(expectedSql, sqlSelect);

			Assert.Throws(typeof(ArgumentException), () => { relation.ToPostgreSQLSelect(id: 7, offset: 30, limit: 300); });
		}

		[Test]
		public void TestOSMRelationToBson()
		{
			var relation = this.GetDefaultOSMRelation();

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
			Assert.AreEqual(2, membersElement.Value.AsBsonArray.Count);

			var tagName = tagsElement.Value.AsBsonDocument.GetElement("name");
			var tagRef = tagsElement.Value.AsBsonDocument.GetElement("ref");
			Assert.AreEqual("this country", tagName.Value.AsString);
			Assert.AreEqual("DE", tagRef.Value.AsString);

			var membersBsonArray = membersElement.Value.AsBsonArray;
			var member1Doc = membersBsonArray[0].AsBsonDocument;
			var member2Doc = membersBsonArray[1].AsBsonDocument;
			Assert.AreEqual("way", member1Doc.GetElement("type").Value.AsString);
			Assert.AreEqual(123, member1Doc.GetElement("ref").Value.AsInt64);
			Assert.AreEqual("inner", member1Doc.GetElement("role").Value.AsString);
			Assert.AreEqual("way", member2Doc.GetElement("type").Value.AsString);
			Assert.AreEqual(234, member2Doc.GetElement("ref").Value.AsInt64);
			Assert.AreEqual("outer", member2Doc.GetElement("role").Value.AsString);
		}
	}
}
