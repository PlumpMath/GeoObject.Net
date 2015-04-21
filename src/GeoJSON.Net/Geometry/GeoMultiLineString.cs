﻿//  Author: Weiqing Chen <kevincwq@gmail.com>
//
//  Copyright (c) 2015 Weiqing Chen
//
//  Adapted from GeoJSON.Net https://github.com/jbattermann/GeoJSON.Net
//  Copyright © 2014 Jörg Battermann & Other Contributors

using System.Collections.Generic;
using System.Linq;
using GeoJSON.Net.Converters;
using Newtonsoft.Json;

namespace GeoJSON.Net.Geometry
{
	/// <summary>
    /// Defines the <see cref="!:http://geojson.org/geojson-spec.html#multilinestring">MultiLineString</see> type.
    /// </summary>
    public class GeoMultiLineString : GeoObject
    {

        /// <summary>
        /// Gets the Coordinates.
        /// </summary>
        /// <value>The Coordinates.</value>
        [JsonProperty(PropertyName = "coordinates", Required = Required.Always)]
        [JsonConverter(typeof(PolygonConverter))]
        public List<GeoLineString> Coordinates { get; private set; }

        protected bool Equals(GeoMultiLineString other)
        {
            return base.Equals(other) && Coordinates.SequenceEqual(other.Coordinates);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMultiLineString" /> class.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public GeoMultiLineString(List<GeoLineString> coordinates)
        {
            Coordinates = coordinates ?? new List<GeoLineString>();
            Type = GeoObjectType.MultiLineString;
        }

        public static bool operator !=(GeoMultiLineString left, GeoMultiLineString right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(GeoMultiLineString left, GeoMultiLineString right)
        {
            return Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((GeoMultiLineString)obj);
        }

        public override int GetHashCode()
        {
            return Coordinates.GetHashCode();
        }
    }
}