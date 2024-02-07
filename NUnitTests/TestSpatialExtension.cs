using System;
using System.Xml;

using NUnit.Framework;
using OSMDataPrimitives;
using OSMDataPrimitives.Spatial;

namespace NUnit
{
	[TestFixture]
	public class TestSpatialExtension
	{
		private static XmlElement BuildOsmXmlNode()
		{
			var xml = new XmlDocument();
			var xmlNode = xml.CreateElement("node");
			xmlNode.SetAttribute("id", "1");
			xmlNode.SetAttribute("changeset", "2");
			xmlNode.SetAttribute("version", "3");
			xmlNode.SetAttribute("uid", "4");
			xmlNode.SetAttribute("user", "unknown");
			xmlNode.SetAttribute("timestamp", "2023-06-01T12:00:00");
			xmlNode.SetAttribute("lat", "52.1234");
			xmlNode.SetAttribute("lon", "12.4321");
			var xmlTag = xml.CreateElement("tag");
			xmlTag.SetAttribute("k", "building");
			xmlTag.SetAttribute("v", "house");
			xmlNode.AppendChild(xmlTag);

			return xmlNode;
		}

		private static XmlElement BuildOsmXmlWay()
		{
			var xml = new XmlDocument();
			var xmlWay = xml.CreateElement("way");
			xmlWay.SetAttribute("id", "1");
			xmlWay.SetAttribute("changeset", "2");
			xmlWay.SetAttribute("version", "3");
			xmlWay.SetAttribute("uid", "4");
			xmlWay.SetAttribute("user", "unknown");
			xmlWay.SetAttribute("timestamp", "2023-06-01T12:00:00");
			var xmlTag = xml.CreateElement("tag");
			xmlTag.SetAttribute("k", "building");
			xmlTag.SetAttribute("v", "house");
			xmlWay.AppendChild(xmlTag);
			var xmlNd1 = xml.CreateElement("nd");
			xmlNd1.SetAttribute("ref", "10");
			var xmlNd2 = xml.CreateElement("nd");
			xmlNd2.SetAttribute("ref", "11");
			xmlWay.AppendChild(xmlNd1);
			xmlWay.AppendChild(xmlNd2);

			return xmlWay;
		}

		private static XmlElement BuildOsmXmlRelation()
		{
			var xml = new XmlDocument();
			var xmlRelation = xml.CreateElement("relation");
			xmlRelation.SetAttribute("id", "1");
			xmlRelation.SetAttribute("changeset", "2");
			xmlRelation.SetAttribute("version", "3");
			xmlRelation.SetAttribute("uid", "4");
			xmlRelation.SetAttribute("user", "unknown");
			xmlRelation.SetAttribute("timestamp", "2023-06-01T12:00:00");
			var xmlTag = xml.CreateElement("tag");
			xmlTag.SetAttribute("k", "building");
			xmlTag.SetAttribute("v", "house");
			xmlRelation.AppendChild(xmlTag);
			var xmlNodeMember = xml.CreateElement("member");
			xmlNodeMember.SetAttribute("type", "node");
			xmlNodeMember.SetAttribute("ref", "10");
			xmlNodeMember.SetAttribute("role", "");
			var xmlWayMember = xml.CreateElement("member");
			xmlWayMember.SetAttribute("type", "way");
			xmlWayMember.SetAttribute("ref", "20");
			xmlWayMember.SetAttribute("role", "");
			var xmlRelationMember = xml.CreateElement("member");
			xmlRelationMember.SetAttribute("type", "relation");
			xmlRelationMember.SetAttribute("ref", "20");
			xmlRelationMember.SetAttribute("role", "");
			xmlRelation.AppendChild(xmlNodeMember);
			xmlRelation.AppendChild(xmlWayMember);
			xmlRelation.AppendChild(xmlRelationMember);

			return xmlRelation;
		}

		[Test]
		public void TestXmlNodeToOSMSpatialElement()
		{
			var xmlNode = BuildOsmXmlNode();
			var spatialElement = xmlNode.ToOSMSpatialElement();
			if (spatialElement is OsmNode osmNode) {
				Assert.That(osmNode.Id, Is.EqualTo(1));
				Assert.That(osmNode.Changeset, Is.EqualTo(2));
				Assert.That(osmNode.Version, Is.EqualTo(3));
				Assert.That(osmNode.UserId, Is.EqualTo(4));
				Assert.That(osmNode.UserName, Is.EqualTo("unknown"));
				Assert.That(osmNode.Timestamp, Is.EqualTo(new DateTime(2023, 6, 1, 12, 0, 0)));
				Assert.That(osmNode.Latitude, Is.EqualTo(52.1234));
				Assert.That(osmNode.Longitude, Is.EqualTo(12.4321));
				Assert.That(osmNode.Tags.Count, Is.EqualTo(1));
				Assert.That(osmNode.Tags["building"], Is.EqualTo("house"));
			}
        }

		[Test]
		public void TestXmlNodeStringToOSMSpatialElement()
		{
			var xmlNode = BuildOsmXmlNode();
			var spatialElement = xmlNode.OuterXml.ToOSMSpatialElement();
			if (spatialElement is OsmNode osmNode) {
				Assert.That(osmNode.Id, Is.EqualTo(1));
				Assert.That(osmNode.Changeset, Is.EqualTo(2));
				Assert.That(osmNode.Version, Is.EqualTo(3));
				Assert.That(osmNode.UserId, Is.EqualTo(4));
				Assert.That(osmNode.UserName, Is.EqualTo("unknown"));
				Assert.That(osmNode.Timestamp, Is.EqualTo(new DateTime(2023, 6, 1, 12, 0, 0)));
				Assert.That(osmNode.Latitude, Is.EqualTo(52.1234));
				Assert.That(osmNode.Longitude, Is.EqualTo(12.4321));
				Assert.That(osmNode.Tags.Count, Is.EqualTo(1));
				Assert.That(osmNode.Tags["building"], Is.EqualTo("house"));
			}
        }

		[Test]
		public void TestOSMNodeToWKT()
		{
			var node = new OsmNode(1, 52.1234, 12.4321);
			Assert.That(node.ToWkt(), Is.EqualTo("POINT (12.4321 52.1234)"));
		}

		[Test]
		public void TestOSMNodeSpatialToWKT()
		{
			var node = new OsmNodeSpatial(1, 52.1234, 12.4321);
			Assert.That(node.ToWkt(), Is.EqualTo("POINT (12.4321 52.1234)"));
		}

		[Test]
		public void TestXmlWayToOSMSpatialElement()
		{
			var xmlWay = BuildOsmXmlWay();
			var spatialElement = xmlWay.ToOSMSpatialElement();
			if (spatialElement is OsmWay osmWay) {
				Assert.That(osmWay.Id, Is.EqualTo(1));
				Assert.That(osmWay.Changeset, Is.EqualTo(2));
				Assert.That(osmWay.Version, Is.EqualTo(3));
				Assert.That(osmWay.UserId, Is.EqualTo(4));
				Assert.That(osmWay.UserName, Is.EqualTo("unknown"));
				Assert.That(osmWay.Timestamp, Is.EqualTo(new DateTime(2023, 6, 1, 12, 0, 0)));
				Assert.That(osmWay.Tags.Count, Is.EqualTo(1));
				Assert.That(osmWay.Tags["building"], Is.EqualTo("house"));
				Assert.That(osmWay.NodeRefs.Count, Is.EqualTo(2));
			}
        }

		[Test]
		public void TestXmlWayStringToOSMSpatialElement()
		{
			var xmlWay = BuildOsmXmlWay();
			var spatialElement = xmlWay.OuterXml.ToOSMSpatialElement();
			if (spatialElement is OsmWay osmWay) {
				Assert.That(osmWay.Id, Is.EqualTo(1));
				Assert.That(osmWay.Changeset, Is.EqualTo(2));
				Assert.That(osmWay.Version, Is.EqualTo(3));
				Assert.That(osmWay.UserId, Is.EqualTo(4));
				Assert.That(osmWay.UserName, Is.EqualTo("unknown"));
				Assert.That(osmWay.Timestamp, Is.EqualTo(new DateTime(2023, 6, 1, 12, 0, 0)));
				Assert.That(osmWay.Tags.Count, Is.EqualTo(1));
				Assert.That(osmWay.Tags["building"], Is.EqualTo("house"));
				Assert.That(osmWay.NodeRefs.Count, Is.EqualTo(2));
			}
        }

		[Test]
		public void TestOSMWayToWKT()
		{
			var way = new OsmWaySpatial(1);
			way.Nodes.Add(new OsmNodeSpatial(2, 52.1234, 10.4321));
			way.Nodes.Add(new OsmNodeSpatial(3, 52.4321, 10.1234));
			Assert.That(way.ToWkt(WktType.LineString), Is.EqualTo("LINESTRING (10.4321 52.1234, 10.1234 52.4321)"));
		}

		[Test]
		public void TestXmlRelationToOSMSpatialElement()
		{
			var xmlRelation = BuildOsmXmlRelation();
			var spatialElement = xmlRelation.ToOSMSpatialElement();
			if (spatialElement is OsmRelation osmRelation) {
				Assert.That(osmRelation.Id, Is.EqualTo(1));
				Assert.That(osmRelation.Changeset, Is.EqualTo(2));
				Assert.That(osmRelation.Version, Is.EqualTo(3));
				Assert.That(osmRelation.UserId, Is.EqualTo(4));
				Assert.That(osmRelation.UserName, Is.EqualTo("unknown"));
				Assert.That(osmRelation.Timestamp, Is.EqualTo(new DateTime(2023, 6, 1, 12, 0, 0)));
				Assert.That(osmRelation.Tags.Count, Is.EqualTo(1));
				Assert.That(osmRelation.Tags["building"], Is.EqualTo("house"));
				Assert.That(osmRelation.Members.Count, Is.EqualTo(3));
			}
        }

		[Test]
		public void TestXmlRelationStringToOSMSpatialElement()
		{
			var xmlRelation = BuildOsmXmlRelation();
			var spatialElement = xmlRelation.OuterXml.ToOSMSpatialElement();
			if (spatialElement is OsmRelation osmRelation) {
				Assert.That(osmRelation.Id, Is.EqualTo(1));
				Assert.That(osmRelation.Changeset, Is.EqualTo(2));
				Assert.That(osmRelation.Version, Is.EqualTo(3));
				Assert.That(osmRelation.UserId, Is.EqualTo(4));
				Assert.That(osmRelation.UserName, Is.EqualTo("unknown"));
				Assert.That(osmRelation.Timestamp, Is.EqualTo(new DateTime(2023, 6, 1, 12, 0, 0)));
				Assert.That(osmRelation.Tags.Count, Is.EqualTo(1));
				Assert.That(osmRelation.Tags["building"], Is.EqualTo("house"));
				Assert.That(osmRelation.Members.Count, Is.EqualTo(3));
			}
        }
    }
}
