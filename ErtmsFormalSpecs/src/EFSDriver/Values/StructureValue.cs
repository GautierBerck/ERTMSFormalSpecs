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
namespace EFSService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    public partial class StructureValue : Value
    {
        /// <summary>
        /// Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            string retVal = "{";

            foreach (KeyValuePair<string, Value> item in Value)
            {
                if (retVal.Length != 1)
                {
                    retVal += ", ";
                }

                retVal += item.Key + " => " + item.Value.ToString();
            }

            retVal += "}";

            return retVal;
        }

    }
}