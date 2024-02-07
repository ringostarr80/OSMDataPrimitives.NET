using System;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMNode.
	/// </summary>
	public class OsmNode : OsmElement
	{
		private double latitude = 0.0;
		private double longitude = 0.0;

		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <value>The latitude.</value>
		/// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
		public double Latitude {
			get { return this.latitude; }
			set {
				if(value < -90.0 || value > 90.0) {
					throw new ArgumentOutOfRangeException("Latitude", value, "The value for Latitude must be between -90.0 and 90.0.");
				}
				this.latitude = value;
			}
		}
		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <value>The longitude.</value>
		public double Longitude {
			get { return this.longitude; }
			set {
				if(value < -180.0 || value > 180.0) {
					throw new ArgumentOutOfRangeException("Longitude", value, "The value for Longitude must be between -180.0 and 180.0.");
				}
				this.longitude = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OsmNode(ulong id) : base(id)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public OsmNode(ulong id, double latitude, double longitude) : base(id)
		{
			this.Latitude = latitude;
			this.Longitude = longitude;
		}
	}
}
