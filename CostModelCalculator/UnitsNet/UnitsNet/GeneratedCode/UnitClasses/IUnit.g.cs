using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitsNet.Units;

namespace UnitsNet
{
    public enum KatanaUnitType
    {
        Geometric, 
        UserInput, 
        CalculatedCost, 
        TotalCost,
        CalculatedTime,
        //TotalTime,
        //ToDo: Eliminate all internal values if reasonable
        InternalValue,
        None
    }


    public interface IUnit
    {
        double Value { get; set;  }

        string BaseUnitAbbreviation { get; }

        string BaseUnitString { get; }

        //ANSI unit system operators
        double ANSIValue { get; set; }

        string ANSIUnitString { get; }

        string ANSIUnitAbbreviation { get;}

        //Returns the value in the given units
        double To(CostModelViewUnit unit);

        //Sets the IUnit from a given unit
        void From(double value, CostModelViewUnit unit);

    }

    public class Equation : Attribute
    {
        public string String;

        public Equation(string equation)
        {
            String = equation;
        }
    }

    public class Source : Attribute
    {
        public string String;

        public Source(string source)
        {
            String = source;
        }
    }

    public class Notes : Attribute
    {
        public string String;

        public Notes(string note)
        {
            String = note;
        }
    }

    public class IsInfeasiblity : Attribute
    {
        public bool IsInfeasibility;

        public IsInfeasiblity(bool isInfeasibility)
        {
            IsInfeasibility = isInfeasibility;
        }
    }

    public class OutputUnitType : Attribute
    {
        public KatanaUnitType UnitType;

        public OutputUnitType(KatanaUnitType unitType)
        {
            UnitType = unitType;
        }
    }


    public class CostModelViewUnit : Attribute
    {
        //Default to Undefined (= 0)
        public DurationUnit DurationUnit = 0;
        public AreaUnit AreaUnit = 0;
        public LengthUnit LengthUnit = 0;
        public VolumeUnit VolumeUnit = 0;
        public CostUnit CostUnit = 0;

        public CostModelViewUnit(DurationUnit unit)
        {
            DurationUnit = unit;
        }
        public CostModelViewUnit(AreaUnit unit)
        {
            AreaUnit = unit;
        }
        public CostModelViewUnit(LengthUnit unit)
        {
            LengthUnit = unit;
        }

        public CostModelViewUnit(VolumeUnit unit)
        {
            VolumeUnit = unit;
        }
        public CostModelViewUnit(CostUnit unit)
        {
            CostUnit = unit;
        }


        public override string ToString()
        {
            if (DurationUnit != 0) return DurationUnit.ToString();
            else if (AreaUnit != 0) return AreaUnit.ToString();
            else if (LengthUnit != 0) return LengthUnit.ToString();
            else if (VolumeUnit != 0) return VolumeUnit.ToString();
            else return "Undefined";
        }

        public string Abbreviation()
        {
            if (DurationUnit != 0) return Duration.GetAbbreviation(DurationUnit);
            else if (AreaUnit != 0) return Area.GetAbbreviation(AreaUnit);
            else if (LengthUnit != 0) return Length.GetAbbreviation(LengthUnit);
            else if (VolumeUnit != 0) return Volume.GetAbbreviation(VolumeUnit);
            else return "Undefined";
        }
    }
}
