using System;

namespace OSMDataPrimitives
{
	/// <summary>
	/// OSMNode.
	/// </summary>
	public class OSMNode : OSMElement
	{
		private double _latitude = 0.0;
		private double _longitude = 0.0;

		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <value>The latitude.</value>
		/// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
		public double Latitude {
			get { return this._latitude; }
			set {
				if(value < -90.0 || value > 90.0) {
					throw new ArgumentOutOfRangeException(nameof(value), value, "The value for Latitude must be between -90.0 and 90.0.");
				}
				this._latitude = value;
			}
		}
		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <value>The longitude.</value>
		public double Longitude {
			get { return this._longitude; }
			set {
				if(value < -180.0 || value > 180.0) {
					throw new ArgumentOutOfRangeException(nameof(value), value, "The value for Longitude must be between -180.0 and 180.0.");
				}
				this._longitude = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:OSMDataPrimitives.OSMNode"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public OSMNode(long id) : base(id)
		{

		}
	}
}
