using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OSMDataPrimitives.PostgreSql
{
	/// <summary>
	/// Extension to provide PostgreSql-capabilities.
	/// </summary>
	public static class Extension
	{
		private static string GetTableNameByElementType(IOsmElement element)
		{
			return element switch
			{
				OsmNode => "nodes",
				OsmWay => "ways",
				OsmRelation => "relations",
				_ => string.Empty,
			};
		}

		private static StringBuilder GetRelationSpecificInsert(OsmRelation relationElement,
			NameValueCollection parameters)
		{
			var insertStringBuilder = new StringBuilder();
			if (relationElement.Members.Count > 0)
			{
				insertStringBuilder.Append(", ARRAY[");
				var membersCounter = 0;
				foreach (var member in relationElement.Members)
				{
					membersCounter++;
					if (membersCounter > 1)
					{
						insertStringBuilder.Append(", ");
					}

					insertStringBuilder.Append("@member_" + membersCounter + "::hstore");
					var memberStringBuilder = new StringBuilder();
					memberStringBuilder.Append("\"type\"=>\"" + member.Type.ToString().ToLower() + "\",");
					memberStringBuilder.Append("\"ref\"=>\"" + member.Ref + "\",");
					memberStringBuilder.Append("\"role\"=>\"" + ReplaceHstoreValue(member.Role) + "\"");
					parameters.Add("member_" + membersCounter, memberStringBuilder.ToString());
				}

				insertStringBuilder.Append(']');
			}
			else
			{
				insertStringBuilder.Append(", '{}'");
			}

			return insertStringBuilder;
		}

		private static string GetTagsSpecificInsert(IOsmElement element)
		{
			var tagsStringBuilder = new StringBuilder();
			if (element.Tags.Count > 0)
			{
				var tagCounter = 0;
				foreach (var tag in element.Tags)
				{
					tagCounter++;
					if (tagCounter > 1)
					{
						tagsStringBuilder.Append(", ");
					}

					tagsStringBuilder.Append($"\"{ReplaceHstoreValue(tag.Key)}\"=>\"{ReplaceHstoreValue(tag.Value)}\"");
				}
			}
			else
			{
				tagsStringBuilder.Append("");
			}

			return tagsStringBuilder.ToString();
		}

		/// <summary>
		/// Parses the PostgreSql Fields.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="parameters">Parameters.</param>
		public static void ParsePostgreSqlFields(this IOsmElement element, NameValueCollection parameters)
		{
			if (parameters["osm_id"] is not null)
			{
				element.OverrideId(Convert.ToUInt64(parameters["osm_id"]));
			}

			if (parameters["tags"] is not null)
			{
				element.Tags = ParseHstore(parameters["tags"]);
			}

			if (element is OsmWay wayElement && parameters["node_refs"] is not null)
			{
				wayElement.NodeRefs = ParseNodeRefs(parameters["node_refs"]);
			}
		}

		/// <summary>
		/// Converts the OsmElement to a PostgreSql Select String.
		/// </summary>
		/// <returns>The PostgreSql Select String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="inclusiveMetaField">If set to <c>true</c> inclusive meta field.</param>
		public static string ToPostgreSqlSelect(this IOsmElement element, bool inclusiveMetaField = false)
		{
			var table = string.Empty;
			var selectStringBuilder = new StringBuilder("SELECT osm_id");
			if (inclusiveMetaField)
			{
				selectStringBuilder.Append(", version, changeset, uid, user, timestamp");
			}

			if (element is OsmNode)
			{
				selectStringBuilder.Append(", lat, lon");
				table = "nodes";
			}

			selectStringBuilder.Append(", tags::text");
			switch (element)
			{
				case OsmWay:
					selectStringBuilder.Append(", node_refs::text");
					table = "ways";
					break;
				case OsmRelation:
					selectStringBuilder.Append(", members::text");
					table = "relations";
					break;
			}

			selectStringBuilder.Append(" FROM " + table);
			selectStringBuilder.Append(" WHERE osm_id = " + element.Id);

			return selectStringBuilder.ToString();
		}

		/// <summary>
		/// Converts the OsmElement to a PostgreSql Delete String.
		/// </summary>
		/// <returns>The PostgreSql Delete String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="tableName">TableName.</param>
		public static string ToPostgreSqlDelete(this IOsmElement element, string tableName = null)
		{
			var table = tableName ?? GetTableNameByElementType(element);
			return $"DELETE FROM {table} WHERE osm_id = {element.Id}";
		}

		/// <summary>
		/// Converts the OsmElement to a PostgreSql Insert String and fills the Parameters.
		/// </summary>
		/// <returns>The PostgreSql Insert String.</returns>
		/// <param name="element">Element.</param>
		/// <param name="parameters">Parameters.</param>
		/// <param name="inclusiveMetaFields">If set to <c>true</c> inclusive meta fields.</param>
		public static string ToPostgreSqlInsert(this IOsmElement element, out NameValueCollection parameters,
			bool inclusiveMetaFields = false)
		{
			parameters = new NameValueCollection
			{
				{ "osm_id", element.Id.ToString() }
			};

			var tableName = GetTableNameByElementType(element);

			var insertStringBuilder = new StringBuilder($"INSERT INTO {tableName} (osm_id");
			if (inclusiveMetaFields)
			{
				insertStringBuilder.Append(", version, changeset, uid, user, timestamp");
			}

			if (element is OsmNode nodeElement)
			{
				insertStringBuilder.Append(", lat");
				insertStringBuilder.Append(", lon");
				parameters.Add("lat", nodeElement.Latitude.ToString(CultureInfo.InvariantCulture));
				parameters.Add("lon", nodeElement.Longitude.ToString(CultureInfo.InvariantCulture));
			}

			insertStringBuilder.Append(", tags");
			switch (element)
			{
				case OsmWay:
					insertStringBuilder.Append(", node_refs");
					break;
				case OsmRelation:
					insertStringBuilder.Append(", members");
					break;
			}

			insertStringBuilder.Append(") VALUES(@osm_id::bigint");
			if (inclusiveMetaFields)
			{
				insertStringBuilder.Append(
					", @version::bigint, @changeset::bigint, @uid::bigint, @user, TIMESTAMP @timestamp");
				parameters.Add("version", element.Version.ToString());
				parameters.Add("changeset", element.Changeset.ToString());
				parameters.Add("uid", element.UserId.ToString());
				parameters.Add("user", element.UserName);
				parameters.Add("timestamp",
					element.Timestamp.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
			}

			if (element is OsmNode)
			{
				insertStringBuilder.Append(", @lat::double precision, @lon::double precision");
			}

			parameters.Add("tags", GetTagsSpecificInsert(element));

			insertStringBuilder.Append(", @tags::hstore");
			switch (element)
			{
				case OsmWay way:
					insertStringBuilder.Append(", @node_refs::bigint[]");
					var wayElement = way;
					parameters.Add("node_refs", "{" + string.Join(", ", wayElement.NodeRefs) + "}");
					break;
				case OsmRelation relationElement:
					insertStringBuilder.Append(GetRelationSpecificInsert(relationElement, parameters));
					break;
			}

			insertStringBuilder.Append(')');

			return insertStringBuilder.ToString();
		}

		private static List<long> ParseNodeRefs(string nodeRefsString)
		{
			if (nodeRefsString.StartsWith('{') && nodeRefsString.EndsWith('}'))
			{
				nodeRefsString = nodeRefsString[1..^1];
			}

			var nodeRefsArray = nodeRefsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
			var nodeRefs = new List<long>();
			foreach (var nodeRef in nodeRefsArray)
			{
				nodeRefs.Add(Convert.ToInt64(nodeRef));
			}

			return nodeRefs;
		}

		private static string ReplaceHstoreValue(string val)
		{
			return val.Replace("'", "''").Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		public static Dictionary<string, string> ParseHstore(string hstoreString)
		{
			var result = new Dictionary<string, string>();
			var matches = Regex.Matches(
				hstoreString,
				"(\"(?:[^\"]|\"\")*\")\\s*=>\\s*(\"(?:[^\"]|\"\")*\")(,\\s*|$)",
				RegexOptions.Compiled,
				new TimeSpan(0, 0, 5)
			);

			foreach (var groups in matches.Select(match => match.Groups))
			{
				var key = groups[1].Value.Replace("\"\"", "\"").Trim('"');
				var value = groups[2].Value.Replace("\"\"", "\"").Trim('"');
				result[key] = value;
			}

			return result;
		}
	}
}
