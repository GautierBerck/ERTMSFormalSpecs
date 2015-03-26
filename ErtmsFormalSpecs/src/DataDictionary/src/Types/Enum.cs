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

using System.Collections;
using System.Collections.Generic;
using DataDictionary.Constants;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Types
{
    public class Enum : Generated.Enum, IEnumerateValues, ISubDeclarator, TextualExplain
    {
        /// <summary>
        ///     Indicates if this Enum contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (Enum anEnum in SubEnums)
                {
                    if (anEnum.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        ///     Provides all the enum values this enumeration can take
        /// </summary>
        public ArrayList Values
        {
            get
            {
                if (allValues() == null)
                {
                    setAllValues(new ArrayList());
                }
                return allValues();
            }
        }

        /// <summary>
        ///     Provides all the sub enums this enumeration can define
        /// </summary>
        public ArrayList SubEnums
        {
            get
            {
                if (allSubEnums() == null)
                {
                    setAllSubEnums(new ArrayList());
                }
                return allSubEnums();
            }
        }

        /// <summary>
        ///     Provides all constant values for this type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="retVal"></param>
        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            foreach (EnumValue value in Values)
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

            foreach (EnumValue value in Values)
            {
                ISubDeclaratorUtils.AppendNamable(this, value);
            }
            foreach (Enum subEnum in SubEnums)
            {
                ISubDeclaratorUtils.AppendNamable(this, subEnum);
            }
        }

        /// <summary>
        ///     Provides all the values that can be stored in this structure
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            foreach (EnumValue item in Values)
            {
                if (item.Name.CompareTo(name) == 0)
                {
                    retVal.Add(item);
                    break;
                }
            }
            foreach (Enum item in SubEnums)
            {
                if (item.Name.CompareTo(name) == 0)
                {
                    retVal.Add(item);
                    break;
                }
            }
        }

        /// <summary>
        ///     Provides the enclosing enum
        /// </summary>
        public Enum EnclosingEnum
        {
            get { return Enclosing as Enum; }
        }

        /// <summary>
        ///     Provides the enum value which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EnumValue findEnumValue(string name)
        {
            EnumValue retVal = (EnumValue) INamableUtils.findByName(name, Values);

            if (retVal != null && EnclosingEnum != null)
            {
                retVal = EnclosingEnum.findEnumValue(name);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public IValue findValue(string[] names, int index)
        {
            // HaCK: we should check the enclosing enums names
            return findEnumValue(names[names.Length - 1]);
        }

        /// <summary>
        ///     Parses the image and provides the corresponding value
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            string[] names = image.Split('.');

            // HaCK: we should check the enclosing enums names
            retVal = findEnumValue(names[names.Length - 1]);

            if (retVal == null)
            {
                Log.Error("Value " + image + " is not a valid value for " + Name);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the enclosing collection to allow deletion of this enumeration
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (EnclosingEnum != null)
                {
                    retVal = EnclosingEnum.SubEnums;
                }
                else
                {
                    retVal = NameSpace.Enumerations;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates that the other type may be placed in this
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal && otherType is Enum)
            {
                Enum current = (Enum) otherType;

                while (current != null && !retVal)
                {
                    retVal = current == this;
                    current = current.EnclosingEnum;
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
                    appendValues(item);
                }
            }
            {
                Enum item = element as Enum;
                if (item != null)
                {
                    appendSubEnums(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Provides an explanation of the enumeration
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            retVal += TextualExplainUtilities.Pad(Name + "{\\b : ENUMERATION}", indentLevel);
            foreach (EnumValue enumValue in Values)
            {
                retVal += "\\par" + enumValue.getExplain(indentLevel + 2);
            }


            return retVal;
        }

        /// <summary>
        ///     Provides an explanation of the enumeration
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public override string getExplain(bool explainSubElements)
        {
            string retVal = getExplain(0);

            return TextualExplainUtilities.Encapsule(retVal);
        }
    }
}