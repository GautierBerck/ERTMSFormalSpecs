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
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Values;
using Utils;
using EnumValue = DataDictionary.Constants.EnumValue;

namespace DataDictionary.Types
{
    public class Enum : Generated.Enum, IEnumerateValues, ISubDeclarator, ITextualExplain
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
        /// The default value of the enum
        /// </summary>
        public override string Default
        {
            get
            {
                string retVal = getDefault();
                if (retVal.Contains("."))
                {
                    string[] elements = retVal.Split('.');
                    retVal = elements[elements.Length - 1];
                }
                return FullName + "." + retVal;
            }
            set
            {
                setDefault (value);
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
                string name = Utils.Util.concat(scope, value.Name);
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
        public EnumValue FindEnumValue(string name)
        {
            EnumValue retVal = (EnumValue) NamableUtils.FindByName(name, Values);

            if (retVal == null && EnclosingEnum != null)
            {
                retVal = EnclosingEnum.FindEnumValue(name);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the sub enum which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Enum FindSubEnum(string name)
        {
            return (Enum) NamableUtils.FindByName(name, SubEnums);
        }

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public IValue findValue(string[] names, int index)
        {
            // HACK: we should check the enclosing enums names
            return FindEnumValue(names[names.Length - 1]);
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
            retVal = FindEnumValue(names[names.Length - 1]);

            if (retVal == null)
            {
                AddError("Value " + image + " is not a valid value for " + Name);
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
        /// An enum is equal to another enum if:
        /// - it has the same value
        /// - or it is a sub-enum with the same value
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override bool CompareForEquality(IValue left, IValue right) // left == right
        {
            bool retVal = false;

            if (left != null && right != null)
            {
                if (left == right)
                {
                    retVal = true;
                }
                else if (left.Type is Enum && right.Type is Enum)
                {
                    // if left is sub-enum of right or right is sub-enum of left,
                    // then we can only compare their names
                    Enum leftTypeEnum = left.Type as Enum;
                    Enum rightTypeEnum = right.Type as Enum;
                    if (leftTypeEnum.SubEnums.Contains(rightTypeEnum) || rightTypeEnum.SubEnums.Contains(leftTypeEnum))
                    {
                        retVal = left.Name == right.Name;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
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
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            base.GetExplain(explanation, explainSubElements);

            explanation.Write("ENUMERATION ");
            explanation.WriteLine(Name);

            explanation.Indent(2, () =>
            {
                foreach (EnumValue enumValue in Values)
                {
                    explanation.Write(enumValue, explainSubElements);
                }
            });
        }

        /// <summary>
        ///     Creates a copy of the enum in the designated dictionary. The namespace structure is copied over.
        ///     The new enum is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Enum CreateEnumUpdate(Dictionary dictionary)
        {
            Enum retVal = (Enum) GetTopLevelEnum().Duplicate();
            retVal.SetUpdateInformation(GetTopLevelEnum());
            retVal.ClearAllRequirements();

            String[] names = GetTopLevelEnum().FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
            if (nameSpace != null)
            {
                nameSpace.appendEnumerations(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Looks through enclosing elements for the enumeration that is at the name space level
        /// </summary>
        /// <returns></returns>
        private Enum GetTopLevelEnum()
        {
            Enum retVal = this;

            String[] names = FullName.Split('.');
            string fullName = names[0];
            int i = 0;
            while (!(Dictionary.FindByFullName(fullName) is Enum))
            {
                i++;
                fullName += "." + names[i];
            }

            if (fullName != FullName)
            {
                retVal = Dictionary.FindByFullName(fullName) as Enum;
            }

            return retVal;
        }

        /// <summary>
        ///     Handles setting all the update information for this element (this updates source)
        /// </summary>
        /// <param name="source"></param>
        public override void SetUpdateInformation(ModelElement source)
        {
            base.SetUpdateInformation(source);

            Enum sourceEnum = source as Enum;
            if (sourceEnum != null)
            {
                Requirements.Clear();
                setUpdates(source.Guid);

                foreach (EnumValue value in Values)
                {
                    EnumValue matchingValue = sourceEnum.FindEnumValue(value.Name);
                    value.setUpdates(matchingValue.Guid);
                }
                foreach (Enum subEnum in SubEnums)
                {
                    Enum matchingEnum = sourceEnum.FindSubEnum(subEnum.Name);
                    subEnum.SetUpdateInformation(matchingEnum);
                }
            }
        }

        /// <summary>
        ///     Deletes all requirements for this enum and all sub elements
        /// </summary>
        public override void ClearAllRequirements()
        {
            base.ClearAllRequirements();

            foreach (Enum subEnum in SubEnums)
            {
                subEnum.ClearAllRequirements();
            }
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            result += "Enumeration " + Name + " with " + Values.Count + " values";

            return result;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Enum CreateDefault(ICollection enclosingCollection)
        {
            Enum retVal = (Enum) acceptor.getFactory().createEnum();

            Util.DontNotify(() => { retVal.Name = "Enumeration" + GetElementNumber(enclosingCollection); });

            return retVal;
        }
    }
}