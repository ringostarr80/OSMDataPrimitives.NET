using NUnit.Framework;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOSMNodeSpatial
	{
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
	}
}
