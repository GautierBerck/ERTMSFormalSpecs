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
namespace DataDictionary
{
    public interface IExpressionable
    {
        /// <summary>
        /// The expression text for this expressionable
        /// </summary>
        string ExpressionText { get; set; }

        /// <summary>
        /// The corresponding expression tree
        /// </summary>
        Interpreter.InterpreterTreeNode Tree { get; }

        /// <summary>
        /// Clears the expression tree to ensure new compilation
        /// </summary>
        void CleanCompilation();

        /// <summary>
        /// Creates the tree according to the expression text
        /// </summary>
        void Compile();
    }
}
