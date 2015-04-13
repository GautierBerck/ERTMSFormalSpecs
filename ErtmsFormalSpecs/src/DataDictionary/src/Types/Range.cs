// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using Utils;
using EnumValue = DataDictionary.Constants.EnumValue;

namespace DataDictionary.Types
{
    public class Range : Generated.Range, IEnumerateValues, ISubDeclarator, TextualExplain
    {
        /// <summary>
        ///     The min value of the range
        /// </summary>
        public string MinValue
        {
            get { return getMinValue(); }
            set
            {
                setMinValue(value);
                minValueSet = false;
            }
        }

        /// <summary>
        ///     A cache for the min value
        /// </summary>
        private bool minValueSet;

        private Decimal minValueAsLong;
        private double minValueAsDouble;

        public Decimal MinValueAsLong
        {
            get
            {
                if (!minValueSet)
                {
                    minValueAsLong = Decimal.Parse(MinValue);
                    minValueSet = true;
                }
                return minValueAsLong;
            }
        }

        public double MinValueAsDouble
        {
            get
            {
                if (!minValueSet)
                {
                    minValueAsDouble = getDouble(MinValue);
                    minValueSet = true;
                }
                return minValueAsDouble;
            }
        }

        /// <summary>
        ///     The max value of the range
        /// </summary>
        public string MaxValue
        {
            get { return getMaxValue(); }
            set { setMaxValue(value); }
        }

        /// <summary>
        ///     A cache for the min value
        /// </summary>
        private bool maxValueSet = false;

        private Decimal maxValueAsLong;
        private double maxValueAsDouble;

        public Decimal MaxValueAsLong
        {
            get
            {
                if (!maxValueSet)
                {
                    maxValueAsLong = Decimal.Parse(MaxValue);
                    maxValueSet = true;
                }
                return maxValueAsLong;
            }
        }

        public double MaxValueAsDouble
        {
            get
            {
                if (!maxValueSet)
                {
                    maxValueAsDouble = getDouble(MaxValue);
                    maxValueSet = true;
                }
                return maxValueAsDouble;
            }
        }

        /// <summary>
        ///     The special values of the range
        /// </summary>
        public ArrayList SpecialValues
        {
            get
            {
                if (allSpecialValues() == null)
                {
                    setAllSpecialValues(new ArrayList());
                }
                return allSpecialValues();
            }
            set { setAllSpecialValues(value); }
        }

        /// <summary>
        ///     Just a cache
        /// </summary>
        private static Dictionary<string, double> cache = new Dictionary<string, double>();

        /// <summary>
        ///     Faster method of getting a double from its string representation
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static double getDouble(string image)
        {
            double retVal;

            if (!cache.TryGetValue(image, out retVal))
            {
                retVal = double.Parse(image, CultureInfo.InvariantCulture);
                cache.Add(image, retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Parses the image and provides the corresponding value
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            if (Char.IsLetter(image[0]) || image[0] == '_')
            {
                retVal = findEnumValue(image);
                if (retVal == null)
                {
                    Log.Error("Cannot create range value from " + image);
                }
            }
            else
            {
                try
                {
                    switch (getPrecision())
                    {
                        case acceptor.PrecisionEnum.aIntegerPrecision:
                        {
                            Decimal val = Decimal.Parse(image);
                            Decimal min = MinValueAsLong;
                            Decimal max = MaxValueAsLong;
                            if (val >= min && val <= max)
                            {
                                retVal = new IntValue(this, val);
                            }
                        }
                            break;

                        case acceptor.PrecisionEnum.aDoublePrecision:
                        {
                            CultureInfo info = CultureInfo.InvariantCulture;

                            double val = getDouble(image);
                            double min = MinValueAsDouble;
                            double max = MaxValueAsDouble;
                            if (val >= min && val <= max && image.IndexOf('.') >= 0)
                            {
                                retVal = new DoubleValue(this, val);
                            }
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Cannot create range value", exception);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates whether a value can be cast into this type
        /// </summary>
        public override bool CanBeCastInto
        {
            get { return true; }
        }

        /// <summary>
        ///     Converts a value in this type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public override IValue convert(IValue value)
        {
            IValue retVal = null;

            EnumValue enumValue = value as EnumValue;
            if (enumValue != null && enumValue.Range != null)
            {
                retVal = findEnumValue(enumValue.Name);
                if (retVal == null)
                {
                    Log.Error("Cannot convert " + enumValue.Name + " to " + FullName);
                }
            }
            else
            {
                try
                {
                    switch (getPrecision())
                    {
                        case acceptor.PrecisionEnum.aIntegerPrecision:
                        {
                            Decimal val = getValueAsInt(value);
                            Decimal min = MinValueAsLong;
                            Decimal max = MaxValueAsLong;
                            if (val >= min && val <= max)
                            {
                                retVal = new IntValue(this, val);
                            }
                        }
                            break;
                        case acceptor.PrecisionEnum.aDoublePrecision:
                        {
                            double val = getValueAsDouble(value);
                            double min = MinValueAsDouble;
                            double max = MaxValueAsDouble;
                            if (val >= min && val <= max)
                            {
                                retVal = new DoubleValue(this, val);
                            }
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Cannot convert range value", exception);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value as a decimal value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Decimal getValueAsInt(IValue value)
        {
            Decimal retVal;

            IntValue intVal = value as IntValue;
            if (intVal != null)
            {
                retVal = intVal.Val;
            }
            else
            {
                DoubleValue doubleVal = value as DoubleValue;
                if (doubleVal != null)
                {
                    retVal = new Decimal(Math.Round(doubleVal.Val));
                }
                else
                {
                    throw new Exception("Cannot convert value " + value + " to " + FullName);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value as a double value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Double getValueAsDouble(IValue value)
        {
            Double retVal;

            IntValue intVal = value as IntValue;
            if (intVal != null)
            {
                retVal = Decimal.ToDouble(intVal.Val);
            }
            else
            {
                DoubleValue doubleVal = value as DoubleValue;
                if (doubleVal != null)
                {
                    retVal = doubleVal.Val;
                }
                else
                {
                    throw new Exception("Cannot convert value " + value + " to " + FullName);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the enclosing collection to allow deletion of a range
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return NameSpace.Ranges; }
        }

        /// <summary>
        ///     Derefs enumerate values
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private IValue derefEnum(IValue val)
        {
            IValue retVal = val;

            EnumValue enumValue = retVal as EnumValue;
            if (enumValue != null)
            {
                retVal = enumValue.Value;
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that binary operation is valid for this type and the other type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool ValidBinaryOperation(BinaryExpression.OPERATOR operation, Type otherType)
        {
            bool retVal = base.ValidBinaryOperation(operation, otherType);

            if (!retVal)
            {
                if (operation == BinaryExpression.OPERATOR.ADD || operation == BinaryExpression.OPERATOR.DIV ||
                    operation == BinaryExpression.OPERATOR.MULT || operation == BinaryExpression.OPERATOR.SUB)
                {
                    // Allow implicit conversions
                    IntegerType integerType = otherType as IntegerType;
                    if (integerType != null)
                    {
                        retVal = true;
                    }
                    else
                    {
                        DoubleType doubleType = otherType as DoubleType;
                        retVal = (doubleType != null);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the arithmetic operation based on the type of the result
        /// </summary>
        /// <param name="context">The context used to perform this operation</param>
        /// <param name="left"></param>
        /// <param name="Operation"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override IValue PerformArithmericOperation(InterpretationContext context, IValue left,
            BinaryExpression.OPERATOR Operation, IValue right) // left +/-/*/div/exp right
        {
            IValue retVal = null;

            left = derefEnumForArithmeticOperation(left);
            right = derefEnumForArithmeticOperation(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 == null || int2 == null)
            {
                retVal = EFSSystem.DoubleType.PerformArithmericOperation(context, left, Operation, right);
            }
            else
            {
                retVal = EFSSystem.IntegerType.PerformArithmericOperation(context, left, Operation, right);
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the dereferencing and launches an exception if the enum cannot be used for arithmetic operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IValue derefEnumForArithmeticOperation(IValue value)
        {
            IValue retVal = value;

            EnumValue enumValue = value as EnumValue;
            if (enumValue != null)
            {
                if (enumValue.getForbidArithmeticOperation())
                {
                    throw new Exception("Cannot perform arithmetic operation with " + value.LiteralName);
                }
                else
                {
                    retVal = enumValue.Value;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Compares two ranges for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override bool CompareForEquality(IValue left, IValue right)
        {
            bool retVal = false;

            left = derefEnum(left);
            right = derefEnum(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 != null && int2 != null)
            {
                retVal = (int1.Val == int2.Val);
            }
            else
            {
                DoubleValue double1 = left as DoubleValue;
                DoubleValue double2 = right as DoubleValue;

                if (double1 != null && double2 != null)
                {
                    retVal = DoubleType.CompareDoubleForEquality(double1.Val, double2.Val);
                    ;
                }
                else
                {
                    retVal = base.CompareForEquality(left, right);
                }
            }

            return retVal;
        }

        public override bool Less(IValue left, IValue right) // left < right
        {
            bool retVal = false;

            left = derefEnum(left);
            right = derefEnum(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 != null && int2 != null)
            {
                retVal = int1.Val < int2.Val;
            }
            else
            {
                retVal = EFSSystem.DoubleType.Less(left, right);
            }

            return retVal;
        }

        public override bool Greater(IValue left, IValue right) // left > right
        {
            bool retVal = false;

            left = derefEnum(left);
            right = derefEnum(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 != null && int2 != null)
            {
                retVal = int1.Val > int2.Val;
            }
            else
            {
                retVal = EFSSystem.DoubleType.Greater(left, right);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides all constant values for this type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="retVal"></param>
        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            foreach (EnumValue value in SpecialValues)
            {
                string name = Utils.Utils.concat(scope, value.Name);
                retVal[name] = retVal;
            }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (EnumValue value in SpecialValues)
            {
                ISubDeclaratorUtils.AppendNamable(this, value);
            }
        }

        /// <summary>
        ///     Provides all the values that can be stored in this range
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        ///     Provides the enum value which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EnumValue findEnumValue(string name)
        {
            EnumValue retVal = null;

            retVal = (EnumValue) INamableUtils.findByName(name, SpecialValues);

            return retVal;
        }

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public IValue findValue(string[] names, int index)
        {
            // HaCK: we should check the enclosing range names
            return findEnumValue(names[names.Length - 1]);
        }

        /// <summary>
        ///     Indicates that the other type may be placed in this range
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal)
            {
                if (otherType is IntegerType && getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision)
                {
                    retVal = true;
                }
                else if (otherType is DoubleType && getPrecision() == acceptor.PrecisionEnum.aDoublePrecision)
                {
                    retVal = true;
                }
                else
                {
                    Range otherRange = otherType as Range;
                    if (otherRange != null && getPrecision() == otherRange.getPrecision())
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                EnumValue item = element as EnumValue;
                if (item != null)
                {
                    appendSpecialValues(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Provides an explanation of the range
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            retVal += TextualExplainUtilities.Pad(Name + "{\\b : RANGE FROM }" + MinValue + " {\\b TO }" + MaxValue,
                indentLevel);
            foreach (EnumValue enumValue in SpecialValues)
            {
                retVal += "\\par " + enumValue.getExplain(indentLevel + 2);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides an explanation of the range
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public override string getExplain(bool explainSubElements)
        {
            string retVal = getExplain(0);

            return TextualExplainUtilities.Encapsule(retVal);
        }

        /// <summary>
        ///     Combines two types to create a new one
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public override Type CombineType(Type right, BinaryExpression.OPERATOR Operator)
        {
            Type retVal = null;

            if (Operator == BinaryExpression.OPERATOR.MULT)
            {
                if (FullName.CompareTo("Default.BaseTypes.Speed") == 0 &&
                    right.FullName.CompareTo("Default.BaseTypes.Time") == 0)
                {
                    NameSpace nameSpace = EnclosingNameSpaceFinder.find(this);
                    retVal = nameSpace.findTypeByName("Distance");
                }
            }
            else
            {
                if (IsDouble())
                {
                    if (right == EFSSystem.DoubleType)
                    {
                        retVal = this;
                    }
                }
                else
                {
                    if (right == EFSSystem.IntegerType)
                    {
                        retVal = this;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates a copy of the range in the designated dictionary. The namespace structure is copied over.
        /// The new range is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Range CreateRangeUpdate(Dictionary dictionary)
        {
            Range retVal = (Range)Duplicate();
            retVal.setUpdates(Guid);

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            NameSpace nameSpace = dictionary.GetNameSpace(names, Dictionary);
            nameSpace.appendRanges(retVal);

            return retVal;
        }
    }
}