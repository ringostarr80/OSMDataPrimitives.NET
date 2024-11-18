using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

namespace OSMDataPrimitives.Xml
{
	/// <summary>
	/// Extension to provide Xml-capabilities.
	/// </summary>
	public static class Extension
	{
		private static void SetGeneralProperties(IOsmElement osmElement, XmlElement xmlElement)
		{
			var changesetAttribute = xmlElement.Attributes.GetNamedItem("changeset");
			if (changesetAttribute is not null)
			{
				osmElement.Changeset = Convert.ToUInt64(changesetAttribute.Value);
			}

			var versionAttribute = xmlElement.Attributes.GetNamedItem("version");
			if (versionAttribute is not null)
			{
				osmElement.Version = Convert.ToUInt64(versionAttribute.Value);
			}

			var uidAttribute = xmlElement.Attributes.GetNamedItem("uid");
			if (uidAttribute is not null)
			{
				osmElement.UserId = Convert.ToUInt64(uidAttribute.Value);
			}

			var userAttribute = xmlElement.Attributes.GetNamedItem("user");
			if (userAttribute is not null)
			{
				osmElement.UserName = userAttribute.Value;
			}

			var timestampAttribute = xmlElement.Attributes.GetNamedItem("timestamp");
			if (timestampAttribute?.Value is not null)
			{
				osmElement.Timestamp = DateTime.Parse(timestampAttribute.Value, CultureInfo.InvariantCulture,
					DateTimeStyles.AdjustToUniversal);
			}
		}

		private static void SetOsmNodeProperties(OsmNode osmNode, XmlElement xmlElement)
		{
			var latAttribute = xmlElement.Attributes.GetNamedItem("lat");
			if (latAttribute is not null)
			{
				osmNode.Latitude = Convert.ToDouble(latAttribute.Value, CultureInfo.InvariantCulture);
			}

			var lonAttribute = xmlElement.Attributes.GetNamedItem("lon");
			if (lonAttribute is not null)
			{
				osmNode.Longitude = Convert.ToDouble(lonAttribute.Value, CultureInfo.InvariantCulture);
			}
		}

		private static void SetOsmRelationProperties(OsmRelation osmRelation, XmlElement xmlElement)
		{
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.Name != "member")
				{
					continue;
				}

				if (childNode.Attributes is null)
				{
					continue;
				}

				var typeAttribute = childNode.Attributes.GetNamedItem("type");
				var refAttribute = childNode.Attributes.GetNamedItem("ref");
				var roleAttribute = childNode.Attributes.GetNamedItem("role");
				if (typeAttribute is not null && refAttribute is not null && roleAttribute is not null)
				{
					MemberType? memberType = typeAttribute.Value switch
					{
						"node" => MemberType.Node,
						"way" => MemberType.Way,
						"relation" => MemberType.Relation,
						_ => throw new XmlException(
							$"invalid xml-attribute value ({typeAttribute.Value}) for 'member[@type]'.")
					};

					var refValue = Convert.ToUInt64(refAttribute.Value);
					osmRelation.Members.Add(new OsmMember(memberType.Value, refValue, roleAttribute.Value));
				}
			}
		}

		private static void SetOsmWayProperties(OsmWay osmWay, XmlElement xmlElement)
		{
			if (!xmlElement.HasChildNodes)
			{
				return;
			}

			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.Name != "nd")
				{
					continue;
				}

				var refAttribute = childNode.Attributes?.GetNamedItem("ref");
				if (refAttribute is not null)
				{
					osmWay.NodeRefs.Add(Convert.ToInt64(refAttribute.Value));
				}
			}
		}

		/// <summary>
		/// Converts the OsmElement to a XmlElement.
		/// </summary>
		/// <param name="element">IOsmElement.</param>
		/// <returns>The XmlElement.</returns>
		public static XmlElement ToXml(this IOsmElement element)
		{
			return ToXml(element, new XmlDocument());
		}

		/// <summary>
		/// Converts the OsmElement to an XmlElement.
		/// </summary>
		/// <returns>The XmlElement.</returns>
		/// <param name="element">IOsmElement.</param>
		/// <param name="doc">XmlDocument.</param>
		private static XmlElement ToXml(this IOsmElement element, XmlDocument doc)
		{
			var elementName = element switch
			{
				OsmNode => "node",
				OsmWay => "way",
				OsmRelation => "relation",
				_ => "element"
			};

			var osmElement = doc.CreateElement(elementName);
			osmElement.SetAttribute("id", element.Id.ToString());
			if (element is OsmNode nodeElement)
			{
				osmElement.SetAttribute("lat", nodeElement.Latitude.ToString(CultureInfo.InvariantCulture));
				osmElement.SetAttribute("lon", nodeElement.Longitude.ToString(CultureInfo.InvariantCulture));
			}

			osmElement.SetAttribute("version", element.Version.ToString());
			if (element.UserId != 0)
			{
				osmElement.SetAttribute("uid", element.UserId.ToString());
				osmElement.SetAttribute("user", element.UserName);
			}

			osmElement.SetAttribute("changeset", element.Changeset.ToString());
			osmElement.SetAttribute("timestamp",
				element.Timestamp.ToString("yyyy-MM-ddThh:mm:ssZ", CultureInfo.InvariantCulture));

			if (element is OsmWay wayElement)
			{
				foreach (var nodeRef in wayElement.NodeRefs)
				{
					var ndElement = doc.CreateElement("nd");
					ndElement.SetAttribute("ref", nodeRef.ToString());
					osmElement.AppendChild(ndElement);
				}
			} else if (element is OsmRelation relationElement)
			{
				foreach (var osmMember in relationElement.Members)
				{
					var memberElement = doc.CreateElement("member");
					memberElement.SetAttribute("type", osmMember.Type.ToString().ToLower());
					memberElement.SetAttribute("ref", osmMember.Ref.ToString());
					memberElement.SetAttribute("role", osmMember.Role);
					osmElement.AppendChild(memberElement);
				}
			}

			foreach (KeyValuePair<string, string> tag in element.Tags)
			{
				var tagElement = doc.CreateElement("tag");
				tagElement.SetAttribute("k", tag.Key);
				tagElement.SetAttribute("v", tag.Value);
				osmElement.AppendChild(tagElement);
			}

			return osmElement;
		}

		/// <summary>
		/// Converts the OsmElement to a Xml-String.
		/// </summary>
		/// <returns>The Xml-String.</returns>
		/// <param name="element">IOsmElement.</param>
		public static string ToXmlString(this IOsmElement element)
		{
			var xmlElement = ToXml(element);
			return xmlElement.OuterXml;
		}

		/// <summary>
		/// Converts the XmlElement to an OsmElement.
		/// </summary>
		/// <returns>The OsmElement.</returns>
		/// <param name="element">XmlElement.</param>
		public static IOsmElement ToOsmElement(this XmlElement element)
		{
			var idAttribute = element.Attributes.GetNamedItem("id") ??
			                  throw new XmlException("Missing required xml-attribute 'id'.");
			var id = Convert.ToUInt64(idAttribute.Value);
			IOsmElement osmElement = element.Name switch
			{
				"node" => new OsmNode(id),
				"way" => new OsmWay(id),
				"relation" => new OsmRelation(id),
				_ => throw new XmlException("Invalid xml-element name '" + element.Name +
				                            "'. Expected 'node', 'way' or 'relation'."),
			};

			SetGeneralProperties(osmElement, element);

			switch (osmElement)
			{
				case OsmNode nodeElement:
					SetOsmNodeProperties(nodeElement, element);
					break;
				case OsmWay wayElement:
					SetOsmWayProperties(wayElement, element);
					break;
				case OsmRelation relationElement:
					SetOsmRelationProperties(relationElement, element);
					break;
			}

			foreach (XmlNode childNode in element.ChildNodes)
			{
				switch (childNode.Name)
				{
					case "tag":
						var kAttribute = childNode.Attributes?.GetNamedItem("k");
						var vAttribute = childNode.Attributes?.GetNamedItem("v");
						if (kAttribute?.Value is not null && vAttribute?.Value is not null)
						{
							osmElement.Tags.Add(kAttribute.Value, vAttribute.Value);
						}

						break;
				}
			}

			return osmElement;
		}

		/// <summary>
		/// Converts the Xml-String to an OsmElement.
		/// </summary>
		/// <returns>The OsmElement.</returns>
		/// <param name="element">Xml-String.</param>
		public static IOsmElement ToOsmElement(this string element)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(element);
			return xmlDoc.DocumentElement.ToOsmElement();
		}
	}
}
