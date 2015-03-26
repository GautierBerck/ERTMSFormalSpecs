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

using DataDictionary.Values;
using Utils;

namespace DataDictionary.Interpreter.Filter
{
    /// <summary>
    ///     Predicate which indicates that the namable can be called
    /// </summary>
    public class IsLiteral : BaseFilter
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        protected IsLiteral()
            : base()
        {
        }

        /// <summary>
        ///     Predicate which indicates whether the namable provided matches the expectation for the semantic analysis
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool AcceptableChoice(INamable value)
        {
            return Predicate(value);
        }

        /// <summary>
        ///     Predicate, so that the code can be reused
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Predicate(INamable value)
        {
            return (value is IValue);
        }

        /// <summary>
        ///     Singleton
        /// </summary>
        public static IsLiteral INSTANCE = new IsLiteral();
    }
}