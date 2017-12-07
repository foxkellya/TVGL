// Copyright © 2007 by Initial Force AS.  All rights reserved.
// https://github.com/anjdreas/UnitsNet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnitsNet.Units;

#if WINDOWS_UWP
using Culture = System.String;
#else
using Culture = System.IFormatProvider;
#endif

// ReSharper disable once CheckNamespace

namespace UnitsNet
{
    /// <summary>
    ///     Speed of a joining operation, used for linear and rotary friction welding
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
#if WINDOWS_UWP
    public sealed partial class JoiningRate
#else
    [DataContract]
    public partial struct JoiningRate : IUnit, IComparable, IComparable<JoiningRate>
#endif
    {
        /// <summary>
        ///     Base unit of JoiningRate.
        /// </summary>
        [DataMember]
        private double _squareMetersPerSecond;

        public double Value
        {
            get { return _squareMetersPerSecond; }
            set { _squareMetersPerSecond = value; }
        }

        public double ANSIValue
        {
            get { return SquareInchesPerSecond; }
            set { _squareMetersPerSecond = FromSquareInchesPerSecond(value).SquareMetersPerSecond; }
        }

        public string ANSIUnitString => JoiningRateUnit.SquareInchPerSecond.ToString();
        public string ANSIUnitAbbreviation => GetAbbreviation(JoiningRateUnit.SquareInchPerSecond);
        public double To(CostModelViewUnit unit)
        {
            throw new NotImplementedException();
        }

        public void From(double value, CostModelViewUnit unit)
        {
            throw new NotImplementedException();
        }

        public string BaseUnitString => ToStringDefaultUnit.ToString();
        public string BaseUnitAbbreviation => GetAbbreviation(ToStringDefaultUnit);


#if WINDOWS_UWP
        public JoiningRate() : this(0)
        {
        }
#endif

        public JoiningRate(double squaremeterspersecond)
        {
            _squareMetersPerSecond = Convert.ToDouble(squaremeterspersecond);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        JoiningRate(long squaremeterspersecond)
        {
            _squareMetersPerSecond = Convert.ToDouble(squaremeterspersecond);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
        // Decimal type not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        JoiningRate(decimal squaremeterspersecond)
        {
            _squareMetersPerSecond = Convert.ToDouble(squaremeterspersecond);
        }

        #region Properties

        public static JoiningRateUnit BaseUnit
        {
            get { return JoiningRateUnit.SquareMeterPerSecond; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareCentimetersPerMinute.
        /// </summary>
        public double SquareCentimetersPerMinute
        {
            get { return _squareMetersPerSecond*6e+5; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareCentimetersPerSecond.
        /// </summary>
        public double SquareCentimetersPerSecond
        {
            get { return _squareMetersPerSecond*1e+4; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareInchesPerMinute.
        /// </summary>
        public double SquareInchesPerMinute
        {
            get { return _squareMetersPerSecond*93000.2; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareInchesPerSecond.
        /// </summary>
        public double SquareInchesPerSecond
        {
            get { return _squareMetersPerSecond*1550; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareMetersPerMinute.
        /// </summary>
        public double SquareMetersPerMinute
        {
            get { return _squareMetersPerSecond*60; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareMetersPerSecond.
        /// </summary>
        public double SquareMetersPerSecond
        {
            get { return _squareMetersPerSecond; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareMillimetersPerMinute.
        /// </summary>
        public double SquareMillimetersPerMinute
        {
            get { return _squareMetersPerSecond*6e+7; }
        }

        /// <summary>
        ///     Get JoiningRate in SquareMillimetersPerSecond.
        /// </summary>
        public double SquareMillimetersPerSecond
        {
            get { return _squareMetersPerSecond*1e+6; }
        }

        #endregion

        #region Static

        public static JoiningRate Zero
        {
            get { return new JoiningRate(); }
        }

        /// <summary>
        ///     Get JoiningRate from SquareCentimetersPerMinute.
        /// </summary>
        public static JoiningRate FromSquareCentimetersPerMinute(double squarecentimetersperminute)
        {
            return new JoiningRate(squarecentimetersperminute*6e-5);
        }

        /// <summary>
        ///     Get JoiningRate from SquareCentimetersPerSecond.
        /// </summary>
        public static JoiningRate FromSquareCentimetersPerSecond(double squarecentimeterspersecond)
        {
            return new JoiningRate(squarecentimeterspersecond*1e-4);
        }

        /// <summary>
        ///     Get JoiningRate from SquareInchesPerMinute.
        /// </summary>
        public static JoiningRate FromSquareInchesPerMinute(double squareinchesperminute)
        {
            return new JoiningRate(squareinchesperminute/93000.2);
        }

        /// <summary>
        ///     Get JoiningRate from SquareInchesPerSecond.
        /// </summary>
        public static JoiningRate FromSquareInchesPerSecond(double squareinchespersecond)
        {
            return new JoiningRate(squareinchespersecond/1550);
        }

        /// <summary>
        ///     Get JoiningRate from SquareMetersPerMinute.
        /// </summary>
        public static JoiningRate FromSquareMetersPerMinute(double squaremetersperminute)
        {
            return new JoiningRate(squaremetersperminute/60);
        }

        /// <summary>
        ///     Get JoiningRate from SquareMetersPerSecond.
        /// </summary>
        public static JoiningRate FromSquareMetersPerSecond(double squaremeterspersecond)
        {
            return new JoiningRate(squaremeterspersecond);
        }

        /// <summary>
        ///     Get JoiningRate from SquareMillimetersPerMinute.
        /// </summary>
        public static JoiningRate FromSquareMillimetersPerMinute(double squaremillimetersperminute)
        {
            return new JoiningRate(squaremillimetersperminute*6e-7);
        }

        /// <summary>
        ///     Get JoiningRate from SquareMillimetersPerSecond.
        /// </summary>
        public static JoiningRate FromSquareMillimetersPerSecond(double squaremillimeterspersecond)
        {
            return new JoiningRate(squaremillimeterspersecond*1e-6);
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareCentimetersPerMinute.
        /// </summary>
        public static JoiningRate? FromSquareCentimetersPerMinute(double? squarecentimetersperminute)
        {
            if (squarecentimetersperminute.HasValue)
            {
                return FromSquareCentimetersPerMinute(squarecentimetersperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareCentimetersPerSecond.
        /// </summary>
        public static JoiningRate? FromSquareCentimetersPerSecond(double? squarecentimeterspersecond)
        {
            if (squarecentimeterspersecond.HasValue)
            {
                return FromSquareCentimetersPerSecond(squarecentimeterspersecond.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareInchesPerMinute.
        /// </summary>
        public static JoiningRate? FromSquareInchesPerMinute(double? squareinchesperminute)
        {
            if (squareinchesperminute.HasValue)
            {
                return FromSquareInchesPerMinute(squareinchesperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareInchesPerSecond.
        /// </summary>
        public static JoiningRate? FromSquareInchesPerSecond(double? squareinchespersecond)
        {
            if (squareinchespersecond.HasValue)
            {
                return FromSquareInchesPerSecond(squareinchespersecond.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareMetersPerMinute.
        /// </summary>
        public static JoiningRate? FromSquareMetersPerMinute(double? squaremetersperminute)
        {
            if (squaremetersperminute.HasValue)
            {
                return FromSquareMetersPerMinute(squaremetersperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareMetersPerSecond.
        /// </summary>
        public static JoiningRate? FromSquareMetersPerSecond(double? squaremeterspersecond)
        {
            if (squaremeterspersecond.HasValue)
            {
                return FromSquareMetersPerSecond(squaremeterspersecond.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareMillimetersPerMinute.
        /// </summary>
        public static JoiningRate? FromSquareMillimetersPerMinute(double? squaremillimetersperminute)
        {
            if (squaremillimetersperminute.HasValue)
            {
                return FromSquareMillimetersPerMinute(squaremillimetersperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable JoiningRate from nullable SquareMillimetersPerSecond.
        /// </summary>
        public static JoiningRate? FromSquareMillimetersPerSecond(double? squaremillimeterspersecond)
        {
            if (squaremillimeterspersecond.HasValue)
            {
                return FromSquareMillimetersPerSecond(squaremillimeterspersecond.Value);
            }
            else
            {
                return null;
            }
        }

#endif

        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="JoiningRateUnit" /> to <see cref="JoiningRate" />.
        /// </summary>
        /// <param name="val">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>JoiningRate unit value.</returns>
        public static JoiningRate From(double val, JoiningRateUnit fromUnit)
        {
            switch (fromUnit)
            {
                case JoiningRateUnit.SquareCentimeterPerMinute:
                    return FromSquareCentimetersPerMinute(val);
                case JoiningRateUnit.SquareCentimeterPerSecond:
                    return FromSquareCentimetersPerSecond(val);
                case JoiningRateUnit.SquareInchPerMinute:
                    return FromSquareInchesPerMinute(val);
                case JoiningRateUnit.SquareInchPerSecond:
                    return FromSquareInchesPerSecond(val);
                case JoiningRateUnit.SquareMeterPerMinute:
                    return FromSquareMetersPerMinute(val);
                case JoiningRateUnit.SquareMeterPerSecond:
                    return FromSquareMetersPerSecond(val);
                case JoiningRateUnit.SquareMillimeterPerMinute:
                    return FromSquareMillimetersPerMinute(val);
                case JoiningRateUnit.SquareMillimeterPerSecond:
                    return FromSquareMillimetersPerSecond(val);

                default:
                    throw new NotImplementedException("fromUnit: " + fromUnit);
            }
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="JoiningRateUnit" /> to <see cref="JoiningRate" />.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>JoiningRate unit value.</returns>
        public static JoiningRate? From(double? value, JoiningRateUnit fromUnit)
        {
            if (!value.HasValue)
            {
                return null;
            }
            switch (fromUnit)
            {
                case JoiningRateUnit.SquareCentimeterPerMinute:
                    return FromSquareCentimetersPerMinute(value.Value);
                case JoiningRateUnit.SquareCentimeterPerSecond:
                    return FromSquareCentimetersPerSecond(value.Value);
                case JoiningRateUnit.SquareInchPerMinute:
                    return FromSquareInchesPerMinute(value.Value);
                case JoiningRateUnit.SquareInchPerSecond:
                    return FromSquareInchesPerSecond(value.Value);
                case JoiningRateUnit.SquareMeterPerMinute:
                    return FromSquareMetersPerMinute(value.Value);
                case JoiningRateUnit.SquareMeterPerSecond:
                    return FromSquareMetersPerSecond(value.Value);
                case JoiningRateUnit.SquareMillimeterPerMinute:
                    return FromSquareMillimetersPerMinute(value.Value);
                case JoiningRateUnit.SquareMillimeterPerSecond:
                    return FromSquareMillimetersPerSecond(value.Value);

                default:
                    throw new NotImplementedException("fromUnit: " + fromUnit);
            }
        }
#endif

        /// <summary>
        ///     Get unit abbreviation string.
        /// </summary>
        /// <param name="unit">Unit to get abbreviation for.</param>
        /// <returns>Unit abbreviation string.</returns>
        [UsedImplicitly]
        public static string GetAbbreviation(JoiningRateUnit unit)
        {
            return GetAbbreviation(unit, null);
        }

        /// <summary>
        ///     Get unit abbreviation string.
        /// </summary>
        /// <param name="unit">Unit to get abbreviation for.</param>
        /// <param name="culture">Culture to use for localization. Defaults to Thread.CurrentUICulture.</param>
        /// <returns>Unit abbreviation string.</returns>
        [UsedImplicitly]
        public static string GetAbbreviation(JoiningRateUnit unit, [CanBeNull] Culture culture)
        {
            return UnitSystem.GetCached(culture).GetDefaultAbbreviation(unit);
        }

        #endregion

        #region Arithmetic Operators

#if !WINDOWS_UWP
        public static JoiningRate operator -(JoiningRate right)
        {
            return new JoiningRate(-right._squareMetersPerSecond);
        }

        public static JoiningRate operator +(JoiningRate left, JoiningRate right)
        {
            return new JoiningRate(left._squareMetersPerSecond + right._squareMetersPerSecond);
        }

        public static JoiningRate operator -(JoiningRate left, JoiningRate right)
        {
            return new JoiningRate(left._squareMetersPerSecond - right._squareMetersPerSecond);
        }

        public static JoiningRate operator *(double left, JoiningRate right)
        {
            return new JoiningRate(left*right._squareMetersPerSecond);
        }

        public static JoiningRate operator *(JoiningRate left, double right)
        {
            return new JoiningRate(left._squareMetersPerSecond*(double)right);
        }

        public static JoiningRate operator /(JoiningRate left, double right)
        {
            return new JoiningRate(left._squareMetersPerSecond/(double)right);
        }

        public static double operator /(JoiningRate left, JoiningRate right)
        {
            return Convert.ToDouble(left._squareMetersPerSecond/right._squareMetersPerSecond);
        }
#endif

        #endregion

        #region Equality / IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!(obj is JoiningRate)) throw new ArgumentException("Expected type JoiningRate.", "obj");
            return CompareTo((JoiningRate) obj);
        }

#if WINDOWS_UWP
        internal
#else
        public
#endif
        int CompareTo(JoiningRate other)
        {
            return _squareMetersPerSecond.CompareTo(other._squareMetersPerSecond);
        }

#if !WINDOWS_UWP
        public static bool operator <=(JoiningRate left, JoiningRate right)
        {
            return left._squareMetersPerSecond <= right._squareMetersPerSecond;
        }

        public static bool operator >=(JoiningRate left, JoiningRate right)
        {
            return left._squareMetersPerSecond >= right._squareMetersPerSecond;
        }

        public static bool operator <(JoiningRate left, JoiningRate right)
        {
            return left._squareMetersPerSecond < right._squareMetersPerSecond;
        }

        public static bool operator >(JoiningRate left, JoiningRate right)
        {
            return left._squareMetersPerSecond > right._squareMetersPerSecond;
        }

        public static bool operator ==(JoiningRate left, JoiningRate right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._squareMetersPerSecond == right._squareMetersPerSecond;
        }

        public static bool operator !=(JoiningRate left, JoiningRate right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._squareMetersPerSecond != right._squareMetersPerSecond;
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return _squareMetersPerSecond.Equals(((JoiningRate) obj)._squareMetersPerSecond);
        }

        public override int GetHashCode()
        {
            return _squareMetersPerSecond.GetHashCode();
        }

        #endregion

        #region Conversion

        /// <summary>
        ///     Convert to the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>Value in new unit if successful, exception otherwise.</returns>
        /// <exception cref="NotImplementedException">If conversion was not successful.</exception>
        public double As(JoiningRateUnit unit)
        {
            switch (unit)
            {
                case JoiningRateUnit.SquareCentimeterPerMinute:
                    return SquareCentimetersPerMinute;
                case JoiningRateUnit.SquareCentimeterPerSecond:
                    return SquareCentimetersPerSecond;
                case JoiningRateUnit.SquareInchPerMinute:
                    return SquareInchesPerMinute;
                case JoiningRateUnit.SquareInchPerSecond:
                    return SquareInchesPerSecond;
                case JoiningRateUnit.SquareMeterPerMinute:
                    return SquareMetersPerMinute;
                case JoiningRateUnit.SquareMeterPerSecond:
                    return SquareMetersPerSecond;
                case JoiningRateUnit.SquareMillimeterPerMinute:
                    return SquareMillimetersPerMinute;
                case JoiningRateUnit.SquareMillimeterPerSecond:
                    return SquareMillimetersPerSecond;

                default:
                    throw new NotImplementedException("unit: " + unit);
            }
        }

        #endregion

        #region Parsing

        /// <summary>
        ///     Parse a string with one or two quantities of the format "&lt;quantity&gt; &lt;unit&gt;".
        /// </summary>
        /// <param name="str">String to parse. Typically in the form: {number} {unit}</param>
        /// <example>
        ///     Length.Parse("5.5 m", new CultureInfo("en-US"));
        /// </example>
        /// <exception cref="ArgumentNullException">The value of 'str' cannot be null. </exception>
        /// <exception cref="ArgumentException">
        ///     Expected string to have one or two pairs of quantity and unit in the format
        ///     "&lt;quantity&gt; &lt;unit&gt;". Eg. "5.5 m" or "1ft 2in"
        /// </exception>
        /// <exception cref="AmbiguousUnitParseException">
        ///     More than one unit is represented by the specified unit abbreviation.
        ///     Example: Volume.Parse("1 cup") will throw, because it can refer to any of
        ///     <see cref="VolumeUnit.MetricCup" />, <see cref="VolumeUnit.UsLegalCup" /> and <see cref="VolumeUnit.UsCustomaryCup" />.
        /// </exception>
        /// <exception cref="UnitsNetException">
        ///     If anything else goes wrong, typically due to a bug or unhandled case.
        ///     We wrap exceptions in <see cref="UnitsNetException" /> to allow you to distinguish
        ///     Units.NET exceptions from other exceptions.
        /// </exception>
        public static JoiningRate Parse(string str)
        {
            return Parse(str, null);
        }

        /// <summary>
        ///     Parse a string with one or two quantities of the format "&lt;quantity&gt; &lt;unit&gt;".
        /// </summary>
        /// <param name="str">String to parse. Typically in the form: {number} {unit}</param>
        /// <param name="culture">Format to use when parsing number and unit. If it is null, it defaults to <see cref="NumberFormatInfo.CurrentInfo"/> for parsing the number and <see cref="CultureInfo.CurrentUICulture"/> for parsing the unit abbreviation by culture/language.</param>
        /// <example>
        ///     Length.Parse("5.5 m", new CultureInfo("en-US"));
        /// </example>
        /// <exception cref="ArgumentNullException">The value of 'str' cannot be null. </exception>
        /// <exception cref="ArgumentException">
        ///     Expected string to have one or two pairs of quantity and unit in the format
        ///     "&lt;quantity&gt; &lt;unit&gt;". Eg. "5.5 m" or "1ft 2in"
        /// </exception>
        /// <exception cref="AmbiguousUnitParseException">
        ///     More than one unit is represented by the specified unit abbreviation.
        ///     Example: Volume.Parse("1 cup") will throw, because it can refer to any of
        ///     <see cref="VolumeUnit.MetricCup" />, <see cref="VolumeUnit.UsLegalCup" /> and <see cref="VolumeUnit.UsCustomaryCup" />.
        /// </exception>
        /// <exception cref="UnitsNetException">
        ///     If anything else goes wrong, typically due to a bug or unhandled case.
        ///     We wrap exceptions in <see cref="UnitsNetException" /> to allow you to distinguish
        ///     Units.NET exceptions from other exceptions.
        /// </exception>
        public static JoiningRate Parse(string str, [CanBeNull] Culture culture)
        {
            if (str == null) throw new ArgumentNullException("str");

#if WINDOWS_UWP
            IFormatProvider formatProvider = culture == null ? null : new CultureInfo(culture);
#else
            IFormatProvider formatProvider = culture;
#endif
            var numFormat = formatProvider != null ?
                (NumberFormatInfo) formatProvider.GetFormat(typeof (NumberFormatInfo)) :
                NumberFormatInfo.CurrentInfo;

            var numRegex = string.Format(@"[\d., {0}{1}]*\d",  // allows digits, dots, commas, and spaces in the quantity (must end in digit)
                            numFormat.NumberGroupSeparator,    // adds provided (or current) culture's group separator
                            numFormat.NumberDecimalSeparator); // adds provided (or current) culture's decimal separator
            var exponentialRegex = @"(?:[eE][-+]?\d+)?)";
            var regexString = string.Format(@"(?:\s*(?<value>[-+]?{0}{1}{2}{3})?{4}{5}",
                            numRegex,                // capture base (integral) Quantity value
                            exponentialRegex,        // capture exponential (if any), end of Quantity capturing
                            @"\s?",                  // ignore whitespace (allows both "1kg", "1 kg")
                            @"(?<unit>[^\s\d,]+)",   // capture Unit (non-whitespace) input
                            @"(and)?,?",             // allow "and" & "," separators between quantities
                            @"(?<invalid>[a-z]*)?"); // capture invalid input

            var quantities = ParseWithRegex(regexString, str, formatProvider);
            if (quantities.Count == 0)
            {
                throw new ArgumentException(
                    "Expected string to have at least one pair of quantity and unit in the format"
                    + " \"&lt;quantity&gt; &lt;unit&gt;\". Eg. \"5.5 m\" or \"1ft 2in\"");
            }
            return quantities.Aggregate((x, y) => JoiningRate.FromSquareMetersPerSecond(x.SquareMetersPerSecond + y.SquareMetersPerSecond));
        }

        /// <summary>
        ///     Parse a string given a particular regular expression.
        /// </summary>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        private static List<JoiningRate> ParseWithRegex(string regexString, string str, IFormatProvider formatProvider = null)
        {
            var regex = new Regex(regexString);
            MatchCollection matches = regex.Matches(str.Trim());
            var converted = new List<JoiningRate>();

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

                var valueString = groups["value"].Value;
                var unitString = groups["unit"].Value;
                if (groups["invalid"].Value != "")
                {
                    var newEx = new UnitsNetException("Invalid string detected: " + groups["invalid"].Value);
                    newEx.Data["input"] = str;
                    newEx.Data["matched value"] = valueString;
                    newEx.Data["matched unit"] = unitString;
                    newEx.Data["formatprovider"] = formatProvider == null ? null : formatProvider.ToString();
                    throw newEx;
                }
                if (valueString == "" && unitString == "") continue;

                try
                {
                    JoiningRateUnit unit = ParseUnit(unitString, formatProvider);
                    double value = double.Parse(valueString, formatProvider);

                    converted.Add(From(value, unit));
                }
                catch(AmbiguousUnitParseException)
                {
                    throw;
                }
                catch(Exception ex)
                {
                    var newEx = new UnitsNetException("Error parsing string.", ex);
                    newEx.Data["input"] = str;
                    newEx.Data["matched value"] = valueString;
                    newEx.Data["matched unit"] = unitString;
                    newEx.Data["formatprovider"] = formatProvider == null ? null : formatProvider.ToString();
                    throw newEx;
                }
            }
            return converted;
        }

        /// <summary>
        ///     Parse a unit string.
        /// </summary>
        /// <example>
        ///     Length.ParseUnit("m", new CultureInfo("en-US"));
        /// </example>
        /// <exception cref="ArgumentNullException">The value of 'str' cannot be null. </exception>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        public static JoiningRateUnit ParseUnit(string str)
        {
            return ParseUnit(str, (IFormatProvider)null);
        }

        /// <summary>
        ///     Parse a unit string.
        /// </summary>
        /// <example>
        ///     Length.ParseUnit("m", new CultureInfo("en-US"));
        /// </example>
        /// <exception cref="ArgumentNullException">The value of 'str' cannot be null. </exception>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        public static JoiningRateUnit ParseUnit(string str, [CanBeNull] string cultureName)
        {
            return ParseUnit(str, cultureName == null ? null : new CultureInfo(cultureName));
        }

        /// <summary>
        ///     Parse a unit string.
        /// </summary>
        /// <example>
        ///     Length.ParseUnit("m", new CultureInfo("en-US"));
        /// </example>
        /// <exception cref="ArgumentNullException">The value of 'str' cannot be null. </exception>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
#if WINDOWS_UWP
        internal
#else
        public
#endif
        static JoiningRateUnit ParseUnit(string str, IFormatProvider formatProvider = null)
        {
            if (str == null) throw new ArgumentNullException("str");

            var unitSystem = UnitSystem.GetCached(formatProvider);
            var unit = unitSystem.Parse<JoiningRateUnit>(str.Trim());

            if (unit == JoiningRateUnit.Undefined)
            {
                var newEx = new UnitsNetException("Error parsing string. The unit is not a recognized JoiningRateUnit.");
                newEx.Data["input"] = str;
                newEx.Data["formatprovider"] = formatProvider?.ToString() ?? "(null)";
                throw newEx;
            }

            return unit;
        }

        #endregion

        /// <summary>
        ///     Set the default unit used by ToString(). Default is SquareMeterPerSecond
        /// </summary>
        public static JoiningRateUnit ToStringDefaultUnit { get; set; } = JoiningRateUnit.SquareMeterPerSecond;

        /// <summary>
        ///     Get default string representation of value and unit.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ToString(ToStringDefaultUnit);
        }

        /// <summary>
        ///     Get string representation of value and unit. Using current UI culture and two significant digits after radix.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <returns>String representation.</returns>
        public string ToString(JoiningRateUnit unit)
        {
            return ToString(unit, null, 2);
        }

        /// <summary>
        ///     Get string representation of value and unit. Using two significant digits after radix.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <returns>String representation.</returns>
        public string ToString(JoiningRateUnit unit, [CanBeNull] Culture culture)
        {
            return ToString(unit, culture, 2);
        }

        /// <summary>
        ///     Get string representation of value and unit.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <param name="significantDigitsAfterRadix">The number of significant digits after the radix point.</param>
        /// <returns>String representation.</returns>
        [UsedImplicitly]
        public string ToString(JoiningRateUnit unit, [CanBeNull] Culture culture, int significantDigitsAfterRadix)
        {
            double value = As(unit);
            string format = UnitFormatter.GetFormat(value, significantDigitsAfterRadix);
            return ToString(unit, culture, format);
        }

        /// <summary>
        ///     Get string representation of value and unit.
        /// </summary>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="format">String format to use. Default:  "{0:0.##} {1} for value and unit abbreviation respectively."</param>
        /// <param name="args">Arguments for string format. Value and unit are implictly included as arguments 0 and 1.</param>
        /// <returns>String representation.</returns>
        [UsedImplicitly]
        public string ToString(JoiningRateUnit unit, [CanBeNull] Culture culture, [NotNull] string format,
            [NotNull] params object[] args)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (args == null) throw new ArgumentNullException(nameof(args));

#if WINDOWS_UWP
            IFormatProvider formatProvider = culture == null ? null : new CultureInfo(culture);
#else
            IFormatProvider formatProvider = culture;
#endif
            double value = As(unit);
            object[] formatArgs = UnitFormatter.GetFormatArgs(unit, value, formatProvider, args);
            return string.Format(formatProvider, format, formatArgs);
        }

        /// <summary>
        /// Represents the largest possible value of JoiningRate
        /// </summary>
        public static JoiningRate MaxValue
        {
            get
            {
                return new JoiningRate(double.MaxValue);
            }
        }

        /// <summary>
        /// Represents the smallest possible value of JoiningRate
        /// </summary>
        public static JoiningRate MinValue
        {
            get
            {
                return new JoiningRate(double.MinValue);
            }
        }
    }
}
