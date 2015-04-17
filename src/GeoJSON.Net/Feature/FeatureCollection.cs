﻿//  Author:
//       Weiqing Chen <kevincwq@gmail.com>
//
//  Copyright (c) 2015 Weiqing Chen
//
//  Adapted from GeoJSON.Net https://github.com/jbattermann/GeoJSON.Net
//  Copyright © 2014 Jörg Battermann & Other Contributors

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeoJSON.Net.Feature
{
	/// <summary>
    /// Defines the FeatureCollection type.
    /// </summary>
    public class FeatureCollection : GeoJSONObject
    {

        /// <summary>
        /// Gets the features.
        /// </summary>
        /// <value>The features.</value>
        [JsonProperty(PropertyName = "features", Required = Required.Always)]
        public List<Feature> Features { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCollection" /> class.
        /// </summary>
        public FeatureCollection()
            : this(new List<Feature>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCollection" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public FeatureCollection(List<Feature> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException("features");
            }

            Features = features;
            Type = GeoJSONObjectType.FeatureCollection;
        }
    }
}