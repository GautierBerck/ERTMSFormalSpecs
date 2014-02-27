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

namespace DataDictionary.Types
{
    public class StateMachine : Generated.StateMachine, IEnumerateValues, Utils.ISubDeclarator, Utils.IFinder
    {
        public override string FullName
        {
            get
            {
                string retVal = "";

                StateMachine current = this;
                while (current.EnclosingStateMachine != null)
                {
                    if (string.IsNullOrEmpty(retVal))
                    {
                        retVal = current.EnclosingState.Name;
                    }
                    else
                    {
                        retVal = current.EnclosingState.Name + "." + retVal;
                    }
                    current = current.EnclosingStateMachine;
                }

                // Current.EnclosingStateMachine is null
                if (string.IsNullOrEmpty(retVal))
                {
                    retVal = ((Utils.INamable)current.Enclosing).FullName + "." + current.Name;
                }
                else
                {
                    retVal = ((Utils.INamable)current.Enclosing).FullName + "." + current.Name + "." + retVal;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Indicates if this StateMachine contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (DataDictionary.Rules.Rule rule in Rules)
                {
                    if (rule.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StateMachine()
        {
            Utils.FinderRepository.INSTANCE.Register(this);
        }

        /// <summary>
        /// The states 
        /// </summary>
        public System.Collections.ArrayList States
        {
            get
            {
                if (allStates() == null)
                {
                    setAllStates(new System.Collections.ArrayList());
                }
                return allStates();
            }
        }

        /// <summary>
        /// Gets the state which name corresponds to the image provided
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override Values.IValue getValue(string image)
        {
            Values.IValue retVal = null;

            foreach (Constants.State state in States)
            {
                if (state.Name.CompareTo(image) == 0)
                {
                    retVal = state;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// The rules
        /// </summary>
        public System.Collections.ArrayList Rules
        {
            get
            {
                if (allRules() == null)
                {
                    setAllRules(new System.Collections.ArrayList());
                }
                return allRules();
            }
        }

        /// <summary>
        /// Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public Values.IValue findValue(string[] names, int index)
        {
            Constants.State retVal = null;

            if (index < names.Length)
            {
                retVal = (Constants.State)Utils.INamableUtils.findByName(names[index], States); ;

                if (retVal != null && index < names.Length - 1)
                {
                    StateMachine stateMachine = retVal.StateMachine;
                    if (stateMachine != null)
                    {
                        retVal = (Constants.State)stateMachine.findValue(names, index + 1);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the state which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Constants.State findState(string name)
        {
            Constants.State retVal = (Constants.State)findValue(name.Split('.'), 0);

            if (retVal == null)
            {
                Log.Error("Cannot find state " + name);
            }

            return retVal;
        }

        public Constants.State EnclosingState
        {
            get { return Enclosing as Constants.State; }
        }

        public NameSpace EnclosingNameSpace
        {
            get { return Enclosing as NameSpace; }
        }

        public override void Delete()
        {
            if (EnclosingState != null)
            {
                EnclosingState.StateMachine = null;
            }
            else if (EnclosingNameSpace != null)
            {
                EnclosingNameSpace.StateMachines.Remove(this);
            }
            else
            {
                Structure structure = Enclosing as Structure;
                if (structure != null)
                {
                    structure.StateMachines.Remove(this);
                }
            }
        }

        public override System.Collections.ArrayList EnclosingCollection
        {
            get
            {
                System.Collections.ArrayList retVal = base.EnclosingCollection;

                if (EnclosingNameSpace != null)
                {
                    retVal = EnclosingNameSpace.Structures;
                }
                else
                {
                    Structure structure = Enclosing as Structure;
                    if (structure != null)
                    {
                        retVal = structure.StateMachines;
                    }
                }

                return retVal;
            }
        }

        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            foreach (Constants.State state in this.States)
            {
                state.Constants(scope, retVal);
            }
        }

        public void ClearCache()
        {
            cachedValues = null;
            DeclaredElements = null;
        }

        /// <summary>
        /// Provides the set of states available in this state machine 
        /// </summary>
        public List<Values.IValue> cachedValues;
        public List<Values.IValue> AllValues
        {
            get
            {
                if (cachedValues == null)
                {
                    cachedValues = new List<Values.IValue>();

                    foreach (Constants.State state in StateFinder.INSTANCE.find(this))
                    {
                        cachedValues.Add(state);
                    }
                }

                return cachedValues;
            }
        }

        public override bool Contains(Values.IValue first, Values.IValue other)
        {
            bool retVal = false;

            Constants.State state1 = first as Constants.State;
            Constants.State state2 = other as Constants.State;
            if (state1 != null && state2 != null)
            {
                if (state1.Type == state2.Type)
                {
                    Constants.State current = state2;
                    while (current != null & retVal == false)
                    {
                        retVal = (current == state1);
                        current = current.EnclosingState;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Initialises the declared elements 
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<Utils.INamable>>();

            foreach (Constants.State state in States)
            {
                Utils.ISubDeclaratorUtils.AppendNamable(this, state);
            }
        }

        /// <summary>
        /// Provides all the states that can be stored in this state machine
        /// </summary>
        public Dictionary<string, List<Utils.INamable>> DeclaredElements { get; set; }

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
        /// Provides the states used in an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static List<Constants.State> GetStates(Interpreter.Expression expression)
        {
            List<Constants.State> retval = new List<Constants.State>();

            if (expression != null)
            {
                foreach (Values.IValue value in expression.GetLiterals())
                {
                    Constants.State state = value as Constants.State;
                    if (state != null)
                    {
                        retval.Add(state);
                    }
                }

                Interpreter.Call call = expression as Interpreter.Call;
                if (call != null)
                {
                    Functions.Function function = call.Called.getStaticCallable() as Functions.Function;
                    if (function != null)
                    {
                        foreach (Values.IValue value in function.GetLiterals())
                        {
                            Constants.State state = value as Constants.State;
                            if (state != null)
                            {
                                retval.Add(state);
                            }
                        }
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// This class is used to find all transitions in the model
        /// </summary>
        private class TransitionFinder : Generated.Visitor
        {
            /// <summary>
            /// The transitions currently found
            /// </summary>
            private List<Rules.Transition> transitions = new List<Rules.Transition>();
            public List<Rules.Transition> Transitions
            {
                get { return transitions; }
            }

            /// <summary>
            /// The state machine for which this transition creator has been created
            /// </summary>
            private StateMachine stateMachine;
            public StateMachine StateMachine
            {
                get { return stateMachine; }
                private set
                {
                    stateMachine = value;
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="stateMachine"></param>
            public TransitionFinder(StateMachine stateMachine)
            {
                Utils.FinderRepository.INSTANCE.ClearCache();
                Transitions.Clear();
                StateMachine = stateMachine;
                Constants.State initialState = StateMachine.DefaultValue as Constants.State;
                if (initialState != null)
                {
                    Transitions.Add(new Rules.Transition(null, null, null, initialState));
                }
            }

            /// <summary>
            /// Check if this rule corresponds to a transition for this state machine
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.RuleCondition obj, bool visitSubNodes)
            {
                Rules.RuleCondition ruleCondition = (Rules.RuleCondition)obj;

                foreach (Rules.Action action in ruleCondition.Actions)
                {
                    foreach (Interpreter.Statement.VariableUpdateStatement update in action.UpdateStatements)
                    {
                        Types.Type targetType = update.TargetType;
                        if (targetType is StateMachine)
                        {
                            Interpreter.Expression expressionTree = update.Expression;
                            if (expressionTree != null)
                            {
                                // HaCK: This is a bit rough, but should be sufficient for now...
                                foreach (Constants.State stt1 in GetStates(expressionTree))
                                {
                                    // TargetState is the target state either in this state machine or in a sub state machine
                                    Constants.State targetState = StateMachine.StateInThisStateMachine(stt1);

                                    int transitionCount = Transitions.Count;
                                    bool filteredOut = false;

                                    // Finds the enclosing state of this action to determine the source state of this transition
                                    Constants.State enclosingState = Utils.EnclosingFinder<Constants.State>.find(action);
                                    if (enclosingState != null)
                                    {
                                        filteredOut = filteredOut || AddTransition(update, stt1, null, enclosingState);
                                    }

                                    if (!filteredOut)
                                    {
                                        foreach (Rules.PreCondition preCondition in ruleCondition.AllPreConditions)
                                        {
                                            // A transition from one state to another has been found
                                            foreach (Constants.State stt2 in GetStates(preCondition.Expression))
                                            {
                                                filteredOut = filteredOut || AddTransition(update, stt1, preCondition, stt2);
                                            }
                                        }
                                    }

                                    if (Transitions.Count == transitionCount)
                                    {
                                        if (targetState == stt1 && targetState.EnclosingStateMachine == StateMachine)
                                        {
                                            if (!filteredOut)
                                            {
                                                // No precondition could be found => one can reach this state at anytime
                                                Transitions.Add(new Rules.Transition(null, null, update, targetState));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                action.AddError("Cannot parse expression");
                            }
                        }
                    }
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            /// Adds a transition in the transitions sets
            /// </summary>
            /// <param name="update">The update state which provides the target of the transition</param>
            /// <param name="target">The target state, as determined by the update statement</param>
            /// <param name="filteredOut"></param>
            /// <param name="preCondition">the precondition (if any) which is used to determine the initial state</param>
            /// <param name="initial">The initial state</param>
            /// <returns>true if the transition has been filtered out. A transition can be filtered out if the target state is equal to the initial state or the initial state is null
            /// </returns>
            private bool AddTransition(Interpreter.Statement.VariableUpdateStatement update, Constants.State target, Rules.PreCondition preCondition, Constants.State initial)
            {
                bool retVal = false;

                if (SameParentStateMachine(initial, target))
                {
                    Constants.State initialState = StateMachine.StateInThisStateMachine(initial);
                    // TargetState is the target state either in this state machine or in a sub state machine
                    Constants.State targetState = StateMachine.StateInThisStateMachine(target);

                    // Determine the rule condition (if any) related to this state machine
                    Rules.RuleCondition condition = null;
                    if (update != null)
                    {
                        Rules.Action action = update.Root as Rules.Action;
                        condition = action.RuleCondition;
                    }

                    if (targetState != null || initialState != null)
                    {
                        // This transition is about this state machine.
                        if (initialState != targetState && initialState != null)
                        {
                            // Check that the transition is not yet present
                            // This case can occur when the same RuleCondition references two different states 
                            // in a substate machine. Only draws the transition once.
                            if (!findMatchingTransition(condition, initialState, targetState))
                            {
                                Transitions.Add(new Rules.Transition(preCondition, initialState, update, targetState));
                            }
                        }
                        else
                        {
                            if (initialState == initial)
                            {
                                retVal = true;
                            }
                        }
                    }
                }

                return retVal;
            }

            /// <summary>
            /// Indicates that the two states have the same outermost state machine
            /// </summary>
            /// <param name="state1"></param>
            /// <param name="state2"></param>
            /// <returns></returns>
            private bool SameParentStateMachine(Constants.State state1, Constants.State state2)
            {
                return GetParentStateMachine(state1) == GetParentStateMachine(state2);
            }

            /// <summary>
            /// Provides the outermost state machine enclosing the state provided
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            private StateMachine GetParentStateMachine(Constants.State state)
            {
                StateMachine retVal = state.EnclosingStateMachine;

                while (retVal.EnclosingStateMachine != null)
                {
                    retVal = retVal.EnclosingStateMachine;
                }

                return retVal;
            }

            /// <summary>
            /// Finds a transition which matches the initial state, target state and rule condition in the existing transitions
            /// </summary>
            /// <param name="condition"></param>
            /// <param name="initialState"></param>
            /// <param name="targetState"></param>
            /// <returns></returns>
            private bool findMatchingTransition(Rules.RuleCondition condition, Constants.State initialState, Constants.State targetState)
            {
                bool retVal = false;

                foreach (Rules.Transition t in Transitions)
                {
                    if (t.RuleCondition == condition && t.Source == initialState && t.Target == targetState)
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// Provides the transitions associated to this state machine, based on the underlying rules
        /// </summary>
        public List<Rules.Transition> Transitions
        {
            get
            {
                TransitionFinder finder = new TransitionFinder(this);
                foreach (Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    finder.visit(dictionary);
                }
                return finder.Transitions;
            }
        }

        /// <summary>
        /// Indicates that the state machine contains (either directly or indirectly) the state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal Constants.State StateInThisStateMachine(DataDictionary.Constants.State state)
        {
            Constants.State retVal = null;

            foreach (Constants.State other in States)
            {
                if (other == state)
                {
                    retVal = state;
                    break;
                }

                retVal = other.StateMachine.StateInThisStateMachine(state);
                if (retVal != null)
                {
                    retVal = other;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the enclosing state machine, if any
        /// </summary>
        public StateMachine EnclosingStateMachine
        {
            get
            {
                StateMachine retVal = null;

                if (EnclosingState != null)
                {
                    retVal = EnclosingState.EnclosingStateMachine;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            StateMachine current = otherType as StateMachine;
            while (current != null && !retVal)
            {
                retVal = this == current;
                current = current.EnclosingStateMachine;
            }

            return retVal;
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            {
                Constants.State item = element as Constants.State;
                if (item != null)
                {
                    appendStates(item);
                }
            }
            {
                Rules.Rule item = element as Rules.Rule;
                if (item != null)
                {
                    appendRules(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        /// Instanciates this state machine for the instanciation of a StructureProcedure into a Procedure
        /// </summary>
        /// <returns></returns>
        public StateMachine instanciate()
        {
            StateMachine retVal = (StateMachine)Generated.acceptor.getFactory().createStateMachine();
            retVal.Name = Name;
            retVal.setFather(getFather());
            retVal.Default = Default;
            foreach (Constants.State state in States)
            {
                Constants.State newState = state.duplicate();
                retVal.appendStates(newState);
            }
            foreach (Rules.Rule rule in Rules)
            {
                Rules.Rule newRule = rule.duplicate();
                retVal.appendRules(newRule);
            }

            return retVal;
        }
    }
}
