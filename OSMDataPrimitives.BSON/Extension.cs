using MongoDB.Bson;

namespace OSMDataPrimitives.BSON
{
    public static class Extension
    {
		public static BsonDocument ToBson(this OSMElement element)
		{
			var bsonDoc = new BsonDocument();
			bsonDoc.Add("id", element.Id);
			if(element is OSMNode) {
				var nodeElement = (OSMNode)element;
				var locationDoc = new BsonDocument() {
					{ "lat", nodeElement.Latitude },
					{ "lon", nodeElement.Longitude }
				};
				bsonDoc.Add("location", locationDoc);
			}
			bsonDoc.Add("version",  (long)element.Version);
			if(element.UserId != 0) {
				bsonDoc.Add("uid", (long)element.UserId);
				bsonDoc.Add("user", element.UserName);
			}
			bsonDoc.Add("changeset", element.Changeset);
			bsonDoc.Add("timestamp", element.Timestamp);

			if(element is OSMWay) {
				var wayElement = (OSMWay)element;
				bsonDoc.Add("node_refs", new BsonArray(wayElement.NodeRefs));
			}

			if(element is OSMRelation) {
				var relationElement = (OSMRelation)element;
				var bsonMembersArray = new BsonArray();
				foreach(var osmMember in relationElement.Members) {
					var memberDocument = new BsonDocument() {
						{ "type", osmMember.Type.ToString().ToLower() },
						{ "ref", osmMember.Ref },
						{ "role", osmMember.Role }
					};
					bsonMembersArray.Add(memberDocument);
				}
				bsonDoc.Add("members", bsonMembersArray);
			}

			if(element.Tags.Count > 0) {
				var tagsDoc = new BsonDocument();
				foreach(string tagKey in element.Tags) {
					tagsDoc.Add(tagKey, element.Tags[tagKey]);
				}
				bsonDoc.Add("tags", tagsDoc);
			}

			return bsonDoc;
		}
	}
}
