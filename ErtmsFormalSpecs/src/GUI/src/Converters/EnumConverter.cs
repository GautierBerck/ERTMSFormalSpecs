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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace GUI.Converters
{
    public class GenericEnumConverter<T> : EnumConverter
    {
        private Type _enumType;

        /// <summary>
        /// Provides the translation between a enum and its textual representation
        /// </summary>
        protected struct Converter
        {
            public T val;
            public string display;

            public Converter(T val, string display)
            {
                this.val = val;
                this.display = display;
            }
        }

        /// <summary>
        /// Holds the convertion rules
        /// </summary>
        protected List<Converter> converters = new List<Converter>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public GenericEnumConverter(Type type)
            : base(type)
        {
            _enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            string retVal = converters[0].display;

            T val = (T)value;
            foreach (Converter converter in converters)
            {
                if (converter.val.Equals(val))
                {
                    retVal = converter.display;
                    break;
                }
            }

            return retVal;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            T retVal = converters[0].val;

            foreach (Converter converter in converters)
            {
                if (converter.display.CompareTo(value) == 0)
                {
                    retVal = converter.val;
                    break;
                }
            }

            return retVal;
        }
    }

    public class VariableModeConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.VariableModeEnumType>
    {
        public VariableModeConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.VariableModeEnumType.aConstant, "Constant"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.VariableModeEnumType.aIncoming, "In"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.VariableModeEnumType.aInOut, "In Out"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.VariableModeEnumType.aInternal, "Internal"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.VariableModeEnumType.aOutgoing, "Out"));
        }
    }

    public class ImplementationStatusConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM>
    {
        public ImplementationStatusConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA, "N/A"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented, "Implemented"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable, "Not implementable"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NewRevisionAvailable, "New revision available"));
        }
    }

    public class ScopeConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.Paragraph_scope>
    {
        public ScopeConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_scope.aOBU, "On Board Unit"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_scope.aOBU_AND_TRACK, "On Board Unit and Track"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_scope.aTRACK, "Track"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_scope.aROLLING_STOCK, "Rolling stock"));
        }
    }

    public class SpecTypeConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.Paragraph_type>
    {
        public SpecTypeConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aDEFINITION, "Definition"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aDELETED, "Deleted"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aNOTE, "Note"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aPROBLEM, "Problem"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aREQUIREMENT, "Requirement"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aTITLE, "Title"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.Paragraph_type.aTABLE_HEADER, "Table header"));
        }
    }

    public class RulePriorityConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.RulePriority>
    {
        public RulePriorityConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.RulePriority.aProcessing, "Processing"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.RulePriority.aUpdateINTERNAL, "Update INTERNAL variables"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.RulePriority.aUpdateOUT, "Update OUT variables"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.RulePriority.aVerification, "INPUT Verification"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.RulePriority.aCleanUp, "Clean Up"));
        }
    }

    public class RangePrecisionConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.PrecisionEnum>
    {
        public RangePrecisionConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.PrecisionEnum.aIntegerPrecision, "Integer"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.PrecisionEnum.aDoublePrecision, "Floating point"));
        }
    }

    public class ExpectationKindConverter : GenericEnumConverter<DataDictionary.Generated.acceptor.ExpectationKind>
    {
        public ExpectationKindConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(DataDictionary.Generated.acceptor.ExpectationKind.aInstantaneous, "Instantaneous"));
            converters.Add(new Converter(DataDictionary.Generated.acceptor.ExpectationKind.aContinuous, "Continuous"));
        }
    }

    public class ChangeActionConverter : GenericEnumConverter<HistoricalData.Generated.acceptor.ChangeOperationEnum>
    {
        public ChangeActionConverter(Type type)
            : base(type)
        {
            converters.Add(new Converter(HistoricalData.Generated.acceptor.ChangeOperationEnum.aAdd, "Add"));
            converters.Add(new Converter(HistoricalData.Generated.acceptor.ChangeOperationEnum.aChange, "Change"));
            converters.Add(new Converter(HistoricalData.Generated.acceptor.ChangeOperationEnum.aRemove, "Remove"));
        }
    }
}
