using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace OSMDataPrimitives.BSON
{
	/// <summary>
	/// Extension to provide Bson-capabilities.
	/// </summary>
	public static class Extension
	{
		static Extension()
		{
			BsonClassMap.RegisterClassMap<OsmNode>(cm =>
			{
				cm.AutoMap();
				cm.MapCreator(n => new OsmNode(n.Id));
			});
			BsonClassMap.RegisterClassMap<OsmWay>(cm =>
			{
				cm.AutoMap();
				cm.MapCreator(w => new OsmWay(w.Id));
			});
			BsonClassMap.RegisterClassMap<OsmRelation>(cm =>
			{
				cm.AutoMap();
				cm.MapCreator(r => new OsmRelation(r.Id));
			});
		}

		/// <summary>
		/// Converts the OsmElement to a BsonDocument.
		/// </summary>
		/// <param name="element">IOsmElement.</param>
		/// <returns>The BsonDocument.</returns>
		public static BsonDocument ToBson(this IOsmElement element)
		{
			var bsonDoc = new BsonDocument
			{
				{ "id", (long)element.Id }
			};
			if (element is OsmNode nodeElement)
			{
				var locationDoc = new BsonDocument
				{
					{ "lon", nodeElement.Longitude },
					{ "lat", nodeElement.Latitude }
				};
				bsonDoc.Add("location", locationDoc);
			}

			bsonDoc.Add("version", (long)element.Version);
			if (element.UserId != 0)
			{
				bsonDoc.Add("uid", (long)element.UserId);
				bsonDoc.Add("user", element.UserName);
			}

			bsonDoc.Add("changeset", (long)element.Changeset);
			bsonDoc.Add("timestamp", element.Timestamp);

			switch (element)
			{
				case OsmWay wayElement:
					bsonDoc.Add("node_refs", new BsonArray(wayElement.NodeRefs));
					break;
				case OsmRelation relationElement:
					var bsonMembersArray = new BsonArray();
					foreach (var memberDocument in relationElement.Members.Select(osmMember => new BsonDocument
					         {
						         { "type", osmMember.Type.ToString().ToLower() },
						         { "ref", (long)osmMember.Ref },
						         { "role", osmMember.Role }
					         }))
					{
						bsonMembersArray.Add(memberDocument);
					}

					bsonDoc.Add("members", bsonMembersArray);
					break;
			}

			if (element.Tags.Count <= 0)
			{
				return bsonDoc;
			}

			var tagsDoc = new BsonDocument();
			foreach (KeyValuePair<string, string> tag in element.Tags)
			{
				tagsDoc.Add(tag.Key.Replace(".", "\uFF0E").Replace("$", "\uFF04"), tag.Value);
			}

			bsonDoc.Add("tags", tagsDoc);

			return bsonDoc;
		}

		/// <summary>
		/// Parses the BsonDocument and writes the data into this instance.
		/// </summary>
		/// <param name="element">IOsmElement.</param>
		/// <param name="doc">BsonDocument.</param>
		public static void ParseBsonDocument(this IOsmElement element, BsonDocument doc)
		{
			element.OverrideId(0);

			if (doc.Contains("id"))
			{
				element.OverrideId((ulong)doc["id"].AsInt64);
			}

			if (element is OsmNode nodeElement)
			{
				ParseBsonDocumentForOsmNode(nodeElement, doc);
			}

			if (doc.Contains("uid"))
			{
				element.UserId = (ulong)doc["uid"].AsInt64;
				element.UserName = doc["user"].AsString;
			}
			else
			{
				element.UserId = 0;
				element.UserName = string.Empty;
			}

			element.Version = doc.Contains("version") ? (ulong)doc["version"].AsInt64 : 0;
			element.Changeset = doc.Contains("changeset") ? (ulong)doc["changeset"].AsInt64 : 0;
			element.Timestamp = doc.Contains("timestamp")
				? doc["timestamp"].AsBsonDateTime.ToUniversalTime()
				: DateTime.UnixEpoch;

			switch (element)
			{
				case OsmWay wayElement:
					ParseBsonDocumentForOsmWay(wayElement, doc);
					break;
				case OsmRelation relationElement:
					ParseBsonDocumentForOsmRelation(relationElement, doc);
					break;
			}

			element.Tags.Clear();
			if (!doc.Contains("tags"))
			{
				return;
			}

			var tags = doc["tags"].AsBsonDocument;
			foreach (var key in tags.Names)
			{
				element.Tags.Add(key, tags[key].AsString);
			}
		}

		/// <summary>
		/// Parses the BsonDocument and writes the data into the node.
		/// </summary>
		/// <param name="node">OsmNode.</param>
		/// <param name="doc">BsonDocument.</param>
		private static void ParseBsonDocumentForOsmNode(OsmNode node, BsonDocument doc)
		{
			if (doc.Contains("location"))
			{
				var locationDoc = doc["location"].AsBsonDocument;
				if (!locationDoc.Contains("lat") || !locationDoc.Contains("lon"))
				{
					return;
				}

				node.Latitude = locationDoc["lat"].AsDouble;
				node.Longitude = locationDoc["lon"].AsDouble;
			}
			else
			{
				node.Latitude = 0.0;
				node.Longitude = 0.0;
			}
		}

		/// <summary>
		/// Parses the BsonDocument and writes the data into the way.
		/// </summary>
		/// <param name="way">OsmWay.</param>
		/// <param name="doc">BsonDocument.</param>
		private static void ParseBsonDocumentForOsmWay(OsmWay way, BsonDocument doc)
		{
			way.NodeRefs.Clear();
			if (!doc.Contains("node_refs"))
			{
				return;
			}

			var nodeRefsArray = doc["node_refs"].AsBsonArray;
			foreach (var nodeRef in nodeRefsArray)
			{
				way.NodeRefs.Add(nodeRef.AsInt64);
			}
		}

		/// <summary>
		/// Parses the BsonDocument and writes the data into the relation.
		/// </summary>
		/// <param name="relation">OsmRelation.</param>
		/// <param name="doc">BsonDocument.</param>
		private static void ParseBsonDocumentForOsmRelation(OsmRelation relation, BsonDocument doc)
		{
			relation.Members.Clear();
			if (!doc.Contains("members"))
			{
				return;
			}

			var membersArray = doc["members"].AsBsonArray;
			var filteredMembers = membersArray.Where(member =>
				member.AsBsonDocument.Contains("type") && member.AsBsonDocument.Contains("ref") &&
				member.AsBsonDocument.Contains("role")
			);
			foreach (var memberDoc in filteredMembers.Select(member => member.AsBsonDocument))
			{
				MemberType? memberType = memberDoc["type"].AsString switch
				{
					"node" => MemberType.Node,
					"way" => MemberType.Way,
					"relation" => MemberType.Relation,
					_ => null
				};
				if (memberType.HasValue)
				{
					relation.Members.Add(new OsmMember(memberType.Value, (ulong)memberDoc["ref"].AsInt64,
						memberDoc["role"].AsString));
				}
			}
		}
	}
}
