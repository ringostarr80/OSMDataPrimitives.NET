﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace OSMDataPrimitives.Spatial
{
    /// <summary>
    /// Extension to provide WKT-capabilities (Well-Known-Text).
    /// </summary>
    public static class Extension
    {
        private static void SetOSMGeneralProperties(IOsmElement osmElement, XmlElement element)
        {
            var changesetAttribute = element.Attributes.GetNamedItem("changeset");
            if (changesetAttribute != null)
            {
                osmElement.Changeset = Convert.ToUInt64(changesetAttribute.Value);
            }
            var versionAttribute = element.Attributes.GetNamedItem("version");
            if (versionAttribute != null)
            {
                osmElement.Version = Convert.ToUInt64(versionAttribute.Value);
            }
            var uidAttribute = element.Attributes.GetNamedItem("uid");
            if (uidAttribute != null)
            {
                osmElement.UserId = Convert.ToUInt64(uidAttribute.Value);
            }
            var userAttribute = element.Attributes.GetNamedItem("user");
            if (userAttribute != null)
            {
                osmElement.UserName = userAttribute.Value;
            }
            var timestampAttribute = element.Attributes.GetNamedItem("timestamp");
            if (timestampAttribute != null)
            {
                osmElement.Timestamp = DateTime.Parse(timestampAttribute.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }
        }

        private static void SetOSMNodeProperties(OSMNodeSpatial node, XmlElement element)
        {
            var latAttribute = element.Attributes.GetNamedItem("lat");
            if (latAttribute != null)
            {
                node.Latitude = Convert.ToDouble(latAttribute.Value, CultureInfo.InvariantCulture);
            }
            var lonAttribute = element.Attributes.GetNamedItem("lon");
            if (lonAttribute != null)
            {
                node.Longitude = Convert.ToDouble(lonAttribute.Value, CultureInfo.InvariantCulture);
            }
        }

        private static void SetOSMWayProperties(OSMWaySpatial way, XmlElement element)
        {
            foreach (XmlNode childNode in element.ChildNodes)
            {
                if (childNode.Name != "nd")
                {
                    continue;
                }

                var refAttribute = childNode.Attributes.GetNamedItem("ref");
                if (refAttribute != null)
                {
                    way.NodeRefs.Add(Convert.ToInt64(refAttribute.Value));
                }
            }
        }

        private static void SetOSMRelationProperties(OsmRelation relation, XmlElement element)
        {
            foreach (XmlNode childNode in element.ChildNodes)
            {
                if (childNode.Name != "member")
                {
                    continue;
                }

                var typeAttribute = childNode.Attributes.GetNamedItem("type");
                var refAttribute = childNode.Attributes.GetNamedItem("ref");
                var roleAttribute = childNode.Attributes.GetNamedItem("role");
                if (typeAttribute is not null && refAttribute is not null && roleAttribute is not null)
                {
                    MemberType? memberType = typeAttribute.Value switch {
                        "node" => MemberType.Node,
                        "way" => MemberType.Way,
                        "relation" => MemberType.Relation,
                        _ => null
                    };
                    if (!memberType.HasValue)
                    {
                        throw new XmlException("invalid xml-attribute value (" + typeAttribute.Value + ") for 'member[@type]'.");
                    }

                    var refValue = Convert.ToUInt64(refAttribute.Value);
                    relation.Members.Add(new OsmMember(memberType.Value, refValue, roleAttribute.Value));
                }
            }
        }

        /// <summary>
        /// Converts the XmlElement to an OSMElement.
        /// </summary>
        /// <returns>The OSMElement.</returns>
        /// <param name="element">XmlElement.</param>
        public static IOsmElement ToOSMSpatialElement(this XmlElement element)
        {
            var idAttribute = element.Attributes.GetNamedItem("id") ?? throw new XmlException("Missing required xml-attribute 'id'.");
            var id = Convert.ToUInt64(idAttribute.Value);
            IOsmElement osmElement = element.Name switch
            {
                "node" => new OSMNodeSpatial(id),
                "way" => new OSMWaySpatial(id),
                "relation" => new OsmRelation(id),
                _ => throw new XmlException("Invalid xml-element name '" + element.Name + "'. Expected 'node', 'way' or 'relation'."),
            };

            SetOSMGeneralProperties(osmElement, element);

            if (osmElement is OSMNodeSpatial nodeElement)
            {
                SetOSMNodeProperties(nodeElement, element);
            }
            else if (osmElement is OSMWaySpatial wayElement && element.HasChildNodes)
            {
                SetOSMWayProperties(wayElement, element);
            }
            else if (osmElement is OsmRelation relationElement && element.HasChildNodes)
            {
                SetOSMRelationProperties(relationElement, element);
            }

            foreach (XmlNode childNode in element.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "tag":
                        var kAttribute = childNode.Attributes.GetNamedItem("k");
                        var vAttribute = childNode.Attributes.GetNamedItem("v");
                        if (kAttribute is not null && vAttribute is not null)
                        {
                            osmElement.Tags.Add(kAttribute.Value, vAttribute.Value);
                        }
                        break;
                }
            }

            return osmElement;
        }

        /// <summary>
        /// Converts the Xml-String to an OSMElement.
        /// </summary>
        /// <returns>The OSMElement.</returns>
        /// <param name="element">Xml-String.</param>
        public static IOsmElement ToOSMSpatialElement(this string element)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(element);
            return xmlDoc.DocumentElement.ToOSMSpatialElement();
        }

        /// <summary>
        /// Converts the OSMNode to WKT-string.
        /// </summary>
        /// <returns>The WKT-string.</returns>
        /// <param name="node">Node.</param>
        public static string ToWkt(this OsmNode node)
        {
            return string.Format("POINT ({0})", node.ToWktPart());
        }

        /// <summary>
        /// Converts the OSMNodeSpatial to WKT-string.
        /// </summary>
        /// <returns>The WKT-string.</returns>
        /// <param name="node">Node.</param>
        public static string ToWkt(this OSMNodeSpatial node)
        {
            return string.Format("POINT ({0})", node.ToWktPart());
        }

        /// <summary>
        /// Converts the OSMWaySpatial to WKT-string.
        /// </summary>
        /// <returns>The WKT-string.</returns>
        /// <param name="way">Way.</param>
        /// <param name="wktType">WktType.</param>
        public static string ToWkt(this OSMWaySpatial way, WktType? wktType = null)
        {
            var wktTypeString = (way.IsClosed) ? "POLYGON" : "LINESTRING";
            if (wktType.HasValue)
            {
                wktTypeString = wktType.Value switch
                {
                    WktType.LineString or WktType.Polygon => wktType.Value.ToString().ToUpper(),
                    _ => throw new XmlException("invalid wktType-value (" + wktType.Value + ") for 'OSMWaySpatial.ToWkt(WktType? wktType = null)'."),
                };
            }

            var result = wktTypeString;
            result += (wktTypeString == "POLYGON") ? " ((" : " (";
            result += way.ToWktPart();
            result += (wktTypeString == "POLYGON") ? "))" : ")";

            return result;
        }

        /// <summary>
        /// Converts the OSMWaySpatialCollection to WKT-string.
        /// </summary>
        /// <returns>The WKT-string.</returns>
        /// <param name="ways">Ways.</param>
        /// <param name="wktType">WktType.</param>
        public static string ToWkt(this OSMWaySpatialCollection ways, WktType? wktType = null)
        {
            if (wktType.HasValue) {
                return wktType.Value switch {
                    WktType.MultiLineString => ToWktMultiLineString(ways),
                    WktType.MultiPolygon => ToWktMultiPolygon(ways),
                    _ => throw new XmlException("invalid wktType-value (" + wktType.Value + ") for 'OSMWaySpatialCollection.ToWkt(WktType? wktType = null)'.")
                };
            }

            return ToWktMultiLineString(ways);
        }

        /// <summary>
        /// Converts the OSMWaySpatialCollection to WKT-string.
        /// </summary>
        /// <returns>The WKT-string.</returns>
        /// <param name="ways">Ways.</param>
        public static string ToWktMultiLineString(this OSMWaySpatialCollection ways)
        {
            var resultSB = new StringBuilder();
            resultSB.Append("MULTILINESTRING");
            resultSB.Append(" (");
            var wayCounter = 0;

            foreach (var way in ways)
            {
                wayCounter++;
                if (wayCounter > 1)
                {
                    resultSB.Append(", ");
                }
                resultSB.Append("(" + way.ToWktPart() + ")");
            }

            resultSB.Append(')');

            return resultSB.ToString();
        }

        /// <summary>
        /// Converts the OSMWaySpatialCollection to WKT-string.
        /// </summary>
        /// <returns>The WKT-string.</returns>
        /// <param name="ways">Ways.</param>
        public static string ToWktMultiPolygon(this OSMWaySpatialCollection ways)
        {
            var resultSB = new StringBuilder();
            resultSB.Append("MULTIPOLYGON");
            resultSB.Append(" (");
            var wayCounter = 0;

            var outerWays = new OSMWaySpatialCollection(ways.Where(w => w.Role == "outer")).Merge();
            outerWays.RemoveInvalidPolygons();
            outerWays.EnsurePolygonDirection();
            var innerWays = new OSMWaySpatialCollection(ways.Where(w => w.Role == "inner")).Merge();
            innerWays.RemoveInvalidPolygons();
            innerWays.EnsurePolygonDirection();

            if (outerWays.Count == 0)
            {
                throw new DataException("invalid polygon data.");
            }

            foreach (var outerWay in outerWays)
            {
                wayCounter++;
                if (wayCounter > 1)
                {
                    resultSB.Append(", ");
                }
                resultSB.Append("((" + outerWay.ToWktPart() + ")");
                foreach (var innerWay in innerWays)
                {
                    if (!outerWay.PointInPolygon(innerWay.Nodes[0].Latitude, innerWay.Nodes[0].Longitude))
                    {
                        continue;
                    }

                    resultSB.Append(", (" + innerWay.ToWktPart() + ")");
                }
                resultSB.Append(')');
            }

            resultSB.Append(')');

            return resultSB.ToString();
        }

        private static string ToWktPart(this OsmNode node)
        {
            return string.Format("{0} {1}", node.Longitude.ToString(CultureInfo.InvariantCulture), node.Latitude.ToString(CultureInfo.InvariantCulture));
        }

        private static string ToWktPart(this OSMNodeSpatial node)
        {
            return string.Format("{0} {1}", node.Longitude.ToString(CultureInfo.InvariantCulture), node.Latitude.ToString(CultureInfo.InvariantCulture));
        }

        private static string ToWktPart(this OSMWaySpatial way)
        {
            var resultSB = new StringBuilder();

            var nodeCounter = 0;
            foreach (var node in way.Nodes)
            {
                nodeCounter++;
                if (nodeCounter > 1)
                {
                    resultSB.Append(", ");
                }
                resultSB.Append(node.ToWktPart());
            }

            return resultSB.ToString();
        }
    }
}
