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
    ///     The cost of something per unit a unit of time.
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
#if WINDOWS_UWP
    public sealed partial class CostRate
#else
    [DataContract]
    public partial struct CostRate : IUnit, IComparable, IComparable<CostRate>
#endif
    {
        /// <summary>
        ///     Base unit of CostRate.
        /// </summary>
        [DataMember]
        private double _dollarsPerSecond;

#if WINDOWS_UWP
        public CostRate() : this(0)
        {
        }
#endif

        public double Value
        {
            get { return _dollarsPerSecond; }
            set { _dollarsPerSecond = value; }
        }

        public double ANSIValue
        {
            get { return DollarsPerHour; }
            set { _dollarsPerSecond = FromDollarsPerHour(value).DollarsPerSecond; }
        }

        public string ANSIUnitString => CostRateUnit.DollarPerHour.ToString();
        public string ANSIUnitAbbreviation => GetAbbreviation(CostRateUnit.DollarPerHour);
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

        public CostRate(double dollarspersecond)
        {
            _dollarsPerSecond = Convert.ToDouble(dollarspersecond);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        CostRate(long dollarspersecond)
        {
            _dollarsPerSecond = Convert.ToDouble(dollarspersecond);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
        // Decimal type not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        CostRate(decimal dollarspersecond)
        {
            _dollarsPerSecond = Convert.ToDouble(dollarspersecond);
        }

        #region Properties

        public static CostRateUnit BaseUnit
        {
            get { return CostRateUnit.DollarPerSecond; }
        }

        /// <summary>
        ///     Get CostRate in DollarsPerDay.
        /// </summary>
        public double DollarsPerDay
        {
            get { return _dollarsPerSecond/(3600*24); }
        }

        /// <summary>
        ///     Get CostRate in DollarsPerHour.
        /// </summary>
        public double DollarsPerHour
        {
            get { return _dollarsPerSecond/3600; }
        }

        /// <summary>
        ///     Get CostRate in DollarsPerMinute.
        /// </summary>
        public double DollarsPerMinute
        {
            get { return _dollarsPerSecond/60; }
        }

        /// <summary>
        ///     Get CostRate in DollarsPerSecond.
        /// </summary>
        public double DollarsPerSecond
        {
            get { return _dollarsPerSecond; }
        }

        /// <summary>
        ///     Get CostRate in DollarsPerYear.
        /// </summary>
        public double DollarsPerYear
        {
            get { return _dollarsPerSecond/(3600*24*365); }
        }

        #endregion

        #region Static

        public static CostRate Zero
        {
            get { return new CostRate(); }
        }

        /// <summary>
        ///     Get CostRate from DollarsPerDay.
        /// </summary>
        public static CostRate FromDollarsPerDay(double dollarsperday)
        {
            return new CostRate(dollarsperday*3600*24);
        }

        /// <summary>
        ///     Get CostRate from DollarsPerHour.
        /// </summary>
        public static CostRate FromDollarsPerHour(double dollarsperhour)
        {
            return new CostRate(dollarsperhour*3600);
        }

        /// <summary>
        ///     Get CostRate from DollarsPerMinute.
        /// </summary>
        public static CostRate FromDollarsPerMinute(double dollarsperminute)
        {
            return new CostRate(dollarsperminute*60);
        }

        /// <summary>
        ///     Get CostRate from DollarsPerSecond.
        /// </summary>
        public static CostRate FromDollarsPerSecond(double dollarspersecond)
        {
            return new CostRate(dollarspersecond);
        }

        /// <summary>
        ///     Get CostRate from DollarsPerYear.
        /// </summary>
        public static CostRate FromDollarsPerYear(double dollarsperyear)
        {
            return new CostRate(dollarsperyear*3600*24*365);
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Get nullable CostRate from nullable DollarsPerDay.
        /// </summary>
        public static CostRate? FromDollarsPerDay(double? dollarsperday)
        {
            if (dollarsperday.HasValue)
            {
                return FromDollarsPerDay(dollarsperday.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostRate from nullable DollarsPerHour.
        /// </summary>
        public static CostRate? FromDollarsPerHour(double? dollarsperhour)
        {
            if (dollarsperhour.HasValue)
            {
                return FromDollarsPerHour(dollarsperhour.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostRate from nullable DollarsPerMinute.
        /// </summary>
        public static CostRate? FromDollarsPerMinute(double? dollarsperminute)
        {
            if (dollarsperminute.HasValue)
            {
                return FromDollarsPerMinute(dollarsperminute.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostRate from nullable DollarsPerSecond.
        /// </summary>
        public static CostRate? FromDollarsPerSecond(double? dollarspersecond)
        {
            if (dollarspersecond.HasValue)
            {
                return FromDollarsPerSecond(dollarspersecond.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable CostRate from nullable DollarsPerYear.
        /// </summary>
        public static CostRate? FromDollarsPerYear(double? dollarsperyear)
        {
            if (dollarsperyear.HasValue)
            {
                return FromDollarsPerYear(dollarsperyear.Value);
            }
            else
            {
                return null;
            }
        }

#endif

        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="CostRateUnit" /> to <see cref="CostRate" />.
        /// </summary>
        /// <param name="val">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>CostRate unit value.</returns>
        public static CostRate From(double val, CostRateUnit fromUnit)
        {
            switch (fromUnit)
            {
                case CostRateUnit.DollarPerDay:
                    return FromDollarsPerDay(val);
                case CostRateUnit.DollarPerHour:
                    return FromDollarsPerHour(val);
                case CostRateUnit.DollarPerMinute:
                    return FromDollarsPerMinute(val);
                case CostRateUnit.DollarPerSecond:
                    return FromDollarsPerSecond(val);
                case CostRateUnit.DollarPerYear:
                    return FromDollarsPerYear(val);

                default:
                    throw new NotImplementedException("fromUnit: " + fromUnit);
            }
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="CostRateUnit" /> to <see cref="CostRate" />.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>CostRate unit value.</returns>
        public static CostRate? From(double? value, CostRateUnit fromUnit)
        {
            if (!value.HasValue)
            {
                return null;
            }
            switch (fromUnit)
            {
                case CostRateUnit.DollarPerDay:
                    return FromDollarsPerDay(value.Value);
                case CostRateUnit.DollarPerHour:
                    return FromDollarsPerHour(value.Value);
                case CostRateUnit.DollarPerMinute:
                    return FromDollarsPerMinute(value.Value);
                case CostRateUnit.DollarPerSecond:
                    return FromDollarsPerSecond(value.Value);
                case CostRateUnit.DollarPerYear:
                    return FromDollarsPerYear(value.Value);

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
        public static string GetAbbreviation(CostRateUnit unit)
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
        public static string GetAbbreviation(CostRateUnit unit, [CanBeNull] Culture culture)
        {
            return UnitSystem.GetCached(culture).GetDefaultAbbreviation(unit);
        }

        #endregion

        #region Arithmetic Operators

#if !WINDOWS_UWP
        public static CostRate operator -(CostRate right)
        {
            return new CostRate(-right._dollarsPerSecond);
        }

        public static CostRate operator +(CostRate left, CostRate right)
        {
            return new CostRate(left._dollarsPerSecond + right._dollarsPerSecond);
        }

        public static CostRate operator -(CostRate left, CostRate right)
        {
            return new CostRate(left._dollarsPerSecond - right._dollarsPerSecond);
        }

        public static CostRate operator *(double left, CostRate right)
        {
            return new CostRate(left*right._dollarsPerSecond);
        }

        public static CostRate operator *(CostRate left, double right)
        {
            return new CostRate(left._dollarsPerSecond*(double)right);
        }

        public static CostRate operator /(CostRate left, double right)
        {
            return new CostRate(left._dollarsPerSecond/(double)right);
        }

        public static double operator /(CostRate left, CostRate right)
        {
            return Convert.ToDouble(left._dollarsPerSecond/right._dollarsPerSecond);
        }
#endif

        #endregion

        #region Equality / IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!(obj is CostRate)) throw new ArgumentException("Expected type CostRate.", "obj");
            return CompareTo((CostRate) obj);
        }

#if WINDOWS_UWP
        internal
#else
        public
#endif
        int CompareTo(CostRate other)
        {
            return _dollarsPerSecond.CompareTo(other._dollarsPerSecond);
        }

#if !WINDOWS_UWP
        public static bool operator <=(CostRate left, CostRate right)
        {
            return left._dollarsPerSecond <= right._dollarsPerSecond;
        }

        public static bool operator >=(CostRate left, CostRate right)
        {
            return left._dollarsPerSecond >= right._dollarsPerSecond;
        }

        public static bool operator <(CostRate left, CostRate right)
        {
            return left._dollarsPerSecond < right._dollarsPerSecond;
        }

        public static bool operator >(CostRate left, CostRate right)
        {
            return left._dollarsPerSecond > right._dollarsPerSecond;
        }

        public static bool operator ==(CostRate left, CostRate right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._dollarsPerSecond == right._dollarsPerSecond;
        }

        public static bool operator !=(CostRate left, CostRate right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._dollarsPerSecond != right._dollarsPerSecond;
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return _dollarsPerSecond.Equals(((CostRate) obj)._dollarsPerSecond);
        }

        public override int GetHashCode()
        {
            return _dollarsPerSecond.GetHashCode();
        }

        #endregion

        #region Conversion

        /// <summary>
        ///     Convert to the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>Value in new unit if successful, exception otherwise.</returns>
        /// <exception cref="NotImplementedException">If conversion was not successful.</exception>
        public double As(CostRateUnit unit)
        {
            switch (unit)
            {
                case CostRateUnit.DollarPerDay:
                    return DollarsPerDay;
                case CostRateUnit.DollarPerHour:
                    return DollarsPerHour;
                case CostRateUnit.DollarPerMinute:
                    return DollarsPerMinute;
                case CostRateUnit.DollarPerSecond:
                    return DollarsPerSecond;
                case CostRateUnit.DollarPerYear:
                    return DollarsPerYear;

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
        public static CostRate Parse(string str)
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
        public static CostRate Parse(string str, [CanBeNull] Culture culture)
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
            return quantities.Aggregate((x, y) => CostRate.FromDollarsPerSecond(x.DollarsPerSecond + y.DollarsPerSecond));
        }

        /// <summary>
        ///     Parse a string given a particular regular expression.
        /// </summary>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        private static List<CostRate> ParseWithRegex(string regexString, string str, IFormatProvider formatProvider = null)
        {
            var regex = new Regex(regexString);
            MatchCollection matches = regex.Matches(str.Trim());
            var converted = new List<CostRate>();

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
                    CostRateUnit unit = ParseUnit(unitString, formatProvider);
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
        public static CostRateUnit ParseUnit(string str)
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
        public static CostRateUnit ParseUnit(string str, [CanBeNull] string cultureName)
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
        static CostRateUnit ParseUnit(string str, IFormatProvider formatProvider = null)
        {
            if (str == null) throw new ArgumentNullException("str");

            var unitSystem = UnitSystem.GetCached(formatProvider);
            var unit = unitSystem.Parse<CostRateUnit>(str.Trim());

            if (unit == CostRateUnit.Undefined)
            {
                var newEx = new UnitsNetException("Error parsing string. The unit is not a recognized CostRateUnit.");
                newEx.Data["input"] = str;
                newEx.Data["formatprovider"] = formatProvider?.ToString() ?? "(null)";
                throw newEx;
            }

            return unit;
        }

        #endregion

        /// <summary>
        ///     Set the default unit used by ToString(). Default is DollarPerSecond
        /// </summary>
        public static CostRateUnit ToStringDefaultUnit { get; set; } = CostRateUnit.DollarPerSecond;

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
        public string ToString(CostRateUnit unit)
        {
            return ToString(unit, null, 2);
        }

        /// <summary>
        ///     Get string representation of value and unit. Using two significant digits after radix.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <returns>String representation.</returns>
        public string ToString(CostRateUnit unit, [CanBeNull] Culture culture)
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
        public string ToString(CostRateUnit unit, [CanBeNull] Culture culture, int significantDigitsAfterRadix)
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
        public string ToString(CostRateUnit unit, [CanBeNull] Culture culture, [NotNull] string format,
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
        /// Represents the largest possible value of CostRate
        /// </summary>
        public static CostRate MaxValue
        {
            get
            {
                return new CostRate(double.MaxValue);
            }
        }

        /// <summary>
        /// Represents the smallest possible value of CostRate
        /// </summary>
        public static CostRate MinValue
        {
            get
            {
                return new CostRate(double.MinValue);
            }
        }
    }
}
