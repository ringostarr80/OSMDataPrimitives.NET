using System;
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
			BsonClassMap.RegisterClassMap<OSMNode>(cm => {
				cm.AutoMap();
				cm.MapCreator(n => new OSMNode(n.Id));
			});
			BsonClassMap.RegisterClassMap<OSMWay>(cm => {
				cm.AutoMap();
				cm.MapCreator(w => new OSMWay(w.Id));
			});
			BsonClassMap.RegisterClassMap<OSMRelation>(cm => {
				cm.AutoMap();
				cm.MapCreator(r => new OSMRelation(r.Id));
			});
		}

		/// <summary>
		/// Converts the OSMElement to a BsonDocument.
		/// </summary>
		/// <param name="element">IOSMElement.</param>
		/// <returns>The BsonDocument.</returns>
		public static BsonDocument ToBson(this IOSMElement element)
		{
			var bsonDoc = new BsonDocument {
				{ "id", (long)element.Id }
			};
			if (element is OSMNode nodeElement) {
				var locationDoc = new BsonDocument {
					{ "lon", nodeElement.Longitude },
					{ "lat", nodeElement.Latitude }
				};
				bsonDoc.Add("location", locationDoc);
			}
			bsonDoc.Add("version", (long)element.Version);
			if (element.UserId != 0) {
				bsonDoc.Add("uid", (long)element.UserId);
				bsonDoc.Add("user", element.UserName);
			}
			bsonDoc.Add("changeset", (long)element.Changeset);
			bsonDoc.Add("timestamp", element.Timestamp);

			if (element is OSMWay wayElement) {
				bsonDoc.Add("node_refs", new BsonArray(wayElement.NodeRefs));
			}

			if (element is OSMRelation relationElement) {
				var bsonMembersArray = new BsonArray();
				foreach (var osmMember in relationElement.Members) {
					var memberDocument = new BsonDocument {
						{ "type", osmMember.Type.ToString().ToLower() },
						{ "ref", (long)osmMember.Ref },
						{ "role", osmMember.Role }
					};
					bsonMembersArray.Add(memberDocument);
				}
				bsonDoc.Add("members", bsonMembersArray);
			}

			if (element.Tags.Count > 0) {
				var tagsDoc = new BsonDocument();
				foreach (string tagKey in element.Tags) {
					tagsDoc.Add(tagKey.Replace(".", "\uFF0E").Replace("$", "\uFF04"), element.Tags[tagKey]);
				}
				bsonDoc.Add("tags", tagsDoc);
			}

			return bsonDoc;
		}

		/// <summary>
		/// Parses the BsonDocument and writes the data into this instance.
		/// </summary>
		/// <param name="element">IOSMElement.</param>
		/// <param name="doc">BsonDocument.</param>
		public static void ParseBsonDocument(this IOSMElement element, BsonDocument doc)
		{
			element.OverrideId(0);

			if (doc.Contains("id")) {
				element.OverrideId((ulong)doc["id"].AsInt64);
			}

			if (element is OSMNode nodeElement) {
				if (doc.Contains("location")) {
					var locationDoc = doc["location"].AsBsonDocument;
					if (locationDoc.Contains("lat") && locationDoc.Contains("lon")) {
						nodeElement.Latitude = locationDoc["lat"].AsDouble;
						nodeElement.Longitude = locationDoc["lon"].AsDouble;
					}
				} else {
					nodeElement.Latitude = 0.0;
					nodeElement.Longitude = 0.0;
				}
			}

			if (doc.Contains("uid")) {
				element.UserId = (ulong)doc["uid"].AsInt64;
				element.UserName = doc["user"].AsString;
			} else {
				element.UserId = 0;
				element.UserName = string.Empty;
			}

			element.Version = doc.Contains("version") ? (ulong)doc["version"].AsInt64 : 0;
			element.Changeset = doc.Contains("changeset") ? (ulong)doc["changeset"].AsInt64 : 0;
			element.Timestamp = doc.Contains("timestamp") ? doc["timestamp"].AsBsonDateTime.ToUniversalTime() : DateTime.UnixEpoch;

			if (element is OSMWay wayElement) {
				wayElement.NodeRefs.Clear();
				if (doc.Contains("node_refs")) {
					var nodeRefsArray = doc["node_refs"].AsBsonArray;
					foreach (var nodeRef in nodeRefsArray) {
						wayElement.NodeRefs.Add((ulong)nodeRef.AsInt64);
					}
				}
			}

			if (element is OSMRelation relationElement) {
				relationElement.Members.Clear();
				if (doc.Contains("members")) {
					var membersArray = doc["members"].AsBsonArray;
					var filteredMembers = membersArray.Where(member => {
						return member.AsBsonDocument.Contains("type") &&
							member.AsBsonDocument.Contains("ref") &&
							member.AsBsonDocument.Contains("role");
					});
					foreach (var memberDoc in filteredMembers.Select(member => member.AsBsonDocument)) {
						MemberType? memberType = null;
						switch (memberDoc["type"].AsString) {
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
						if (!memberType.HasValue) {
							continue;
						}

						relationElement.Members.Add(new OSMMember(memberType.Value, (ulong)memberDoc["ref"].AsInt64, memberDoc["role"].AsString));
					}
				}
			}

			element.Tags.Clear();
			if (doc.Contains("tags")) {
				var tags = doc["tags"].AsBsonDocument;
				foreach (string key in tags.Names) {
					element.Tags.Add(key, tags[key].AsString);
				}
			}
		}
	}
}
