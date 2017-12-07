// Copyright(c) 2007 Andreas Gullberg Larsen
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

// Operator overloads not supported in Universal Windows Platform (WinRT Components)

using System;

#if !WINDOWS_UWP
namespace UnitsNet
{
    public partial struct Area
    {
        public static Length operator /(Area area, Length length)
        {
            return Length.FromMeters(area.SquareMeters/length.Meters);
        }

        public static Area FromTesselatedSolidBaseUnit(double area, string baseUnit)
        {
            switch (baseUnit)
            {
                case "millimeter":
                    return FromSquareMillimeters(area);
                case "centimeter":
                    return FromSquareCentimeters(area);
                case "micron":
                    return FromSquareMicrometers(area);
                case "inch":
                    return FromSquareInches(area);
                case "foot":
                    return FromSquareFeet(area);
                case "meter":
                    return FromSquareMeters(area);
                default:
                    throw new Exception("Unit type not set OR needs to be added to above list of functions");
            }
        }

        public double TesselatedSolidBaseUnit(string baseUnit)
        {
            switch (baseUnit)
            {
                case "millimeter":
                    return SquareMillimeters;
                case "centimeter":
                    return SquareCentimeters;
                case "micron":
                    return SquareMicrometers;
                case "inch":
                    return SquareInches;
                case "foot":
                    return SquareFeet;
                case "meter":
                    return SquareMeters;
                default:
                    throw new Exception("Unit type not set OR needs to be added to above list of functions");
            }
        }
    }
}
#endif