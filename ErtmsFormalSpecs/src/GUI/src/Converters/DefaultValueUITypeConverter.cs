﻿// ------------------------------------------------------------------------------
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

using System.ComponentModel;
using System.Globalization;
using DataDictionary.Types;
using Type = System.Type;

namespace GUI.Converters
{
    /// <summary>
    ///     Converts IExpressionable to string, by getting the Expression property
    /// </summary>
    public class DefaultValueUITypeConverter : StringConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            BaseTreeNode.BaseEditor editor = context.Instance as BaseTreeNode.BaseEditor;
            string text = value as string;
            if (editor != null && text != null)
            {
                IDefaultValueElement defaultValueElement = editor.Model as IDefaultValueElement;
                if (defaultValueElement != null)
                {
                    defaultValueElement.Default = text;
                    return defaultValueElement;
                }
                else
                {
                    return base.ConvertFrom(context, culture, value);
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            string retVal = "<unknown>";
            IDefaultValueElement defaultValueElement = value as IDefaultValueElement;
            if (defaultValueElement != null)
            {
                retVal = defaultValueElement.Default;
                if (retVal != null)
                {
                    int index = retVal.IndexOf("\n");
                    if (index > 0)
                    {
                        retVal = retVal.Substring(0, index) + "...";
                    }
                }
                else
                {
                    retVal = "";
                }
            }

            return retVal;
        }
    }
}