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
using System.Linq;
using DataDictionary.Functions;

namespace DataDictionary
{
    /// <summary>
    /// Logs messages on the rules according to the validity of the rule
    /// </summary>
    public class RuleCheckerVisitor : Generated.Visitor
    {
        /// <summary>
        /// The dictionary used for this visit
        /// </summary>
        private Dictionary dictionary;
        public Dictionary Dictionary
        {
            get { return dictionary; }
            private set { dictionary = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public RuleCheckerVisitor(Dictionary dictionary)
        {
            Utils.FinderRepository.INSTANCE.ClearCache();
            Dictionary = dictionary;
            declaredTypes.Clear();
        }

        /// <summary>
        /// Checks an expression associated to a model element
        /// </summary>
        /// <param name="model">The model element on which the expression is defined</param>
        /// <param name="expression">The expression to check</param>
        /// <returns>the expression parse tree</returns>
        private Interpreter.Expression checkExpression(ModelElement model, string expression)
        {
            Interpreter.Expression retVal = null;

            Interpreter.Parser parser = model.EFSSystem.Parser;
            try
            {
                retVal = parser.Expression(model, expression);

                if (retVal != null)
                {
                    retVal.checkExpression();
                    Types.Type type = retVal.GetExpressionType();
                    if (type == null)
                    {
                        model.AddError("Cannot determine expression type (5) for " + retVal.ToString());
                    }
                }
                else
                {
                    model.AddError("Cannot parse expression");
                }
            }
            catch (Exception exception)
            {
                model.AddException(exception);
            }
            return retVal;
        }

        /// <summary>
        /// Checks a statement associated to a model element
        /// </summary>
        /// <param name="model">The model element on which the expression is defined</param>
        /// <param name="expression">The expression to check</param>
        /// <returns>the expression parse tree</returns>
        private Interpreter.Statement.Statement checkStatement(ModelElement model, string expression)
        {
            Interpreter.Statement.Statement retVal = null;

            Interpreter.Parser parser = model.EFSSystem.Parser;
            try
            {
                retVal = parser.Statement(model, expression);
                retVal.CheckStatement();
            }
            catch (Exception exception)
            {
                model.AddException(exception);
            }
            return retVal;
        }

        public override void visit(Generated.Frame obj, bool visitSubNodes)
        {
            Tests.Frame frame = (Tests.Frame)obj;

            if (frame != null)
            {
                checkExpression(frame, frame.getCycleDuration());

                Types.Type type = frame.CycleDuration.GetExpressionType();
                if (type != null)
                {
                    if (!frame.EFSSystem.DoubleType.Match(type))
                    {
                        frame.AddError("Cycle duration should be compatible with the Time type");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.SubSequence obj, bool visitSubNodes)
        {
            Tests.SubSequence subSequence = (Tests.SubSequence)obj;

            if (subSequence != null)
            {
                if (subSequence.TestCases.Count == 0)
                {
                    subSequence.AddWarning("Sub sequences should hold at least one test case");
                }
                else
                {
                    Tests.TestCase testCase = (Tests.TestCase)subSequence.TestCases[0];

                    if (testCase.Steps.Count == 0)
                    {
                        testCase.AddWarning("First test case of a subsequence should hold at least one step");
                    }
                    else
                    {
                        Tests.Step step = (Tests.Step)testCase.Steps[0];

                        if (step.Name.IndexOf("Setup") < 0 && step.Name.IndexOf("Initialize") < 0)
                        {
                            step.AddWarning("First step of the first test case of a subsequence should be used to setup the system, and should hold 'Setup' or 'Initialize' in its name");
                        }
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Applied to all nodes of the tree
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Namable obj, bool visitSubNodes)
        {
            Namable namable = (Namable)obj;

            if (obj is Types.ITypedElement)
            {
                Types.ITypedElement typedElement = obj as Types.ITypedElement;

                Types.Type type = typedElement.Type;
                if (type == null)
                {
                    namable.AddError("Cannot find type " + typedElement.TypeName);
                }
                else if (!(typedElement is Parameter) && !(type is Types.StateMachine))
                {
                    Types.ITypedElement enclosingTypedElement = Utils.EnclosingFinder<Types.ITypedElement>.find(typedElement);

                    while (enclosingTypedElement != null)
                    {
                        if (enclosingTypedElement.Type == type)
                        {
                            namable.AddError("Recursive types are not allowed for " + type.Name);
                            enclosingTypedElement = null;
                        }
                        else
                        {
                            enclosingTypedElement = Utils.EnclosingFinder<Types.ITypedElement>.find(enclosingTypedElement);
                        }
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }


        /// <summary>
        /// Indicates that the enclosed mode matches the enclosing mode, that is, that the enclosed mode is = or more restrictive than the enclosing mode
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="enclosed"></param>
        /// <returns></returns>
        private static bool ValidMode(Generated.acceptor.VariableModeEnumType enclosing, Generated.acceptor.VariableModeEnumType enclosed)
        {
            bool retVal = false;

            switch (enclosing)
            {
                case Generated.acceptor.VariableModeEnumType.aConstant:
                    retVal = enclosed == Generated.acceptor.VariableModeEnumType.aConstant;
                    break;

                case Generated.acceptor.VariableModeEnumType.aIncoming:
                    retVal = enclosed == Generated.acceptor.VariableModeEnumType.aIncoming
                        || enclosed == Generated.acceptor.VariableModeEnumType.aInternal
                        || enclosed == Generated.acceptor.VariableModeEnumType.aConstant;
                    break;

                case Generated.acceptor.VariableModeEnumType.aInOut:
                    retVal = enclosed == Generated.acceptor.VariableModeEnumType.aIncoming
                        || enclosed == Generated.acceptor.VariableModeEnumType.aInOut
                        || enclosed == Generated.acceptor.VariableModeEnumType.aInternal
                        || enclosed == Generated.acceptor.VariableModeEnumType.aOutgoing
                        || enclosed == Generated.acceptor.VariableModeEnumType.aConstant;
                    break;
                case Generated.acceptor.VariableModeEnumType.aInternal:
                    retVal = enclosed == Generated.acceptor.VariableModeEnumType.aInternal
                        || enclosed == Generated.acceptor.VariableModeEnumType.aConstant;
                    break;

                case Generated.acceptor.VariableModeEnumType.aOutgoing:
                    retVal = enclosed == Generated.acceptor.VariableModeEnumType.aInternal
                        || enclosed == Generated.acceptor.VariableModeEnumType.aOutgoing
                        || enclosed == Generated.acceptor.VariableModeEnumType.aConstant;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Checks that a comment is attached to this ICommentable
        /// </summary>
        /// <param name="commentable"></param>
        private static void checkComment(ICommentable commentable)
        {
            if (string.IsNullOrEmpty(commentable.Comment))
            {
                ((ModelElement)commentable).AddInfo("This element should be documented");
            }
        }

        public override void visit(Generated.StructureElement obj, bool visitSubNodes)
        {
            Types.StructureElement element = (Types.StructureElement)obj;

            if (!Utils.Utils.isEmpty(element.getDefault()))
            {
                checkExpression(element, element.getDefault());
            }

            if (element.DefaultValue != null)
            {
                if (!element.DefaultValue.Type.Match(element.Type))
                {
                    element.AddError("Type of default value (" + element.DefaultValue.Type.FullName + ") does not match element type (" + element.Type.FullName + ")");
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Variable obj, bool visitSubNodes)
        {
            DataDictionary.Variables.Variable variable = obj as Variables.Variable;

            if (variable != null)
            {
                if (variable.Type == null)
                {
                    variable.AddError("Cannot find type for variable");
                }
                else
                {
                    Types.Structure structure = variable.Type as Types.Structure;
                    if (structure != null)
                    {
                        foreach (Types.StructureElement element in structure.Elements)
                        {
                            if (!ValidMode(variable.Mode, element.Mode))
                            {
                                variable.AddWarning("Invalid mode for " + element.Name);
                            }
                        }
                    }
                }
                if (Utils.Utils.isEmpty(variable.Comment) && variable.Type != null && Utils.Utils.isEmpty(variable.Type.Comment))
                {
                    variable.AddInfo("Missing variable semantics. Update the 'Comment' associated to the variable or to the corresponding type");
                }

                if (!Utils.Utils.isEmpty(variable.getDefaultValue()))
                {
                    checkExpression(variable, variable.getDefaultValue());
                }

                if (variable.DefaultValue != null)
                {
                    if (!variable.DefaultValue.Type.Match(variable.Type))
                    {
                        variable.AddError("Type of default value (" + variable.DefaultValue.Type.FullName + ")does not match variable type (" + variable.Type.FullName + ")");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Structure obj, bool visitSubNodes)
        {
            DataDictionary.Types.Structure structure = obj as Types.Structure;

            if (structure != null)
            {
                foreach (Types.StructureElement element in structure.Elements)
                {
                    Types.Structure elementType = element.Type as Types.Structure;
                    if (elementType != null)
                    {
                        foreach (Types.StructureElement subElement in elementType.Elements)
                        {
                            if (!ValidMode(element.Mode, subElement.Mode))
                            {
                                element.AddWarning("Invalid mode for " + subElement.Name);
                            }
                        }
                    }
                    if (!Utils.Utils.isEmpty(element.getDefault()))
                    {
                        checkExpression(element, element.getDefault());
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.ReqRef obj, bool visitSubNodes)
        {
            ReqRef reqRef = obj as ReqRef;
            if (reqRef != null)
            {
                if (reqRef.Paragraph == null)
                {
                    reqRef.AddError("Invalid reference to a requirement (" + reqRef.getId() + ")");
                }
            }
        }

        public override void visit(Generated.ReqRelated obj, bool visitSubNodes)
        {
            ReqRelated init = obj as ReqRelated;
            ReqRelated reqRelated = obj as ReqRelated;
            if (init != null && init.NeedsRequirement)  // the object must be associated to a requirement
            {
                bool noReq = true;  // no requirement found for this object
                while (noReq && reqRelated != null)
                {
                    for (int i = 0; i < reqRelated.Requirements.Count; i++)
                    {
                        ReqRef reqRef = reqRelated.Requirements[i] as ReqRef;
                        if (reqRef.Paragraph == null)
                        {
                            reqRef.AddError("Cannot find paragraph corresponding to " + reqRef.getId());
                        }
                        else if (reqRef.Paragraph.getType() == Generated.acceptor.Paragraph_type.aREQUIREMENT)
                        {
                            noReq = false;  // found a requirement
                        }
                    }
                    if (noReq)  // if no requirement found, we explore the requirements of the enclosing element
                    {
                        reqRelated = Utils.EnclosingFinder<DataDictionary.ReqRelated>.find(reqRelated);
                    }
                }
                if (noReq)
                {
                    init.AddInfo("No requirement found for element");
                }
            }

            reqRelated = init;
            if (!reqRelated.getImplemented())
            {
                ModelElement parent = reqRelated.getFather() as ModelElement;
                while (parent != null)
                {
                    ReqRelated other = parent as ReqRelated;
                    if (other != null)
                    {
                        if (other.getImplemented())
                        {
                            other.AddWarning("This element is set as implemented whereas one of its children " + reqRelated.FullName + " is not");
                        }
                    }
                    parent = parent.getFather() as ModelElement;
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Rule obj, bool visitSubNodes)
        {
            Rules.Rule rule = (Rules.Rule)obj;

            checkComment(rule);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.RuleCondition obj, bool subNodes)
        {
            Rules.RuleCondition ruleCondition = obj as Rules.RuleCondition;

            if (ruleCondition != null)
            {
                try
                {
                    bool found = false;
                    ruleCondition.Messages.Clear();

                    foreach (Rules.PreCondition preCondition in ruleCondition.PreConditions)
                    {
                        Interpreter.BinaryExpression expression = checkExpression(preCondition, preCondition.Expression) as Interpreter.BinaryExpression;
                        if (expression != null)
                        {
                            if (expression.IsSimpleEquality())
                            {
                                Types.ITypedElement variable = expression.Left.Ref as Types.ITypedElement;
                                if (variable != null)
                                {
                                    if (variable.Type != null)
                                    {
                                        // Check that when preconditions are based on a request, 
                                        // the corresponding action affects the value Request.Disabled to the same variable
                                        if (variable.Type.Name.Equals("Request") && expression.Right != null && expression.Right is Interpreter.UnaryExpression)
                                        {
                                            Values.IValue val2 = expression.Right.Ref as Values.IValue;
                                            if (val2 != null && "Response".CompareTo(val2.Name) == 0)
                                            {
                                                if (ruleCondition != null)
                                                {
                                                    found = false;
                                                    foreach (Rules.Action action in ruleCondition.Actions)
                                                    {
                                                        Variables.IVariable var = OverallVariableFinder.INSTANCE.findByName(action, preCondition.findVariable());
                                                        Interpreter.Statement.VariableUpdateStatement update = action.Modifies(var);
                                                        if (update != null)
                                                        {
                                                            Interpreter.UnaryExpression updateExpr = update.Expression as Interpreter.UnaryExpression;
                                                            if (updateExpr != null)
                                                            {
                                                                Values.IValue val3 = updateExpr.Ref as Values.IValue;
                                                                if (val3 != null && val3.Name.CompareTo("Disabled") == 0)
                                                                {
                                                                    found = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (!found)
                                                    {
                                                        preCondition.AddError("Rules where the Pre conditions is based on a Request type variable must assign that variable the value 'Request.Disabled'");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Check that the outgoing variables are not read
                                    if (variable.Mode == Generated.acceptor.VariableModeEnumType.aOutgoing)
                                    {
                                        if (ruleCondition.Reads(variable))
                                        {
                                            preCondition.AddError("An outgoing variable cannot be read");
                                        }
                                    }

                                    // Check that the incoming variables are not modified
                                    if (variable.Mode == Generated.acceptor.VariableModeEnumType.aIncoming)
                                    {
                                        if (ruleCondition.Modifies(variable) != null)
                                        {
                                            preCondition.AddError("An incoming variable cannot be written");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ruleCondition.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(Generated.Procedure obj, bool visitSubNodes)
        {
            Procedure procedure = (Procedure)obj;

            checkComment(procedure);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.PreCondition obj, bool subNodes)
        {
            Rules.PreCondition preCondition = obj as Rules.PreCondition;

            if (preCondition != null)
            {
                try
                {
                    // Check whether the expression is valid
                    Interpreter.Expression expression = checkExpression(preCondition, preCondition.Condition);
                    if (expression != null)
                    {
                        if (!preCondition.Dictionary.EFSSystem.BoolType.Match(expression.GetExpressionType()))
                        {
                            preCondition.AddError("Expression type should be Boolean");
                        }

                        Types.ITypedElement element = OverallTypedElementFinder.INSTANCE.findByName(preCondition, preCondition.findVariable());
                        if (element != null)
                        {
                            if (element.Type is Types.StateMachine)
                            {
                                if (preCondition.findOperator() != null)
                                {
                                    if (preCondition.findOperator().CompareTo("==") == 0)
                                    {
                                        preCondition.AddWarning("Operator == should not be used for state machines");
                                    }
                                    else if (preCondition.findOperator().CompareTo("!=") == 0)
                                    {
                                        preCondition.AddWarning("Operator != should not be used for state machines");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        preCondition.AddError("Cannot parse pre condition");
                    }
                }
                catch (Exception exception)
                {
                    preCondition.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(Generated.Action obj, bool subNodes)
        {
            Rules.Action action = obj as Rules.Action;

            if (action != null)
            {
                try
                {
                    action.Messages.Clear();
                    if (!action.Expression.Contains('%'))
                    {
                        Interpreter.Statement.Statement statement = checkStatement(action, action.Expression);
                    }
                }
                catch (Exception exception)
                {
                    action.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(Generated.Expectation obj, bool subNodes)
        {
            Tests.Expectation expect = obj as Tests.Expectation;

            if (expect != null)
            {
                try
                {
                    expect.Messages.Clear();
                    if (!expect.Expression.Contains("%"))
                    {
                        Interpreter.Expression expression = checkExpression(expect, expect.Expression);
                        if (!expect.EFSSystem.BoolType.Match(expression.GetExpressionType()))
                        {
                            expect.AddError("Expression type should be Boolean");
                        }
                    }
                    if (expect.getCondition() != null && !expect.getCondition().Contains("%"))
                    {
                        Interpreter.Expression expression = checkExpression(expect, expect.getCondition());
                        if (expression != null)
                        {
                            if (!expect.EFSSystem.BoolType.Match(expression.GetExpressionType()))
                            {
                                expect.AddError("Condition type should be Boolean");
                            }
                        }
                        else
                        {
                            expect.AddError("Cannot parse condition " + expect.getCondition());
                        }
                    }
                }
                catch (Exception exception)
                {
                    expect.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(Generated.StateMachine obj, bool visitSubNodes)
        {
            DataDictionary.Types.StateMachine stateMachine = (DataDictionary.Types.StateMachine)obj;

            if (stateMachine != null)
            {
                stateMachine.Messages.Clear();

                if (stateMachine.AllValues.Count > 0)
                {
                    if (Utils.Utils.isEmpty(stateMachine.InitialState))
                    {
                        stateMachine.AddError("Empty initial state");
                    }
                    if (stateMachine.DefaultValue == null)
                    {
                        stateMachine.AddError("Cannot find default value");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.State obj, bool visitSubNodes)
        {
            DataDictionary.Constants.State state = (DataDictionary.Constants.State)obj;

            if (state != null)
            {
                state.Messages.Clear();

                if (state.Name.Contains(' '))
                {
                    state.AddError("A state name cannot contain white spaces");
                }
            }

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// The type names encountered
        /// </summary>
        private Dictionary<string, Types.Type> declaredTypes = new Dictionary<string, Types.Type>();

        private static string TYPE_DECLARED_SEVERAL_TIMES = "Another type with the same name is declared somewhere else in the model";

        public override void visit(Generated.Type obj, bool visitSubNodes)
        {
            Types.Type type = obj as Types.Type;

            if (type != null)
            {
                checkComment(type);

                if (type is Types.StateMachine)
                {
                    // Ignore state machines
                }
                else
                {
                    if (!(type is Types.Structure) && !(type is Functions.Function))
                    {
                        if (Utils.Utils.isEmpty(type.getDefault()))
                        {
                            type.AddError("Types should define their default value");
                        }
                        else
                        {
                            if (type.DefaultValue == null)
                            {
                                type.AddError("Invalid default value");
                            }
                        }
                        if (type is Types.Range)
                        {
                            Types.Range range = type as Types.Range;
                            if ((range.getPrecision() == Generated.acceptor.PrecisionEnum.aIntegerPrecision && range.Default != null && range.Default.IndexOf('.') > 0)
                                    || (range.getPrecision() == Generated.acceptor.PrecisionEnum.aDoublePrecision && range.Default != null && range.Default.IndexOf('.') <= 0))
                            {
                                type.AddError("Default value's precision does not correspond to the type's precision");
                            }
                            foreach (Constants.EnumValue specValue in range.SpecialValues)
                            {
                                String value = specValue.getValue();
                                if (range.getPrecision() == Generated.acceptor.PrecisionEnum.aDoublePrecision && value.IndexOf('.') <= 0
                                    || range.getPrecision() == Generated.acceptor.PrecisionEnum.aIntegerPrecision && value.IndexOf('.') > 0)
                                {
                                    type.AddError("Precision of the special value + " + specValue.Name + " does not correspond to the type's precision");
                                }
                            }
                        }
                    }

                    if (declaredTypes.ContainsKey(type.FullName))
                    {
                        declaredTypes[type.Name].AddError(TYPE_DECLARED_SEVERAL_TIMES);
                        type.AddError(TYPE_DECLARED_SEVERAL_TIMES);
                    }
                    else
                    {
                        declaredTypes[type.Name] = type;
                    }

                    Types.Collection collection = type as Types.Collection;
                    if (collection != null)
                    {
                        if (collection.getMaxSize() == 0)
                        {
                            type.AddError("Collections should be upper bounded");
                        }
                    }
                }

                if (!Utils.Utils.isEmpty(type.getDefault()))
                {
                    checkExpression(type, type.getDefault());
                }
            }

            base.visit(obj, visitSubNodes);
        }


        /// <summary>
        /// The sets of defined paragraphs
        /// </summary>
        private Dictionary<string, Specification.Paragraph> Paragraphs = new Dictionary<string, Specification.Paragraph>();

        public override void visit(Generated.Paragraph obj, bool visitSubNodes)
        {
            Specification.Paragraph paragraph = obj as Specification.Paragraph;

            if (paragraph != null)
            {
                Specification.Paragraph otherParagraph;
                if (Paragraphs.TryGetValue(paragraph.Name, out otherParagraph))
                {
                    paragraph.AddError("Duplicate paragraph id " + paragraph.Name);
                    otherParagraph.AddError("Duplicate paragraph id " + paragraph.Name);
                }
                else
                {
                    Paragraphs.Add(paragraph.Name, paragraph);
                }

                switch (paragraph.getImplementationStatus())
                {
                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented:
                        if (!paragraph.isApplicable())
                        {
                            paragraph.AddWarning("Paragraph state does not correspond to implementation status (Implemented but not applicable)");
                        }
                        break;

                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA:
                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM:
                        if (!paragraph.isApplicable())
                        {
                            paragraph.AddWarning("Paragraph state does not correspond to implementation status (N/A but not applicable)");
                        }
                        break;

                    case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable:
                        if (paragraph.isApplicable())
                        {
                            paragraph.AddWarning("Paragraph state does not correspond to implementation status (Not implementable but applicable)");
                        }
                        break;
                }

                if (paragraph.getImplementationStatus() == Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented)
                {
                    foreach (ReqRef reqRef in ImplementedParagraphsFinder.INSTANCE.findRefs(paragraph))
                    {
                        ReqRelated model = reqRef.Enclosing as ReqRelated;
                        if (!model.ImplementationCompleted)
                        {
                            model.AddWarning("Requirement implementation is complete, while model element implementation is not");
                            paragraph.AddWarning("Requirement implementation is complete, while model element implementation is not");
                        }
                    }
                }

                if ((!paragraph.getScopeOnBoard()) && paragraph.SubParagraphScopeOnboard)
                {
                    paragraph.AddWarning("Paragraph scope should be On Board, according to its sub-paragraphs");
                }
                if ((!paragraph.getScopeTrackside()) && paragraph.SubParagraphScopeTrackside)
                {
                    paragraph.AddWarning("Paragraph scope should be Trackside, according to its sub-paragraphs");
                }
                if ((!paragraph.getScopeRollingStock()) && paragraph.SubParagraphScopeRollingStock)
                {
                    paragraph.AddWarning("Paragraph scope should be Rolling Stock, according to its sub-paragraphs");
                }
                if (!paragraph.getScopeOnBoard() && !paragraph.getScopeTrackside() && !paragraph.getScopeRollingStock())
                {
                    paragraph.AddWarning("Paragraph scope not set");
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Function obj, bool visitSubNodes)
        {
            Function function = (Function)obj;

            checkComment(function);

            if (function.ReturnType == null)
            {
                function.AddError("Cannot determine function return type");
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Case obj, bool visitSubNodes)
        {
            Functions.Case cas = obj as Functions.Case;

            try
            {
                Interpreter.Expression expression = cas.Expression;
                if (expression != null)
                {
                    expression.checkExpression();
                    Types.Type expressionType = cas.Expression.GetExpressionType();
                    if (expressionType != null && cas.EnclosingFunction != null && cas.EnclosingFunction.ReturnType != null)
                    {
                        if (!cas.EnclosingFunction.ReturnType.Match(expressionType))
                        {
                            cas.AddError("Expression type (" + expressionType.FullName + ") does not match function return type (" + cas.EnclosingFunction.ReturnType.Name + ")");
                        }
                    }
                    else
                    {
                        cas.AddError("Cannot determine expression type (6) for " + cas.Expression.ToString());
                    }
                }
                else
                {
                    cas.AddError("Cannot evaluate expression " + cas.ExpressionText);
                }
            }
            catch (Exception e)
            {
                cas.AddException(e);
            }

            base.visit(obj, visitSubNodes);
        }
    }
}
