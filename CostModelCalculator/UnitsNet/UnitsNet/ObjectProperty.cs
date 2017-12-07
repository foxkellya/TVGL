using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnitsNet
{
    public class CostProperty : PropertyClass
    {
        public Cost Cost => (Cost)UnitValue;

        public CostProperty(string name, Cost value, string equation = "", string nameModifier = null)
            : base(name, value, equation, nameModifier)
        {
        }

        public double Dollars => Cost.Dollars;
    }

    public class VolumeProperty: PropertyClass
    {
        public Volume Volume => (Volume) UnitValue;

        public VolumeProperty(string name, Volume value, string equation = "", string nameModifier = null)
                  : base(name, value, equation, nameModifier)
        {
        }

        public double CubicMeters => Volume.CubicMeters;
        public double CubicCentimeters => Volume.CubicCentimeters;
        public double CubicMillimeters => Volume.CubicMillimeters;
    }

    public class MassProperty : PropertyClass
    {
        public Mass Mass => (Mass) UnitValue;

        public MassProperty(string name, Mass value, string equation = "", string nameModifier = null)
            : base(name, value, equation, nameModifier)
        {
            
        }
    }

    public class DurationProperty : PropertyClass
    {
        public Duration Duration => (Duration)UnitValue;

        public DurationProperty(string name, Duration value, string equation = "", string nameModifier = null)
            : base(name, value, equation, nameModifier)
        {
        }
    }

    public class DensityProperty : PropertyClass
    {
        public Density Density => (Density) UnitValue;
        
        //The input value must be of type "Density", since this value will be cast to density
        public DensityProperty(string name, Density value, string equation = "", string nameModifier = null) : base (name, value, equation, nameModifier)
        {
        }

        //Most common unit call to density
        public double KilogramsPerCubicMillimeter => Density.KilogramsPerCubicMillimeter;

        //Common unit call to density
        public double KilogramsPerCubicMeter => Density.KilogramsPerCubicMeter;
    }

    public abstract class PropertyClass
    {
        public string Name;
        public string Source;
        public string Notes;
        public string ExcelName;
        public string Equation;
        public string ExcelEquation;
        public KatanaUnitType KatanaUnitType;
        public IUnit UnitValue;
        public CostModelViewUnit CostModelViewUnit = null;

        public bool IsFeasibility { get; set; } = false; //Default is false. Not an infeasibility

        protected PropertyClass(string name, IUnit value, string equation = "", string nameModifier = null)
        {
            Name = name;
            Equation = equation;
            UnitValue = value;

            try
            {
                ExcelName = GetExcelName(nameModifier);
                ExcelEquation = GetExcelEquation(nameModifier);
            }
            catch
            {
                throw new Exception("Excel Name or Equation failed.");
            }
        }

        public string GetExcelEquation(string nameModifier)
        {
            if (Equation == "") return "";
            //Not Null, create the equation

            var chars = Equation.ToCharArray();
            var equationCharacters = new List<char>() {'+', '-', '=', '*', '/', '(', ')', '^'};
            //Find each variable in the equation
            var currentVariable = "";
            var equation = "= ";
            for (var i = 0; i < chars.Length; i++)
            {
                var character = chars[i];

                //If the last character, add the variable.
                if (i == chars.Length - 1)
                {
                    //Add to the current variable string
                    currentVariable = currentVariable + character;

                    //Trim, add underscores, and add name modifiers (if applicable) to the variable
                    var temp = currentVariable.Trim();
                    var variableName = Abbreviate(temp);
                    variableName = variableName.Replace(' ', '_');

                    //Add the variable to the equation
                    equation = equation + variableName;
                }

                //Check if it is an equation character
                if (equationCharacters.Any(eqCharacter => character == eqCharacter))
                {
                    //We have reached the end of the current variable,
                    var variableName = currentVariable.Trim();

                    //If the variable name is not null, 
                    if (variableName != "")
                    {
                        //Check if the variable is a number
                        var firstCharacter = variableName.ToCharArray().First();
                        if (!char.IsNumber(firstCharacter))
                        {
                            //If it starts with a number, then assume it is a number
                            //because ExcelVariables cannot start with numbers

                            //If not a number, get the correct Excel Name
                            //Trim, add underscores, and add name modifiers (if applicable) to the variable
                            variableName = Abbreviate(variableName);
                            variableName = variableName.Replace(' ', '_');
                        }
                    }
                  
                    //Else the variable name is empty or it is complete.
                    //Either way, we can add it and the character to the quation.
                    //Add the variable to the equation, and add the equation character, adding spaces before and after it.
                    equation = equation + variableName + " " + character + " ";

                    //Reset the current variable
                    currentVariable = "";
                }
                else
                {
                    //Add to the current variable string
                    currentVariable = currentVariable + character;
                }
            }
            return equation;
        }

        public string GetExcelName(string nameModifier)
        {
            if (Name == "") { return "";}
            //Not Null, add underscores wherever there are spaces


            //There are many restrictions on what is allowed in a name
            //Learn about syntax rules for names: https://support.office.com/en-us/article/Define-and-use-names-in-formulas-4d0f13ac-53b7-422e-afd2-abd7ff379c64
            //1) Remove unnecessary spaces before and after
            var temp = Name.Trim();
            //2) Replace spaces with underscores and abbreviate pre-defined phrases
            var name = Abbreviate(temp);
            //Add the name modifier
            if (!string.IsNullOrEmpty(nameModifier)) name = name + "_" + nameModifier;
            //3) Check if the first character of the name is valid
            var chars = name.ToCharArray();
            if (!char.IsLetter(chars[0]) && chars[0] != '_')
                throw new Exception("The first character of this variable must be a letter or and underscore");
            //4) Check if the rest of the characters in the variable are valid

            var validName = "";
            var isLeftBracket = false;
            foreach (var character in chars)
            {
                //Ignore the characters inside the bracket, we will remove them from the Excel Name
                if (character == '(' || character == '[' || character == '{')
                {
                    isLeftBracket = true;
                }
                //Search for the right bracket
                if (isLeftBracket && character == ')' || character == '}' || character == ']')
                {
                    isLeftBracket = false;
                    continue;
                }

                //If not inside a bracket, then check if the character is valid
                if (isLeftBracket) continue;
                if (!char.IsLetter(character) && !char.IsNumber(character) && !IsAcceptableCharacter(character))
                {
                    throw new Exception("The following character in this variable is invalid: " + character);
                }

                //It is valid, so add it to the valid name
                validName = validName + character;
            }
            //Trim after possibly removing a portion of the string
            validName = validName.Trim();
            //Replace spaces with underscores
            validName = validName.Replace(' ', '_');
            return validName;
        }

        public static bool IsAcceptableCharacter(char c)
        {
            //It is ok for c to == space for now
            return (c == '.' || c == '_' || c == ' ');
        }

        public static string Abbreviate(string name)
        {
            var index = name.IndexOf("Material Removal Rate", StringComparison.Ordinal);
            if (index != -1)
            {
                name = name.Replace("Material Removal Rate", "MRR");
            }

            index = name.IndexOf("Non-Destructive Testing", StringComparison.Ordinal);
            if (index != -1)
            {
                name = name.Replace("Non-Destructive Testing", "NDT");
            }

            return name;
        }
    }
}
