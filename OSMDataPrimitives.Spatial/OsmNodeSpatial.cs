﻿using System;
using System.Collections.Generic;

namespace OSMDataPrimitives.Spatial
{
	/// <summary>
	/// OsmNodeSpatial.
	/// </summary>
	public class OsmNodeSpatial : OsmNode
	{
		private const double EquatorialRadius = 6378137.0;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OsmNodeSpatial"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OsmNodeSpatial(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OsmNodeSpatial"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public OsmNodeSpatial(ulong id, double latitude, double longitude) : base(id, latitude, longitude)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMNodeSpatial"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		public OsmNodeSpatial(OsmNode node) : base(node.Id)
		{
			this.Latitude = node.Latitude;
			this.Longitude = node.Longitude;
			this.Changeset = node.Changeset;
			this.Timestamp = node.Timestamp;
			this.UserId = node.UserId;
			this.UserName = node.UserName;
			this.Version = node.Version;
			this.Tags = new Dictionary<string, string>(node.Tags);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return 1 + base.GetHashCode();
			}
		}

		/// <summary>
		/// Converts the angle from degree to radian
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		private static double DegreeToRadian(double angle)
		{
			return Math.PI * angle / 180.0;
		}

		private static double SqlMod(double n, double m)
		{
			return n - (int)(n / m) * m;
		}

		/// <summary>
		/// Gets the distance.
		/// </summary>
		/// <returns>The distance in m.</returns>
		/// <param name="node">Node.</param>
		public double GetDistance(OsmNode node)
		{
			var latitudeOrigin = this.Latitude / 180.0 * Math.PI;
			var longitudeOrigin = this.Longitude / 180.0 * Math.PI;
			var latitudeDestination = node.Latitude / 180.0 * Math.PI;
			var longitudeDestination = node.Longitude / 180.0 * Math.PI;

			var distance = Math.Acos(Math.Sin(latitudeOrigin) * Math.Sin(latitudeDestination) +
			                         Math.Cos(latitudeOrigin) * Math.Cos(latitudeDestination) *
			                         Math.Cos(longitudeDestination - longitudeOrigin));
			distance *= EquatorialRadius;

			return distance;
		}

		/// <summary>
		/// Gets the direction in degree (clockwise) from north.
		/// </summary>
		/// <returns>The direction in degree.</returns>
		/// <param name="node">Node.</param>
		public double GetDirection(OsmNode node)
		{
			var radLatitudeOrigin = DegreeToRadian(this.Latitude);
			var radLongitudeOrigin = DegreeToRadian(this.Longitude);
			var radLatitudeDestination = DegreeToRadian(node.Latitude);
			var radLongitudeDestination = DegreeToRadian(node.Longitude);
			var cosLatitudeDestination = Math.Cos(radLatitudeDestination);

			var sqlMod = SqlMod(Math.Atan2(
					Math.Cos(radLatitudeOrigin) * Math.Sin(radLatitudeDestination) - Math.Sin(radLatitudeOrigin) *
					cosLatitudeDestination * Math.Cos(radLongitudeDestination - radLongitudeOrigin),
					Math.Sin(radLongitudeDestination - radLongitudeOrigin) * cosLatitudeDestination) -
				(5 * Math.PI / 2),
				2 * Math.PI);
			var direction = (-180.0 / Math.PI) * sqlMod;

			return direction;
		}
	}
}
