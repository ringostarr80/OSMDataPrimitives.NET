using System.Collections.Generic;
using NUnit.Framework;
using OSMDataPrimitives;

namespace NUnitTests
{
    [TestFixture]
    public class TestOsmMember
    {
        private static OsmMember GetDefaultOsmMemberNode()
        {
            return new OsmMember(MemberType.Node, 1, "");
        }

        private static OsmMember GetDefaultOsmMemberWay()
        {
            return new OsmMember(MemberType.Way, 2, "outer");
        }

        private static OsmMember GetDefaultOsmMemberRelation()
        {
            return new OsmMember(MemberType.Relation, 3, "");
        }

        [Test]
        public void TestOsmMemberNode()
        {
            var memberNode = GetDefaultOsmMemberNode();

            Assert.That(memberNode.Type, Is.EqualTo(MemberType.Node));
            Assert.That(memberNode.Ref, Is.EqualTo(1));
            Assert.That(memberNode.Role, Is.Empty);
        }

        [Test]
        public void TestOsmMemberWay()
        {
            var memberWay = GetDefaultOsmMemberWay();

            Assert.That(memberWay.Type, Is.EqualTo(MemberType.Way));
            Assert.That(memberWay.Ref, Is.EqualTo(2));
            Assert.That(memberWay.Role, Is.EqualTo("outer"));
        }

        [Test]
        public void TestOsmMemberRelation()
        {
            var memberRelation = GetDefaultOsmMemberRelation();

            Assert.That(memberRelation.Type, Is.EqualTo(MemberType.Relation));
            Assert.That(memberRelation.Ref, Is.EqualTo(3));
            Assert.That(memberRelation.Role, Is.Empty);
        }

        [Test]
        public void TestOsmMemberNodeToDictionary()
        {
            var memberNode = GetDefaultOsmMemberNode();
            var expectedDictionary = new Dictionary<string, string>
            {
                { "type", "node" },
                { "ref", "1" },
                { "role", "" }
            };

            Assert.That(memberNode.ToDictionary(), Is.EqualTo(expectedDictionary));
        }

        [Test]
        public void TestOsmMemberWayToDictionary()
        {
            var memberWay = GetDefaultOsmMemberWay();
            var expectedDictionary = new Dictionary<string, string>
            {
                { "type", "way" },
                { "ref", "2" },
                { "role", "outer" }
            };

            Assert.That(memberWay.ToDictionary(), Is.EqualTo(expectedDictionary));
        }

        [Test]
        public void TestOsmMemberRelationToDictionary()
        {
            var memberRelation = GetDefaultOsmMemberRelation();
            var expectedDictionary = new Dictionary<string, string>
            {
                { "type", "relation" },
                { "ref", "3" },
                { "role", "" }
            };

            Assert.That(memberRelation.ToDictionary(), Is.EqualTo(expectedDictionary));
        }
    }
}
