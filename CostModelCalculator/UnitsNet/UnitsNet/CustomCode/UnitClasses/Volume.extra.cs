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
    public partial struct Volume
    {
        public static Area operator /(Volume volume, Length length)
        {
            return Area.FromSquareMeters(volume.CubicMeters/length.Meters);
        }

        public static Length operator /(Volume volume, Area area)
        {
            return Length.FromMeters(volume.CubicMeters/area.SquareMeters);
        }

        public static Volume FromTesselatedSolidBaseUnit(double volume, string baseUnit)
        {
            switch (baseUnit)
            {
                case "millimeter":
                    return FromCubicMillimeters(volume);
                case "centimeter":
                    return FromCubicCentimeters(volume);
                case "micron":
                    return FromCubicMicrometers(volume);
                case "inch":
                    return FromCubicInches(volume);
                case "foot":
                    return FromCubicFeet(volume);
                case "meter":
                    return FromCubicMeters(volume);
                default:
                    throw new Exception("Unit type not set OR needs to be added to above list of functions");
            }
        }
    }
}
#endif