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
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Variables
{
    public interface IVariable : ITypedElement, ISubDeclarator
    {
        /// <summary>
        ///     Provides the value of the variable
        /// </summary>
        IValue Value { get; set; }

        /// <summary>
        ///     Provides the default value to give to the variable
        /// </summary>
        IValue DefaultValue { get; }

        /// <summary>
        /// Actions to be taken when the value of the variable changes
        /// </summary>
        /// <param name="references"></param>
        void HandleChange(CacheImpact references);

    }
}