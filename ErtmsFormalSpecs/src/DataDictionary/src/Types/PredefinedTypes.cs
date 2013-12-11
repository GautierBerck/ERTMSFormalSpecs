using System;
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
using System.Collections.Generic;

namespace DataDictionary.Types
{
    public class PredefinedType : Type
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        public PredefinedType(EFSSystem system, string name)
        {
            Enclosing = system;
            Name = name;
        }

        /// <summary>
        /// Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public virtual Values.IValue findValue(string[] names, int index)
        {
            Values.IValue retVal = null;

            if (index == names.Length - 1)
            {
                retVal = getValue(names[index]);
            }

            return retVal;
        }
    }

    public class BoolType : PredefinedType, IEnumerateValues, Utils.ISubDeclarator
    {
        public override string Name
        {
            get { return "Boolean"; }
            set { }
        }

        /// <summary>
        /// The true constant value
        /// </summary>
        private Values.BoolValue trueValue;
        public Values.BoolValue True
        {
            get { return trueValue; }
            private set { trueValue = value; }
        }

        /// <summary>
        /// The false constant value
        /// </summary>
        private Values.BoolValue falseValue;
        public Values.BoolValue False
        {
            get { return falseValue; }
            private set { falseValue = value; }
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<Utils.INamable>>();

            Utils.ISubDeclaratorUtils.AppendNamable(this, True);
            Utils.ISubDeclaratorUtils.AppendNamable(this, False);
        }

        /// <summary>
        /// The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements { get; set; }

        /// <summary>
        /// Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<Utils.INamable> retVal)
        {
            Utils.ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public BoolType(EFSSystem efsSystem)
            : base(efsSystem, "Boolean")
        {
            True = new Values.BoolValue(this, true);
            False = new Values.BoolValue(this, false);

            InitDeclaredElements();
        }

        /// <summary>
        /// Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override Values.IValue getValue(string image)
        {
            image = image.ToLowerInvariant();
            if (image.CompareTo("false") == 0)
            {
                return False;
            }
            else if (image.CompareTo("true") == 0)
            {
                return True;
            }
            return null;
        }

        /// <summary>
        /// Provides all constant values for this type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="retVal"></param>
        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            if (Utils.Utils.isEmpty(scope))
            {
                retVal[True.Name] = True;
                retVal[False.Name] = False;
            }
        }

        public override string Default
        {
            get { return "false"; }
        }

        public override bool CompareForEquality(Values.IValue left, Values.IValue right)
        {
            bool retVal = false;

            Values.BoolValue bool1 = left as Values.BoolValue;
            Values.BoolValue bool2 = right as Values.BoolValue;

            if (bool1 != null && bool2 != null)
            {
                retVal = bool1.Val == bool2.Val;
            }

            return retVal;
        }
    }

    public class IntegerType : PredefinedType
    {
        public override string Name
        {
            get { return "Integer"; }
            set { }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public IntegerType(EFSSystem efsSystem)
            : base(efsSystem, "Integer")
        {
        }

        /// <summary>
        /// The default value
        /// </summary>
        public override Values.IValue DefaultValue
        {
            get
            {
                Values.IValue retVal = new Values.IntValue(EFSSystem.IntegerType, 0);

                return retVal;
            }
        }

        /// <summary>
        /// Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal)
            {
                Range range = otherType as Range;
                if (range != null && range.getPrecision() == Generated.acceptor.PrecisionEnum.aIntegerPrecision)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override Values.IValue getValue(string image)
        {
            Values.IValue retVal = null;

            try
            {
                retVal = new Values.IntValue(this, Decimal.Parse(image));
            }
            catch (Exception e)
            {
                AddException(e);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the int value from the IValue provided
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private int getValue(Values.IValue val)
        {
            int retVal = 0;

            Constants.EnumValue enumValue = val as Constants.EnumValue;
            if (enumValue != null)
            {
                val = enumValue.Value;
            }

            Values.IntValue vi = val as Values.IntValue;
            if (vi != null)
            {
                retVal = (int)vi.Val;
            }
            else
            {
                throw new Exception("Cannot get integer value from " + val.LiteralName);
            }

            return retVal;
        }

        /// <summary>
        /// Performs the arithmetic operation based on the type of the result
        /// </summary>
        /// <param name="context">The context used to perform this operation</param>
        /// <param name="left"></param>
        /// <param name="Operation"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override Values.IValue PerformArithmericOperation(Interpreter.InterpretationContext context, Values.IValue left, Interpreter.BinaryExpression.OPERATOR Operation, Values.IValue right)  // left +/-/*/div/exp right
        {
            Values.IntValue retVal = null;

            int int1 = getValue(left);
            int int2 = getValue(right);

            switch (Operation)
            {
                case Interpreter.BinaryExpression.OPERATOR.EXP:
                    retVal = new Values.IntValue(EFSSystem.IntegerType, (Decimal)Math.Pow((double)int1, (double)int2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.MULT:
                    retVal = new Values.IntValue(EFSSystem.IntegerType, (int1 * int2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.DIV:
                    if (int2 == 0)
                        throw new Exception("Division by zero");
                    else
                        retVal = new Values.IntValue(EFSSystem.IntegerType, (int1 / int2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.ADD:
                    retVal = new Values.IntValue(EFSSystem.IntegerType, (int1 + int2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.SUB:
                    retVal = new Values.IntValue(EFSSystem.IntegerType, (int1 - int2));
                    break;
            }

            return retVal;
        }

        public override bool CompareForEquality(Values.IValue left, Values.IValue right)
        {
            bool retVal = false;

            int int1 = getValue(left);
            int int2 = getValue(right);

            retVal = int1 == int2;

            return retVal;
        }

        public override bool Less(Values.IValue left, Values.IValue right)  // left < right
        {
            bool retVal = false;

            int int1 = getValue(left);
            int int2 = getValue(right);

            retVal = int1 < int2;

            return retVal;
        }

        public override bool Greater(Values.IValue left, Values.IValue right)  // left > right
        {
            bool retVal = false;

            int int1 = getValue(left);
            int int2 = getValue(right);

            retVal = int1 > int2;

            return retVal;
        }
    }

    public class DoubleType : PredefinedType
    {
        public override string Name
        {
            get { return "Double"; }
            set { }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public DoubleType(EFSSystem efsSystem)
            : base(efsSystem, "Double")
        {
        }

        /// <summary>
        /// The default value
        /// </summary>
        public override Values.IValue DefaultValue
        {
            get
            {
                Values.IValue retVal = new Values.DoubleValue(EFSSystem.DoubleType, 0.0);

                return retVal;
            }
        }

        /// <summary>
        /// Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal)
            {
                Range range = otherType as Range;
                if (range != null && range.getPrecision() == Generated.acceptor.PrecisionEnum.aDoublePrecision)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override Values.IValue getValue(string image)
        {
            System.Globalization.CultureInfo info = System.Globalization.CultureInfo.InvariantCulture;
            Values.DoubleValue retVal = new Values.DoubleValue(this, double.Parse(image, info.NumberFormat));

            return retVal;
        }

        /// <summary>
        /// Provides the double value from the IValue provided
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private double getValue(Values.IValue val)
        {
            double retVal = 0;

            Constants.EnumValue enumValue = val as Constants.EnumValue;
            if (enumValue != null)
            {
                val = enumValue.Value;
            }

            Values.DoubleValue vd = val as Values.DoubleValue;
            if (vd != null)
            {
                retVal = vd.Val;
            }
            else
            {
                Values.IntValue vi = val as Values.IntValue;
                if (vi != null)
                {
                    retVal = (double)vi.Val;
                }
                else
                {
                    Functions.Function function = val as Functions.Function;
                    if (function != null)
                    {
                        Functions.Graph graph = function.Graph;
                        if (graph != null)
                        {
                            if (graph.Segments.Count == 1)
                            {
                                retVal = graph.Val(0);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Performs the arithmetic operation based on the type of the result
        /// </summary>
        /// <param name="context">The context used to perform this operation</param>
        /// <param name="left"></param>
        /// <param name="Operation"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override Values.IValue PerformArithmericOperation(Interpreter.InterpretationContext context, Values.IValue left, Interpreter.BinaryExpression.OPERATOR Operation, Values.IValue right)  // left +/-/*/div/exp right
        {
            Values.DoubleValue retVal = null;

            double double1 = getValue(left);
            double double2 = getValue(right);

            switch (Operation)
            {
                case Interpreter.BinaryExpression.OPERATOR.EXP:
                    retVal = new Values.DoubleValue(this, Math.Pow(double1, double2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.MULT:
                    retVal = new Values.DoubleValue(this, (double1 * double2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.DIV:
                    if (double2 == 0)
                        throw new Exception("Division by zero");
                    else
                        retVal = new Values.DoubleValue(this, (double1 / double2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.ADD:
                    retVal = new Values.DoubleValue(this, (double1 + double2));
                    break;

                case Interpreter.BinaryExpression.OPERATOR.SUB:
                    retVal = new Values.DoubleValue(this, (double1 - double2));
                    break;
            }

            return retVal;
        }

        public override bool CompareForEquality(Values.IValue left, Values.IValue right)
        {
            return CompareDoubleForEquality(getValue(left), getValue(right));
        }

        /// <summary>
        /// Compares two double values for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool CompareDoubleForEquality(double left, double right)
        {
            bool retVal = false;
            const double EPSILON = 0.000000001;

            retVal = Math.Abs(left - right) < EPSILON;

            return retVal;
        }

        public override bool Less(Values.IValue left, Values.IValue right)  // left < right
        {
            bool retVal = false;

            double double1 = getValue(left);
            double double2 = getValue(right);
            retVal = double1 < double2;

            return retVal;
        }

        public override bool Greater(Values.IValue left, Values.IValue right)  // left > right
        {
            bool retVal = false;

            double double1 = getValue(left);
            double double2 = getValue(right);
            retVal = double1 > double2;

            return retVal;
        }

        /// <summary>
        /// Combines two types to create a new one
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public override Types.Type CombineType(Type right, DataDictionary.Interpreter.BinaryExpression.OPERATOR Operator)
        {
            Types.Type retVal = null;

            if (Operator == DataDictionary.Interpreter.BinaryExpression.OPERATOR.MULT)
            {
                Range range = right as Range;
                if (range != null)
                {
                    if (range.getPrecision() == Generated.acceptor.PrecisionEnum.aIntegerPrecision)
                    {
                        retVal = this;
                    }
                }
            }

            return retVal;
        }
    }

    public class StringType : PredefinedType
    {
        public override string Name
        {
            get { return "String"; }
            set { }
        }

        public StringType(EFSSystem efsSystem)
            : base(efsSystem, "String")
        {
        }

        /// <summary>
        /// Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override Values.IValue getValue(string image)
        {
            return new Values.StringValue(this, image);
        }

        public override string Default
        {
            get { return ""; }
        }

        public override Values.IValue DefaultValue
        {
            get
            {
                return getValue(Default);
            }
        }

        public override bool CompareForEquality(Values.IValue left, Values.IValue right)
        {
            bool retVal = false;

            Values.StringValue val1 = left as Values.StringValue;
            Values.StringValue val2 = right as Values.StringValue;

            if (val1 != null && val2 != null)
            {
                retVal = val1.Val.CompareTo(val2.Val) == 0;
            }

            return retVal;
        }
    }
}
