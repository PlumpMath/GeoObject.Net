﻿//  Author: Weiqing Chen <kevincwq@gmail.com>
//
//  Copyright (c) 2015 Weiqing Chen

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using GeoObject.Net.Geometry;
using BitConverter = System.BitConverter;

namespace GeoObject.Net
{
    /// <summary>
    /// Interface describing objects that can expand themselves by objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects that can expand clients</typeparam>
    public interface IExpandable<T>
    {
        /// <summary>
        /// Method to expand this object by <paramref name="other"/>
        /// </summary>
        /// <param name="other">The object to expand with</param>
        void ExpandToInclude(T other);

        /// <summary>
        /// Function to expand compute a new object that is this object by expanded by <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The object to expand with</param>
        /// <returns>The expanded object</returns>
        T ExpandedBy(T other);
    }

    /// <summary>
    /// Interface describing objects that can perform an intersects predicate with <typeparamref name="T"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the component that can intersect</typeparam>
    public interface IIntersectable<in T>
    {
        /// <summary>
        /// Predicate function to test if <paramref name="other"/> intersects with this object.
        /// </summary>
        /// <param name="other">The object to test</param>
        /// <returns><value>true</value> if this objects intersects with <paramref name="other"/></returns>
        bool Intersects(T other);
    }

    /// <summary>
    /// Defines a rectangular region of the 2D coordinate plane.
    /// It is often used to represent the bounding box of a <c>Geometry</c>,
    /// e.g. the minimum and maximum x and y values of the <c>Coordinate</c>s.
    /// Note that Envelopes support infinite or half-infinite regions, by using the values of
    /// <c>Double.PositiveInfinity</c> and <c>Double.NegativeInfinity</c>.
    /// When Envelope objects are created or initialized,
    /// the supplies extent values are automatically sorted into the correct order.
    /// </summary>
#pragma warning disable 612,618
    [DataContract]
    public class Envelope : IEquatable<Envelope>, IComparable<Envelope>, IIntersectable<Envelope>, IExpandable<Envelope>
#pragma warning restore 612,618
    {
        /// <summary>
        /// Test the point q to see whether it intersects the Envelope
        /// defined by p1-p2.
        /// </summary>
        /// <param name="p1">One extrema point of the envelope.</param>
        /// <param name="p2">Another extrema point of the envelope.</param>
        /// <param name="q">Point to test for intersection.</param>
        /// <returns><c>true</c> if q intersects the envelope p1-p2.</returns>
        public static bool Intersects(IGeoEntity p1, IGeoEntity p2, IGeoEntity q)
        {
            return ((q.X >= (p1.X < p2.X ? p1.X : p2.X)) && (q.X <= (p1.X > p2.X ? p1.X : p2.X))) &&
                   ((q.Y >= (p1.Y < p2.Y ? p1.Y : p2.Y)) && (q.Y <= (p1.Y > p2.Y ? p1.Y : p2.Y)));
        }

        /// <summary>
        /// Tests whether the envelope defined by p1-p2
        /// and the envelope defined by q1-q2
        /// intersect.
        /// </summary>
        /// <param name="p1">One extrema point of the envelope Point.</param>
        /// <param name="p2">Another extrema point of the envelope Point.</param>
        /// <param name="q1">One extrema point of the envelope Q.</param>
        /// <param name="q2">Another extrema point of the envelope Q.</param>
        /// <returns><c>true</c> if Q intersects Point</returns>
        public static bool Intersects(IGeoEntity p1, IGeoEntity p2, IGeoEntity q1, IGeoEntity q2)
        {
            double minp = Math.Min(p1.X, p2.X);
            double maxq = Math.Max(q1.X, q2.X);
            if (minp > maxq)
                return false;

            double minq = Math.Min(q1.X, q2.X);
            double maxp = Math.Max(p1.X, p2.X);
            if (maxp < minq)
                return false;

            minp = Math.Min(p1.Y, p2.Y);
            maxq = Math.Max(q1.Y, q2.Y);
            if (minp > maxq)
                return false;

            minq = Math.Min(q1.Y, q2.Y);
            maxp = Math.Max(p1.Y, p2.Y);
            if (maxp < minq)
                return false;

            return true;
        }

        /*
        *  the minimum x-coordinate
        */
        [System.Runtime.Serialization.DataMember]
        private double _minx;

        /*
        *  the maximum x-coordinate
        */
        [System.Runtime.Serialization.DataMember]
        private double _maxx;

        /*
        * the minimum y-coordinate
        */
        [System.Runtime.Serialization.DataMember]
        private double _miny;

        /*
        *  the maximum y-coordinate
        */
        [System.Runtime.Serialization.DataMember]
        private double _maxy;

        public double[] JsonBox
        {
            get
            {
                if (IsNull)
                    return null;
                else
                    return new double[] { _minx, _miny, _maxx, _maxy };
            }
        }

        /// <summary>
        /// Creates a null <c>Envelope</c>.
        /// </summary>
        public Envelope()
        {
            Init();
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        public Envelope(double x1, double x2, double y1, double y2)
        {
            Init(x1, x2, y1, y2);
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by two Coordinates.
        /// </summary>
        /// <param name="p1">The first Coordinate.</param>
        /// <param name="p2">The second Coordinate.</param>
        public Envelope(IGeoEntity p1, IGeoEntity p2)
        {
            Init(p1.X, p2.X, p1.Y, p2.Y);
        }

        /// <summary>
        /// Creates an <c>Envelope</c> for a region defined by a single Coordinate.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public Envelope(IGeoEntity p)
        {
            Init(p.X, p.X, p.Y, p.Y);
        }

        /// <summary>
        /// Create an <c>Envelope</c> from an existing Envelope.
        /// </summary>
        /// <param name="env">The Envelope to initialize from.</param>
        public Envelope(Envelope env)
        {
            Init(env);
        }

        /// <summary>
        /// Initialize to a null <c>Envelope</c>.
        /// </summary>
        public void Init()
        {
            SetToNull();
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by maximum and minimum values.
        /// </summary>
        /// <param name="x1">The first x-value.</param>
        /// <param name="x2">The second x-value.</param>
        /// <param name="y1">The first y-value.</param>
        /// <param name="y2">The second y-value.</param>
        public void Init(double x1, double x2, double y1, double y2)
        {
            if (x1 < x2)
            {
                _minx = x1;
                _maxx = x2;
            }
            else
            {
                _minx = x2;
                _maxx = x1;
            }

            if (y1 < y2)
            {
                _miny = y1;
                _maxy = y2;
            }
            else
            {
                _miny = y2;
                _maxy = y1;
            }
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by two Coordinates.
        /// </summary>
        /// <param name="p1">The first Coordinate.</param>
        /// <param name="p2">The second Coordinate.</param>
        public void Init(IGeoEntity p1, IGeoEntity p2)
        {
            Init(p1.X, p2.X, p1.Y, p2.Y);
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> for a region defined by a single Coordinate.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public void Init(IGeoEntity p)
        {
            Init(p.X, p.X, p.Y, p.Y);
        }

        /// <summary>
        /// Initialize an <c>Envelope</c> from an existing Envelope.
        /// </summary>
        /// <param name="env">The Envelope to initialize from.</param>
        public void Init(Envelope env)
        {
            _minx = env.MinX;
            _maxx = env.MaxX;
            _miny = env.MinY;
            _maxy = env.MaxY;
        }

        /// <summary>
        /// Makes this <c>Envelope</c> a "null" envelope..
        /// </summary>
        public void SetToNull()
        {
            _minx = 0;
            _maxx = -1;
            _miny = 0;
            _maxy = -1;
        }

        /// <summary>
        /// Returns <c>true</c> if this <c>Envelope</c> is a "null" envelope.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this <c>Envelope</c> is uninitialized
        /// or is the envelope of the empty point.
        /// </returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public bool IsNull
        {
            get
            {
                return _maxx < _minx;
            }
        }

        /// <summary>
        /// Returns the difference between the maximum and minimum x values.
        /// </summary>
        /// <returns>max x - min x, or 0 if this is a null <c>Envelope</c>.</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double Width
        {
            get
            {
                if (IsNull)
                    return 0;
                return _maxx - _minx;
            }
        }

        /// <summary>
        /// Returns the difference between the maximum and minimum y values.
        /// </summary>
        /// <returns>max y - min y, or 0 if this is a null <c>Envelope</c>.</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double Height
        {
            get
            {
                if (IsNull)
                    return 0;
                return _maxy - _miny;
            }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s minimum x-value. min x > max x
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The minimum x-coordinate.</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double MinX
        {
            get { return _minx; }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s maximum x-value. min x > max x
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The maximum x-coordinate.</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double MaxX
        {
            get { return _maxx; }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s minimum y-value. min y > max y
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The minimum y-coordinate.</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double MinY
        {
            get { return _miny; }
        }

        /// <summary>
        /// Returns the <c>Envelope</c>s maximum y-value. min y > max y
        /// indicates that this is a null <c>Envelope</c>.
        /// </summary>
        /// <returns>The maximum y-coordinate.</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double MaxY
        {
            get { return _maxy; }
        }

        /// <summary>
        /// Gets the area of this envelope.
        /// </summary>
        /// <returns>The area of the envelope, or 0.0 if envelope is null</returns>
        [System.Runtime.Serialization.IgnoreDataMember]
        public double Area
        {
            get
            {
                return Width * Height;
            }
        }

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="distance">The distance to expand the envelope.</param>
        public void ExpandBy(double distance)
        {
            ExpandBy(distance, distance);
        }

        /// <summary>
        /// Expands this envelope by a given distance in all directions.
        /// Both positive and negative distances are supported.
        /// </summary>
        /// <param name="deltaX">The distance to expand the envelope along the the X axis.</param>
        /// <param name="deltaY">The distance to expand the envelope along the the Y axis.</param>
        public void ExpandBy(double deltaX, double deltaY)
        {
            if (IsNull)
                return;

            _minx -= deltaX;
            _maxx += deltaX;
            _miny -= deltaY;
            _maxy += deltaY;

            // check for envelope disappearing
            if (_minx > _maxx || _miny > _maxy)
                SetToNull();
        }

        /// <summary>
        /// Gets the minimum extent of this envelope across both dimensions.
        /// </summary>
        /// <returns></returns>
        public double MinExtent
        {
            get
            {
                if (IsNull) return 0.0;
                double w = Width;
                double h = Height;
                if (w < h) return w;
                return h;
            }
        }

        /// <summary>
        /// Gets the maximum extent of this envelope across both dimensions.
        /// </summary>
        /// <returns></returns>
        public double MaxExtent
        {
            get
            {
                if (IsNull) return 0.0;
                double w = Width;
                double h = Height;
                if (w > h) return w;
                return h;
            }
        }

        /// <summary>
        /// Enlarges this <code>Envelope</code> so that it contains
        /// the given <see cref="Coordinate"/>.
        /// Has no effect if the point is already on or within the envelope.
        /// </summary>
        /// <param name="p">The Coordinate.</param>
        public void ExpandToInclude(IGeoEntity p)
        {
            ExpandToInclude(p.X, p.Y);
        }

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the given <see cref="Coordinate"/>.
        /// </summary>
        /// <remarks>Has no effect if the point is already on or within the envelope.</remarks>
        /// <param name="x">The value to lower the minimum x to or to raise the maximum x to.</param>
        /// <param name="y">The value to lower the minimum y to or to raise the maximum y to.</param>
        public void ExpandToInclude(double x, double y)
        {
            if (IsNull)
            {
                _minx = x;
                _maxx = x;
                _miny = y;
                _maxy = y;
            }
            else
            {
                if (x < _minx)
                    _minx = x;
                if (x > _maxx)
                    _maxx = x;
                if (y < _miny)
                    _miny = y;
                if (y > _maxy)
                    _maxy = y;
            }
        }

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the <c>other</c> Envelope.
        /// Has no effect if <c>other</c> is wholly on or
        /// within the envelope.
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to expand to include.</param>
        public void ExpandToInclude(Envelope other)
        {
            if (other.IsNull)
                return;
            if (IsNull)
            {
                _minx = other.MinX;
                _maxx = other.MaxX;
                _miny = other.MinY;
                _maxy = other.MaxY;
            }
            else
            {
                if (other.MinX < _minx)
                    _minx = other.MinX;
                if (other.MaxX > _maxx)
                    _maxx = other.MaxX;
                if (other.MinY < _miny)
                    _miny = other.MinY;
                if (other.MaxY > _maxy)
                    _maxy = other.MaxY;
            }
        }

        /// <summary>
        /// Enlarges this <c>Envelope</c> so that it contains
        /// the <c>other</c> Envelope.
        /// Has no effect if <c>other</c> is wholly on or
        /// within the envelope.
        /// </summary>
        /// <param name="other">the <c>Envelope</c> to expand to include.</param>
        public Envelope ExpandedBy(Envelope other)
        {
            if (other.IsNull)
                return this;
            if (IsNull)
                return other;

            var minx = (other._minx < _minx) ? other._minx : _minx;
            var maxx = (other._maxx > _maxx) ? other._maxx : _maxx;
            var miny = (other._miny < _miny) ? other._miny : _miny;
            var maxy = (other._maxy > _maxy) ? other._maxy : _maxy;
            return new Envelope(minx, maxx, miny, maxy);
        }

        /// <summary>
        /// Translates this envelope by given amounts in the X and Y direction.
        /// </summary>
        /// <param name="transX">The amount to translate along the X axis.</param>
        /// <param name="transY">The amount to translate along the Y axis.</param>
        public void Translate(double transX, double transY)
        {
            if (IsNull)
                return;
            Init(MinX + transX, MaxX + transX, MinY + transY, MaxY + transY);
        }

        /// <summary>
        /// Computes the coordinate of the centre of this envelope (as long as it is non-null).
        /// </summary>
        /// <returns>
        /// The centre coordinate of this envelope,
        /// or <c>null</c> if the envelope is null.
        /// </returns>.
        [System.Runtime.Serialization.IgnoreDataMember]
        public IGeoEntity Centre
        {
            get
            {
                return IsNull ? null : new GeoEntity((MinX + MaxX) / 2.0, (MinY + MaxY) / 2.0);
            }
        }

        /// <summary>
        /// Computes the intersection of two <see cref="Envelope"/>s.
        /// </summary>
        /// <param name="env">The envelope to intersect with</param>
        /// <returns>
        /// A new Envelope representing the intersection of the envelopes (this will be
        /// the null envelope if either argument is null, or they do not intersect
        /// </returns>
        public Envelope Intersection(Envelope env)
        {
            if (IsNull || env.IsNull || !Intersects(env))
                return new Envelope();

            return new Envelope(Math.Max(MinX, env.MinX),
                                Math.Min(MaxX, env.MaxX),
                                Math.Max(MinY, env.MinY),
                                Math.Min(MaxY, env.MaxY));
        }

        /// <summary>
        /// Check if the region defined by <c>other</c>
        /// overlaps (intersects) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="other"> the <c>Envelope</c> which this <c>Envelope</c> is
        /// being checked for overlapping.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>Envelope</c>s overlap.
        /// </returns>
        public bool Intersects(Envelope other)
        {
            if (IsNull || other.IsNull)
                return false;
            return !(other.MinX > _maxx || other.MaxX < _minx || other.MinY > _maxy || other.MaxY < _miny);
        }

        /// <summary>
        /// Check if the point <c>p</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="p"> the <c>Coordinate</c> to be tested.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        public bool Intersects(IGeoEntity p)
        {
            return Intersects(p.X, p.Y);
        }

        /// <summary>
        /// Check if the point <c>(x, y)</c> overlaps (lies inside) the region of this <c>Envelope</c>.
        /// </summary>
        /// <param name="x"> the x-ordinate of the point.</param>
        /// <param name="y"> the y-ordinate of the point.</param>
        /// <returns><c>true</c> if the point overlaps this <c>Envelope</c>.</returns>
        public bool Intersects(double x, double y)
        {
            return !(x > _maxx || x < _minx || y > _maxy || y < _miny);
        }

        ///<summary>
        /// Tests if the <c>Envelope other</c> lies wholely inside this <c>Envelope</c> (inclusive of the boundary).
        ///</summary>
        /// <remarks>
        /// Note that this is <b>not</b> the same definition as the SFS <i>contains</i>,
        /// which would exclude the envelope boundary.
        /// </remarks>
        /// <para>The <c>Envelope</c> to check</para>
        /// <returns>true if <c>other</c> is contained in this <c>Envelope</c></returns>
        /// <see cref="Covers(Envelope)"/>
        public bool Contains(Envelope other)
        {
            return Covers(other);
        }

        ///<summary>
        /// Tests if the given point lies in or on the envelope.
        ///</summary>
        /// <remarks>
        /// Note that this is <b>not</b> the same definition as the SFS <i>contains</i>,
        /// which would exclude the envelope boundary.
        /// </remarks>
        /// <param name="p">the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns><c>true</c> if the point lies in the interior or on the boundary of this <c>Envelope</c>. </returns>
        /// <see cref="Covers(Coordinate)"/>
        public bool Contains(IGeoEntity p)
        {
            return Covers(p);
        }

        ///<summary>
        /// Tests if the given point lies in or on the envelope.
        ///</summary>
        /// <remarks>
        /// Note that this is <b>not</b> the same definition as the SFS <i>contains</i>, which would exclude the envelope boundary.
        /// </remarks>
        /// <param name="x">the x-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <param name="y">the y-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns>
        /// <c>true</c> if <c>(x, y)</c> lies in the interior or on the boundary of this <c>Envelope</c>.
        /// </returns>
        /// <see cref="Covers(double, double)"/>
        public bool Contains(double x, double y)
        {
            return Covers(x, y);
        }

        ///<summary>
        /// Tests if the given point lies in or on the envelope.
        ///</summary>
        /// <param name="x">the x-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <param name="y">the y-coordinate of the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns> <c>true</c> if <c>(x, y)</c> lies in the interior or on the boundary of this <c>Envelope</c>.</returns>
        public bool Covers(double x, double y)
        {
            if (IsNull) return false;
            return x >= _minx &&
                x <= _maxx &&
                y >= _miny &&
                y <= _maxy;
        }

        ///<summary>
        /// Tests if the given point lies in or on the envelope.
        ///</summary>
        /// <param name="p">the point which this <c>Envelope</c> is being checked for containing</param>
        /// <returns><c>true</c> if the point lies in the interior or on the boundary of this <c>Envelope</c>.</returns>
        public bool Covers(IGeoEntity p)
        {
            return Covers(p.X, p.Y);
        }

        ///<summary>
        /// Tests if the <c>Envelope other</c> lies wholely inside this <c>Envelope</c> (inclusive of the boundary).
        ///</summary>
        /// <param name="other">the <c>Envelope</c> to check</param>
        /// <returns>true if this <c>Envelope</c> covers the <c>other</c></returns>
        public bool Covers(Envelope other)
        {
            if (IsNull || other.IsNull)
                return false;
            return other.MinX >= _minx &&
                other.MaxX <= _maxx &&
                other.MinY >= _miny &&
                other.MaxY <= _maxy;
        }

        /// <summary>
        /// Computes the distance between this and another
        /// <c>Envelope</c>.
        /// The distance between overlapping Envelopes is 0.  Otherwise, the
        /// distance is the Euclidean distance between the closest points.
        /// </summary>
        /// <returns>The distance between this and another <c>Envelope</c>.</returns>
        public double Distance(Envelope env)
        {
            if (Intersects(env))
                return 0;

            double dx = 0.0;

            if (_maxx < env.MinX)
                dx = env.MinX - _maxx;
            else if (_minx > env.MaxX)
                dx = _minx - env.MaxX;

            double dy = 0.0;

            if (_maxy < env.MinY)
                dy = env.MinY - _maxy;
            else if (_miny > env.MaxY)
                dy = _miny - env.MaxY;

            // if either is zero, the envelopes overlap either vertically or horizontally
            if (dx == 0.0)
                return dy;
            if (dy == 0.0)
                return dx;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            var otherE = other as Envelope;
            if (otherE != null)
                return Equals(otherE);

#pragma warning disable 612,618
            if (!(other is Envelope))
                return false;

            return this.Equals((Envelope)other);
#pragma warning restore 612,618
        }

        /// <inheritdoc/>
        public bool Equals(Envelope other)
        {
            if (IsNull)
                return other.IsNull;

            return _maxx == other.MaxX && _maxy == other.MaxY &&
                   _minx == other.MinX && _miny == other.MinY;
        }

        /// <inheritdoc/>
        public int CompareTo(object other)
        {
            return CompareTo((Envelope)other);
        }

        /// <inheritdoc/>
        public int CompareTo(Envelope other)
        {
            if (IsNull && other.IsNull)
                return 0;
            if (!IsNull && other.IsNull)
                return 1;
            if (IsNull && !other.IsNull)
                return -1;

            if (Area > other.Area)
                return 1;
            if (Area < other.Area)
                return -1;
            return 0;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var result = 17;
            // ReSharper disable NonReadonlyFieldInGetHashCode
            result = 37 * result + GetHashCode(_minx);
            result = 37 * result + GetHashCode(_maxx);
            result = 37 * result + GetHashCode(_miny);
            result = 37 * result + GetHashCode(_maxy);
            // ReSharper restore NonReadonlyFieldInGetHashCode
            return result;
        }

        private static int GetHashCode(double value)
        {
            var f = BitConverter.DoubleToInt64Bits(value);
            return (int)(f ^ (f >> 32));
        }

        //public static bool operator ==(Envelope obj1, Envelope obj2)
        //{
        //    return Equals(obj1, obj2);
        //}

        //public static bool operator !=(Envelope obj1, Envelope obj2)
        //{
        //    return !(obj1 == obj2);
        //}

        /// <summary>
        /// Creates a deep copy of the current envelope.
        /// </summary>
        /// <returns></returns>
        public Envelope Clone()
        {
            if (IsNull)
            {
                // #179: This will create a new 'NULL' envelope
                return new Envelope();
            }
            return new Envelope(_minx, _maxx, _miny, _maxy);
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Env[");
            if (IsNull)
            {
                sb.Append("Null]");
            }
            else
            {
                sb.AppendFormat(NumberFormatInfo.InvariantInfo, "{0:R} : {1:R}, ", _minx, _maxx);
                sb.AppendFormat(NumberFormatInfo.InvariantInfo, "{0:R} : {1:R}]", _miny, _maxy);
            }
            return sb.ToString();

            //return "Env[" + _minx + " : " + _maxx + ", " + _miny + " : " + _maxy + "]";
        }

        /// <summary>
        /// Method to parse an envelope from its <see cref="Envelope.ToString"/> value
        /// </summary>
        /// <param name="envelope">The envelope string</param>
        /// <returns>The envelope</returns>
        public static Envelope Parse(string envelope)
        {
            if (string.IsNullOrEmpty(envelope))
                throw new ArgumentNullException("envelope");
            if (!(envelope.StartsWith("Env[") && envelope.EndsWith("]")))
                throw new ArgumentException("Not a valid envelope string", "envelope");

            // test for null
            envelope = envelope.Substring(4, envelope.Length - 5);
            if (envelope == "Null")
                return new Envelope();

            // Parse values
            var ordinatesValues = new double[4];
            var ordinateLabel = new[] { "x", "y" };
            var j = 0;

            // split into ranges
            var parts = envelope.Split(',');
            if (parts.Length != 2)
                throw new ArgumentException("Does not provide two ranges", "envelope");

            foreach (var part in parts)
            {
                // Split int min/max
                var ordinates = part.Split(':');
                if (ordinates.Length != 2)
                    throw new ArgumentException("Does not provide just min and max values", "envelope");

                if (!double.TryParse(ordinates[0].Trim(), NumberStyles.Number, NumberFormatInfo.InvariantInfo, out ordinatesValues[2 * j]))
                    throw new ArgumentException(string.Format("Could not parse min {0}-Ordinate", ordinateLabel[j]), "envelope");
                if (!double.TryParse(ordinates[1].Trim(), NumberStyles.Number, NumberFormatInfo.InvariantInfo, out ordinatesValues[2 * j + 1]))
                    throw new ArgumentException(string.Format("Could not parse max {0}-Ordinate", ordinateLabel[j]), "envelope");
                j++;
            }

            return new Envelope(ordinatesValues[0], ordinatesValues[1],
                                ordinatesValues[2], ordinatesValues[3]);
        }
    }
}