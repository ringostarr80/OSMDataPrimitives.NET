using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace OSMDataPrimitives.PostgreSQL
{
	/// <summary>
	/// Extension to provide PostgreSQL-capabilities.
	/// </summary>
	public static class Extension
	{
		/// <summary>
		/// Parses the PostgreSQL Fields.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="parameters">Parameters.</param>
		public static void ParsePostgreSQLFields(this IOSMElement element, NameValueCollection parameters)
		{
			if(parameters["osm_id"] != null) {
				element.OverrideId(Convert.ToUInt64(parameters["osm_id"]));
			}

			if(parameters["tags"] != null) {
				element.Tags = ParseHstore(parameters["tags"]);
			}

			if(element is OSMWay) {
				var wayElement = (OSMWay)element;
				if(parameters["node_refs"] != null) {
					wayElement.NodeRefs = ParseNodeRefs(parameters["node_refs"]);
				}
			}
		}

		/// <summary>
		/// Converts the OSMElement to a PostgreSQL Select String.
		/// </summary>
		/// <returns>The PostgreSQL Select String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="inclusiveMetaField">If set to <c>true</c> inclusive meta field.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="offset">Offset.</param>
		public static string ToPostgreSQLSelect(this IOSMElement element, bool inclusiveMetaField = false, ulong id = 0, ulong limit = 0, ulong offset = 0)
		{
			var table = string.Empty;
			var selectSB = new StringBuilder("SELECT osm_id");
			if(inclusiveMetaField) {
				selectSB.Append(", version, changeset, uid, user, timestamp");
			}
			if(element is OSMNode) {
				selectSB.Append(", lat, lon");
				table = "nodes";
			}
			selectSB.Append(", tags");
			if(element is OSMWay) {
				selectSB.Append(", node_refs");
				table = "ways";
			}
			if(element is OSMRelation) {
				selectSB.Append(", members");
				table = "relations";
			}

			selectSB.Append(" FROM " + table);
			if(id != 0) {
				selectSB.Append(" WHERE osm_id = " + id);
				if(offset != 0 || limit != 0) {
					throw new ArgumentException("The parameter 'offset' and/or 'limit' are not allowed (makes no sense), if 'id' is set!", nameof(id));
				}
			}
			if(offset != 0) {
				selectSB.Append(" OFFSET " + offset);
			}
			if(limit != 0) {
				selectSB.Append(" LIMIT " + limit);
			}

			return selectSB.ToString();
		}

		/// <summary>
		/// Converts the OSMElement to a PostgreSQL Insert String and fills the Parameters.
		/// </summary>
		/// <returns>The PostgreSQL Insert String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="parameters">Parameters.</param>
		/// <param name="inclusiveMetaFields">If set to <c>true</c> inclusive meta fields.</param>
		public static string ToPostgreSQLInsert(this IOSMElement element, out NameValueCollection parameters, bool inclusiveMetaFields = false)
		{
			parameters = new NameValueCollection {
				{"osm_id", element.Id.ToString()}
			};

			var tableName = "";
			if(element is OSMNode) {
				tableName = "nodes";
			} else if(element is OSMWay) {
				tableName = "ways";
			} else if(element is OSMRelation) {
				tableName = "relations";
			}

			var insertSB = new StringBuilder("INSERT INTO " + tableName + " (osm_id");
			if(inclusiveMetaFields) {
				insertSB.Append(", version, changeset, uid, user, timestamp");
			}
			if(element is OSMNode) {
				var nodeElement = (OSMNode)element;
				insertSB.Append(", lat");
				insertSB.Append(", lon");
				parameters.Add("lat", nodeElement.Latitude.ToString(CultureInfo.InvariantCulture));
				parameters.Add("lon", nodeElement.Longitude.ToString(CultureInfo.InvariantCulture));
			}
			insertSB.Append(", tags");
			if(element is OSMWay) {
				insertSB.Append(", node_refs");
			}
			if(element is OSMRelation) {
				insertSB.Append(", members");
			}
			insertSB.Append(") VALUES(@osm_id::bigint");
			if(inclusiveMetaFields) {
				insertSB.Append(", @version::bigint, @changeset::bigint, @uid::bigint, @user, TIMESTAMP @timestamp");
				parameters.Add("version", element.Version.ToString());
				parameters.Add("changeset", element.Changeset.ToString());
				parameters.Add("uid", element.UserId.ToString());
				parameters.Add("user", element.UserName);
				parameters.Add("timestamp", element.Timestamp.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
			}
			if(element is OSMNode) {
				insertSB.Append(", @lat::double precision, @lon::double precision");
			}

			var tagsSB = new StringBuilder();
			if(element.Tags.Count >= 0) {
				var tagCounter = 0;
				foreach(string tagKey in element.Tags) {
					tagCounter++;
					if(tagCounter > 1) {
						tagsSB.Append(",");
					}
					tagsSB.Append("\"" + tagKey.Replace("'", "''") + "\"=>\"" + element.Tags[tagKey].Replace("'", "''") + "\"");
				}
			}
			parameters.Add("tags", tagsSB.ToString());

			//insertSB.Append(", " + tagsSB);
			insertSB.Append(", @tags::hstore");
			if(element is OSMWay) {
				var wayElement = (OSMWay)element;
				insertSB.Append(", ARRAY[");
				var nodeRefCounter = 0;
				foreach(long nodeRef in wayElement.NodeRefs) {
					nodeRefCounter++;
					if(nodeRefCounter > 1) {
						insertSB.Append(", ");
					}
					insertSB.Append(nodeRef);
				}
				insertSB.Append("]");
			}
			if(element is OSMRelation) {
				var relationElement = (OSMRelation)element;
				insertSB.Append(", ARRAY[");
				var membersCounter = 0;
				foreach(var member in relationElement.Members) {
					membersCounter++;
					if(membersCounter > 1) {
						insertSB.Append(", ");
					}
					insertSB.Append("hstore(ARRAY[");
					insertSB.Append("'type','" + member.Type.ToString().ToLower() + "'");
					insertSB.Append(",'ref','" + member.Ref + "'");
					insertSB.Append(",'role','" + member.Role.Replace("'", "''") + "'");
					insertSB.Append("])");
				}
				insertSB.Append("]");
			}
			insertSB.Append(")");

			return insertSB.ToString();
		}

		private static List<ulong> ParseNodeRefs(string nodeRefsString)
		{
			if(nodeRefsString.StartsWith("{", StringComparison.InvariantCulture) && nodeRefsString.EndsWith("}", StringComparison.InvariantCulture)) {
				nodeRefsString = nodeRefsString.Substring(1, nodeRefsString.Length - 2);
			}

			var nodeRefsArray = nodeRefsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var nodeRefs = new List<ulong>();
			foreach(var nodeRef in nodeRefsArray) {
				nodeRefs.Add(Convert.ToUInt64(nodeRef));
			}

			return nodeRefs;
		}

		private static NameValueCollection ParseHstore(string hstoreString)
		{
			var hstore = new NameValueCollection();

			var tagFound = false;
			var startIndex = 0;
			do {
				tagFound = false;

				var beginKeyQuoteIndex = hstoreString.IndexOf("\"", startIndex, StringComparison.InvariantCulture);
				if(beginKeyQuoteIndex == -1) {
					break;
				}
				var endKeyQuoteIndex = hstoreString.IndexOf("\"", beginKeyQuoteIndex + 1, StringComparison.InvariantCulture);
				if(endKeyQuoteIndex == -1) {
					break;
				}
				var tagKey = hstoreString.Substring(beginKeyQuoteIndex + 1, endKeyQuoteIndex - beginKeyQuoteIndex - 1);
				if(hstoreString.Substring(endKeyQuoteIndex + 1, 2) != "=>") {
					break;
				}

				var beginValueQuoteIndex = hstoreString.IndexOf("\"", endKeyQuoteIndex + 2, StringComparison.InvariantCulture);
				if(beginValueQuoteIndex == -1) {
					break;
				}
				var endValueQuoteIndex = hstoreString.IndexOf("\"", beginValueQuoteIndex + 1, StringComparison.InvariantCulture);
				if(endValueQuoteIndex == -1) {
					break;
				}
				if(hstoreString[endValueQuoteIndex - 1] == '\\') {
					var valueStartIndex = endValueQuoteIndex + 1;
					var realValueFinishQuoteFound = false;
					do {
						realValueFinishQuoteFound = false;
						endValueQuoteIndex = hstoreString.IndexOf("\"", valueStartIndex, StringComparison.InvariantCulture);
						if(endValueQuoteIndex == -1) {
							break;
						}
						if(hstoreString[endValueQuoteIndex - 1] != '\\') {
							realValueFinishQuoteFound = true;
						} else {
							valueStartIndex = endValueQuoteIndex + 1;
						}
					} while(!realValueFinishQuoteFound);
				}

				var tagValue = hstoreString.Substring(beginValueQuoteIndex + 1, endValueQuoteIndex - beginValueQuoteIndex - 1);
				hstore.Add(tagKey, tagValue);

				startIndex = endValueQuoteIndex + 1;
				tagFound = true;
			} while(tagFound);

			return hstore;
		}
	}
}
