using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.PostgreSQL;

namespace NUnit
{
	[TestFixture]
	public class TestPostgreSQLExtension
	{
        [Test]
		public void TestOSMNodeParsePostgreSQLFields()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>\"bar\"" }
            };
            var osmNode = new OSMNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            var expectedTags = new NameValueCollection {
                { "foo", "bar" }
            };
            Assert.AreEqual(2, osmNode.Id);
            Assert.AreEqual(expectedTags, osmNode.Tags);
        }

        [Test]
		public void TestOSMWayParsePostgreSQLFields()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>\"bar\"" },
                { "node_refs", "{10,12,15,213,18}" }
            };
            var osmWay = new OSMWay(1);
            osmWay.ParsePostgreSQLFields(parameters);

            var expectedTags = new NameValueCollection {
                { "foo", "bar" }
            };
            var expectedList = new List<ulong> { 10, 12, 15, 213, 18 };
            Assert.AreEqual(2, osmWay.Id);
            Assert.AreEqual(expectedTags, osmWay.Tags);
            Assert.AreEqual(expectedList, osmWay.NodeRefs);
        }
    }
}