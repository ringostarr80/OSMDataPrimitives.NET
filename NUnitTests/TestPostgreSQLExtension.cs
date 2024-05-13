using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.PostgreSQL;

namespace NUnit
{
	[TestFixture]
	public class TestPostgreSqlExtension
	{
        [Test]
		public void TestOSMNodeParsePostgreSQLFields()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>\"bar\"" }
            };
            var osmNode = new OsmNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            var expectedTags = new Dictionary<string, string> {
                { "foo", "bar" }
            };
            Assert.That(osmNode.Id, Is.EqualTo(2));
            Assert.That(osmNode.Tags, Is.EqualTo(expectedTags));
        }

        [Test]
		public void TestOSMWayParsePostgreSQLFields()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>\"bar\"" },
                { "node_refs", "{10,12,15,213,18}" }
            };
            var osmWay = new OsmWay(1);
            osmWay.ParsePostgreSQLFields(parameters);

            var expectedTags = new Dictionary<string, string> {
                { "foo", "bar" }
            };
            var expectedList = new List<ulong> { 10, 12, 15, 213, 18 };
            Assert.That(osmWay.Id, Is.EqualTo(2));
            Assert.That(osmWay.Tags, Is.EqualTo(expectedTags));
            Assert.That(osmWay.NodeRefs, Is.EqualTo(expectedList));
        }

        [Test]
		public void TestOSMNodeParsePostgreSQLFieldsFailed1()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo" }
            };
            var osmNode = new OsmNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            Assert.That(osmNode.Tags, Is.EqualTo(new NameValueCollection()));
        }

        [Test]
		public void TestOSMNodeParsePostgreSQLFieldsFailed2()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"" }
            };
            var osmNode = new OsmNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            Assert.That(osmNode.Tags, Is.EqualTo(new NameValueCollection()));
        }

        [Test]
		public void TestOSMNodeParsePostgreSQLFieldsFailed3()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>" }
            };
            var osmNode = new OsmNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            var expectedTags = new Dictionary<string, string>();
            Assert.That(osmNode.Tags, Is.EqualTo(expectedTags));
        }

        [Test]
		public void TestOSMNodeParsePostgreSQLFieldsFailed4()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>\"bar" }
            };
            var osmNode = new OsmNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            Assert.That(osmNode.Tags, Is.EqualTo(new NameValueCollection()));
        }

        [Test]
		public void TestOSMNodeParsePostgreSQLFieldsWithEscapedBackslash()
		{
            var parameters = new NameValueCollection() {
                { "osm_id", "2" },
                { "tags", "\"foo\"=>\"bar\\baz\"" }
            };
            var osmNode = new OsmNode(1, 52.1234, 10.1234);
            osmNode.ParsePostgreSQLFields(parameters);

            Assert.That(osmNode.Tags["foo"], Is.EqualTo("bar\\baz"));
        }

        [Test]
        public void ParseHstore_ValidHstoreString_ReturnsDictionary()
        {
            // Arrange
            string hstoreString = "\"key1\"=>\"value1\", \"key2\"=>\"value2\"";

            // Act
            var result = Extension.ParseHstore(hstoreString);

            // Assert
            Assert.That(result, Is.InstanceOf<Dictionary<string, string>>());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["key1"], Is.EqualTo("value1"));
            Assert.That(result["key2"], Is.EqualTo("value2"));
        }

        [Test]
        public void ParseHstore_InvalidHstoreString_ReturnsDictionaryWith1Entry()
        {
            // Arrange
            string hstoreString = "\"key1\"=>\"value1\",\"key2\"=>";

            // Act
            var result = Extension.ParseHstore(hstoreString);

            // Assert
            Assert.That(result, Is.InstanceOf<Dictionary<string, string>>());
            Assert.That(result, Is.EqualTo(new Dictionary<string, string> {{"key1", "value1"}}));
        }
    }
}
