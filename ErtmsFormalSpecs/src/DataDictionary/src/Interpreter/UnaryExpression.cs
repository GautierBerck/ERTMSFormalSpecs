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
using DataDictionary.Functions;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class UnaryExpression : Expression
    {
        /// <summary>
        ///     The term of this expression
        /// </summary>
        public Term Term { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root for which this expression should be evaluated</param>
        /// <param name="term"></parparam>
        ///     <param name="start">The start character for this expression in the original string</param>
        ///     <param name="end">The end character for this expression in the original string</param>
        public UnaryExpression(ModelElement root, ModelElement log, Term term, int start, int end)
            : base(root, log, start, end)
        {
            Term = term;
            Term.Enclosing = this;
        }

        /// <summary>
        ///     The expression for the unary op
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        ///     The unary operator used
        /// </summary>
        public string UnaryOp { get; private set; }

        /// <summary>
        ///     The not operator
        /// </summary>
        public static string NOT = "NOT";

        public static string MINUS = "-";
        public static string[] UNARY_OPERATORS = {NOT, MINUS};


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root for which this expression should be evaluated</param>
        /// <param name="expression">The enclosed expression</param>
        /// <param name="unaryOp">
        ///     the unary operator for this unary expression</parparam>
        ///     <param name="start">The start character for this expression in the original string</param>
        ///     <param name="end">The end character for this expression in the original string</param>
        public UnaryExpression(ModelElement root, ModelElement log, Expression expression, string unaryOp, int start,
            int end)
            : base(root, log, start, end)
        {
            Expression = expression;
            Expression.Enclosing = this;

            UnaryOp = unaryOp;
        }

        /// <summary>
        ///     Provides the possible references for this dereference expression (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public override ReturnValue getReferences(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = ReturnValue.Empty;

            if (Term != null)
            {
                retVal = Term.getReferences(instance, expectation, last);
            }
            else
            {
                if (UnaryOp == null)
                {
                    retVal = Expression.getReferences(instance, expectation, last);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the possible references types for this expression (used in semantic analysis)
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public override ReturnValue getReferenceTypes(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = ReturnValue.Empty;

            if (Term != null)
            {
                retVal = Term.getReferenceTypes(instance, expectation, last);
            }
            else
            {
                if (UnaryOp == null)
                {
                    retVal = Expression.getReferenceTypes(instance, expectation, true);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">
        ///     Indicates the kind of element we are looking for</paraparam>
        ///     <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                if (Term != null)
                {
                    Term.SemanticAnalysis(instance, expectation, true);
                    StaticUsage = Term.StaticUsage;
                }
                else if (Expression != null)
                {
                    Expression.SemanticAnalysis(instance, expectation);
                    StaticUsage = Expression.StaticUsage;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the ICallable that is statically defined
        /// </summary>
        public override ICallable getStaticCallable()
        {
            ICallable retVal = null;

            if (Term != null)
            {
                retVal = base.getStaticCallable();
            }
            else if (Expression != null)
            {
                retVal = Expression.getStaticCallable();
            }

            return retVal;
        }

        /// <summary>
        ///     The model element referenced by this expression.
        /// </summary>
        public override INamable Ref
        {
            get
            {
                INamable retVal = null;

                if (Term != null)
                {
                    retVal = Term.Ref;
                }
                else if (Expression != null)
                {
                    if (UnaryOp == null)
                    {
                        retVal = Expression.Ref;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            if (Term != null)
            {
                retVal = Term.GetExpressionType();
            }
            else if (Expression != null)
            {
                if (NOT.CompareTo(UnaryOp) == 0)
                {
                    Type type = Expression.GetExpressionType();
                    if (type is BoolType)
                    {
                        retVal = type;
                    }
                    else
                    {
                        AddError("Cannot apply NOT on non boolean types");
                    }
                }
                else if (MINUS.CompareTo(UnaryOp) == 0)
                {
                    Type type = Expression.GetExpressionType();
                    if (type == EFSSystem.IntegerType || type == EFSSystem.DoubleType || type is Range)
                    {
                        retVal = type;
                    }
                    else
                    {
                        AddError("Cannot apply - on non integral types");
                    }
                }
                else
                {
                    retVal = Expression.GetExpressionType();
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the variable referenced by this expression, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <returns></returns>
        public override IVariable GetVariable(InterpretationContext context)
        {
            IVariable retVal = null;

            if (Term != null)
            {
                retVal = Term.GetVariable(context);
            }
            else if (Expression != null && UnaryOp == null)
            {
                retVal = null;
            }
            else
            {
                AddError("Cannot get variable from expression" + ToString());
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            if (Term != null)
            {
                retVal = Term.GetValue(context, explain);
            }
            else
            {
                if (NOT.CompareTo(UnaryOp) == 0)
                {
                    BoolValue b = Expression.GetValue(context, explain) as BoolValue;
                    if (b != null)
                    {
                        if (b.Val)
                        {
                            retVal = EFSSystem.BoolType.False;
                        }
                        else
                        {
                            retVal = EFSSystem.BoolType.True;
                        }
                    }
                    else
                    {
                        AddError("Expression " + Expression.ToString() + " does not evaluate to boolean");
                    }
                }
                else if (MINUS.CompareTo(UnaryOp) == 0)
                {
                    IValue val = Expression.GetValue(context, explain);
                    IntValue intValue = val as IntValue;
                    if (intValue != null)
                    {
                        retVal = new IntValue(intValue.Type, -intValue.Val);
                    }
                    else
                    {
                        DoubleValue doubleValue = val as DoubleValue;
                        if (doubleValue != null)
                        {
                            retVal = new DoubleValue(doubleValue.Type, -doubleValue.Val);
                        }
                    }

                    if (retVal == null)
                    {
                        AddError("Cannot negate value for " + Expression.ToString());
                    }
                }
                else
                {
                    retVal = Expression.GetValue(context, explain);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override ICallable getCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            if (Term != null)
            {
                retVal = Term.getCalled(context, explain);
            }
            else if (Expression != null)
            {
                retVal = Expression.getCalled(context, explain);
            }

            // TODO : Investigate why this 
            if (retVal == null)
            {
                retVal = GetValue(context, explain) as ICallable;
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            if (Term != null)
            {
                Term.fill(retVal, filter);
            }
            if (Expression != null)
            {
                Expression.fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Provides the indented expression text
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <returns></returns>
        public override string ToString(int indentLevel)
        {
            string retVal = "";

            if (Term != null)
            {
                retVal = Term.ToString(indentLevel);
            }
            else
            {
                if (UnaryOp != null)
                {
                    retVal = UnaryOp + " " + Expression.ToString(indentLevel);
                }
                else
                {
                    retVal = "(" + Expression.ToString() + ")";
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void checkExpression()
        {
            base.checkExpression();

            if (Term != null)
            {
                Term.checkExpression();
            }
            if (Expression != null)
            {
                Expression.checkExpression();
            }
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = base.createGraph(context, parameter, explain);

            if (Term != null)
            {
                retVal = Graph.createGraph(GetValue(context, explain), parameter, explain);
            }
            else if (Expression != null)
            {
                if (UnaryOp == null)
                {
                    retVal = Expression.createGraph(context, parameter, explain);
                }
                else if (UnaryOp == MINUS)
                {
                    retVal = Expression.createGraph(context, parameter, explain);
                    retVal.Negate();
                }
                else
                {
                    throw new Exception("Cannot create graph where NOT operator is defined");
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="xParam">The X axis of this surface</param>
        /// <param name="yParam">The Y axis of this surface</param>
        /// <param name="explain"></param>
        /// <returns>The surface which corresponds to this expression</returns>
        public override Surface createSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = base.createSurface(context, xParam, yParam, explain);

            if (Term != null)
            {
                retVal = Surface.createSurface(xParam, yParam, GetValue(context, explain), explain);
            }
            else if (Expression != null)
            {
                if (UnaryOp == null)
                {
                    retVal = Expression.createSurface(context, xParam, yParam, explain);
                }
                else
                {
                    if (UnaryOp == MINUS)
                    {
                        retVal = Expression.createSurface(context, xParam, yParam, explain);
                        retVal.Negate();
                    }
                    else
                    {
                        AddError("Cannot create surface with unary op " + UnaryOp);
                    }
                }
            }
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }
    }
}