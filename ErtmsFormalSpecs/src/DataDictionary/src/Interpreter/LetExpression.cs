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
using System.Net.Mime;
using System.Runtime.Remoting.Contexts;
using DataDictionary.Interpreter.Filter;
using Utils;

namespace DataDictionary.Interpreter
{
    /// <summary>
    /// LET variable '<-' expression IN expression
    /// LET variable '=>' expression IN expression
    /// </summary>
    public class LetExpression : Expression, Utils.ISubDeclarator
    {
        /// <summary>
        /// The variable bound by the LET expression
        /// </summary>
        public Variables.Variable BoundVariable { get; private set; }
        
        /// <summary>
        /// The binding expression
        /// </summary>
        public Expression BindingExpression { get; private set; }

        /// <summary>
        /// The expression to be evaluated
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        /// <param name="boundVariableName">The name of the bound variable</param>
        /// <param name="bindingExpression">The binding expression which provides the value of the variable</param>
        /// <param name="expression">The expression to be evaluated</param>
        public LetExpression(ModelElement root, ModelElement log, string boundVariableName, Expression bindingExpression, Expression expression, int start, int end)
            : base(root, log, start, end)
        {
            BoundVariable = (Variables.Variable)Generated.acceptor.getFactory().createVariable();
            BoundVariable.Enclosing = this;
            BoundVariable.Name = boundVariableName;

            BindingExpression = bindingExpression;
            BindingExpression.Enclosing = this;
            Expression = expression;
            Expression.Enclosing = this;

            InitDeclaredElements();
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            Utils.ISubDeclaratorUtils.AppendNamable(this, BoundVariable);
        }

        /// <summary>
        /// The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements { get; private set; }

        /// <summary>
        /// Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<Utils.INamable> retVal)
        {
            Utils.ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        /// Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(Utils.INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Binding expression
                BindingExpression.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                StaticUsage.AddUsages(BindingExpression.StaticUsage, Usage.ModeEnum.Read);


                Types.Type  bindingExpressionType = BindingExpression.GetExpressionType();
                if (bindingExpressionType != null)
                {
                    StaticUsage.AddUsage(bindingExpressionType, Root, Usage.ModeEnum.Type);
                    BoundVariable.Type = bindingExpressionType;

                    Expression.SemanticAnalysis(instance, expectation);
                    StaticUsage.AddUsages(Expression.StaticUsage, Usage.ModeEnum.Read);
                }
                else
                {
                    AddError("Cannot determine binding expression type for " + ToString());
                }
            }

            return retVal;
        }

        /// <summary>
        /// Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<Utils.INamable> retVal, BaseFilter filter)
        {
            BindingExpression.fill(retVal, filter);
            Expression.fill(retVal, filter);
        }

        /// <summary>
        /// Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void checkExpression()
        {
            base.checkExpression();

            BindingExpression.checkExpression();
            Expression.checkExpression();
        }


        /// <summary>
        /// Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Types.Type GetExpressionType()
        {
            return Expression.GetExpressionType();
        }

        /// <summary>
        /// Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override Values.IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            Values.IValue retVal = null;

            ExplanationPart subPart = ExplanationPart.CreateSubExplanation(explain, BoundVariable.Name);
            BoundVariable.Value = BindingExpression.GetValue(context, explain);
            ExplanationPart.SetNamable(subPart, BoundVariable.Value);

            int token = context.LocalScope.PushContext();
            context.LocalScope.setVariable(BoundVariable);
            retVal = Expression.GetValue(context, explain);
            context.LocalScope.PopContext(token);

            return retVal;
        }

        /// <summary>
        /// Provides the expression text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retVal = "LET " + BoundVariable.Name + " <- " +  BindingExpression.ToString() + " IN " + Expression.ToString();

            return retVal;
        }

    }
}
