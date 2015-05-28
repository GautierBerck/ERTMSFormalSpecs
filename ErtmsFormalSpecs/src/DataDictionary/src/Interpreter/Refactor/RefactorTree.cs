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

using System.Collections.Generic;
using DataDictionary.Types;

namespace DataDictionary.Interpreter.Refactor
{
    /// <summary>
    ///     This visitor is used to handle refactoring of expressions.
    /// </summary>
    public class RefactorTree : BaseRefactorTree
    {
        /// <summary>
        ///     The model element which should be refactored
        /// </summary>
        private ModelElement Ref { get; set; }

        /// <summary>
        ///     The user of the element
        /// </summary>
        private ModelElement User { get; set; }

        protected override void VisitDerefExpression(DerefExpression derefExpression)
        {
            ModelElement context = User;

            foreach (Expression expression in derefExpression.Arguments)
            {
                if (expression != null)
                {
                    if (expression.Ref == Ref)
                    {
                        string replacementValue = Ref.ReferenceName(User);

                        ReplaceText(replacementValue, expression.Start, expression.End);
                        break;
                    }
                    else if (expression.Ref is NameSpace)
                    {
                        // Remove all namespace prefixes, they will be taken into account in ReferenceName function
                        ReplaceText("", expression.Start, expression.End+1);
                    }
                    else
                    {
                        VisitExpression(expression);
                        User = expression.GetExpressionType();
                    }
                }
            }
            User = context;
        }

        protected override void VisitDesignator(Designator designator)
        {
            if (!designator.IsPredefined())
            {
                ModelElement referencedElement = designator.Ref as ModelElement;
                if ( referencedElement != null )
                {
                    string replacementValue = referencedElement.ReferenceName(User);
                    ReplaceText(replacementValue, designator.Start, designator.End);
                }
            }
        }

        /// <summary>
        ///     Visits a struct expression
        /// </summary>
        /// <param name="structExpression"></param>
        protected override void VisitStructExpression(StructExpression structExpression)
        {
            ModelElement backup = User;
            Structure structure = null;
            if (structExpression.Structure != null)
            {
                VisitExpression(structExpression.Structure);
                structure = structExpression.Structure.GetExpressionType() as Structure;
            }

            foreach (KeyValuePair<Designator, Expression> pair in structExpression.Associations)
            {
                if (pair.Key != null)
                {
                    User = structure;
                    VisitDesignator(pair.Key);
                    User = backup;
                }
                if (pair.Value != null)
                {
                    VisitExpression(pair.Value);
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="interpreterTreeNode"></param>
        /// <param name="text"></param>
        /// <param name="reference"></param>
        /// <param name="replacementValue"></param>
        public RefactorTree(ModelElement reference, ModelElement user)
        {
            Ref = reference;
            User = user;
        }
    }
}