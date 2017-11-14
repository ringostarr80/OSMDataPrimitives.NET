using System;
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
					{ "lat", nodeElement.Latitude },
					{ "lon", nodeElement.Longitude }
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
			if (doc.Contains("id")) {
				element.OverrideId((ulong)doc["id"].AsInt64);
			} else {
				element.OverrideId(0);
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

			if (doc.Contains("version")) {
				element.Version = (ulong)doc["version"].AsInt64;
			} else {
				element.Version = 0;
			}

			if (doc.Contains("uid")) {
				element.UserId = (ulong)doc["uid"].AsInt64;
				element.UserName = doc["user"].AsString;
			} else {
				element.UserId = 0;
				element.UserName = string.Empty;
			}

			if (doc.Contains("changeset")) {
				element.Changeset = (ulong)doc["changeset"].AsInt64;
			} else {
				element.Changeset = 0;
			}

			if (doc.Contains("timestamp")) {
				element.Timestamp = doc["timestamp"].AsBsonDateTime.ToUniversalTime();
			} else {
				element.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}

			if (element is OSMWay wayElement) {
				if (doc.Contains("node_refs")) {
					var nodeRefsArray = doc["node_refs"].AsBsonArray;
					foreach (var nodeRef in nodeRefsArray) {
						wayElement.NodeRefs.Add((ulong)nodeRef.AsInt64);
					}
				} else {
					wayElement.NodeRefs.Clear();
				}
			}

			if (element is OSMRelation relationElement) {
				if (doc.Contains("members")) {
					var membersArray = doc["members"].AsBsonArray;
					foreach (var member in membersArray) {
						var memberDoc = member.AsBsonDocument;
						if (!memberDoc.Contains("type")) {
							continue;
						}
						if (!memberDoc.Contains("ref")) {
							continue;
						}
						if (!memberDoc.Contains("role")) {
							continue;
						}
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
				} else {
					relationElement.Members.Clear();
				}
			}

			if (doc.Contains("tags")) {
				var tags = doc["tags"].AsBsonDocument;
				foreach (string key in tags.Names) {
					element.Tags.Add(key, tags[key].AsString);
				}
			} else {
				element.Tags.Clear();
			}
		}
	}
}
