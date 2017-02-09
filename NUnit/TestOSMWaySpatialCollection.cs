using NUnit.Framework;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOSMWaySpatialCollection
	{
		private OSMWaySpatial GetDefaultLine1()
		{
			var line = new OSMWaySpatial(1);
			line.Nodes.Add(new OSMNodeSpatial(1, 10, 10));
			line.Nodes.Add(new OSMNodeSpatial(2, 20, 20));
			line.Nodes.Add(new OSMNodeSpatial(3, 40, 10));

			return line;
		}

		private OSMWaySpatial GetDefaultLine2()
		{
			var line = new OSMWaySpatial(2);
			line.Nodes.Add(new OSMNodeSpatial(4, 40, 40));
			line.Nodes.Add(new OSMNodeSpatial(5, 30, 30));
			line.Nodes.Add(new OSMNodeSpatial(6, 20, 40));
			line.Nodes.Add(new OSMNodeSpatial(7, 10, 30));

			return line;
		}

		private OSMWaySpatial GetOuterPolygon1()
		{
			var polygon = new OSMWaySpatial(3);
			polygon.Nodes.Add(new OSMNodeSpatial(8, 20, 30));
			polygon.Nodes.Add(new OSMNodeSpatial(9, 40, 45));
			polygon.Nodes.Add(new OSMNodeSpatial(10, 40, 10));
			polygon.Nodes.Add(new OSMNodeSpatial(11, 20, 30));

			return polygon;
		}

		private OSMWaySpatial GetOuterPolygon2()
		{
			var polygon = new OSMWaySpatial(4);
			polygon.Nodes.Add(new OSMNodeSpatial(12, 5, 15));
			polygon.Nodes.Add(new OSMNodeSpatial(13, 10, 40));
			polygon.Nodes.Add(new OSMNodeSpatial(14, 20, 10));
			polygon.Nodes.Add(new OSMNodeSpatial(15, 10, 5));
			polygon.Nodes.Add(new OSMNodeSpatial(16, 5, 15));

			return polygon;
		}

		private OSMWaySpatial GetOuterPolygon3()
		{
			var polygon = new OSMWaySpatial(5);
			polygon.Nodes.Add(new OSMNodeSpatial(17, 40, 40));
			polygon.Nodes.Add(new OSMNodeSpatial(18, 45, 20));
			polygon.Nodes.Add(new OSMNodeSpatial(19, 30, 45));
			polygon.Nodes.Add(new OSMNodeSpatial(20, 40, 40));

			return polygon;
		}

		private OSMWaySpatial GetOuterPolygon4()
		{
			var polygon = new OSMWaySpatial(6);
			polygon.Nodes.Add(new OSMNodeSpatial(21, 35, 20));
			polygon.Nodes.Add(new OSMNodeSpatial(22, 30, 10));
			polygon.Nodes.Add(new OSMNodeSpatial(23, 10, 10));
			polygon.Nodes.Add(new OSMNodeSpatial(24, 5, 30));
			polygon.Nodes.Add(new OSMNodeSpatial(25, 20, 45));
			polygon.Nodes.Add(new OSMNodeSpatial(26, 35, 20));

			return polygon;
		}

		private OSMWaySpatial GetInnerPolygon1()
		{
			var polygon = new OSMWaySpatial(7) {
				Role = "inner"
			};
			polygon.Nodes.Add(new OSMNodeSpatial(27, 20, 30));
			polygon.Nodes.Add(new OSMNodeSpatial(28, 15, 20));
			polygon.Nodes.Add(new OSMNodeSpatial(29, 25, 20));
			polygon.Nodes.Add(new OSMNodeSpatial(30, 20, 30));

			return polygon;
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiLineString()
		{
			var line1 = this.GetDefaultLine1();
			var line2 = this.GetDefaultLine2();
			var multiLine = new OSMWaySpatialCollection();
			multiLine.Add(line1);
			multiLine.Add(line2);

			var expectedWkt = "MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))";
			Assert.AreEqual(expectedWkt, multiLine.ToWkt());
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiPolygon1()
		{
			var polygon1 = this.GetOuterPolygon1();
			var polygon2 = this.GetOuterPolygon2();
			var multiPolygon = new OSMWaySpatialCollection();
			multiPolygon.Add(polygon1);
			multiPolygon.Add(polygon2);

			var expectedWkt = "MULTIPOLYGON (((30 20, 10 40, 45 40, 30 20)), ((15 5, 5 10, 10 20, 40 10, 15 5)))";
			Assert.AreEqual(expectedWkt, multiPolygon.ToWkt(WktType.MultiPolygon));
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiPolygonWithInner()
		{
			var outerPolygon1 = this.GetOuterPolygon3();
			var outerPolygon2 = this.GetOuterPolygon4();
			var innerPolygon1 = this.GetInnerPolygon1();
			var multiPolygon = new OSMWaySpatialCollection();
			multiPolygon.Add(outerPolygon1);
			multiPolygon.Add(outerPolygon2);
			multiPolygon.Add(innerPolygon1);

			var expectedWkt = "MULTIPOLYGON (((40 40, 45 30, 20 45, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))";
			Assert.AreEqual(expectedWkt, multiPolygon.ToWkt(WktType.MultiPolygon));
		}
	}
}
