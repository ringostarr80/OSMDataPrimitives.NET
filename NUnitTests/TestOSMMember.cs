using System.Collections.Generic;
using NUnit.Framework;
using OSMDataPrimitives;

namespace NUnit
{
    [TestFixture]
    public class TestOSMMember
    {
        private static OSMMember GetDefaultOSMMemberNode()
        {
            return new OSMMember(MemberType.Node, 1, "");
        }

        private static OSMMember GetDefaultOSMMemberWay()
        {
            return new OSMMember(MemberType.Way, 2, "outer");
        }

        private static OSMMember GetDefaultOSMMemberRelation()
        {
            return new OSMMember(MemberType.Relation, 3, "");
        }

        [Test]
        public void TestOSMMemberNode()
        {
            var memberNode = GetDefaultOSMMemberNode();

            Assert.That(memberNode.Type, Is.EqualTo(MemberType.Node));
            Assert.That(memberNode.Ref, Is.EqualTo(1));
            Assert.That(memberNode.Role, Is.Empty);
        }

        [Test]
        public void TestOSMMemberWay()
        {
            var memberWay = GetDefaultOSMMemberWay();

            Assert.That(memberWay.Type, Is.EqualTo(MemberType.Way));
            Assert.That(memberWay.Ref, Is.EqualTo(2));
            Assert.That(memberWay.Role, Is.EqualTo("outer"));
        }

        [Test]
        public void TestOSMMemberRelation()
        {
            var memberRelation = GetDefaultOSMMemberRelation();

            Assert.That(memberRelation.Type, Is.EqualTo(MemberType.Relation));
            Assert.That(memberRelation.Ref, Is.EqualTo(3));
            Assert.That(memberRelation.Role, Is.Empty);
        }

        [Test]
        public void TestOSMMemberNodeToDictionary()
        {
            var memberNode = GetDefaultOSMMemberNode();
            var expectedDictionary = new Dictionary<string, string>
            {
                { "type", "node" },
                { "ref", "1" },
                { "role", "" }
            };

            Assert.That(memberNode.ToDictionary(), Is.EqualTo(expectedDictionary));
        }

        [Test]
        public void TestOSMMemberWayToDictionary()
        {
            var memberWay = GetDefaultOSMMemberWay();
            var expectedDictionary = new Dictionary<string, string>
            {
                { "type", "way" },
                { "ref", "2" },
                { "role", "outer" }
            };

            Assert.That(memberWay.ToDictionary(), Is.EqualTo(expectedDictionary));
        }

        [Test]
        public void TestOSMMemberRelationToDictionary()
        {
            var memberRelation = GetDefaultOSMMemberRelation();
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
