﻿//  Author: Weiqing Chen <kevincwq@gmail.com>
//
//  Copyright (c) 2015 Weiqing Chen
//
//  Adapted from GeoJSON.Net https://github.com/jbattermann/GeoJSON.Net
//  Copyright © 2014 Jörg Battermann & Other Contributors

using System;
using System.Collections.Generic;
using System.Linq;
using GeoObject.Net.Geometry;
using Newtonsoft.Json;

namespace GeoObject.Net.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiPointConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GeoMultiPoint);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var coordinates = serializer.Deserialize<double[][]>(reader);
            var positions = new List<GeoPoint>(coordinates.Length);
            try
            {
                foreach (var coordinate in coordinates)
                {
                    var x = coordinate[0];
                    var y = coordinate[1];
                    double? z = null;

                    if (coordinate.Length == 3)
                    {
                        z = coordinate[2];
                    }

                    positions.Add(new GeoPoint(new GeoEntity(x, y, z)));
                }

                return positions;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not parse GeoJSON Response. (Y or X missing from Point geometry?)", ex);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var points = (List<GeoPoint>)value;

            if (points.Any())
            {
                var converter = new PointConverter();

                writer.WriteStartArray();

                foreach (var point in points)
                {
                    converter.WriteJson(writer, point.Entity, serializer);
                }

                writer.WriteEndArray();
            }
        }
    }
}