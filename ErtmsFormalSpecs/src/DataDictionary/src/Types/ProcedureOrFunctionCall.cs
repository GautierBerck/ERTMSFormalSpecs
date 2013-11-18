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
namespace DataDictionary.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProcedureOrFunctionCall : IGraphicalArrow<NameSpace>, IComparable<ProcedureOrFunctionCall>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="call"></param>
        public ProcedureOrFunctionCall(NameSpace source, NameSpace target, ModelElement call)
        {
            Source = source;
            Target = target;
            ReferencedModel = call;
        }

        /// <summary>
        /// The source of the arrow
        /// </summary>
        public NameSpace Source { get; private set; }

        /// <summary>
        /// Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public void SetInitialBox(IGraphicalDisplay initialBox)
        {
            // We cannot change a call through this
        }

        /// <summary>
        /// The target of the arrow
        /// </summary>
        public NameSpace Target { get; private set; }

        /// <summary>
        /// Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public void SetTargetBox(IGraphicalDisplay targetBox)
        {
            // We cannot change a call through this
        }

        /// <summary>
        /// The name to be displayed
        /// </summary>
        public string GraphicalName { get { return ReferencedModel.Name; } }

        /// <summary>
        /// The model element which is referenced by this arrow
        /// </summary>
        public ModelElement ReferencedModel { get; private set; }

        // Summary:
        //     Compares the current object with another object of the same type.
        //
        // Parameters:
        //   other:
        //     An object to compare with this object.
        //
        // Returns:
        //     A value that indicates the relative order of the objects being compared.
        //     The return value has the following meanings: Value Meaning Less than zero
        //     This object is less than the other parameter.Zero This object is equal to
        //     other. Greater than zero This object is greater than other.
        public int CompareTo(ProcedureOrFunctionCall other)
        {
            int retVal = 0;

            if (retVal == 0)
            {
                retVal = ReferencedModel.CompareTo(other.ReferencedModel);
            }

            if (retVal == 0)
            {
                retVal = Source.CompareTo(other.Source);
            }

            if (retVal == 0)
            {
                retVal = Target.CompareTo(other.Target);
            }

            return retVal;
        }

    }
}
