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
using System.Collections.Generic;
using System.Linq;
using DataDictionary.Variables;

namespace DataDictionary.Tests.Runner.Events
{
    public class EventTimeLine
    {
        /// <summary>
        /// The current time
        /// </summary>
        private double currentTime;
        public double CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        /// <summary>
        /// The list of events handled by this time line
        /// </summary>
        private List<ModelEvent> modelEvents = new List<ModelEvent>();
        public List<ModelEvent> Events
        {
            get { return modelEvents; }
            set { modelEvents = value; }
        }

        /// <summary>
        /// Provides the maximum number of events that are stored in the time-line.
        /// Expectations do not count in this number
        /// </summary>
        public int MaxNumberOfEvents { get; set; }

        /// <summary>
        /// Keeps track of step activation
        /// </summary>
        private Dictionary<SubStep, SubStepActivated> subStepActivationCache = new Dictionary<SubStep, SubStepActivated>();
        internal Dictionary<SubStep, SubStepActivated> SubStepActivationCache
        {
            get { return subStepActivationCache; }
        }

        /// <summary>
        /// The expectations currently active
        /// </summary>
        private HashSet<Expect> activeExpectations = new HashSet<Expect>();
        public HashSet<Expect> ActiveExpectations
        {
            get { return activeExpectations; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EventTimeLine()
        {
            CurrentTime = 0;
            MaxNumberOfEvents = 10000;
        }

        /// <summary>
        /// Adds a new event in the list of events
        /// </summary>
        /// <param name="modelEvent"></param>
        public void AddModelEvent(ModelEvent modelEvent, bool log)
        {
            modelEvent.Time = CurrentTime;
            modelEvent.TimeLine = this;
            modelEvents.Add(modelEvent);
            modelEvent.Apply(log);
        }

        /// <summary>
        /// Provides the still active and blocking expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<Expect> ActiveBlockingExpectations()
        {
            HashSet<Expect> retVal = new HashSet<Expect>();

            foreach (Expect expect in ActiveExpectations)
            {
                if (expect.State == DataDictionary.Tests.Runner.Events.Expect.EventState.Active && expect.Expectation.Blocking)
                {
                    retVal.Add(expect);
                }
            }

            return retVal;
        }


        /// <summary>
        /// Provides the failed expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<ModelEvent> FailedExpectations()
        {
            HashSet<ModelEvent> retVal = new HashSet<ModelEvent>();

            foreach (ModelEvent modelEvent in Events)
            {
                Expect expect = modelEvent as Expect;
                if ((expect != null) && expect.State == DataDictionary.Tests.Runner.Events.Expect.EventState.TimeOut)
                {
                    retVal.Add(expect);
                }
                ModelInterpretationFailure failure = modelEvent as ModelInterpretationFailure;
                if (failure != null)
                {
                    retVal.Add(failure);
                }
            }

            return retVal;
        }
        /// <summary>
        /// Provides all the events between two time bounds
        /// </summary>
        /// <param name="fromTime">the initial time bound</param>
        /// <param name="toTime">the final time bound</param>
        /// <returns></returns>
        public List<ModelEvent> GetEventsInRange(uint fromTime, uint toTime)
        {
            List<ModelEvent> result = new List<ModelEvent>();

            foreach (ModelEvent e in Events)
            {
                if (e.Time >= fromTime && e.Time < toTime)
                {
                    result.Add(e);
                }
            }

            return result;
        }

        /// <summary>
        /// Steps one step backward
        /// </summary>
        /// <param name="time">the time to step back</param>
        public void StepBack(double time)
        {
            currentTime = currentTime - time;
            if (currentTime < 0)
            {
                currentTime = 0;
            }

            while (Events.Count > 0)
            {
                ModelEvent evt = Events.Last();
                if (evt.Time < currentTime)
                {
                    break;
                }
                else
                {
                    evt.RollBack();
                    Events.Remove(evt);
                }
            }
        }

        /// <summary>
        /// Indicates whether the time line holds the specific model event
        /// </summary>
        /// <param name="modelEvent"></param>
        /// <returns></returns>
        public bool Contains(ModelEvent modelEvent)
        {
            return Events.Contains(modelEvent);
        }

        /// <summary>
        /// Indicates whether the corresponding step has been activated
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        internal bool ContainsStep(Step step)
        {
            bool retVal = false;

            foreach (SubStep subStep in step.SubSteps)
            {
                if (SubStepActivationCache.ContainsKey(subStep))
                {
                    retVal = SubStepActivationCache[subStep] != null;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gives the time of activation of a sub-step
        /// </summary>
        /// <param name="subStep"></param>
        /// <returns>True if the provided rule has been activated</returns>
        public double GetSubStepActivationTime(SubStep subStep)
        {
            double retVal = -1;

            if (SubStepActivationCache.ContainsKey(subStep))
            {
                SubStepActivated subStepActivated = SubStepActivationCache[subStep];
                if (subStepActivated != null)
                {
                    retVal = subStepActivated.Time;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gives the time of activation of a sub-step
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="time"></param>
        /// <returns>True if the provided rule has been activated</returns>
        public double GetNextSubStepActivationTime(SubStep aSubStep)
        {
            double retVal = -1;
            bool stepFound = false;

            foreach (ModelEvent modelEvent in Events)
            {
                if (modelEvent is SubStepActivated)
                {
                    SubStepActivated subStepActivated = modelEvent as SubStepActivated;
                    if (stepFound)
                    {
                        retVal = subStepActivated.Time;
                        break;
                    }
                    else
                    {
                        if (subStepActivated.SubStep == aSubStep)
                        {
                            stepFound = true;
                        }
                    }
                }
            }

            if (retVal == -1)
            {
                retVal = CurrentTime;
            }

            return retVal;
        }

        /// <summary>
        /// Gets the set of activated rules
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public HashSet<DataDictionary.Rules.RuleCondition> GetActivatedRules()
        {
            HashSet<DataDictionary.Rules.RuleCondition> retVal = new HashSet<DataDictionary.Rules.RuleCondition>();

            foreach (ModelEvent modelEvent in Events)
            {
                if (modelEvent is RuleFired)
                {
                    RuleFired ruleFired = modelEvent as RuleFired;
                    retVal.Add(ruleFired.RuleCondition);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the list of activated rules within the interval [start, end]
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<DataDictionary.Rules.RuleCondition> GetActivatedRulesInRange(double start, double end)
        {
            List<DataDictionary.Rules.RuleCondition> retVal = new List<DataDictionary.Rules.RuleCondition>();

            foreach (ModelEvent modelEvent in Events)
            {
                if (modelEvent is RuleFired && (modelEvent.Time >= start && modelEvent.Time < end))
                {
                    RuleFired ruleFired = modelEvent as RuleFired;
                    retVal.Add(ruleFired.RuleCondition);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates whether a rule condition has been activated at the time provided
        /// </summary>
        /// <param name="ruleCondition">The rule condition that should be activated</param>
        /// <param name="time">the time when the rule condition should be activated</param>
        /// <param name="variable">The variable impacted by this rule condition, if any</param>
        /// <returns>True if the provided rule condition has been activated</returns>
        public bool RuleActivatedAtTime(Rules.RuleCondition ruleCondition, double time, IVariable variable)
        {
            bool retVal = false;

            if (variable != null)
            {
                foreach (ModelEvent modelEvent in Events)
                {
                    RuleFired ruleFired = modelEvent as RuleFired;
                    if (modelEvent.Time == time && ruleFired != null)
                    {
                        if (ruleFired.RuleCondition == ruleCondition)
                        {
                            retVal = ruleFired.ImpactVariable(variable);

                            if (retVal)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates whether a step holds active expectations
        /// </summary>
        /// <returns></returns>
        public bool StepActive(Step step)
        {
            bool retVal = false;

            foreach (Expect expect in ActiveExpectations)
            {
                if (expect.Expectation.Step == step)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Cleans up the time line by removing the events exceeding the maximum number of events in the time line, 
        /// not counting the Expects.
        /// </summary>
        public void GarbageCollect()
        {
            int elementCount = Events.Count - MaxNumberOfEvents;
            if (elementCount > 0)
            {
                // Builds a new list of events
                List<ModelEvent> newEvents = new List<ModelEvent>();

                foreach (ModelEvent modelEvent in Events)
                {
                    if (modelEvent is Expect || modelEvent is ModelInterpretationFailure)
                    {
                        // We keep expectations in the event list
                        newEvents.Add(modelEvent);
                    }
                    else
                    {
                        // Keep only the last events in the event list
                        if (elementCount > 0)
                        {
                            elementCount -= 1;
                        }
                        else
                        {
                            newEvents.Add(modelEvent);
                        }
                    }
                }

                Events = newEvents;
            }
        }
    }
}
