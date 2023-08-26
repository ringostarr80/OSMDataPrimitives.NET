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
			var xmlMember = xml.CreateElement("member");
			xmlMember.SetAttribute("type", "way");
			xmlMember.SetAttribute("ref", "1");
			xmlMember.SetAttribute("role", "");
			xmlRelation.AppendChild(xmlMember);

			return xmlRelation;
		}

		[Test]
		public void TestXmlNodeToOSMSpatialElement()
		{
			var xmlNode = BuildOsmXmlNode();
			var spatialElement = xmlNode.ToOSMSpatialElement();
			if (spatialElement is OSMNode osmNode) {
				Assert.AreEqual(1, osmNode.Id);
				Assert.AreEqual(2, osmNode.Changeset);
				Assert.AreEqual(3, osmNode.Version);
				Assert.AreEqual(4, osmNode.UserId);
				Assert.AreEqual("unknown", osmNode.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmNode.Timestamp);
				Assert.AreEqual(52.1234, osmNode.Latitude);
				Assert.AreEqual(12.4321, osmNode.Longitude);
				Assert.AreEqual(1, osmNode.Tags.Count);
				Assert.AreEqual("house", osmNode.Tags["building"]);
			}
        }

		[Test]
		public void TestXmlNodeStringToOSMSpatialElement()
		{
			var xmlNode = BuildOsmXmlNode();
			var spatialElement = xmlNode.OuterXml.ToOSMSpatialElement();
			if (spatialElement is OSMNode osmNode) {
				Assert.AreEqual(1, osmNode.Id);
				Assert.AreEqual(2, osmNode.Changeset);
				Assert.AreEqual(3, osmNode.Version);
				Assert.AreEqual(4, osmNode.UserId);
				Assert.AreEqual("unknown", osmNode.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmNode.Timestamp);
				Assert.AreEqual(52.1234, osmNode.Latitude);
				Assert.AreEqual(12.4321, osmNode.Longitude);
				Assert.AreEqual(1, osmNode.Tags.Count);
				Assert.AreEqual("house", osmNode.Tags["building"]);
			}
        }

		[Test]
		public void TestOSMNodeToWKT()
		{
			var node = new OSMNode(1, 52.1234, 12.4321);
			Assert.AreEqual("POINT (12.4321 52.1234)", node.ToWkt());
		}

		[Test]
		public void TestXmlWayToOSMSpatialElement()
		{
			var xmlWay = BuildOsmXmlWay();
			var spatialElement = xmlWay.ToOSMSpatialElement();
			if (spatialElement is OSMWay osmWay) {
				Assert.AreEqual(1, osmWay.Id);
				Assert.AreEqual(2, osmWay.Changeset);
				Assert.AreEqual(3, osmWay.Version);
				Assert.AreEqual(4, osmWay.UserId);
				Assert.AreEqual("unknown", osmWay.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmWay.Timestamp);
				Assert.AreEqual(1, osmWay.Tags.Count);
				Assert.AreEqual("house", osmWay.Tags["building"]);
				Assert.AreEqual(2, osmWay.NodeRefs.Count);
			}
        }

		[Test]
		public void TestXmlWayStringToOSMSpatialElement()
		{
			var xmlWay = BuildOsmXmlWay();
			var spatialElement = xmlWay.OuterXml.ToOSMSpatialElement();
			if (spatialElement is OSMWay osmWay) {
				Assert.AreEqual(1, osmWay.Id);
				Assert.AreEqual(2, osmWay.Changeset);
				Assert.AreEqual(3, osmWay.Version);
				Assert.AreEqual(4, osmWay.UserId);
				Assert.AreEqual("unknown", osmWay.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmWay.Timestamp);
				Assert.AreEqual(1, osmWay.Tags.Count);
				Assert.AreEqual("house", osmWay.Tags["building"]);
				Assert.AreEqual(2, osmWay.NodeRefs.Count);
			}
        }

		[Test]
		public void TestOSMWayToWKT()
		{
			var way = new OSMWaySpatial(1);
			way.Nodes.Add(new OSMNodeSpatial(2, 52.1234, 10.4321));
			way.Nodes.Add(new OSMNodeSpatial(3, 52.4321, 10.1234));
			Assert.AreEqual("LINESTRING (10.4321 52.1234, 10.1234 52.4321)", way.ToWkt());
		}

		[Test]
		public void TestXmlRelationToOSMSpatialElement()
		{
			var xmlRelation = BuildOsmXmlRelation();
			var spatialElement = xmlRelation.ToOSMSpatialElement();
			if (spatialElement is OSMRelation osmRelation) {
				Assert.AreEqual(1, osmRelation.Id);
				Assert.AreEqual(2, osmRelation.Changeset);
				Assert.AreEqual(3, osmRelation.Version);
				Assert.AreEqual(4, osmRelation.UserId);
				Assert.AreEqual("unknown", osmRelation.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmRelation.Timestamp);
				Assert.AreEqual(1, osmRelation.Tags.Count);
				Assert.AreEqual("house", osmRelation.Tags["building"]);
				Assert.AreEqual(1, osmRelation.Members.Count);
			}
        }

		[Test]
		public void TestXmlRelationStringToOSMSpatialElement()
		{
			var xmlRelation = BuildOsmXmlRelation();
			var spatialElement = xmlRelation.OuterXml.ToOSMSpatialElement();
			if (spatialElement is OSMRelation osmRelation) {
				Assert.AreEqual(1, osmRelation.Id);
				Assert.AreEqual(2, osmRelation.Changeset);
				Assert.AreEqual(3, osmRelation.Version);
				Assert.AreEqual(4, osmRelation.UserId);
				Assert.AreEqual("unknown", osmRelation.UserName);
				Assert.AreEqual(new DateTime(2023, 6, 1, 12, 0, 0), osmRelation.Timestamp);
				Assert.AreEqual(1, osmRelation.Tags.Count);
				Assert.AreEqual("house", osmRelation.Tags["building"]);
				Assert.AreEqual(1, osmRelation.Members.Count);
			}
        }
    }
}
