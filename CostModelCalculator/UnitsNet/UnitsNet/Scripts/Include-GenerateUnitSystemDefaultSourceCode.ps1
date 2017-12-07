function GenerateUnitSystemDefaultSourceCode($unitClasses)
{
@"
// Copyright � 2007 by Initial Force AS.  All rights reserved.
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitsNet.I18n;
using UnitsNet.Units;

// ReSharper disable RedundantCommaInArrayInitializer
// ReSharper disable once CheckNamespace

namespace UnitsNet
{
    public sealed partial class UnitSystem
    {
        private static readonly ReadOnlyCollection<UnitLocalization> DefaultLocalizations
            = new ReadOnlyCollection<UnitLocalization>(new List<UnitLocalization>
            {
"@;
    foreach ($unitClass in $unitClasses) 
    {
        $className = $unitClass.Name;
        $unitEnumName = "$className" + "Unit";
@"
                new UnitLocalization(typeof ($unitEnumName),
                    new[]
                    {
"@;
        foreach ($unit in $unitClass.Units) 
        {
            $enumValue = $unit.SingularName;
@"
                        new CulturesForEnumValue((int) $unitEnumName.$enumValue,
                            new[]
                            {
"@;
            foreach ($localization in $unit.Localization) 
            {
                $cultureName = $localization.Culture;
                $abbreviationParams = $localization.Abbreviations -join '", "'
@"
                                new AbbreviationsForCulture("$cultureName", "$abbreviationParams"),
"@;
            }
@"
                            }),
"@;
        }
@"
                    }),
"@;
    }
@"
             });
    }
}
"@;
}