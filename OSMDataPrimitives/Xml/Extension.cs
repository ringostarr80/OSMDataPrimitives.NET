using System;
using System.Globalization;
using System.Xml;

namespace OSMDataPrimitives.Xml
{
	/// <summary>
	/// Extension to provide Xml-capabilities.
	/// </summary>
	public static class Extension
	{
		/// <summary>
		/// Converts the OSMElement to a XmlElement.
		/// </summary>
		/// <param name="element">IOSMElement.</param>
		/// <returns>The XmlElement.</returns>
		public static XmlElement ToXml(this IOSMElement element)
		{
			return ToXml(element, new XmlDocument());
		}

		/// <summary>
		/// Converts the OSMElement to an XmlElement.
		/// </summary>
		/// <returns>The XmlElement.</returns>
		/// <param name="element">IOSMElement.</param>
		/// <param name="doc">XmlDocument.</param>
		public static XmlElement ToXml(this IOSMElement element, XmlDocument doc)
		{
			var elementName = "element";
			if(element is OSMNode) {
				elementName = "node";
			} else if(element is OSMWay) {
				elementName = "way";
			} else if(element is OSMRelation) {
				elementName = "relation";
			}
			var osmElement = doc.CreateElement(elementName);
			osmElement.SetAttribute("id", element.Id.ToString());
			if(element is OSMNode) {
				var nodeElement = (OSMNode)element;
				osmElement.SetAttribute("lat", nodeElement.Latitude.ToString(CultureInfo.InvariantCulture));
				osmElement.SetAttribute("lon", nodeElement.Longitude.ToString(CultureInfo.InvariantCulture));
			}
			osmElement.SetAttribute("version", element.Version.ToString());
			if(element.UserId != 0) {
				osmElement.SetAttribute("uid", element.UserId.ToString());
				osmElement.SetAttribute("user", element.UserName);
			}
			osmElement.SetAttribute("changeset", element.Changeset.ToString());
			osmElement.SetAttribute("timestamp", element.Timestamp.ToString("yyyy-MM-ddThh:mm:ssZ", CultureInfo.InvariantCulture));

			if(element is OSMWay) {
				var wayElement = (OSMWay)element;
				foreach(var nodeRef in wayElement.NodeRefs) {
					var ndElement = doc.CreateElement("nd");
					ndElement.SetAttribute("ref", nodeRef.ToString());
					osmElement.AppendChild(ndElement);
				}
			}

			if(element is OSMRelation) {
				var relationElement = (OSMRelation)element;
				foreach(var osmMember in relationElement.Members) {
					var memberElement = doc.CreateElement("member");
					memberElement.SetAttribute("type", osmMember.Type.ToString().ToLower());
					memberElement.SetAttribute("ref", osmMember.Ref.ToString());
					memberElement.SetAttribute("role", osmMember.Role);
					osmElement.AppendChild(memberElement);
				}
			}

			foreach(string tagKey in element.Tags) {
				var tagElement = doc.CreateElement("tag");
				tagElement.SetAttribute("k", tagKey);
				tagElement.SetAttribute("v", element.Tags[tagKey]);
				osmElement.AppendChild(tagElement);
			}

			return osmElement;
		}

		/// <summary>
		/// Converts the OSMElement to an Xml-String.
		/// </summary>
		/// <returns>The Xml-String.</returns>
		/// <param name="element">IOSMElement.</param>
		public static string ToXmlString(this IOSMElement element)
		{
			var xmlElement = ToXml(element);
			return xmlElement.OuterXml;
		}

		/// <summary>
		/// Converts the XmlElement to an OSMElement.
		/// </summary>
		/// <returns>The OSMElement.</returns>
		/// <param name="element">XmlElement.</param>
		public static IOSMElement ToOSMElement(this XmlElement element)
		{
			var idAttribute = element.Attributes.GetNamedItem("id");
			if(idAttribute == null) {
				throw new XmlException("Missing required xml-attribute 'id'.");
			}

			var id = Convert.ToUInt64(idAttribute.Value);

			IOSMElement osmElement = null;
			switch(element.Name) {
				case "node":
					osmElement = new OSMNode(id);
					break;
				case "way":
					osmElement = new OSMWay(id);
					break;
				case "relation":
					osmElement = new OSMRelation(id);
					break;
				default:
					throw new XmlException("Invalid xml-element name '" + element.Name + "'. Expected 'node', 'way' or 'relation'.");
			}

			var changesetAttribute = element.Attributes.GetNamedItem("changeset");
			if(changesetAttribute != null) {
				osmElement.Changeset = Convert.ToUInt64(changesetAttribute.Value);
			}
			var versionAttribute = element.Attributes.GetNamedItem("version");
			if(versionAttribute != null) {
				osmElement.Version = Convert.ToUInt64(versionAttribute.Value);
			}
			var uidAttribute = element.Attributes.GetNamedItem("uid");
			if(uidAttribute != null) {
				osmElement.UserId = Convert.ToUInt64(uidAttribute.Value);
			}
			var userAttribute = element.Attributes.GetNamedItem("user");
			if(userAttribute != null) {
				osmElement.UserName = userAttribute.Value;
			}
			var timestampAttribute = element.Attributes.GetNamedItem("timestamp");
			if(timestampAttribute != null) {
				osmElement.Timestamp = DateTime.Parse(timestampAttribute.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
			}

			if(osmElement is OSMNode) {
				var nodeElement = (OSMNode)osmElement;
				var latAttribute = element.Attributes.GetNamedItem("lat");
				if(latAttribute != null) {
					nodeElement.Latitude = Convert.ToDouble(latAttribute.Value, CultureInfo.InvariantCulture);
				}
				var lonAttribute = element.Attributes.GetNamedItem("lon");
				if(lonAttribute != null) {
					nodeElement.Longitude = Convert.ToDouble(lonAttribute.Value, CultureInfo.InvariantCulture);
				}
			} else if(osmElement is OSMWay) {
				var wayElement = (OSMWay)osmElement;
				if(element.HasChildNodes) {
					foreach(XmlNode childNode in element.ChildNodes) {
						if(childNode.Name != "nd") {
							continue;
						}

						var refAttribute = childNode.Attributes.GetNamedItem("ref");
						if(refAttribute != null) {
							wayElement.NodeRefs.Add(Convert.ToUInt64(refAttribute.Value));
						}
					}
				}
			} else if(osmElement is OSMRelation) {
				var relationElement = (OSMRelation)osmElement;
				if(element.HasChildNodes) {
					foreach(XmlNode childNode in element.ChildNodes) {
						if(childNode.Name != "member") {
							continue;
						}

						var typeAttribute = childNode.Attributes.GetNamedItem("type");
						var refAttribute = childNode.Attributes.GetNamedItem("ref");
						var roleAttribute = childNode.Attributes.GetNamedItem("role");
						if(typeAttribute != null && refAttribute != null && roleAttribute != null) {
							MemberType? memberType = null;
							switch(typeAttribute.Value) {
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
							if(!memberType.HasValue) {
								throw new XmlException("invalid xml-attribute value (" + typeAttribute.Value + ") for 'member[@type]'.");
							}

							var refValue = Convert.ToUInt64(refAttribute.Value);
							relationElement.Members.Add(new OSMMember(memberType.Value, refValue, roleAttribute.Value));
						}
					}
				}
			}

			if(element.HasChildNodes) {
				foreach(XmlNode childNode in element.ChildNodes) {
					switch(childNode.Name) {
						case "tag":
							var kAttribute = childNode.Attributes.GetNamedItem("k");
							var vAttribute = childNode.Attributes.GetNamedItem("v");
							if(kAttribute != null && vAttribute != null) {
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
		public static IOSMElement ToOSMElement(this string element)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(element);
			return xmlDoc.DocumentElement.ToOSMElement();
		}
	}
}
