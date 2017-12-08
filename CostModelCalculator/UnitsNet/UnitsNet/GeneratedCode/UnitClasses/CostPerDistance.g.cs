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
    ///     The price of so something per unit distance (absolute value of length). Ex. Waterjetting cost is calculated in dollars per inch.
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
#if WINDOWS_UWP
    public sealed partial class CostPerDistance
#else
    [DataContract]
    public partial struct CostPerDistance : IUnit, IComparable, IComparable<CostPerDistance>
#endif
    {
        /// <summary>
        ///     Base unit of CostPerDistance.
        /// </summary>
        [DataMember]
        private double _dollarsPerMeter;

        public double Value
        {
            get { return _dollarsPerMeter; }
            set { _dollarsPerMeter = value; }
        }
        public double ANSIValue
        {
            get { return DollarsPerInch; }
            set { _dollarsPerMeter = FromDollarsPerInch(value).DollarsPerMeter; }
        }

        public string ANSIUnitString => CostPerDistanceUnit.DollarPerInch.ToString();
        public string ANSIUnitAbbreviation => GetAbbreviation(CostPerDistanceUnit.DollarPerInch);
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
        public CostPerDistance() : this(0)
        {
        }
#endif

        public CostPerDistance(double dollarspermeter)
        {
            _dollarsPerMeter = Convert.ToDouble(dollarspermeter);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        CostPerDistance(long dollarspermeter)
        {
            _dollarsPerMeter = Convert.ToDouble(dollarspermeter);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
        // Decimal type not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        CostPerDistance(decimal dollarspermeter)
        {
            _dollarsPerMeter = Convert.ToDouble(dollarspermeter);
        }

        #region Properties

        public static CostPerDistanceUnit BaseUnit
        {
            get { return CostPerDistanceUnit.DollarPerMeter; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerCentimeter.
        /// </summary>
        public double DollarsPerCentimeter
        {
            get { return Math.Abs(_dollarsPerMeter)*1e-2; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerFoot.
        /// </summary>
        public double DollarsPerFoot
        {
            get { return Math.Abs(_dollarsPerMeter)*0.3048; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerInch.
        /// </summary>
        public double DollarsPerInch
        {
            get { return Math.Abs(_dollarsPerMeter)*0.0254; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerKilometer.
        /// </summary>
        public double DollarsPerKilometer
        {
            get { return Math.Abs(_dollarsPerMeter)*1e+3; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerMeter.
        /// </summary>
        public double DollarsPerMeter
        {
            get { return Math.Abs(_dollarsPerMeter); }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerMile.
        /// </summary>
        public double DollarsPerMile
        {
            get { return Math.Abs(_dollarsPerMeter)*1609.34; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerMillimeter.
        /// </summary>
        public double DollarsPerMillimeter
        {
            get { return Math.Abs(_dollarsPerMeter)*1e-3; }
        }

        /// <summary>
        ///     Get CostPerDistance in DollarsPerNauticalMile.
        /// </summary>
        public double DollarsPerNauticalMile
        {
            get { return Math.Abs(_dollarsPerMeter)*1852; }
        }

        #endregion

        #region Static

        public static CostPerDistance Zero
        {
            get { return new CostPerDistance(); }
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerCentimeter.
        /// </summary>
        public static CostPerDistance FromDollarsPerCentimeter(double dollarspercentimeter)
        {
            return new CostPerDistance(Math.Abs(dollarspercentimeter)*1e+2);
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerFoot.
        /// </summary>
        public static CostPerDistance FromDollarsPerFoot(double dollarsperfoot)
        {
            return new CostPerDistance(Math.Abs(dollarsperfoot)/0.3048);
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerInch.
        /// </summary>
        public static CostPerDistance FromDollarsPerInch(double dollarsperinch)
        {
            return new CostPerDistance(Math.Abs(dollarsperinch)/0.0254);
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerKilometer.
        /// </summary>
        public static CostPerDistance FromDollarsPerKilometer(double dollarsperkilometer)
        {
            return new CostPerDistance(Math.Abs(dollarsperkilometer)*1e-3);
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerMeter.
        /// </summary>
        public static CostPerDistance FromDollarsPerMeter(double dollarspermeter)
        {
            return new CostPerDistance(Math.Abs(dollarspermeter));
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerMile.
        /// </summary>
        public static CostPerDistance FromDollarsPerMile(double dollarspermile)
        {
            return new CostPerDistance(Math.Abs(dollarspermile)/1609.34);
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerMillimeter.
        /// </summary>
        public static CostPerDistance FromDollarsPerMillimeter(double dollarspermillimeter)
        {
            return new CostPerDistance(Math.Abs(dollarspermillimeter)*1e+3);
        }

        /// <summary>
        ///     Get CostPerDistance from DollarsPerNauticalMile.
        /// </summary>
        public static CostPerDistance FromDollarsPerNauticalMile(double dollarspernauticalmile)
        {
            return new CostPerDistance(Math.Abs(dollarspernauticalmile)/1852);
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerCentimeter.
        /// </summary>
        public static CostPerDistance? FromDollarsPerCentimeter(double? dollarspercentimeter)
        {
            if (dollarspercentimeter.HasValue)
            {
                return FromDollarsPerCentimeter(dollarspercentimeter.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerFoot.
        /// </summary>
        public static CostPerDistance? FromDollarsPerFoot(double? dollarsperfoot)
        {
            if (dollarsperfoot.HasValue)
            {
                return FromDollarsPerFoot(dollarsperfoot.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerInch.
        /// </summary>
        public static CostPerDistance? FromDollarsPerInch(double? dollarsperinch)
        {
            if (dollarsperinch.HasValue)
            {
                return FromDollarsPerInch(dollarsperinch.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerKilometer.
        /// </summary>
        public static CostPerDistance? FromDollarsPerKilometer(double? dollarsperkilometer)
        {
            if (dollarsperkilometer.HasValue)
            {
                return FromDollarsPerKilometer(dollarsperkilometer.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerMeter.
        /// </summary>
        public static CostPerDistance? FromDollarsPerMeter(double? dollarspermeter)
        {
            if (dollarspermeter.HasValue)
            {
                return FromDollarsPerMeter(dollarspermeter.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerMile.
        /// </summary>
        public static CostPerDistance? FromDollarsPerMile(double? dollarspermile)
        {
            if (dollarspermile.HasValue)
            {
                return FromDollarsPerMile(dollarspermile.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerMillimeter.
        /// </summary>
        public static CostPerDistance? FromDollarsPerMillimeter(double? dollarspermillimeter)
        {
            if (dollarspermillimeter.HasValue)
            {
                return FromDollarsPerMillimeter(dollarspermillimeter.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostPerDistance from nullable DollarsPerNauticalMile.
        /// </summary>
        public static CostPerDistance? FromDollarsPerNauticalMile(double? dollarspernauticalmile)
        {
            if (dollarspernauticalmile.HasValue)
            {
                return FromDollarsPerNauticalMile(dollarspernauticalmile.Value);
            }
            else
            {
                return null;
            }
        }

#endif

        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="CostPerDistanceUnit" /> to <see cref="CostPerDistance" />.
        /// </summary>
        /// <param name="val">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>CostPerDistance unit value.</returns>
        public static CostPerDistance From(double val, CostPerDistanceUnit fromUnit)
        {
            switch (fromUnit)
            {
                case CostPerDistanceUnit.DollarPerCentimeter:
                    return FromDollarsPerCentimeter(val);
                case CostPerDistanceUnit.DollarPerFoot:
                    return FromDollarsPerFoot(val);
                case CostPerDistanceUnit.DollarPerInch:
                    return FromDollarsPerInch(val);
                case CostPerDistanceUnit.DollarPerKilometer:
                    return FromDollarsPerKilometer(val);
                case CostPerDistanceUnit.DollarPerMeter:
                    return FromDollarsPerMeter(val);
                case CostPerDistanceUnit.DollarPerMile:
                    return FromDollarsPerMile(val);
                case CostPerDistanceUnit.DollarPerMillimeter:
                    return FromDollarsPerMillimeter(val);
                case CostPerDistanceUnit.DollarPerNauticalMile:
                    return FromDollarsPerNauticalMile(val);

                default:
                    throw new NotImplementedException("fromUnit: " + fromUnit);
            }
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="CostPerDistanceUnit" /> to <see cref="CostPerDistance" />.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>CostPerDistance unit value.</returns>
        public static CostPerDistance? From(double? value, CostPerDistanceUnit fromUnit)
        {
            if (!value.HasValue)
            {
                return null;
            }
            switch (fromUnit)
            {
                case CostPerDistanceUnit.DollarPerCentimeter:
                    return FromDollarsPerCentimeter(value.Value);
                case CostPerDistanceUnit.DollarPerFoot:
                    return FromDollarsPerFoot(value.Value);
                case CostPerDistanceUnit.DollarPerInch:
                    return FromDollarsPerInch(value.Value);
                case CostPerDistanceUnit.DollarPerKilometer:
                    return FromDollarsPerKilometer(value.Value);
                case CostPerDistanceUnit.DollarPerMeter:
                    return FromDollarsPerMeter(value.Value);
                case CostPerDistanceUnit.DollarPerMile:
                    return FromDollarsPerMile(value.Value);
                case CostPerDistanceUnit.DollarPerMillimeter:
                    return FromDollarsPerMillimeter(value.Value);
                case CostPerDistanceUnit.DollarPerNauticalMile:
                    return FromDollarsPerNauticalMile(value.Value);

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
        public static string GetAbbreviation(CostPerDistanceUnit unit)
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
        public static string GetAbbreviation(CostPerDistanceUnit unit, [CanBeNull] Culture culture)
        {
            return UnitSystem.GetCached(culture).GetDefaultAbbreviation(unit);
        }

        #endregion

        #region Arithmetic Operators

#if !WINDOWS_UWP
        public static CostPerDistance operator -(CostPerDistance right)
        {
            return new CostPerDistance(-right._dollarsPerMeter);
        }

        public static CostPerDistance operator +(CostPerDistance left, CostPerDistance right)
        {
            return new CostPerDistance(left._dollarsPerMeter + right._dollarsPerMeter);
        }

        public static CostPerDistance operator -(CostPerDistance left, CostPerDistance right)
        {
            return new CostPerDistance(left._dollarsPerMeter - right._dollarsPerMeter);
        }

        public static CostPerDistance operator *(double left, CostPerDistance right)
        {
            return new CostPerDistance(left*right._dollarsPerMeter);
        }

        public static CostPerDistance operator *(CostPerDistance left, double right)
        {
            return new CostPerDistance(left._dollarsPerMeter*(double)right);
        }

        public static CostPerDistance operator /(CostPerDistance left, double right)
        {
            return new CostPerDistance(left._dollarsPerMeter/(double)right);
        }

        public static double operator /(CostPerDistance left, CostPerDistance right)
        {
            return Convert.ToDouble(left._dollarsPerMeter/right._dollarsPerMeter);
        }
#endif

        #endregion

        #region Equality / IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!(obj is CostPerDistance)) throw new ArgumentException("Expected type CostPerDistance.", "obj");
            return CompareTo((CostPerDistance) obj);
        }

#if WINDOWS_UWP
        internal
#else
        public
#endif
        int CompareTo(CostPerDistance other)
        {
            return _dollarsPerMeter.CompareTo(other._dollarsPerMeter);
        }

#if !WINDOWS_UWP
        public static bool operator <=(CostPerDistance left, CostPerDistance right)
        {
            return left._dollarsPerMeter <= right._dollarsPerMeter;
        }

        public static bool operator >=(CostPerDistance left, CostPerDistance right)
        {
            return left._dollarsPerMeter >= right._dollarsPerMeter;
        }

        public static bool operator <(CostPerDistance left, CostPerDistance right)
        {
            return left._dollarsPerMeter < right._dollarsPerMeter;
        }

        public static bool operator >(CostPerDistance left, CostPerDistance right)
        {
            return left._dollarsPerMeter > right._dollarsPerMeter;
        }

        public static bool operator ==(CostPerDistance left, CostPerDistance right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._dollarsPerMeter == right._dollarsPerMeter;
        }

        public static bool operator !=(CostPerDistance left, CostPerDistance right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._dollarsPerMeter != right._dollarsPerMeter;
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return _dollarsPerMeter.Equals(((CostPerDistance) obj)._dollarsPerMeter);
        }

        public override int GetHashCode()
        {
            return _dollarsPerMeter.GetHashCode();
        }

        #endregion

        #region Conversion

        /// <summary>
        ///     Convert to the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>Value in new unit if successful, exception otherwise.</returns>
        /// <exception cref="NotImplementedException">If conversion was not successful.</exception>
        public double As(CostPerDistanceUnit unit)
        {
            switch (unit)
            {
                case CostPerDistanceUnit.DollarPerCentimeter:
                    return DollarsPerCentimeter;
                case CostPerDistanceUnit.DollarPerFoot:
                    return DollarsPerFoot;
                case CostPerDistanceUnit.DollarPerInch:
                    return DollarsPerInch;
                case CostPerDistanceUnit.DollarPerKilometer:
                    return DollarsPerKilometer;
                case CostPerDistanceUnit.DollarPerMeter:
                    return DollarsPerMeter;
                case CostPerDistanceUnit.DollarPerMile:
                    return DollarsPerMile;
                case CostPerDistanceUnit.DollarPerMillimeter:
                    return DollarsPerMillimeter;
                case CostPerDistanceUnit.DollarPerNauticalMile:
                    return DollarsPerNauticalMile;

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
        public static CostPerDistance Parse(string str)
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
        public static CostPerDistance Parse(string str, [CanBeNull] Culture culture)
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
            return quantities.Aggregate((x, y) => CostPerDistance.FromDollarsPerMeter(x.DollarsPerMeter + y.DollarsPerMeter));
        }

        /// <summary>
        ///     Parse a string given a particular regular expression.
        /// </summary>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        private static List<CostPerDistance> ParseWithRegex(string regexString, string str, IFormatProvider formatProvider = null)
        {
            var regex = new Regex(regexString);
            MatchCollection matches = regex.Matches(str.Trim());
            var converted = new List<CostPerDistance>();

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
                    CostPerDistanceUnit unit = ParseUnit(unitString, formatProvider);
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
        public static CostPerDistanceUnit ParseUnit(string str)
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
        public static CostPerDistanceUnit ParseUnit(string str, [CanBeNull] string cultureName)
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
        static CostPerDistanceUnit ParseUnit(string str, IFormatProvider formatProvider = null)
        {
            if (str == null) throw new ArgumentNullException("str");

            var unitSystem = UnitSystem.GetCached(formatProvider);
            var unit = unitSystem.Parse<CostPerDistanceUnit>(str.Trim());

            if (unit == CostPerDistanceUnit.Undefined)
            {
                var newEx = new UnitsNetException("Error parsing string. The unit is not a recognized CostPerDistanceUnit.");
                newEx.Data["input"] = str;
                newEx.Data["formatprovider"] = formatProvider?.ToString() ?? "(null)";
                throw newEx;
            }

            return unit;
        }

        #endregion

        /// <summary>
        ///     Set the default unit used by ToString(). Default is DollarPerMeter
        /// </summary>
        public static CostPerDistanceUnit ToStringDefaultUnit { get; set; } = CostPerDistanceUnit.DollarPerMeter;

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
        public string ToString(CostPerDistanceUnit unit)
        {
            return ToString(unit, null, 2);
        }

        /// <summary>
        ///     Get string representation of value and unit. Using two significant digits after radix.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <returns>String representation.</returns>
        public string ToString(CostPerDistanceUnit unit, [CanBeNull] Culture culture)
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
        public string ToString(CostPerDistanceUnit unit, [CanBeNull] Culture culture, int significantDigitsAfterRadix)
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
        public string ToString(CostPerDistanceUnit unit, [CanBeNull] Culture culture, [NotNull] string format,
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
        /// Represents the largest possible value of CostPerDistance
        /// </summary>
        public static CostPerDistance MaxValue
        {
            get
            {
                return new CostPerDistance(double.MaxValue);
            }
        }

        /// <summary>
        /// Represents the smallest possible value of CostPerDistance
        /// </summary>
        public static CostPerDistance MinValue
        {
            get
            {
                return new CostPerDistance(double.MinValue);
            }
        }
    }
}
