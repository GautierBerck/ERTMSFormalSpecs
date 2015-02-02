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
using System.Collections;
using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using EnumValue = DataDictionary.Constants.EnumValue;
using PreCondition = DataDictionary.Rules.PreCondition;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions
{
    public class Function : Generated.Function, ISubDeclarator, IValue, TextualExplain, ICallable
    {
        /// <summary>
        /// The time spent evaluating this function
        /// </summary>
        public long ExecutionTimeInMilli { get; set; }

        /// <summary>
        /// Provides the number of times this rule has been executed
        /// </summary>
        public int ExecutionCount { get; set; }

        /// <summary>
        /// Provides the type name of the function
        /// </summary>
        public string TypeName
        {
            get { return getTypeName(); }
            set
            {
                returnType = null;
                setTypeName(value);
            }
        }

        /// <summary>
        /// The type associated to this function
        /// </summary>
        public Type Type
        {
            get { return this; }
            set { }
        }

        /// <summary>
        /// The function return type
        /// </summary>
        private Type returnType = null;

        public virtual Type ReturnType
        {
            get
            {
                if (returnType == null)
                {
                    returnType = EFSSystem.findType(NameSpace, getTypeName());
                }
                return returnType;
            }
            set
            {
                if (value != null)
                {
                    setTypeName(value.FullName);
                }
                else
                {
                    setTypeName(null);
                }
                returnType = value;
            }
        }

        /// <summary>
        /// Parameters of the function
        /// </summary>
        public ArrayList FormalParameters
        {
            get
            {
                if (allParameters() == null)
                {
                    setAllParameters(new ArrayList());
                }
                return allParameters();
            }
            set { setAllParameters(value); }
        }

        /// <summary>
        /// Provides the formal parameter whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Parameter getFormalParameter(string name)
        {
            Parameter retVal = null;

            foreach (Parameter parameter in FormalParameters)
            {
                if (parameter.Name.CompareTo(name) == 0)
                {
                    retVal = parameter;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Cases of the function
        /// </summary>
        public ArrayList Cases
        {
            get
            {
                ArrayList retVal = allCases();
                if (retVal == null)
                    retVal = new ArrayList();
                return retVal;
            }
            set { setAllCases(value); }
        }

        /// <summary>
        /// Assigns the values of the function parameters with values provided in the list Parameters
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameterValues">The values of the parameters</param>
        public void AssignParameters(InterpretationContext context, Dictionary<Actual, IValue> parameterValues)
        {
            foreach (KeyValuePair<Actual, IValue> pair in parameterValues)
            {
                context.LocalScope.setVariable(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Provides the mode of the variable
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
            set { }
        }

        /// <summary>
        /// The enclosing collection of the function
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return NameSpace.Functions; }
        }

        /// <summary>
        /// The complete name to access the value
        /// </summary>
        public string LiteralName
        {
            get { return FullName; }
        }

        /// <summary>
        /// Creates a valid right side IValue, according to the target variable (left side)
        /// </summary>
        /// <param name="variable">The target variable</param>
        /// <param name="duplicate">Indicates that a duplication of the variable should be performed</param>
        /// <param name="setEnclosing">Indicates that the new value enclosing element should be set</param>
        /// <returns></returns>
        public virtual IValue RightSide(IVariable variable, bool duplicate, bool setEnclosing)
        {
            return this;
        }

        /// <summary>
        /// Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public void additionalChecks(ModelElement root, Dictionary<string, Expression> actualParameters)
        {
        }

        /// <summary>
        /// Indicates that binary operation is valid for this type and the other type 
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool ValidBinaryOperation(BinaryExpression.OPERATOR operation, Type otherType)
        {
            bool retVal = false;

            if (ReturnType != null)
            {
                Function otherFunction = otherType as Function;
                if (otherFunction != null)
                {
                    if (otherFunction.ReturnType != null)
                    {
                        retVal = ReturnType.ValidBinaryOperation(operation, otherFunction.ReturnType);
                    }
                }
                else
                {
                    retVal = ReturnType.ValidBinaryOperation(operation, otherType);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the graph of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the graph</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = Graph;

            if (retVal == null)
            {
                try
                {
                    InterpretationContext ctxt = new InterpretationContext(context);
                    if (Cases.Count > 0)
                    {
                        // For now, just create graphs for functions using 0 or 1 parameter.
                        if (FormalParameters.Count == 0)
                        {
                            IValue value = Evaluate(ctxt, new Dictionary<Actual, IValue>(), explain);
                            retVal = Graph.createGraph(value, parameter, explain);
                        }
                        else if (FormalParameters.Count == 1)
                        {
                            Parameter param = (Parameter) FormalParameters[0];
                            int token = ctxt.LocalScope.PushContext();
                            IValue actualValue = null;
                            if (parameter != null)
                            {
                                IVariable actual = ctxt.findOnStack(parameter);
                                if (actual != null)
                                {
                                    actualValue = actual.Value;
                                }
                                else
                                {
                                    actualValue = new PlaceHolder(parameter.Type, 1);
                                }

                                ctxt.LocalScope.setParameter(param, actualValue);
                            }
                            retVal = createGraphForParameter(ctxt, param, explain);

                            if (getCacheable() && actualValue is PlaceHolder)
                            {
                                Graph = retVal;
                            }

                            ctxt.LocalScope.PopContext(token);
                        }
                        else
                        {
                            IValue value = Evaluate(ctxt, new Dictionary<Actual, IValue>(), explain);
                            retVal = Graph.createGraph(value, parameter, explain);
                        }
                    }
                }
                catch (Exception e)
                {
                    AddError("Cannot create graph of function, reason : " + e.Message);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the graph for a given parameter, the other parameters are considered fixed by the interpretation context
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameter for the X axis</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual Graph createGraphForParameter(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = Graph;

            if (retVal == null)
            {
                retVal = new Graph();

                foreach (Case cas in Cases)
                {
                    if (PreconditionSatisfied(context, cas, parameter, explain))
                    {
                        Graph subGraph = cas.Expression.createGraph(context, parameter, explain);
                        ReduceGraph(context, subGraph, cas, parameter, explain);
                        retVal.Merge(subGraph);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Combines two types to create a new one
        /// </summary>
        /// <param name="right"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public override Type CombineType(Type right, BinaryExpression.OPERATOR Operator)
        {
            Type retVal = null;

            Function function = right as Function;
            if (function != null)
            {
                if (this.ReturnType == function.ReturnType)
                {
                    if (this.FormalParameters.Count >= function.FormalParameters.Count)
                    {
                        retVal = this;
                    }
                    else
                    {
                        retVal = function;
                    }
                }
                else
                {
                    AddError("Cannot combine types " + this.ReturnType.Name + " and " + function.ReturnType.Name);
                }
            }
            else if (right.IsDouble())
            {
                retVal = this;
            }
            else
            {
                AddError("Cannot combine types " + this.ReturnType.Name + " and " + right.Name);
            }

            return retVal;
        }

        /// <summary>
        /// Indicates whether all preconditions are satisfied for a given case, ignoring expressions like parameter <= xxx or parameter >= xxx
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="cas">The case to evaluate</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private bool PreconditionSatisfied(InterpretationContext context, Case cas, Parameter parameter, ExplanationPart explain)
        {
            bool retVal = true;

            foreach (PreCondition preCondition in cas.PreConditions)
            {
                if (!ExpressionBasedOnParameter(parameter, preCondition.Expression))
                {
                    BoolValue boolValue = preCondition.Expression.GetValue(context, explain) as BoolValue;
                    if (boolValue == null)
                    {
                        throw new Exception("Cannot evaluate precondition " + preCondition.Name);
                    }
                    else if (!boolValue.Val)
                    {
                        retVal = false;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates if the expression if of the form parameter <= xxx or xxx <= parameter
        /// </summary>
        /// <param name="parameter">The parameter of the template</param>
        /// <param name="expression">The expression to analyze</param>
        /// <returns></returns>
        private bool ExpressionBasedOnParameter(Parameter parameter, Expression expression)
        {
            bool retVal = false;

            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                retVal = binaryExpression.Right.Ref == parameter
                         || binaryExpression.Left.Ref == parameter
                         || FunctionCallOnParameter(binaryExpression.Right, parameter)
                         || FunctionCallOnParameter(binaryExpression.Left, parameter);
            }

            return retVal;
        }

        /// <summary>
        /// Indicates that the expression is a function call using the parameter as argument value
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        private bool FunctionCallOnParameter(Expression expression, Parameter parameter)
        {
            bool retVal = false;

            Call call = expression as Call;
            if (call != null)
            {
                foreach (Expression expr in call.AllParameters)
                {
                    foreach (ITypedElement element in expr.GetRightSides())
                    {
                        if (element == parameter)
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Evaluates the boundaries associated to a specific preCondition
        /// </summary>
        /// <param name="context">The context used to evaluate the precondition and segment value</param>
        /// <param name="preCondition">The precondition to evaluate the range</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private List<Graph.Segment> EvaluateBoundaries(InterpretationContext context, PreCondition preCondition, Parameter parameter, ExplanationPart explain)
        {
            List<Graph.Segment> retVal = new List<Graph.Segment>();

            if (parameter != null)
            {
                BinaryExpression expression = preCondition.Expression as BinaryExpression;
                if (ExpressionBasedOnParameter(parameter, expression))
                {
                    IValue val;
                    if (expression.Right.Ref == parameter)
                    {
                        // Expression like xxx <= Parameter
                        val = expression.Left.GetValue(context, explain);
                        switch (expression.Operation)
                        {
                            case BinaryExpression.OPERATOR.LESS:
                            case BinaryExpression.OPERATOR.LESS_OR_EQUAL:
                                retVal.Add(new Graph.Segment(getDoubleValue(val), double.MaxValue, new Graph.Segment.Curve()));
                                break;

                            case BinaryExpression.OPERATOR.GREATER:
                            case BinaryExpression.OPERATOR.GREATER_OR_EQUAL:
                                retVal.Add(new Graph.Segment(0, getDoubleValue(val), new Graph.Segment.Curve()));
                                break;

                            default:
                                throw new Exception("Invalid comparison operator while evaluating Graph of function");
                        }
                    }
                    else
                    {
                        if (expression.Left.Ref == parameter)
                        {
                            // Expression like Parameter <= xxx
                            val = expression.Right.GetValue(context, explain);
                            switch (expression.Operation)
                            {
                                case BinaryExpression.OPERATOR.LESS:
                                case BinaryExpression.OPERATOR.LESS_OR_EQUAL:
                                    retVal.Add(new Graph.Segment(0, getDoubleValue(val), new Graph.Segment.Curve()));
                                    break;

                                case BinaryExpression.OPERATOR.GREATER:
                                case BinaryExpression.OPERATOR.GREATER_OR_EQUAL:
                                    retVal.Add(new Graph.Segment(getDoubleValue(val), double.MaxValue, new Graph.Segment.Curve()));
                                    break;

                                default:
                                    throw new Exception("Invalid comparison operator while evaluating Graph of function");
                            }
                        }
                        else
                        {
                            if (FunctionCallOnParameter(expression.Right, parameter))
                            {
                                Graph graph = expression.Right.createGraph(context, parameter, explain);
                                if (graph != null)
                                {
                                    // Expression like xxx <= f(Parameter)
                                    val = expression.Left.GetValue(context, explain);
                                    retVal = graph.GetSegments(BinaryExpression.Inverse(expression.Operation), getDoubleValue(val));
                                }
                                else
                                {
                                    AddError("Cannot create graph for " + expression.Right);
                                }
                            }
                            else
                            {
                                Graph graph = expression.Left.createGraph(context, parameter, explain);
                                if (graph != null)
                                {
                                    // Expression like f(Parameter) <= xxx
                                    val = expression.Right.GetValue(context, explain);
                                    retVal = graph.GetSegments(expression.Operation, getDoubleValue(val));
                                }
                                else
                                {
                                    throw new Exception("Cannot evaluate bounds of segment");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!ExpressionBasedOnPlaceHolder(context, expression))
                    {
                        BoolValue value = preCondition.Expression.GetValue(context, explain) as BoolValue;
                        if (value != null && value.Val)
                        {
                            retVal.Add(new Graph.Segment(0, double.MaxValue, new Graph.Segment.Curve()));
                        }
                    }
                }
            }
            else
            {
                AddError("Parameter is null");
            }

            return retVal;
        }

        /// <summary>
        /// Indicates whether the expression is based on a placeholder value, ommiting the parameter provided
        /// </summary>
        /// <param name="context">The current interpretation context</param>
        /// <param name="expression">The expression to evaluate</param>
        /// <returns></returns>
        private bool ExpressionBasedOnPlaceHolder(InterpretationContext context, BinaryExpression expression)
        {
            bool retVal = false;

            if (expression != null)
            {
                foreach (ITypedElement element in expression.GetRightSides())
                {
                    Parameter parameter = element as Parameter;
                    if (parameter != null)
                    {
                        IVariable variable = context.findOnStack(parameter);
                        if (variable != null)
                        {
                            PlaceHolder placeHolder = variable.Value as PlaceHolder;

                            if (placeHolder != null)
                            {
                                retVal = true;
                                break;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Reduces the graph to the only part releated to the preconditions
        /// </summary>
        /// <param name="context">The context used to evaluate the precondition and segment value</param>
        /// <param name="graph">The graph to reduce</param>
        /// <param name="cas">The case which is used to reduce the graph</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private void ReduceGraph(InterpretationContext context, Graph graph, Case cas, Parameter parameter, ExplanationPart explain)
        {
            foreach (PreCondition preCondition in cas.PreConditions)
            {
                List<Graph.Segment> boundaries = EvaluateBoundaries(context, preCondition, parameter, explain);
                graph.Reduce(boundaries);
            }
        }

        /// <summary>
        /// Provides the graph associated to the function, if any
        /// </summary>
        private Graph graph;

        public Graph Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                if (graph != null)
                {
                    graph.Function = this;
                }
            }
        }

        /// <summary>
        /// Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the graph</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual Surface createSurface(InterpretationContext context, ExplanationPart explain)
        {
            Surface retVal = Surface;

            if (retVal == null)
            {
                try
                {
                    if (FormalParameters.Count == 2)
                    {
                        // Select which parameter is the X axis of the surface, and which is the Y
                        Parameter Xparameter = SelectXAxisParameter(context);
                        Parameter Yparameter = SelectYAxisParameter(Xparameter);
                        if (Xparameter != null && Yparameter != null)
                        {
                            int token = context.LocalScope.PushContext();
                            context.LocalScope.setSurfaceParameters(Xparameter, Yparameter);
                            retVal = createSurfaceForParameters(context, Xparameter, Yparameter, explain);
                            context.LocalScope.PopContext(token);
                        }
                    }
                    else
                    {
                        AddError("Wrong number of parameters for function " + FullName + " to create a surface");
                    }
                }
                catch (Exception e)
                {
                    AddError("Cannot create surface of function, reason : " + e.Message);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the surface which corresponds to the parameters provided
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public Surface getSurface(Parameter X, Parameter Y)
        {
            Surface retVal = null;

            if (Surface != null)
            {
                // TODO : Ensure parameters are OK
                retVal = Surface;
            }
            else if (Graph != null)
            {
                // TODO : Check parameters name for conversion to X surface (or not done here) Y surface
                Parameter parameter = (Parameter) FormalParameters[0];
                retVal = this.Graph.ToSurfaceX();
            }

            if (getCacheable())
            {
                Surface = retVal;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the graph</param>
        /// <param name="Xparameter">The parameter used for the X axis</param>
        /// <param name="Yparameter">The parameter used for the Y axis</param>
        /// <returns></returns>
        public virtual Surface createSurfaceForParameters(InterpretationContext context, Parameter Xparameter, Parameter Yparameter, ExplanationPart explain)
        {
            Surface retVal = Surface;

            if (retVal == null)
            {
                if (Xparameter != null)
                {
                    if (Yparameter != null)
                    {
                        if (Cases.Count > 0)
                        {
                            retVal = new Surface(Xparameter, Yparameter);

                            foreach (Case cas in Cases)
                            {
                                if (PreconditionSatisfied(context, cas, Xparameter, Yparameter, explain))
                                {
                                    Surface surface = cas.Expression.createSurface(context, Xparameter, Yparameter, explain);
                                    if (surface != null)
                                    {
                                        Parameter parameter = null;
                                        surface = ReduceSurface(context, cas, surface, out parameter, explain);

                                        if (parameter == null || parameter == surface.XParameter)
                                        {
                                            retVal.MergeX(surface);
                                        }
                                        else
                                        {
                                            retVal = Surface.MergeY(retVal, surface);
                                        }
                                    }
                                    else
                                    {
                                        AddError("Cannot create surface for expression " + cas.ExpressionText);
                                        retVal = null;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Graph != null)
                        {
                            // The function is defined by a graph
                            // Extend it to a surface
                            // TODO: Check the right parameter
                            retVal = Graph.ToSurfaceX();
                            retVal.XParameter = Xparameter;
                            retVal.YParameter = Yparameter;
                        }
                        else
                        {
                            AddError("cannot create surface for function " + Name + " with given parameters");
                        }
                    }
                    else
                    {
                        // Function with 1 parameter that ranges over the Xaxis
                        retVal = new Surface(Xparameter, Yparameter);
                        Graph graph = createGraphForParameter(context, Xparameter, explain);
                        foreach (Graph.Segment segment in graph.Segments)
                        {
                            Graph newGraph = Graph.createGraph(segment.Expression.v0);
                            Surface.Segment newSegment = new Surface.Segment(segment.Start, segment.End, newGraph);
                            retVal.AddSegment(newSegment);
                        }
                    }
                }
                else if (Yparameter != null)
                {
                    // Function with 1 parameter that ranges over the Yaxis
                    retVal = new Surface(Xparameter, Yparameter);
                    Graph graph = createGraphForParameter(context, Yparameter, explain);
                    Surface.Segment segment = new Surface.Segment(0, double.MaxValue, graph);
                    retVal.AddSegment(segment);
                }
                else
                {
                    AddError("Invalid parameters for surface creation");
                }
            }

            retVal.XParameter = Xparameter;
            retVal.YParameter = Yparameter;

            return retVal;
        }

        /// <summary>
        /// Indicates whether all preconditions are satisfied for a given case, ignoring expressions like x (y) <= xxx or x (y) >= xxx
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="cas">The case to evaluate</param>
        /// <param name="x">First parameter</param>
        /// <param name="y">Second parameter</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private bool PreconditionSatisfied(InterpretationContext context, Case cas, Parameter x, Parameter y, ExplanationPart explain)
        {
            bool retVal = true;

            foreach (PreCondition preCondition in cas.PreConditions)
            {
                if (!ExpressionBasedOnParameter(x, preCondition.Expression) && !ExpressionBasedOnParameter(y, preCondition.Expression))
                {
                    BoolValue boolValue = preCondition.Expression.GetValue(context, explain) as BoolValue;
                    if (boolValue == null)
                    {
                        throw new Exception("Cannot evaluate precondition " + preCondition.Name);
                    }
                    else if (!boolValue.Val)
                    {
                        retVal = false;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates that the list of segments spans to the full range (0 .. infinity)
        /// </summary>
        /// <param name="boudaries"></param>
        /// <returns></returns>
        private bool fullRange(List<Graph.Segment> boudaries)
        {
            return boudaries.Count == 1 && boudaries[0].Start == 0 && boudaries[0].End == double.MaxValue;
        }

        /// <summary>
        /// Reduces the X axis of the surface according to the preconditions of this case
        /// </summary>
        /// <param name="context">The context used to reduce the surface</param>
        /// <param name="cas">The case used to reduce the surface</param>
        /// <param name="surface">The surface to reduce</param>
        /// <param name="parameter">The parameter for which the reduction has been performed</param>
        /// <returns>The reduced surface</returns>
        private Surface ReduceSurface(InterpretationContext context, Case cas, Surface surface, out Parameter parameter, ExplanationPart explain)
        {
            Surface retVal;

            // Evaluate the axis
            parameter = null;
            foreach (PreCondition preCondition in cas.PreConditions)
            {
                List<Graph.Segment> boundaries = EvaluateBoundaries(context, preCondition, surface.XParameter, explain);
                if (boundaries.Count != 0 && !fullRange(boundaries))
                {
                    if (parameter != surface.YParameter)
                    {
                        parameter = surface.XParameter;
                    }
                    else
                    {
                        throw new Exception("Cannot reduce a graph on both X axis and Y axis on the same time (1)");
                    }
                }
                else
                {
                    boundaries = EvaluateBoundaries(context, preCondition, surface.YParameter, explain);
                    if (boundaries.Count != 0 && !fullRange(boundaries))
                    {
                        if (parameter != surface.XParameter)
                        {
                            parameter = surface.YParameter;
                        }
                        else
                        {
                            throw new Exception("Cannot reduce a graph on both X axis and Y axis on the same time (2)");
                        }
                    }
                }
            }

            if (parameter == surface.XParameter)
            {
                // Reduce the surface on the X axis
                retVal = new Surface(surface.XParameter, surface.YParameter);
                foreach (Surface.Segment segment in surface.Segments)
                {
                    retVal.AddSegment(new Surface.Segment(segment));
                }

                // Reduces the segments according to this preconditions
                foreach (PreCondition preCondition in cas.PreConditions)
                {
                    List<Graph.Segment> boundaries = EvaluateBoundaries(context, preCondition, surface.XParameter, explain);
                    retVal.Reduce(boundaries);
                }
            }
            else if (parameter == surface.YParameter)
            {
                // Reduce the surface, for all segments of the X axis, on the Y axis
                retVal = new Surface(surface.XParameter, surface.YParameter);
                foreach (Surface.Segment segment in surface.Segments)
                {
                    // Reduces the segments according to this preconditions
                    foreach (PreCondition preCondition in cas.PreConditions)
                    {
                        List<Graph.Segment> boundaries = EvaluateBoundaries(context, preCondition, surface.YParameter, explain);
                        segment.Graph.Reduce(boundaries);
                    }
                    if (!segment.Empty())
                    {
                        retVal.AddSegment(segment);
                    }
                }
            }
            else
            {
                retVal = surface;
            }

            return retVal;
        }

        /// <summary>
        /// Selects the X axis of the surface, according to the function cases
        /// </summary>
        /// <param name="context">The context used to create the surface</param>
        /// <returns>the X axus if the surface</returns>
        private Parameter SelectXAxisParameter(InterpretationContext context)
        {
            Parameter retVal = null;

            foreach (Case cas in Cases)
            {
                foreach (Parameter parameter in FormalParameters)
                {
                    foreach (PreCondition preCondition in cas.PreConditions)
                    {
                        if (ExpressionBasedOnParameter(parameter, preCondition.Expression))
                        {
                            if (retVal == null)
                            {
                                retVal = parameter;
                            }
                            else
                            {
                                if (retVal != parameter)
                                {
                                    AddError("Cannot create surface when mixed parameters are used in the function cases");
                                    return null;
                                }
                            }
                        }
                    }
                }
            }

            if (retVal == null)
            {
                retVal = (Parameter) FormalParameters[0];
            }

            return retVal;
        }

        /// <summary>
        /// Selects the Y axis of the surface, according to the X parameter
        /// </summary>
        /// <returns>the Y axis if the surface</returns>
        private Parameter SelectYAxisParameter(Parameter Xparameter)
        {
            Parameter retVal = (Parameter) FormalParameters[0];

            if (retVal == Xparameter)
            {
                retVal = (Parameter) FormalParameters[1];
            }

            return retVal;
        }

        /// <summary>
        /// Provides the surface associated to the function, if any
        /// </summary>
        private Surface surface;

        public Surface Surface
        {
            get { return surface; }
            set
            {
                surface = value;
                if (surface != null)
                {
                    surface.Function = this;
                }
            }
        }

        /// <summary>
        /// The cached value for the latests evaluation
        /// </summary>
        private IValue CachedValue = null;

        /// <summary>
        /// The cached results for this function
        /// </summary>
        private CurryCache CachedResult = null;

        /// <summary>
        /// Provides the average execution time of the function
        /// </summary>
        private int AverageExecutionTime
        {
            get
            {
                int retVal = 0;

                if (ExecutionCount > 0)
                {
                    retVal = (int) (ExecutionTimeInMilli/ExecutionCount);
                }

                return retVal;
            }
        }

        /// <summary>
        /// Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals"></param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public virtual IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals, ExplanationPart explain)
        {
            IValue retVal = null;

            bool useCache = EFSSystem.CacheFunctions;
            // Only use the cached value when the EFSSystem indicates that caches should be used
            // This condition has been added to handle the fact that the user changes the status 
            // of EFSSystem.CacheFunctions within a test session
            if (useCache)
            {
                retVal = CachedValue;

                if (retVal == null)
                {
                    if (CachedResult == null)
                    {
                        CachedResult = new CurryCache(this);
                    }

                    if (EFSSystem.CacheFunctions)
                    {
                        retVal = CachedResult.GetValue(actuals);
                    }
                }
            }

            if (retVal == null)
            {
                int token = context.LocalScope.PushContext();
                AssignParameters(context, actuals);
                if (Cases.Count > 0)
                {
                    // Statically defined function
                    foreach (Case aCase in Cases)
                    {
                        // Evaluate the function
                        ExplanationPart subExplanation = ExplanationPart.CreateSubExplanation(explain, "Case " + aCase.Name + " : ");
                        bool val = aCase.EvaluatePreConditions(context, subExplanation);
                        ExplanationPart.SetNamable(subExplanation, val ? EFSSystem.BoolType.True : EFSSystem.BoolType.False);
                        if (val)
                        {
                            retVal = aCase.Expression.GetValue(context, subExplanation);
                            break;
                        }
                    }
                }
                else if (Surface != null && FormalParameters.Count == 2)
                {
                    double x = 0.0;
                    double y = 0.0;
                    Parameter formal1 = (Parameter) FormalParameters[0];
                    Parameter formal2 = (Parameter) FormalParameters[1];
                    foreach (KeyValuePair<Actual, IValue> pair in actuals)
                    {
                        if (pair.Key.Parameter == formal1)
                        {
                            x = getDoubleValue(pair.Value);
                        }
                        if (pair.Key.Parameter == formal2)
                        {
                            y = getDoubleValue(pair.Value);
                        }
                    }
                    retVal = new DoubleValue(EFSSystem.DoubleType, Surface.Val(x, y));
                }
                else if (Graph != null && FormalParameters.Count < 2)
                {
                    if (FormalParameters.Count == 0)
                    {
                        retVal = new DoubleValue(EFSSystem.DoubleType, Graph.Val(0));
                    }
                    else if (FormalParameters.Count == 1)
                    {
                        double x = 0.0;
                        Parameter formal = (Parameter) FormalParameters[0];
                        foreach (KeyValuePair<Actual, IValue> pair in actuals)
                        {
                            if (pair.Key.Parameter == formal)
                            {
                                x = getDoubleValue(pair.Value);
                            }
                        }
                        retVal = new DoubleValue(EFSSystem.DoubleType, Graph.Val(x));
                    }
                }
                context.LocalScope.PopContext(token);

                ExplanationPart.SetNamable(explain, retVal);
                if (useCache)
                {
                    if (actuals.Count == 0)
                    {
                        CachedValue = retVal;
                    }
                    else
                    {
                        CachedResult.SetValue(actuals, retVal);
                    }
                }
            }
            else
            {
                ExplanationPart subExplain = ExplanationPart.CreateSubExplanation(explain, "Cached result = ");
                ExplanationPart.SetNamable(subExplain, retVal);
            }

            return retVal;
        }

        /// <summary>
        /// Creates the graph for a value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Graph createGraphForValue(IValue value)
        {
            Graph retVal = new Graph();

            double val = getDoubleValue(value);
            retVal.addSegment(new Graph.Segment(0, double.MaxValue, new Graph.Segment.Curve(0.0, val, 0.0)));

            return retVal;
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();
            foreach (Parameter parameter in FormalParameters)
            {
                ISubDeclaratorUtils.AppendNamable(this, parameter);
            }
        }

        /// <summary>
        /// Provides all the parameters declared for this function
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        /// Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        /// Provides an explanation of the function's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            // Creates the function header
            retVal += TextualExplainUtilities.Pad("{ {\\b FUNCTION} " + Name + "(", indentLevel);
            if (FormalParameters.Count > 0)
            {
                bool first = true;
                foreach (Parameter parameter in FormalParameters)
                {
                    if (!first)
                    {
                        retVal += ",";
                    }
                    retVal = retVal + "\\par" + TextualExplainUtilities.Pad(parameter.Name + " : " + parameter.TypeName, indentLevel + 2);
                }
                retVal += "\\par";
                retVal += TextualExplainUtilities.Pad(") {\\b RETURNS } { \\cf2" + TypeName + "}}\\par", indentLevel);
            }
            else
            {
                retVal += ")";
                retVal += TextualExplainUtilities.Pad("{\\b RETURNS } { \\cf2" + TypeName + "}}\\par", indentLevel);
            }

            {
                bool first = true;
                foreach (Case cas in Cases)
                {
                    if (!first)
                    {
                        retVal = retVal + TextualExplainUtilities.Pad("{\\b ELSE }", indentLevel);
                    }
                    retVal += cas.getExplain(indentLevel + 2) + "\\par ";
                    first = false;
                }
            }

            retVal += TextualExplainUtilities.Pad("{ {\\b END FUNCTION } ", indentLevel);


            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the rule's behaviour
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public override string getExplain(bool explainSubElements)
        {
            string retVal = getExplain(0);

            return TextualExplainUtilities.Encapsule(retVal);
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                Case item = element as Case;
                if (item != null)
                {
                    appendCases(item);
                }
            }
            {
                Parameter item = element as Parameter;
                if (item != null)
                {
                    appendParameters(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        /// Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public virtual void additionalChecks(ModelElement root, InterpretationContext context, Dictionary<string, Expression> actualParameters)
        {
        }

        /// <summary>
        /// Provides the double value according to the value provided
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double getDoubleValue(IValue value)
        {
            double retVal = 0;

            if (!(value is EmptyValue))
            {
                EnumValue enumValue = value as EnumValue;
                if (enumValue != null)
                {
                    value = enumValue.Value;
                }

                IntValue intValue = value as IntValue;
                if (intValue != null)
                {
                    retVal = (double) intValue.Val;
                }
                else
                {
                    DoubleValue doubleValue = value as DoubleValue;

                    if (doubleValue != null)
                    {
                        retVal = doubleValue.Val;
                    }
                    else if (value != null)
                    {
                        throw new Exception("Value " + value.Name + " cannot be converted to double");
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the parameter whose name matches the string provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Parameter findParameter(string name)
        {
            Parameter retVal = null;

            foreach (Parameter parameter in FormalParameters)
            {
                if (parameter.Name.CompareTo(name) == 0)
                {
                    retVal = parameter;
                    break;
                }
            }

            return retVal;
        }

        public bool Reads(ITypedElement variable)
        {
            bool retVal = false;

            foreach (Case cas in Cases)
            {
                if (cas.Read(variable))
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        public List<IValue> GetLiterals()
        {
            List<IValue> retVal = new List<IValue>();

            foreach (Case cas in Cases)
            {
                retVal.AddRange(cas.GetLiterals());
            }

            return retVal;
        }

        /// <summary>
        /// Clears the caches for this function
        /// </summary>
        public override void ClearCache()
        {
            base.ClearCache();

            CachedValue = null;
            CachedResult = null;

            Graph = null;
            Surface = null;
        }

        /// <summary>
        /// Converts a structure value to its corresponding structure expression.
        /// null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public string ToExpressionWithDefault()
        {
            return "";
        }
    }
}