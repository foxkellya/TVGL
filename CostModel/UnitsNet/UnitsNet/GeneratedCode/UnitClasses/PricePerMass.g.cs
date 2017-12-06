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
    ///     The price of materials by weight. Often used in cost models.
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
#if WINDOWS_UWP
    public sealed partial class PricePerMass
#else
    [DataContract]
    public partial struct PricePerMass : IUnit, IComparable, IComparable<PricePerMass>
#endif
    {
        /// <summary>
        ///     Base unit of PricePerMass.
        /// </summary>
        [DataMember]
        private double _dollarsPerKilogram;
        public double Value
        {
            get { return _dollarsPerKilogram; }
            set { _dollarsPerKilogram = value; }
        }

        public double ANSIValue
        {
            get { return DollarsPerPound; }
            set { _dollarsPerKilogram = FromDollarsPerPound(value).DollarsPerKilogram; }
        }

        public string ANSIUnitString => PricePerMassUnit.DollarPerPound.ToString();
        public string ANSIUnitAbbreviation => GetAbbreviation(PricePerMassUnit.DollarPerPound);
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
        public PricePerMass() : this(0)
        {
        }
#endif

        public PricePerMass(double dollarsperkilogram)
        {
            _dollarsPerKilogram = Convert.ToDouble(dollarsperkilogram);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        PricePerMass(long dollarsperkilogram)
        {
            _dollarsPerKilogram = Convert.ToDouble(dollarsperkilogram);
        }

        // Method overloads and with same number of parameters not supported in Universal Windows Platform (WinRT Components).
        // Decimal type not supported in Universal Windows Platform (WinRT Components).
#if WINDOWS_UWP
        private
#else
        public
#endif
        PricePerMass(decimal dollarsperkilogram)
        {
            _dollarsPerKilogram = Convert.ToDouble(dollarsperkilogram);
        }

        #region Properties

        public static PricePerMassUnit BaseUnit
        {
            get { return PricePerMassUnit.DollarPerKilogram; }
        }

        /// <summary>
        ///     Get PricePerMass in DollarsPerKilogram.
        /// </summary>
        public double DollarsPerKilogram
        {
            get { return _dollarsPerKilogram; }
        }

        /// <summary>
        ///     Get PricePerMass in DollarsPerPound.
        /// </summary>
        public double DollarsPerPound
        {
            get { return _dollarsPerKilogram*0.45359237; }
        }

        /// <summary>
        ///     Get PricePerMass in DollarsPerTonne.
        /// </summary>
        public double DollarsPerTonne
        {
            get { return _dollarsPerKilogram*1000; }
        }

        #endregion

        #region Static

        public static PricePerMass Zero
        {
            get { return new PricePerMass(); }
        }

        /// <summary>
        ///     Get PricePerMass from DollarsPerKilogram.
        /// </summary>
        public static PricePerMass FromDollarsPerKilogram(double dollarsperkilogram)
        {
            return new PricePerMass(dollarsperkilogram);
        }

        /// <summary>
        ///     Get PricePerMass from DollarsPerPound.
        /// </summary>
        public static PricePerMass FromDollarsPerPound(double dollarsperpound)
        {
            return new PricePerMass(dollarsperpound/0.45359237);
        }

        /// <summary>
        ///     Get PricePerMass from DollarsPerTonne.
        /// </summary>
        public static PricePerMass FromDollarsPerTonne(double dollarspertonne)
        {
            return new PricePerMass(dollarspertonne/1000);
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Get nullable PricePerMass from nullable DollarsPerKilogram.
        /// </summary>
        public static PricePerMass? FromDollarsPerKilogram(double? dollarsperkilogram)
        {
            if (dollarsperkilogram.HasValue)
            {
                return FromDollarsPerKilogram(dollarsperkilogram.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable PricePerMass from nullable DollarsPerPound.
        /// </summary>
        public static PricePerMass? FromDollarsPerPound(double? dollarsperpound)
        {
            if (dollarsperpound.HasValue)
            {
                return FromDollarsPerPound(dollarsperpound.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Get nullable PricePerMass from nullable DollarsPerTonne.
        /// </summary>
        public static PricePerMass? FromDollarsPerTonne(double? dollarspertonne)
        {
            if (dollarspertonne.HasValue)
            {
                return FromDollarsPerTonne(dollarspertonne.Value);
            }
            else
            {
                return null;
            }
        }

#endif

        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="PricePerMassUnit" /> to <see cref="PricePerMass" />.
        /// </summary>
        /// <param name="val">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>PricePerMass unit value.</returns>
        public static PricePerMass From(double val, PricePerMassUnit fromUnit)
        {
            switch (fromUnit)
            {
                case PricePerMassUnit.DollarPerKilogram:
                    return FromDollarsPerKilogram(val);
                case PricePerMassUnit.DollarPerPound:
                    return FromDollarsPerPound(val);
                case PricePerMassUnit.DollarPerTonne:
                    return FromDollarsPerTonne(val);

                default:
                    throw new NotImplementedException("fromUnit: " + fromUnit);
            }
        }

#if !WINDOWS_UWP
        /// <summary>
        ///     Dynamically convert from value and unit enum <see cref="PricePerMassUnit" /> to <see cref="PricePerMass" />.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <param name="fromUnit">Unit to convert from.</param>
        /// <returns>PricePerMass unit value.</returns>
        public static PricePerMass? From(double? value, PricePerMassUnit fromUnit)
        {
            if (!value.HasValue)
            {
                return null;
            }
            switch (fromUnit)
            {
                case PricePerMassUnit.DollarPerKilogram:
                    return FromDollarsPerKilogram(value.Value);
                case PricePerMassUnit.DollarPerPound:
                    return FromDollarsPerPound(value.Value);
                case PricePerMassUnit.DollarPerTonne:
                    return FromDollarsPerTonne(value.Value);

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
        public static string GetAbbreviation(PricePerMassUnit unit)
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
        public static string GetAbbreviation(PricePerMassUnit unit, [CanBeNull] Culture culture)
        {
            return UnitSystem.GetCached(culture).GetDefaultAbbreviation(unit);
        }

        #endregion

        #region Arithmetic Operators

#if !WINDOWS_UWP
        public static PricePerMass operator -(PricePerMass right)
        {
            return new PricePerMass(-right._dollarsPerKilogram);
        }

        public static PricePerMass operator +(PricePerMass left, PricePerMass right)
        {
            return new PricePerMass(left._dollarsPerKilogram + right._dollarsPerKilogram);
        }

        public static PricePerMass operator -(PricePerMass left, PricePerMass right)
        {
            return new PricePerMass(left._dollarsPerKilogram - right._dollarsPerKilogram);
        }

        public static PricePerMass operator *(double left, PricePerMass right)
        {
            return new PricePerMass(left*right._dollarsPerKilogram);
        }

        public static PricePerMass operator *(PricePerMass left, double right)
        {
            return new PricePerMass(left._dollarsPerKilogram*(double)right);
        }

        public static PricePerMass operator /(PricePerMass left, double right)
        {
            return new PricePerMass(left._dollarsPerKilogram/(double)right);
        }

        public static double operator /(PricePerMass left, PricePerMass right)
        {
            return Convert.ToDouble(left._dollarsPerKilogram/right._dollarsPerKilogram);
        }
#endif

        #endregion

        #region Equality / IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!(obj is PricePerMass)) throw new ArgumentException("Expected type PricePerMass.", "obj");
            return CompareTo((PricePerMass) obj);
        }

#if WINDOWS_UWP
        internal
#else
        public
#endif
        int CompareTo(PricePerMass other)
        {
            return _dollarsPerKilogram.CompareTo(other._dollarsPerKilogram);
        }

#if !WINDOWS_UWP
        public static bool operator <=(PricePerMass left, PricePerMass right)
        {
            return left._dollarsPerKilogram <= right._dollarsPerKilogram;
        }

        public static bool operator >=(PricePerMass left, PricePerMass right)
        {
            return left._dollarsPerKilogram >= right._dollarsPerKilogram;
        }

        public static bool operator <(PricePerMass left, PricePerMass right)
        {
            return left._dollarsPerKilogram < right._dollarsPerKilogram;
        }

        public static bool operator >(PricePerMass left, PricePerMass right)
        {
            return left._dollarsPerKilogram > right._dollarsPerKilogram;
        }

        public static bool operator ==(PricePerMass left, PricePerMass right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._dollarsPerKilogram == right._dollarsPerKilogram;
        }

        public static bool operator !=(PricePerMass left, PricePerMass right)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return left._dollarsPerKilogram != right._dollarsPerKilogram;
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return _dollarsPerKilogram.Equals(((PricePerMass) obj)._dollarsPerKilogram);
        }

        public override int GetHashCode()
        {
            return _dollarsPerKilogram.GetHashCode();
        }

        #endregion

        #region Conversion

        /// <summary>
        ///     Convert to the unit representation <paramref name="unit" />.
        /// </summary>
        /// <returns>Value in new unit if successful, exception otherwise.</returns>
        /// <exception cref="NotImplementedException">If conversion was not successful.</exception>
        public double As(PricePerMassUnit unit)
        {
            switch (unit)
            {
                case PricePerMassUnit.DollarPerKilogram:
                    return DollarsPerKilogram;
                case PricePerMassUnit.DollarPerPound:
                    return DollarsPerPound;
                case PricePerMassUnit.DollarPerTonne:
                    return DollarsPerTonne;

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
        public static PricePerMass Parse(string str)
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
        public static PricePerMass Parse(string str, [CanBeNull] Culture culture)
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
            return quantities.Aggregate((x, y) => PricePerMass.FromDollarsPerKilogram(x.DollarsPerKilogram + y.DollarsPerKilogram));
        }

        /// <summary>
        ///     Parse a string given a particular regular expression.
        /// </summary>
        /// <exception cref="UnitsNetException">Error parsing string.</exception>
        private static List<PricePerMass> ParseWithRegex(string regexString, string str, IFormatProvider formatProvider = null)
        {
            var regex = new Regex(regexString);
            MatchCollection matches = regex.Matches(str.Trim());
            var converted = new List<PricePerMass>();

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
                    PricePerMassUnit unit = ParseUnit(unitString, formatProvider);
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
        public static PricePerMassUnit ParseUnit(string str)
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
        public static PricePerMassUnit ParseUnit(string str, [CanBeNull] string cultureName)
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
        static PricePerMassUnit ParseUnit(string str, IFormatProvider formatProvider = null)
        {
            if (str == null) throw new ArgumentNullException("str");

            var unitSystem = UnitSystem.GetCached(formatProvider);
            var unit = unitSystem.Parse<PricePerMassUnit>(str.Trim());

            if (unit == PricePerMassUnit.Undefined)
            {
                var newEx = new UnitsNetException("Error parsing string. The unit is not a recognized PricePerMassUnit.");
                newEx.Data["input"] = str;
                newEx.Data["formatprovider"] = formatProvider?.ToString() ?? "(null)";
                throw newEx;
            }

            return unit;
        }

        #endregion

        /// <summary>
        ///     Set the default unit used by ToString(). Default is DollarPerKilogram
        /// </summary>
        public static PricePerMassUnit ToStringDefaultUnit { get; set; } = PricePerMassUnit.DollarPerKilogram;

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
        public string ToString(PricePerMassUnit unit)
        {
            return ToString(unit, null, 2);
        }

        /// <summary>
        ///     Get string representation of value and unit. Using two significant digits after radix.
        /// </summary>
        /// <param name="unit">Unit representation to use.</param>
        /// <param name="culture">Culture to use for localization and number formatting.</param>
        /// <returns>String representation.</returns>
        public string ToString(PricePerMassUnit unit, [CanBeNull] Culture culture)
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
        public string ToString(PricePerMassUnit unit, [CanBeNull] Culture culture, int significantDigitsAfterRadix)
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
        public string ToString(PricePerMassUnit unit, [CanBeNull] Culture culture, [NotNull] string format,
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
        /// Represents the largest possible value of PricePerMass
        /// </summary>
        public static PricePerMass MaxValue
        {
            get
            {
                return new PricePerMass(double.MaxValue);
            }
        }

        /// <summary>
        /// Represents the smallest possible value of PricePerMass
        /// </summary>
        public static PricePerMass MinValue
        {
            get
            {
                return new PricePerMass(double.MinValue);
            }
        }
    }
}
