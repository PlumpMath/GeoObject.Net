﻿//  Author: Weiqing Chen <kevincwq@gmail.com>
//
//  Copyright (c) 2015 Weiqing Chen
//
//  Adapted from GeoJSON.Net https://github.com/jbattermann/GeoJSON.Net
//  Copyright © 2014 Jörg Battermann & Other Contributors

using System;
using System.Collections.Generic;
using GeoObject.Net.Geometry;
using Newtonsoft.Json;

namespace GeoObject.Net.Converters
{
	/// <summary>
    /// Converter to read and write the <see cref="GeoPolygon" /> type.
    /// </summary>
    public class PolygonConverter : JsonConverter
    {

        private static readonly LineStringConverter LineStringConverter = new LineStringConverter();

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GeoPolygon);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rings = serializer.Deserialize<double[][][]>(reader);
            var lineStrings = new List<GeoLineString>(rings.Length);

            foreach (var ring in rings)
            {
                var positions = (IEnumerable<IGeoEntity>)LineStringConverter.ReadJson(reader, typeof(GeoLineString), ring, serializer);
                lineStrings.Add(new GeoLineString(positions));
            }

            return lineStrings;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var coordinateElements = value as List<GeoLineString>;
            if (coordinateElements != null && coordinateElements.Count > 0)
            {
                if (coordinateElements[0].Entities[0] is GeoEntity)
                {
                    writer.WriteStartArray();

                    foreach (var subPolygon in coordinateElements)
                    {
                        LineStringConverter.WriteJson(writer, subPolygon.Entities, serializer);
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}