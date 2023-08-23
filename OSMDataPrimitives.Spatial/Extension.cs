using System;
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
        /// <summary>
        /// Converts the XmlElement to an OSMElement.
        /// </summary>
        /// <returns>The OSMElement.</returns>
        /// <param name="element">XmlElement.</param>
        public static IOSMElement ToOSMSpatialElement(this XmlElement element)
        {
            var idAttribute = element.Attributes.GetNamedItem("id") ?? throw new XmlException("Missing required xml-attribute 'id'.");
            var id = Convert.ToUInt64(idAttribute.Value);
            IOSMElement osmElement = element.Name switch
            {
                "node" => new OSMNodeSpatial(id),
                "way" => new OSMWaySpatial(id),
                "relation" => new OSMRelation(id),
                _ => throw new XmlException("Invalid xml-element name '" + element.Name + "'. Expected 'node', 'way' or 'relation'."),
            };
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

            if (osmElement is OSMNodeSpatial nodeElement)
            {
                var latAttribute = element.Attributes.GetNamedItem("lat");
                if (latAttribute != null)
                {
                    nodeElement.Latitude = Convert.ToDouble(latAttribute.Value, CultureInfo.InvariantCulture);
                }
                var lonAttribute = element.Attributes.GetNamedItem("lon");
                if (lonAttribute != null)
                {
                    nodeElement.Longitude = Convert.ToDouble(lonAttribute.Value, CultureInfo.InvariantCulture);
                }
            }
            else if (osmElement is OSMWaySpatial wayElement && element.HasChildNodes)
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
                        wayElement.NodeRefs.Add(Convert.ToUInt64(refAttribute.Value));
                    }
                }
            }
            else if (osmElement is OSMRelation relationElement && element.HasChildNodes)
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
                    if (typeAttribute != null && refAttribute != null && roleAttribute != null)
                    {
                        MemberType? memberType = null;
                        switch (typeAttribute.Value)
                        {
                            case "node":
                                memberType = MemberType.Node;
                                break;
                            case "way":
                                memberType = MemberType.Way;
                                break;
                            case "relation":
                                memberType = MemberType.Relation;
                                break;
                        }
                        if (!memberType.HasValue)
                        {
                            throw new XmlException("invalid xml-attribute value (" + typeAttribute.Value + ") for 'member[@type]'.");
                        }

                        var refValue = Convert.ToUInt64(refAttribute.Value);
                        relationElement.Members.Add(new OSMMember(memberType.Value, refValue, roleAttribute.Value));
                    }
                }
            }

            if (element.HasChildNodes)
            {
                foreach (XmlNode childNode in element.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "tag":
                            var kAttribute = childNode.Attributes.GetNamedItem("k");
                            var vAttribute = childNode.Attributes.GetNamedItem("v");
                            if (kAttribute != null && vAttribute != null)
                            {
                                osmElement.Tags.Add(kAttribute.Value, vAttribute.Value);
                            }
                            break;
                    }
                }
            }

            return osmElement;
        }

        /// <summary>
        /// Converts the Xml-String to an OSMElement.
        /// </summary>
        /// <returns>The OSMElement.</returns>
        /// <param name="element">Xml-String.</param>
        public static IOSMElement ToOSMSpatialElement(this string element)
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
        public static string ToWkt(this OSMNode node)
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
            var wktTypeString = "MULTILINESTRING";
            var openingBrace = "(";
            var closingBrace = ")";
            if (wktType.HasValue)
            {
                switch (wktType.Value)
                {
                    case WktType.MultiLineString:
                        wktTypeString = wktType.Value.ToString().ToUpper();
                        break;
                    case WktType.MultiPolygon:
                        wktTypeString = wktType.Value.ToString().ToUpper();
                        openingBrace = "((";
                        closingBrace = "))";
                        break;

                    default:
                        throw new XmlException("invalid wktType-value (" + wktType.Value + ") for 'OSMWaySpatialCollection.ToWkt(WktType? wktType = null)'.");
                }
            }

            var resultSB = new StringBuilder();
            resultSB.Append(wktTypeString);
            resultSB.Append(" (");
            var wayCounter = 0;
            if (wktTypeString == "MULTIPOLYGON")
            {
                var outerWays = new OSMWaySpatialCollection(ways.Where(w => w.Role == "outer")).Merge();
                outerWays.RemoveInvalidPolygons();
                outerWays.EnsurePolygonDirection();
                var innerWays = new OSMWaySpatialCollection(ways.Where(w => w.Role == "inner")).Merge();
                innerWays.RemoveInvalidPolygons();
                innerWays.EnsurePolygonDirection();

                if (outerWays.Count == 0)
                {
                    throw new Exception("invalid polygon data.");
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
            }
            else
            {
                foreach (var way in ways)
                {
                    wayCounter++;
                    if (wayCounter > 1)
                    {
                        resultSB.Append(", ");
                    }
                    resultSB.Append(openingBrace + way.ToWktPart() + closingBrace);
                }
            }
            resultSB.Append(')');

            return resultSB.ToString();
        }

        private static string ToWktPart(this OSMNode node)
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
