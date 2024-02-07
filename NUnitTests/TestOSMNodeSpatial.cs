using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOSMNodeSpatial
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

		private static OSMNodeSpatial GetOSMNodeSpatial1()
		{
			// Position of Hamburg.
			var node = new OSMNodeSpatial(2, 53.553345, 9.992475);
			return node;
		}

		private static OSMNodeSpatial GetOSMNodeSpatial2()
		{
			// Position of Munich
			var node = new OSMNodeSpatial(2, 48.136385, 11.577624);
			return node;
		}

		[Test]
		public void TestOSMNodeSpatialConstructorWithOSMNode()
		{
			var node = GetDefaultOSMNode();
			var nodeSpatial = new OSMNodeSpatial(node);

			nodeSpatial.Changeset += 1;
			nodeSpatial.Version += 1;
			nodeSpatial.Latitude += 2.341;
			nodeSpatial.Longitude -= 1.754325;
			nodeSpatial.UserId = 2;
			nodeSpatial.UserName = "bar";
			nodeSpatial.Timestamp = new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc);
			nodeSpatial.Tags["name"] = "hello";
			nodeSpatial.Tags["ref"] = "world";

			Assert.That(node.Changeset, Is.EqualTo(7));
			Assert.That(nodeSpatial.Changeset, Is.EqualTo(8));
			Assert.That(node.Version, Is.EqualTo(3));
			Assert.That(nodeSpatial.Version, Is.EqualTo(4));
			Assert.That(node.Latitude, Is.EqualTo(52.123456));
			Assert.That(nodeSpatial.Latitude, Is.EqualTo(54.464456));
			Assert.That(node.Longitude, Is.EqualTo(12.654321));
			Assert.That(nodeSpatial.Longitude, Is.EqualTo(10.899996));
			Assert.That(node.UserId, Is.EqualTo(5));
			Assert.That(nodeSpatial.UserId, Is.EqualTo(2));
			Assert.That(node.UserName, Is.EqualTo("foo"));
			Assert.That(nodeSpatial.UserName, Is.EqualTo("bar"));
			Assert.That(node.Timestamp, Is.EqualTo(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc)));
			Assert.That(nodeSpatial.Timestamp, Is.EqualTo(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc)));
			Assert.That(node.Tags["name"], Is.EqualTo("bar"));
			Assert.That(nodeSpatial.Tags["name"], Is.EqualTo("hello"));
			Assert.That(node.Tags["ref"], Is.EqualTo("baz"));
			Assert.That(nodeSpatial.Tags["ref"], Is.EqualTo("world"));
		}

		[Test]
		public void TestNodeDistanceCalculation()
		{
			var node1 = GetOSMNodeSpatial1();
			var node2 = GetOSMNodeSpatial2();

			var distance = node1.GetDistance(node2);
			Assert.That((int)distance, Is.EqualTo(613178));
		}

		[Test]
		public void TestNodeDirectionCalculation()
		{
			var node1 = GetOSMNodeSpatial1();
			var node2 = GetOSMNodeSpatial2();

			var direction = node1.GetDirection(node2);
			Assert.That((int)direction, Is.EqualTo(168));
		}

		[Test]
		public void TestNodeToWkt()
		{
			var node = GetOSMNodeSpatial1();
			var wkt = node.ToWkt();
			var expectedWkt = "POINT (9.992475 53.553345)";
			Assert.That(wkt, Is.EqualTo(expectedWkt));
		}
	}
}
