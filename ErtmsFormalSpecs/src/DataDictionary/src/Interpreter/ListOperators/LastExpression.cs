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

using DataDictionary.Types;
using DataDictionary.Values;

namespace DataDictionary.Interpreter.ListOperators
{
    public class LastExpression : ConditionBasedListExpression
    {
        /// <summary>
        /// The operator for this expression
        /// </summary>
        public static string OPERATOR = "LAST";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listExpression"></param>
        /// <param name="condition"></param>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="iteratorVariableName"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public LastExpression(ModelElement root, ModelElement log, Expression listExpression, string iteratorVariableName, Expression condition, int start, int end)
            : base(root, log, listExpression, iteratorVariableName, condition, start, end)
        {
        }

        /// <summary>
        /// Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            Collection listType = ListExpression.GetExpressionType() as Collection;
            if (listType != null)
            {
                retVal = listType.Type;
            }
            else
            {
                AddError("Cannot evaluate list type of " + ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = EFSSystem.EmptyValue;

            ListValue value = ListExpression.GetValue(context, explain) as ListValue;
            if (value != null)
            {
                int token = PrepareIteration(context);
                for (int i = value.Val.Count - 1; i >= 0; i--)
                {
                    IValue v = value.Val[i];

                    if (v != EFSSystem.EmptyValue)
                    {
                        ElementFound = true;
                        IteratorVariable.Value = v;
                        if (conditionSatisfied(context, explain))
                        {
                            MatchingElementFound = true;
                            retVal = IteratorVariable.Value;
                            break;
                        }
                    }
                    NextIteration();
                }
                EndIteration(context, explain, token);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string retVal = OPERATOR + " " + IteratorVariable.Name + " IN " + ListExpression.ToString(indentLevel);

            if (Condition != null)
            {
                retVal += " | " + Condition.ToString(indentLevel);
            }

            return retVal;
        }
    }
}