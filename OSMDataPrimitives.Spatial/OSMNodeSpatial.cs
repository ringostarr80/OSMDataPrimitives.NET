using System;

namespace OSMDataPrimitives.Spatial
{
	/// <summary>
	/// OSMNodeSpatial.
	/// </summary>
	public class OSMNodeSpatial : OSMNode
	{
		private const double EQUATORIAL_RADIUS = 6378137.0;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMNodeSpatial"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OSMNodeSpatial(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.Spatial.OSMNodeSpatial"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public OSMNodeSpatial(ulong id, double latitude, double longitude) : base(id, latitude, longitude)
		{

		}

		private double DegreeToRadian(double angle)
		{
			return Math.PI * angle / 180.0;
		}

		private double RadianToDegree(double angle)
		{
			return angle * (180.0 / Math.PI);
		}

		private static double SqlMod(double n, double m)
		{
			return n - ((int)(n / m) * m);
		}

		/// <summary>
		/// Gets the distance.
		/// </summary>
		/// <returns>The distance in m.</returns>
		/// <param name="node">Node.</param>
		public double GetDistance(OSMNode node)
		{
			var latitudeOrigin = this.Latitude / 180.0 * Math.PI;
			var longitudeOrigin = this.Longitude / 180.0 * Math.PI;
			var latitudeDestination = node.Latitude / 180.0 * Math.PI;
			var longitudeDestination = node.Longitude / 180.0 * Math.PI;

			var distance = Math.Acos(Math.Sin(latitudeOrigin) * Math.Sin(latitudeDestination) + Math.Cos(latitudeOrigin) * Math.Cos(latitudeDestination) * Math.Cos(longitudeDestination - longitudeOrigin));
			distance *= EQUATORIAL_RADIUS;

			return distance;
		}

		/// <summary>
		/// Gets the direction in degree (clockwise) from north.
		/// </summary>
		/// <returns>The direction in degree.</returns>
		/// <param name="node">Node.</param>
		public double GetDirection(OSMNode node)
		{
			var radLatitudeOrigin = DegreeToRadian(this.Latitude);
			var radLongitudeOrigin = DegreeToRadian(this.Longitude);
			var radLatitudeDestination = DegreeToRadian(node.Latitude);
			var radLongitudeDestination = DegreeToRadian(node.Longitude);
			var cosLatitudeDestination = Math.Cos(radLatitudeDestination);

			var sqlMod = SqlMod(Math.Atan2(Math.Cos(radLatitudeOrigin) * Math.Sin(radLatitudeDestination) - Math.Sin(radLatitudeOrigin) * cosLatitudeDestination * Math.Cos(radLongitudeDestination - radLongitudeOrigin),
										   Math.Sin(radLongitudeDestination - radLongitudeOrigin) * cosLatitudeDestination) - (5 * Math.PI / 2), 2 * Math.PI);
			var direction = (-180.0 / Math.PI) * sqlMod;

			return direction;
		}
	}
}
