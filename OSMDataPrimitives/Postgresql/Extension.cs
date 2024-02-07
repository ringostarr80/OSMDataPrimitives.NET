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
		private static string GetTableNameByElementType(IOsmElement element)
		{
			if (element is OsmNode) {
				return "nodes";
			} else if (element is OsmWay) {
				return "ways";
			} else if (element is OsmRelation) {
				return "relations";
			}

			return string.Empty;
		}

		private static StringBuilder GetRelationSpecificInsert(OsmRelation relationElement, NameValueCollection parameters)
		{
			var insertSB = new StringBuilder();
			if (relationElement.Members.Count > 0) {
				insertSB.Append(", ARRAY[");
				var membersCounter = 0;
				foreach (var member in relationElement.Members) {
					membersCounter++;
					if (membersCounter > 1) {
						insertSB.Append(", ");
					}
					insertSB.Append("@member_" + membersCounter + "::hstore");
					var memberSB = new StringBuilder();
					memberSB.Append("\"type\"=>\"" + member.Type.ToString().ToLower() + "\",");
					memberSB.Append("\"ref\"=>\"" + member.Ref + "\",");
					memberSB.Append("\"role\"=>\"" + ReplaceHstoreValue(member.Role) + "\"");
					parameters.Add("member_" + membersCounter, memberSB.ToString());
				}
				insertSB.Append(']');
			} else {
				insertSB.Append(", '{}'");
			}

			return insertSB;
		}

		private static string GetTagsSpecificInsert(IOsmElement element)
		{
			var tagsSB = new StringBuilder();
			if (element.Tags.Count > 0) {
				var tagCounter = 0;
				foreach (var tag in element.Tags) {
					tagCounter++;
					if (tagCounter > 1) {
						tagsSB.Append(", ");
					}
					tagsSB.Append($"\"{ReplaceHstoreValue(tag.Key)}\"=>\"{ReplaceHstoreValue(tag.Value)}\"");
				}
			} else {
				tagsSB.Append("");
			}

			return tagsSB.ToString();
		}

		/// <summary>
		/// Parses the PostgreSQL Fields.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="parameters">Parameters.</param>
		public static void ParsePostgreSQLFields(this IOsmElement element, NameValueCollection parameters)
		{
			if (parameters["osm_id"] is not null) {
				element.OverrideId(Convert.ToUInt64(parameters["osm_id"]));
			}

			if (parameters["tags"] is not null) {
				element.Tags = ParseHstore(parameters["tags"]);
			}

			if (element is OsmWay wayElement && parameters["node_refs"] is not null) {
				wayElement.NodeRefs = ParseNodeRefs(parameters["node_refs"]);
			}
		}

		/// <summary>
		/// Converts the OSMElement to a PostgreSQL Select String.
		/// </summary>
		/// <returns>The PostgreSQL Select String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="inclusiveMetaField">If set to <c>true</c> inclusive meta field.</param>
		public static string ToPostgreSQLSelect(this IOsmElement element, bool inclusiveMetaField = false)
		{
			var table = string.Empty;
			var selectSB = new StringBuilder("SELECT osm_id");
			if (inclusiveMetaField) {
				selectSB.Append(", version, changeset, uid, user, timestamp");
			}
			if (element is OsmNode) {
				selectSB.Append(", lat, lon");
				table = "nodes";
			}
			selectSB.Append(", tags::text");
			if (element is OsmWay) {
				selectSB.Append(", node_refs::text");
				table = "ways";
			}
			if (element is OsmRelation) {
				selectSB.Append(", members::text");
				table = "relations";
			}

			selectSB.Append(" FROM " + table);
			selectSB.Append(" WHERE osm_id = " + element.Id);

			return selectSB.ToString();
		}

		/// <summary>
		/// Converts the OSMElement to a PostgreSQL Delete String.
		/// </summary>
		/// <returns>The PostgreSQL Delete String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="tableName">TableName.</param>
		public static string ToPostgreSQLDelete(this IOsmElement element, string tableName = null)
		{
            string table;
            if (tableName != null) {
				table = tableName;
			} else {
				table = GetTableNameByElementType(element);
			}

			return string.Format("DELETE FROM {0} WHERE osm_id = {1}", table, element.Id);
		}

		/// <summary>
		/// Converts the OSMElement to a PostgreSQL Insert String and fills the Parameters.
		/// </summary>
		/// <returns>The PostgreSQL Insert String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="parameters">Parameters.</param>
		/// <param name="inclusiveMetaFields">If set to <c>true</c> inclusive meta fields.</param>
		public static string ToPostgreSQLInsert(this IOsmElement element, out NameValueCollection parameters, bool inclusiveMetaFields = false)
		{
			parameters = new NameValueCollection {
				{"osm_id", element.Id.ToString()}
			};

			var tableName = GetTableNameByElementType(element);

			var insertSB = new StringBuilder($"INSERT INTO {tableName} (osm_id");
			if (inclusiveMetaFields) {
				insertSB.Append(", version, changeset, uid, user, timestamp");
			}
			if (element is OsmNode nodeElement) {
				insertSB.Append(", lat");
				insertSB.Append(", lon");
				parameters.Add("lat", nodeElement.Latitude.ToString(CultureInfo.InvariantCulture));
				parameters.Add("lon", nodeElement.Longitude.ToString(CultureInfo.InvariantCulture));
			}
			insertSB.Append(", tags");
			if (element is OsmWay) {
				insertSB.Append(", node_refs");
			}
			if (element is OsmRelation) {
				insertSB.Append(", members");
			}
			insertSB.Append(") VALUES(@osm_id::bigint");
			if (inclusiveMetaFields) {
				insertSB.Append(", @version::bigint, @changeset::bigint, @uid::bigint, @user, TIMESTAMP @timestamp");
				parameters.Add("version", element.Version.ToString());
				parameters.Add("changeset", element.Changeset.ToString());
				parameters.Add("uid", element.UserId.ToString());
				parameters.Add("user", element.UserName);
				parameters.Add("timestamp", element.Timestamp.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
			}
			if (element is OsmNode) {
				insertSB.Append(", @lat::double precision, @lon::double precision");
			}

			parameters.Add("tags", GetTagsSpecificInsert(element));

			insertSB.Append(", @tags::hstore");
			if (element is OsmWay way) {
				insertSB.Append(", @node_refs::bigint[]");
				var wayElement = way;
				parameters.Add("node_refs", "{" + string.Join(", ", wayElement.NodeRefs) + "}");
			}
			if (element is OsmRelation relationElement) {
				insertSB.Append(GetRelationSpecificInsert(relationElement, parameters));
			}
			insertSB.Append(')');

			return insertSB.ToString();
		}

		private static List<long> ParseNodeRefs(string nodeRefsString)
		{
			if (nodeRefsString.StartsWith('{') && nodeRefsString.EndsWith('}')) {
				nodeRefsString = nodeRefsString[1..^1];
			}

			var nodeRefsArray = nodeRefsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
			var nodeRefs = new List<long>();
			foreach (var nodeRef in nodeRefsArray) {
				nodeRefs.Add(Convert.ToInt64(nodeRef));
			}

			return nodeRefs;
		}

		private static string ReplaceHstoreValue(string val)
		{
			return val.Replace("'", "''").Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		private static Dictionary<string, string> ParseHstore(string hstoreString)
		{
			var hstore = new Dictionary<string, string>();
            var startIndex = 0;

            do
            {
                var beginKeyQuoteIndex = hstoreString.IndexOf('"', startIndex);
                if (beginKeyQuoteIndex == -1)
                {
                    break;
                }
                var endKeyQuoteIndex = hstoreString.IndexOf('"', beginKeyQuoteIndex + 1);
                if (endKeyQuoteIndex == -1)
                {
                    break;
                }
                var tagKey = hstoreString.Substring(beginKeyQuoteIndex + 1, endKeyQuoteIndex - beginKeyQuoteIndex - 1);
                if (hstoreString.Length >= endKeyQuoteIndex + 1 + 2 && hstoreString.Substring(endKeyQuoteIndex + 1, 2) != "=>")
                {
                    break;
                }

				if (hstoreString.Length < endKeyQuoteIndex + 2)
				{
					break;
				}
                var beginValueQuoteIndex = hstoreString.IndexOf('"', endKeyQuoteIndex + 2);
                if (beginValueQuoteIndex == -1)
                {
                    break;
                }
                var endValueQuoteIndex = hstoreString.IndexOf('"', beginValueQuoteIndex + 1);
                if (endValueQuoteIndex == -1)
                {
                    break;
                }
                if (hstoreString[endValueQuoteIndex - 1] == '\\')
                {
                    var valueStartIndex = endValueQuoteIndex + 1;
                    bool realValueFinishQuoteFound;
                    do
                    {
                        realValueFinishQuoteFound = false;
                        endValueQuoteIndex = hstoreString.IndexOf('"', valueStartIndex);
                        if (endValueQuoteIndex == -1)
                        {
                            break;
                        }
                        if (hstoreString[endValueQuoteIndex - 1] != '\\')
                        {
                            realValueFinishQuoteFound = true;
                        }
                        else
                        {
                            valueStartIndex = endValueQuoteIndex + 1;
                        }
                    } while (!realValueFinishQuoteFound);
                }

                var tagValue = hstoreString.Substring(beginValueQuoteIndex + 1, endValueQuoteIndex - beginValueQuoteIndex - 1);
                hstore.Add(tagKey, tagValue);

                startIndex = endValueQuoteIndex + 1;
            } while (true);

            return hstore;
		}
	}
}
