﻿// Copyright © 2007 by Initial Force AS.  All rights reserved.
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
    ///     Often referred to as MRR, this is the rate at which material is removed during machining.
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
#if WINDOWS_UWP
    public sealed partial class MaterialRemovalRate
#else
    [DataContract]
    public partial struct MaterialRemovalRate : IUnit, IComparable, IComparable<MaterialRemovalRate>
#endif
    {
        /// <summary>
        ///     Base unit of MaterialRemovalRate.
        /// </summary>
        [DataMember]
        private double _cubicCentimetersPerMinute;

        public double Value
        {
            get { return _cubicCentimetersPerMinute; }
            set { _cubicCentimetersPerMinute = value; }
        }

        public double ANSIValue
        {
            get { return CubicInchesPerMinute; }
            set { _cubicCentimetersPerMinute = FromCubicInchesPerMinute(value).CubicCentimetersPerMinute; }
        }

        public string ANSIUnitString => MaterialRemovalRateUnit.CubicInchPerMinute.ToString();
        public string ANSIUnitAbbreviation => GetAbbreviation(MaterialRemovalRateUnit.CubicInchPerMinute);
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
        public MaterialRemovalRate() : this(0)
        {
        }
#endif

        public MaterialRemovalRate(double cubiccentimetersperminute)
        {
            _cubicCentimetersPerMinute = Convert.ToDouble(cubiccentimetersperminute);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        MaterialRemovalRate(long cubiccentimetersperminute)
        {
            _cubicCentimetersPerMinute = Convert.ToDouble(cubiccentimetersperminute);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
        // Decimal type not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        MaterialRemovalRate(decimal cubiccentimetersperminute)
        {
            _cubicCentimetersPerMinute = Convert.ToDouble(cubiccentimetersperminute);
        }

        #region Properties

        public static MaterialRemovalRateUnit BaseUnit
        {
            get { return MaterialRemovalRateUnit.CubicCentimeterPerMinute; }
        }

        /// <summary>
        ///     Get MaterialRemovalRate in CubicCentimetersPerMinute.
        /// </summary>
        public double CubicCentimetersPerMinute
        {
            get { return _cubicCentimetersPerMinute; }
        }

        /// <summary>
        ///     Get MaterialRemovalRate in CubicInchesPerMinute.
        /// </summary>
        public double CubicInchesPerMinute
        {
            get { return _cubicCentimetersPerMinute*0.0610237; }
        }

        /// <summary>
        ///     Get MaterialRemovalRate in CubicMetersPerHour.
        /// </summary>
        public double CubicMetersPerHour
        {
            get { return _cubicCentimetersPerMinute*6e-5; }
        }

        /// <summary>
        ///     Get MaterialRemovalRate in CubicMetersPerSecond.
        /// </summary>
        public double CubicMetersPerSecond
        {
            get { return _cubicCentimetersPerMinute*1.66667e-8; }
        }

        #endregion

        #region Static

        public static MaterialRemovalRate Zero
        {
            get { return new MaterialRemovalRate(); }
        }

        /// <summary>
        ///     Get MaterialRemovalRate from CubicCentimetersPerMinute.
        /// </summary>
        public static MaterialRemovalRate FromCubicCentimetersPerMinute(double cubiccentimetersperminute)
        {
            return new MaterialRemovalRate(cubiccentimetersperminute);
        }

        /// <summary>
        ///     Get MaterialRemovalRate from CubicInchesPerMinute.
        /// </summary>
        public static MaterialRemovalRate FromCubicInchesPerMinute(double cubicinchesperminute)
        {
            return new MaterialRemovalRate(cubicinchesperminute* 16.3871);
        }

        /// <summary>
        ///     Get MaterialRemovalRate from CubicMetersPerHour.
        /// </summary>
        public static MaterialRemovalRate FromCubicMetersPerHour(double cubicmetersperhour)
        {
            return new MaterialRemovalRate(cubicmetersperhour*6e+5);
        }

        /// <summary>
        ///     Get MaterialRemovalRate from CubicMetersPerSecond.
        /// </summary>
        public static MaterialRemovalRate FromCubicMetersPerSecond(double cubicmeterspersecond)
        {
            return new MaterialRemovalRate(cubicmeterspersecond*1.66667e+8);
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Get nullable MaterialRemovalRate from nullable CubicCentimetersPerMinute.
        /// </summary>
        public static MaterialRemovalRate? FromCubicCentimetersPerMinute(double? cubiccentimetersperminute)
        {
            if (cubiccentimetersperminute.HasValue)
            {
                return FromCubicCentimetersPerMinute(cubiccentimetersperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable MaterialRemovalRate from nullable CubicInchesPerMinute.
        /// </summary>
        public static MaterialRemovalRate? FromCubicInchesPerMinute(double? cubicinchesperminute)
        {
            if (cubicinchesperminute.HasValue)
            {
                return FromCubicInchesPerMinute(cubicinchesperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable MaterialRemovalRate from nullable CubicMetersPerHour.
        /// </summary>
        public static MaterialRemovalRate? FromCubicMetersPerHour(double? cubicmetersperhour)
        {
            if (cubicmetersperhour.HasValue)
            {
                return FromCubicMetersPerHour(cubicmetersperhour.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable MaterialRemovalRate from nullable CubicMetersPerSecond.
        /// </summary>
        public static MaterialRemovalRate? FromCubicMetersPerSecond(double? cubicmeterspersecond)
        {
            if (cubicmeterspersecond.HasValue)
            {
                return FromCubicMetersPerSecond(cubicmeterspersecond.Value);
            }
            else
            {
                return null;
            }
        }

#endif

        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="MaterialRemovalRateUnit" /> to <see cref="MaterialRemovalRate" />.
        /// </summary>
        /// <param name="val">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>MaterialRemovalRate unit value.</returns>
        public static MaterialRemovalRate From(double val, MaterialRemovalRateUnit fromUnit)
        {
            switch (fromUnit)
            {
                case MaterialRemovalRateUnit.CubicCentimeterPerMinute:
                    return FromCubicCentimetersPerMinute(val);
                case MaterialRemovalRateUnit.CubicInchPerMinute:
                    return FromCubicInchesPerMinute(val);
                case MaterialRemovalRateUnit.CubicMeterPerHour:
                    return FromCubicMetersPerHour(val);
                case MaterialRemovalRateUnit.CubicMeterPerSecond:
                    return FromCubicMetersPerSecond(val);

                default:
                    throw new NotImplementedException("fromUnit: " + fromUnit);
            }
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="MaterialRemovalRateUnit" /> to <see cref="MaterialRemovalRate" />.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>MaterialRemovalRate unit value.</returns>
        public static MaterialRemovalRate? From(double? value, MaterialRemovalRateUnit fromUnit)
        {
            if (!value.HasValue)
            {
                return null;
            }
            switch (fromUnit)
            {
                case MaterialRemovalRateUnit.CubicCentimeterPerMinute:
                    return FromCubicCentimetersPerMinute(value.Value);
                case MaterialRemovalRateUnit.CubicInchPerMinute:
                    return FromCubicInchesPerMinute(value.Value);
                case MaterialRemovalRateUnit.CubicMeterPerHour:
                    return FromCubicMetersPerHour(value.Value);
                case MaterialRemovalRateUnit.CubicMeterPerSecond:
                    return FromCubicMetersPerSecond(value.Value);

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
        public static string GetAbbreviation(MaterialRemovalRateUnit unit)
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
        public static string GetAbbreviation(MaterialRemovalRateUnit unit, [CanBeNull] Culture culture)
        {
            return UnitSystem.GetCached(culture).GetDefaultAbbreviation(unit);
        }

        #endregion

        #region Arithmetic Operators

#if !WINDOWS_UWP
        public static MaterialRemovalRate operator -(MaterialRemovalRate right)
        {
            return new MaterialRemovalRate(-right._cubicCentimetersPerMinute);
        }

        public static MaterialRemovalRate operator +(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return new MaterialRemovalRate(left._cubicCentimetersPerMinute + right._cubicCentimetersPerMinute);
        }

        public static MaterialRemovalRate operator -(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return new MaterialRemovalRate(left._cubicCentimetersPerMinute - right._cubicCentimetersPerMinute);
        }

        public static MaterialRemovalRate operator *(double left, MaterialRemovalRate right)
        {
            return new MaterialRemovalRate(left*right._cubicCentimetersPerMinute);
        }

        public static MaterialRemovalRate operator *(MaterialRemovalRate left, double right)
        {
            return new MaterialRemovalRate(left._cubicCentimetersPerMinute*(double)right);
        }

        public static MaterialRemovalRate operator /(MaterialRemovalRate left, double right)
        {
            return new MaterialRemovalRate(left._cubicCentimetersPerMinute/(double)right);
        }

        public static double operator /(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return Convert.ToDouble(left._cubicCentimetersPerMinute/right._cubicCentimetersPerMinute);
        }
#endif

        #endregion

        #region Equality / IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!(obj is MaterialRemovalRate)) throw new ArgumentException("Expected type MaterialRemovalRate.", "obj");
            return CompareTo((MaterialRemovalRate) obj);
        }

#if WINDOWS_UWP
        internal
#else
        public
#endif
        int CompareTo(MaterialRemovalRate other)
        {
            return _cubicCentimetersPerMinute.CompareTo(other._cubicCentimetersPerMinute);
        }

#if !WINDOWS_UWP
        public static bool operator <=(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return left._cubicCentimetersPerMinute <= right._cubicCentimetersPerMinute;
        }

        public static bool operator >=(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return left._cubicCentimetersPerMinute >= right._cubicCentimetersPerMinute;
        }

        public static bool operator <(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return left._cubicCentimetersPerMinute < right._cubicCentimetersPerMinute;
        }

        public static bool operator >(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            return left._cubicCentimetersPerMinute > right._cubicCentimetersPerMinute;
        }

        public static bool operator ==(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._cubicCentimetersPerMinute == right._cubicCentimetersPerMinute;
        }

        public static bool operator !=(MaterialRemovalRate left, MaterialRemovalRate right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._cubicCentimetersPerMinute != right._cubicCentimetersPerMinute;
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return _cubicCentimetersPerMinute.Equals(((MaterialRemovalRate) obj)._cubicCentimetersPerMinute);
        }

        public override int GetHashCode()
        {
            return _cubicCentimetersPerMinute.GetHashCode();
        }

        #endregion

        #region Conversion

        /// <summary>
        ///     Convert to the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>Value in new unit if successful, exception otherwise.</returns>
        /// <exception cref="NotImplementedException">If conversion was not successful.</exception>
        public double As(MaterialRemovalRateUnit unit)
        {
            switch (unit)
            {
                case MaterialRemovalRateUnit.CubicCentimeterPerMinute:
                    return CubicCentimetersPerMinute;
                case MaterialRemovalRateUnit.CubicInchPerMinute:
                    return CubicInchesPerMinute;
                case MaterialRemovalRateUnit.CubicMeterPerHour:
                    return CubicMetersPerHour;
                case MaterialRemovalRateUnit.CubicMeterPerSecond:
                    return CubicMetersPerSecond;

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
        public static MaterialRemovalRate Parse(string str)
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
        public static MaterialRemovalRate Parse(string str, [CanBeNull] Culture culture)
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
            return quantities.Aggregate((x, y) => MaterialRemovalRate.FromCubicCentimetersPerMinute(x.CubicCentimetersPerMinute + y.CubicCentimetersPerMinute));
        }

        /// <summary>
        ///     Parse a string given a particular regular expression.
        /// </summary>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        private static List<MaterialRemovalRate> ParseWithRegex(string regexString, string str, IFormatProvider formatProvider = null)
        {
            var regex = new Regex(regexString);
            MatchCollection matches = regex.Matches(str.Trim());
            var converted = new List<MaterialRemovalRate>();

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
                    MaterialRemovalRateUnit unit = ParseUnit(unitString, formatProvider);
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
        public static MaterialRemovalRateUnit ParseUnit(string str)
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
        public static MaterialRemovalRateUnit ParseUnit(string str, [CanBeNull] string cultureName)
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
        static MaterialRemovalRateUnit ParseUnit(string str, IFormatProvider formatProvider = null)
        {
            if (str == null) throw new ArgumentNullException("str");

            var unitSystem = UnitSystem.GetCached(formatProvider);
            var unit = unitSystem.Parse<MaterialRemovalRateUnit>(str.Trim());

            if (unit == MaterialRemovalRateUnit.Undefined)
            {
                var newEx = new UnitsNetException("Error parsing string. The unit is not a recognized MaterialRemovalRateUnit.");
                newEx.Data["input"] = str;
                newEx.Data["formatprovider"] = formatProvider?.ToString() ?? "(null)";
                throw newEx;
            }

            return unit;
        }

        #endregion

        /// <summary>
        ///     Set the default unit used by ToString(). Default is CubicCentimeterPerMinute
        /// </summary>
        public static MaterialRemovalRateUnit ToStringDefaultUnit { get; set; } = MaterialRemovalRateUnit.CubicCentimeterPerMinute;

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
        public string ToString(MaterialRemovalRateUnit unit)
        {
            return ToString(unit, null, 2);
        }

        /// <summary>
        ///     Get string representation of value and unit. Using two significant digits after radix.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <returns>String representation.</returns>
        public string ToString(MaterialRemovalRateUnit unit, [CanBeNull] Culture culture)
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
        public string ToString(MaterialRemovalRateUnit unit, [CanBeNull] Culture culture, int significantDigitsAfterRadix)
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
        public string ToString(MaterialRemovalRateUnit unit, [CanBeNull] Culture culture, [NotNull] string format,
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
        /// Represents the largest possible value of MaterialRemovalRate
        /// </summary>
        public static MaterialRemovalRate MaxValue
        {
            get
            {
                return new MaterialRemovalRate(double.MaxValue);
            }
        }

        /// <summary>
        /// Represents the smallest possible value of MaterialRemovalRate
        /// </summary>
        public static MaterialRemovalRate MinValue
        {
            get
            {
                return new MaterialRemovalRate(double.MinValue);
            }
        }
    }
}
