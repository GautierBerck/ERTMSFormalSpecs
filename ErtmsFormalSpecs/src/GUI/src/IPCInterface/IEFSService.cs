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
namespace GUI.IPCInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;

    /// <summary>
    /// The cycle priority to execute
    /// </summary>
    public enum Priority { Verification, UpdateInternal, Process, UpdateOutput, CleanUp };

    [ServiceContract]
    public interface IEFSService
    {
        /// <summary>
        /// Indicates that the explain view should be updated according to the scenario execution
        /// </summary>        
        bool Explain
        {
            [OperationContract]
            set;
        }

        /// <summary>
        /// Indicates that the events should be logged
        /// </summary>
        bool LogEvents
        {
            [OperationContract]
            set;
        }

        /// <summary>
        /// The duration (in ms) of an execution cycle
        /// </summary>
        int CycleDuration
        {
            [OperationContract]
            set;
        }

        /// <summary>
        /// The number of events that should be kept in memory
        /// </summary>
        int KeepEventCount
        {
            [OperationContract]
            set;
        }

        /// <summary>
        /// Provides the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        [OperationContract]
        Values.Value GetVariableValue(string variableName);

        /// <summary>
        /// Sets the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        [OperationContract]
        void SetVariableValue(string variableName, Values.Value value);

        /// <summary>
        /// Activates the execution of a single cycle, as the given priority level
        /// </summary>
        /// <param name="priority"></param>
        [OperationContract]
        void Cycle(Priority priority);
    }
}
