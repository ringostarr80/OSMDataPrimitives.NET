![license](https://img.shields.io/github/license/ringostarr80/OSMDataPrimitives.NET)
![codeql_analysis](https://img.shields.io/github/actions/workflow/status/ringostarr80/OSMDataPrimitives.NET/codeql-analysis.yml)
![nuget_version](https://img.shields.io/nuget/v/OSMDataPrimitives)
![github_tag](https://img.shields.io/github/v/tag/ringostarr80/OSMDataPrimitives.NET?sort=semver)

# OSMDataPrimitives

## Intention
This pretty little project is intended to provide the 3 basic data types (nodes, ways and relations) of OpenStreetMap as .NET classes.
I'm about to use it in some other projects.

## Usage
To create new instances of an OSMNode:

```C#
using OSMDataPrimitives;

var node = new OSMNode(1, 52.123456, 12.654321) {
	UserId = 1,
	UserName = "username",
	Version = 1,
	Changeset = 1,
	Timestamp = DateTime.Now
};
node.Tags.Add("name", "foo");
node.Tags.Add("ref", "bar");
```

It's also easy to create new instances of an OSMWay:

```C#
var way = new OSMWay(1) {
	UserId = 1,
	UserName = "username",
	Version = 1,
	Changeset = 1,
	TimeStamp = DateTime.Now
};
way.Tags.Add("name", "first road");
way.Tags.Add("ref", "A1");
way.NodeRefs.Add(1);
way.NodeRefs.Add(2);
way.NodeRefs.Add(3);
way.NodeRefs.Add(4);
```

And last but not least an OSMRelation:

```C#
var relation = new OSMRelation(1) {
	UserId = 1,
	UserName = "username",
	Version = 1,
	Changeset = 1,
	TimeStamp = DateTime.Now
};
relation.Tags.Add("name", "my little country");
relation.Tags.Add("note", "just a test.");
relation.Members.Add(new OSMMember(MemberType.Way, 1, "outer"));
relation.Members.Add(new OSMMember(MemberType.Way, 2, "outer"));
```

To simply convert these OSMElement's to a XML-String or a XmlElement:

```C#
using OSMDataPrimitives;
using OSMDataPrimitives.Xml;

var node = new OSMNode(1, 52.123456, 12.654321) {
	UserId = 1,
	UserName = "username",
	Version = 1,
	Changeset = 1,
	Timestamp = DateTime.Now
};
node.Tags.Add("name", "foo");
node.Tags.Add("ref", "bar");

var xmlString = node.ToXmlString();
Console.WriteLine(xmlString);
/*
 * output:
 * <node id="1" lat="52.123456" lon="12.654321" version="1" uid="1" user="username" changeset="1" timestamp="2017-01-31T12:34:17Z"><tag k="name" v="foo" /><tag k="ref" v="bar" /></node>
 */

var xmlElement = node.ToXml();
Console.WriteLine(xmlElement.OuterXml);
/*
 * output (the same as above):
 * <node id="1" lat="52.123456" lon="12.654321" version="1" uid="1" user="username" changeset="1" timestamp="2017-01-31T12:36:09Z"><tag k="name" v="foo" /><tag k="ref" v="bar" /></node>
 */
```

It is also possible to build PostgreSQL queries for the OSMElement's:

```C#
var node = new OSMNode(1, 52.123456, 12.654321) {
	UserId = 1,
	UserName = "username",
	Version = 1,
	Changeset = 1,
	Timestamp = DateTime.Now
};
node.Tags.Add("name", "foo");
node.Tags.Add("ref", "bar");

NameValueCollection sqlParameters;
var sqlQuery = node.ToPostgreSQLInsert(out sqlParameters);
Console.WriteLine(sqlQuery);
/*
 * output:
 * INSERT INTO nodes (osm_id, lat, lon, tags) VALUES(@osm_id::bigint, @lat::double precision, @lon::double precision, @tags::hstore)
 *
 * The values for @osm_id, @lat, @lon and @tags are put into the sqlParameters-variable.
 */

// to add all the meta-data:
sqlQuery = node.ToPostgreSQLInsert(out sqlParameters, inclusiveMetaFields: true);
Console.WriteLine(sqlQuery);
/*
 * output:
 * INSERT INTO nodes (osm_id, version, changeset, uid, user, timestamp, lat, lon, tags) VALUES(@osm_id::bigint, @version::bigint, @changeset::bigint, @uid::bigint, @user, TIMESTAMP @timestamp, @lat::double precision, @lon::double precision, @tags::hstore)
```

## License
[CC0-1.0](./LICENSE).
