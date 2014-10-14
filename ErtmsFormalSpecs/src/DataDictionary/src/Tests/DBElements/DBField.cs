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
namespace DataDictionary.Tests.DBElements
{
    public class DBField : Generated.DBField
    {
        /// <summary>
        /// The variable to be updated
        /// </summary>
        public string Variable
        {
            get
            {
                return getVariable();
            }
            set
            {
                setVariable(value);
            }
        }

        /// <summary>
        /// The value of the variable
        /// </summary>
        public string Value
        {
            get
            {
                return getValue();
            }
            set
            {
                setValue(value);
            }
        }

        /// <summary>
        /// Merges two fields
        /// </summary>
        /// <param name="p"></param>
        public void Merge(DBField other)
        {
            if (getVariable() == other.getVariable())
            {
                setGuid(other.getGuid());    
            }
        }
    }
}
