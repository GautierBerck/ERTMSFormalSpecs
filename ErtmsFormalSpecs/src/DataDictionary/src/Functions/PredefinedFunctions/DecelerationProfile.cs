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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using ErtmsSolutions.Etcs.Subset26.BrakingCurves;
using ErtmsSolutions.SiUnits;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Creates a new function which describes the maximum value of two functions
    /// </summary>
    public class DecelerationProfile : FunctionOnGraph
    {
        /// <summary>
        ///     The MRSP
        /// </summary>
        public Parameter SpeedRestrictions { get; private set; }

        /// <summary>
        ///     The deceleration factor
        /// </summary>
        public Parameter DecelerationFactor { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public DecelerationProfile(EfsSystem efsSystem)
            : base(efsSystem, "DecelerationProfile")
        {
            SpeedRestrictions = (Parameter) acceptor.getFactory().createParameter();
            SpeedRestrictions.Name = "SpeedRestrictions";
            SpeedRestrictions.Type = EFSSystem.AnyType;
            SpeedRestrictions.setFather(this);
            FormalParameters.Add(SpeedRestrictions);

            DecelerationFactor = (Parameter) acceptor.getFactory().createParameter();
            DecelerationFactor.Name = "DecelerationFactor";
            DecelerationFactor.Type = EFSSystem.AnyType;
            DecelerationFactor.setFather(this);
            FormalParameters.Add(DecelerationFactor);
        }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public override void AdditionalChecks(ModelElement root, InterpretationContext context,
            Dictionary<string, Expression> actualParameters)
        {
            CheckFunctionalParameter(root, context, actualParameters[SpeedRestrictions.Name], 1);
            CheckFunctionalParameter(root, context, actualParameters[DecelerationFactor.Name], 2);
        }

        /// <summary>
        ///     Provides the graph of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the graph</param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = null;

            Graph mrspGraph = null;
            Function speedRestriction = context.FindOnStack(SpeedRestrictions).Value as Function;
            if (speedRestriction != null)
            {
                Parameter p = (Parameter) speedRestriction.FormalParameters[0];

                int token = context.LocalScope.PushContext();
                context.LocalScope.SetGraphParameter(p);
                mrspGraph = createGraphForValue(context, context.FindOnStack(SpeedRestrictions).Value, explain, p);
                context.LocalScope.PopContext(token);
            }

            if (mrspGraph != null)
            {
                Function deceleratorFactor = context.FindOnStack(DecelerationFactor).Value as Function;
                if (deceleratorFactor != null)
                {
                    Surface decelerationSurface = deceleratorFactor.CreateSurface(context, explain);
                    if (decelerationSurface != null)
                    {
                        FlatSpeedDistanceCurve mrspCurve = mrspGraph.FlatSpeedDistanceCurve(mrspGraph.ExpectedEndX());
                        AccelerationSpeedDistanceSurface accelerationSurface =
                            decelerationSurface.createAccelerationSpeedDistanceSurface(double.MaxValue, double.MaxValue);
                        QuadraticSpeedDistanceCurve brakingCurve = null;
                        try
                        {
                            brakingCurve = EtcsBrakingCurveBuilder.Build_A_Safe_Backward(accelerationSurface, mrspCurve);
                        }
                        catch (Exception)
                        {
                            retVal = new Graph();
                            retVal.AddSegment(new Graph.Segment(0, double.MaxValue, new Graph.Segment.Curve(0, 0, 0)));
                        }

                        if (brakingCurve != null)
                        {
                            retVal = new Graph();

                            // TODO : Remove the distinction between linear curves and quadratic curves
                            bool isLinear = true;
                            for (int i = 0; i < brakingCurve.SegmentCount; i++)
                            {
                                QuadraticCurveSegment segment = brakingCurve[i];
                                if (segment.A.ToUnits() != 0.0 || segment.V0.ToUnits() != 0.0)
                                {
                                    isLinear = false;
                                    break;
                                }
                            }

                            for (int i = 0; i < brakingCurve.SegmentCount; i++)
                            {
                                QuadraticCurveSegment segment = brakingCurve[i];

                                Graph.Segment newSegment;
                                if (isLinear)
                                {
                                    newSegment = new Graph.Segment(
                                        segment.X.X0.ToUnits(),
                                        segment.X.X1.ToUnits(),
                                        new Graph.Segment.Curve(0.0,
                                            segment.V0.ToSubUnits(SiSpeed_SubUnits.KiloMeter_per_Hour), 0.0));
                                }
                                else
                                {
                                    newSegment = new Graph.Segment(
                                        segment.X.X0.ToUnits(),
                                        segment.X.X1.ToUnits(),
                                        new Graph.Segment.Curve(
                                            segment.A.ToSubUnits(SiAcceleration_SubUnits.Meter_per_SecondSquare),
                                            segment.V0.ToSubUnits(SiSpeed_SubUnits.KiloMeter_per_Hour),
                                            segment.D0.ToSubUnits(SiDistance_SubUnits.Meter)
                                            )
                                        );
                                }
                                retVal.AddSegment(newSegment);
                            }
                        }
                    }
                    else
                    {
                        DecelerationFactor.AddError("Cannot create surface for " + DecelerationFactor);
                    }
                }
                else
                {
                    DecelerationFactor.AddError("Cannot evaluate " + DecelerationFactor + " as a function");
                }
            }
            else
            {
                SpeedRestrictions.AddError("Cannot create graph for " + SpeedRestrictions);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            Function function = (Function) acceptor.getFactory().createFunction();
            function.Name = "DecelerationProfile ( SpeedRestrictions => " + getName(SpeedRestrictions) +
                            ", DecelerationFactor => " + getName(DecelerationFactor) + ")";
            function.Enclosing = EFSSystem;
            Parameter parameter = (Parameter) acceptor.getFactory().createParameter();
            parameter.Name = "X";
            parameter.Type = EFSSystem.DoubleType;
            function.appendParameters(parameter);
            function.ReturnType = EFSSystem.DoubleType;
            function.Graph = CreateGraph(context, parameter, explain);

            retVal = function;
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}