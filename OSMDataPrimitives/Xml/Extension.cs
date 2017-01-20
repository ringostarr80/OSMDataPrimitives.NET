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
		/// Converts the OSMElement to an XmlElement.
		/// </summary>
		/// <returns>The XmlElement.</returns>
		/// <param name="element">IOSMElement.</param>
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
	}
}
