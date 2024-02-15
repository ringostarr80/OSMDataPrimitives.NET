using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestOsmWaySpatialCollection
	{
		private static OsmWaySpatial GetDefaultLine1()
		{
			var line = new OsmWaySpatial(1);
			line.Nodes.Add(new OsmNodeSpatial(1, 10, 10));
			line.Nodes.Add(new OsmNodeSpatial(2, 20, 20));
			line.Nodes.Add(new OsmNodeSpatial(3, 40, 10));

			return line;
		}

		private static OsmWaySpatial GetDefaultLine2()
		{
			var line = new OsmWaySpatial(2);
			line.Nodes.Add(new OsmNodeSpatial(4, 40, 40));
			line.Nodes.Add(new OsmNodeSpatial(5, 30, 30));
			line.Nodes.Add(new OsmNodeSpatial(6, 20, 40));
			line.Nodes.Add(new OsmNodeSpatial(7, 10, 30));

			return line;
		}

		private static OsmWaySpatial GetOuterPolygon1()
		{
			var polygon = new OsmWaySpatial(3);
			polygon.Nodes.Add(new OsmNodeSpatial(8, 20, 30));
			polygon.Nodes.Add(new OsmNodeSpatial(9, 40, 45));
			polygon.Nodes.Add(new OsmNodeSpatial(10, 40, 10));
			polygon.Nodes.Add(new OsmNodeSpatial(11, 20, 30));

			return polygon;
		}

		private static OsmWaySpatial GetOuterPolygon2()
		{
			var polygon = new OsmWaySpatial(4);
			polygon.Nodes.Add(new OsmNodeSpatial(12, 5, 15));
			polygon.Nodes.Add(new OsmNodeSpatial(13, 10, 40));
			polygon.Nodes.Add(new OsmNodeSpatial(14, 20, 10));
			polygon.Nodes.Add(new OsmNodeSpatial(15, 10, 5));
			polygon.Nodes.Add(new OsmNodeSpatial(16, 5, 15));

			return polygon;
		}

		private static OsmWaySpatial GetOuterPolygon3()
		{
			var polygon = new OsmWaySpatial(5);
			polygon.Nodes.Add(new OsmNodeSpatial(17, 40, 40));
			polygon.Nodes.Add(new OsmNodeSpatial(18, 45, 20));
			polygon.Nodes.Add(new OsmNodeSpatial(19, 30, 45));
			polygon.Nodes.Add(new OsmNodeSpatial(20, 40, 40));

			return polygon;
		}

		private static OsmWaySpatial GetOuterPolygon4()
		{
			var polygon = new OsmWaySpatial(6);
			polygon.Nodes.Add(new OsmNodeSpatial(21, 35, 20));
			polygon.Nodes.Add(new OsmNodeSpatial(22, 30, 10));
			polygon.Nodes.Add(new OsmNodeSpatial(23, 10, 10));
			polygon.Nodes.Add(new OsmNodeSpatial(24, 5, 30));
			polygon.Nodes.Add(new OsmNodeSpatial(25, 20, 45));
			polygon.Nodes.Add(new OsmNodeSpatial(26, 35, 20));

			return polygon;
		}

		private static OsmWaySpatial GetInnerPolygon1()
		{
			var polygon = new OsmWaySpatial(7) {
				Role = "inner"
			};
			polygon.Nodes.Add(new OsmNodeSpatial(27, 20, 30));
			polygon.Nodes.Add(new OsmNodeSpatial(28, 15, 20));
			polygon.Nodes.Add(new OsmNodeSpatial(29, 25, 20));
			polygon.Nodes.Add(new OsmNodeSpatial(30, 20, 30));

			return polygon;
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiLineString()
		{
			var line1 = GetDefaultLine1();
			var line2 = GetDefaultLine2();
			var multiLine = new OsmWaySpatialCollection {
				line1,
				line2
			};
			var expectedWkt = "MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))";
			Assert.That(multiLine.ToWkt(), Is.EqualTo(expectedWkt));
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiPolygon1()
		{
			var polygon1 = GetOuterPolygon1();
			var polygon2 = GetOuterPolygon2();
			var multiPolygon = new OsmWaySpatialCollection {
				polygon1,
				polygon2
			};
			var expectedWkt = "MULTIPOLYGON (((30 20, 10 40, 45 40, 30 20)), ((15 5, 5 10, 10 20, 40 10, 15 5)))";
			Assert.That(multiPolygon.ToWkt(WktType.MultiPolygon), Is.EqualTo(expectedWkt));
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiPolygonWithInner()
		{
			var outerPolygon1 = GetOuterPolygon3();
			var outerPolygon2 = GetOuterPolygon4();
			var innerPolygon1 = GetInnerPolygon1();
			var multiPolygon = new OsmWaySpatialCollection {
				outerPolygon1,
				outerPolygon2,
				innerPolygon1
			};
			var expectedWkt = "MULTIPOLYGON (((40 40, 45 30, 20 45, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))";
			Assert.That(multiPolygon.ToWkt(WktType.MultiPolygon), Is.EqualTo(expectedWkt));
		}

		[Test]
		public void TestOSMWaySpatialCollectionMultiPolygonWithNoData()
		{
			Assert.Throws<DataException>(() => {
				var multiPolygon = new OsmWaySpatialCollection();
				multiPolygon.ToWkt(WktType.MultiPolygon);
			});
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge2SimpleWays()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, 1),
					new OsmNodeSpatial(4, 0, 2)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				new OsmWaySpatial(1) {
					Nodes = {
						new OsmNodeSpatial(1, 0, 0),
						new OsmNodeSpatial(2, 0, 1),
						new OsmNodeSpatial(4, 0, 2)
					}
				}
			};

			Assert.That(mergedWay.Count, Is.EqualTo(1));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge2SimpleWaysWhereOneIsReversed()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, 2),
					new OsmNodeSpatial(4, 0, 1)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				new OsmWaySpatial(1) {
					Nodes = {
						new OsmNodeSpatial(1, 0, 0),
						new OsmNodeSpatial(2, 0, 1),
						new OsmNodeSpatial(3, 0, 2)
					}
				}
			};

			Assert.That(mergedWay.Count, Is.EqualTo(1));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge2SimpleWaysWhereSecondIsPrependedToTheFirst()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, 0),
					new OsmNodeSpatial(4, 0, -1)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				new OsmWaySpatial(1) {
					Nodes = {
						new OsmNodeSpatial(4, 0, -1),
						new OsmNodeSpatial(1, 0, 0),
						new OsmNodeSpatial(2, 0, 1)
					}
				}
			};

			Assert.That(mergedWay.Count, Is.EqualTo(1));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge2SimpleWaysWhereSecondIsPrependedToTheFirstReversed()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, -1),
					new OsmNodeSpatial(4, 0, 0)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				new OsmWaySpatial(1) {
					Nodes = {
						new OsmNodeSpatial(3, 0, -1),
						new OsmNodeSpatial(1, 0, 0),
						new OsmNodeSpatial(2, 0, 1)
					}
				}
			};

			Assert.That(mergedWay.Count, Is.EqualTo(1));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge3SimpleWays()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, 1),
					new OsmNodeSpatial(4, 0, 2)
				}
			};
			var way3 = new OsmWaySpatial(3) {
				Nodes = {
					new OsmNodeSpatial(5, 0, 2),
					new OsmNodeSpatial(6, 0, 3)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2,
				way3
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				new OsmWaySpatial(1) {
					Nodes = {
						new OsmNodeSpatial(1, 0, 0),
						new OsmNodeSpatial(2, 0, 1),
						new OsmNodeSpatial(4, 0, 2),
						new OsmNodeSpatial(6, 0, 3)
					}
				}
			};

			Assert.That(mergedWay.Count, Is.EqualTo(1));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge3SimpleWaysWhereTheMiddleWayIsReversed()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, 2),
					new OsmNodeSpatial(4, 0, 1)
				}
			};
			var way3 = new OsmWaySpatial(3) {
				Nodes = {
					new OsmNodeSpatial(5, 0, 2),
					new OsmNodeSpatial(6, 0, 3)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2,
				way3
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				new OsmWaySpatial(1) {
					Nodes = {
						new OsmNodeSpatial(1, 0, 0),
						new OsmNodeSpatial(2, 0, 1),
						new OsmNodeSpatial(3, 0, 2),
						new OsmNodeSpatial(6, 0, 3)
					}
				}
			};

			Assert.That(mergedWay.Count, Is.EqualTo(1));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}

		[Test]
		public void TestOsmWaySpatialCollectionMerge2SimpleWaysThatCanNotBeenMerged()
		{
			var way1 = new OsmWaySpatial(1) {
				Nodes = {
					new OsmNodeSpatial(1, 0, 0),
					new OsmNodeSpatial(2, 0, 1)
				}
			};
			var way2 = new OsmWaySpatial(2) {
				Nodes = {
					new OsmNodeSpatial(3, 0, 2),
					new OsmNodeSpatial(4, 0, 3)
				}
			};
			var mergedWay = new OsmWaySpatialCollection {
				way1,
				way2
			}.Merge();

			var expectedMergedWay = new OsmWaySpatialCollection {
				(OsmWaySpatial)way1.Clone(),
				(OsmWaySpatial)way2.Clone()
			};

			Assert.That(mergedWay.Count, Is.EqualTo(2));
			Assert.That(mergedWay, Is.EqualTo(expectedMergedWay));
		}
	}
}
