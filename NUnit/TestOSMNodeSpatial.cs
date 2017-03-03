using System;
using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOSMNodeSpatial
	{
		private OSMNode GetDefaultOSMNode()
		{
			var node = new OSMNode(2) {
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

		private OSMNodeSpatial GetOSMNodeSpatial1()
		{
			// Position of Hamburg.
			var node = new OSMNodeSpatial(2, 53.553345, 9.992475);
			return node;
		}

		private OSMNodeSpatial GetOSMNodeSpatial2()
		{
			// Position of Munich
			var node = new OSMNodeSpatial(2, 48.136385, 11.577624);
			return node;
		}

		[Test]
		public void TestOSMNodeSpatialConstructorWithOSMNode()
		{
			var node = this.GetDefaultOSMNode();
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

			Assert.AreEqual(7, node.Changeset);
			Assert.AreEqual(8, nodeSpatial.Changeset);
			Assert.AreEqual(3, node.Version);
			Assert.AreEqual(4, nodeSpatial.Version);
			Assert.AreEqual(52.123456, node.Latitude);
			Assert.AreEqual(54.464456, nodeSpatial.Latitude);
			Assert.AreEqual(12.654321, node.Longitude);
			Assert.AreEqual(10.899996, nodeSpatial.Longitude);
			Assert.AreEqual(5, node.UserId);
			Assert.AreEqual(2, nodeSpatial.UserId);
			Assert.AreEqual("foo", node.UserName);
			Assert.AreEqual("bar", nodeSpatial.UserName);
			Assert.AreEqual(new DateTime(2017, 1, 20, 12, 03, 43, DateTimeKind.Utc), node.Timestamp);
			Assert.AreEqual(new DateTime(2017, 1, 25, 15, 20, 55, DateTimeKind.Utc), nodeSpatial.Timestamp);
			Assert.AreEqual("bar", node.Tags["name"]);
			Assert.AreEqual("hello", nodeSpatial.Tags["name"]);
			Assert.AreEqual("baz", node.Tags["ref"]);
			Assert.AreEqual("world", nodeSpatial.Tags["ref"]);
		}

		[Test]
		public void TestNodeDistanceCalculation()
		{
			var node1 = this.GetOSMNodeSpatial1();
			var node2 = this.GetOSMNodeSpatial2();

			var distance = node1.GetDistance(node2);
			Assert.AreEqual(613178, (int)distance);
		}

		[Test]
		public void TestNodeDirectionCalculation()
		{
			var node1 = this.GetOSMNodeSpatial1();
			var node2 = this.GetOSMNodeSpatial2();

			var direction = node1.GetDirection(node2);
			Assert.AreEqual(168, (int)direction);
		}

		public void TestNodeToWkt()
		{
			var node = this.GetOSMNodeSpatial1();
			var wkt = node.ToWkt();
			var expectedWkt = "POINT (53.553345 9.992475)";
			Assert.AreEqual(expectedWkt, wkt);
		}
	}
}
