﻿// Copyright(c) 2007 Andreas Gullberg Larsen
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

namespace UnitsNet.Tests.CustomCode
{
    public class EnergyTests : EnergyTestsBase
    {
        // TODO Override properties in base class here
        protected override double JoulesInOneJoule => 1;

        protected override double KilojoulesInOneJoule => 1E-3;

        protected override double MegajoulesInOneJoule => 1E-6;

        protected override double BritishThermalUnitsInOneJoule => 0.00094781712;

        protected override double CaloriesInOneJoule => 0.239005736;

        protected override double ElectronVoltsInOneJoule => 6.241509343260179e18;

        protected override double ErgsInOneJoule => 10000000;

        protected override double FootPoundsInOneJoule => 0.737562149;

        protected override double GigawattHoursInOneJoule => 2.77777778e-13;

        protected override double KilocaloriesInOneJoule => 0.00023900573614;

        protected override double KilowattHoursInOneJoule => 2.77777778e-7;

        protected override double MegawattHoursInOneJoule => 2.77777778e-10;

        protected override double WattHoursInOneJoule => 0.000277777778;
    }
}