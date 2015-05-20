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
using DataDictionary.Generated;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Enum = DataDictionary.Types.Enum;
using NameSpace = DataDictionary.Types.NameSpace;
using Range = DataDictionary.Types.Range;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Constants
{
    public class EnumValue : Generated.EnumValue, IValue
    {
        /// <summary>
        ///     The corresponding type
        /// </summary>
        public Type Type
        {
            get
            {
                Type retVal = null;

                if (Enum != null)
                {
                    retVal = Enum;
                }
                else if (Range != null)
                {
                    retVal = Range;
                }

                return retVal;
            }
            set { }
        }

        public string LiteralName
        {
            get
            {
                string retVal = "";

                if (Enum != null)
                {
                    retVal = Enum.Name + "." + Name;
                }
                else if (Range != null)
                {
                    retVal = Range.Name + "." + Name;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     The enclosing enumeration type
        /// </summary>
        public Enum Enum
        {
            get { return Enclosing as Enum; }
        }

        /// <summary>
        ///     The enclosing range
        /// </summary>
        public Range Range
        {
            get { return Enclosing as Range; }
        }

        public IValue Value
        {
            get
            {
                IValue retVal = this;

                if (Range != null)
                {
                    retVal = Range.getValue(getValue());
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the enclosing collection, for deletion purposes
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (Enum != null)
                {
                    retVal = Enum.Values;
                }
                else if (Range != null)
                {
                    retVal = Range.SpecialValues;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Creates a valid right side IValue, according to the target variable (left side)
        /// </summary>
        /// <param name="variable">The target variable</param>
        /// <param name="duplicate">Indicates that a duplication of the variable should be performed</param>
        /// <param name="setEnclosing">Indicates that the new value enclosing element should be set</param>
        /// <returns></returns>
        public virtual IValue RightSide(IVariable variable, bool duplicate, bool setEnclosing)
        {
            return this;
        }

        /// <summary>
        ///     The namespace related to the typed element
        /// </summary>
        public NameSpace NameSpace
        {
            get { return null; }
        }

        /// <summary>
        ///     Provides the type name of the element
        /// </summary>
        public string TypeName
        {
            get { return Type.FullName; }
            set { }
        }

        /// <summary>
        ///     Provides the mode of the typed element
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
        }

        /// <summary>
        ///     Provides the default value of the typed element
        /// </summary>
        public string Default
        {
            get { return FullName; }
            set { }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Provides an explanation of the enumeration
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            retVal += Name;
            if (!String.IsNullOrEmpty(getValue()))
            {
                retVal += " : " + getValue();
            }

            return TextualExplainUtilities.Pad(retVal, indentLevel);
        }

        /// <summary>
        ///     The explanation of the element
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool subElements)
        {
            return getExplain(0);
        }

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public string ToExpressionWithDefault()
        {
            return FullName;
        }
    }
}