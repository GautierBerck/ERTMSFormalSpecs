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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Tests;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using GUI.DataDictionaryView;
using Utils;
using Action = DataDictionary.Rules.Action;
using ModelElement = DataDictionary.ModelElement;
using Window = GUI.StructureValueEditor.Window;

namespace GUI
{
    public partial class EditorTextBox : BaseEditorTextBox
    {
        /// <summary>
        ///     The enclosing IBaseForm
        /// </summary>
        private IBaseForm EnclosingForm
        {
            get
            {
                Control current = Parent;
                while (current != null && !(current is IBaseForm))
                {
                    current = current.Parent;
                }

                return current as IBaseForm;
            }
        }

        private object _instance;

        /// <summary>
        ///     Provides the instance on which this editor is based
        /// </summary>
        public override object Instance
        {
            get
            {
                if (_instance == null && EnclosingForm != null)
                {
                    _instance = EnclosingForm.Selected;
                }
                return _instance;
            }
            set { _instance = value; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public EditorTextBox()
        {
            InitializeComponent();

            EditionTextBox.AllowDrop = true;
            EditionTextBox.DragDrop += Editor_DragDropHandler;
            EditionTextBox.MouseUp += EditionTextBox_MouseClick;

            EditionTextBox.GotFocus += Editor_GotFocus;
            EditionTextBox.LostFocus += Editor_LostFocus;
        }

        private void EditionTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                List<INamable> instances = GetInstances(e.Location);
                if (instances.Count == 1)
                {
                    MainWindow mdiWindow = GUIUtils.MDIWindow;
                    if (mdiWindow != null)
                    {
                        mdiWindow.Select(instances[0] as Utils.ModelElement, true);
                    }
                }
            }
        }

        private void Editor_LostFocus(object sender, EventArgs e)
        {
            if (GUIUtils.MDIWindow != null)
            {
                GUIUtils.MDIWindow.SelectedRichTextBox = null;
            }
        }

        private void Editor_GotFocus(object sender, EventArgs e)
        {
            if (GUIUtils.MDIWindow != null)
            {
                GUIUtils.MDIWindow.SelectedRichTextBox = this;
            }
        }

        /// <summary>
        ///     Called when the drop operation is performed on this text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_DragDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                object data = e.Data.GetData("WindowsForms10PersistentObject");
                BaseTreeNode sourceNode = data as BaseTreeNode;
                if (sourceNode != null)
                {
                    VariableTreeNode variableNode = sourceNode as VariableTreeNode;
                    if (variableNode != null)
                    {
                        EditionTextBox.SelectedText = SetVariable(variableNode.Item);
                    }
                    else
                    {
                        StructureTreeNode structureTreeNode = sourceNode as StructureTreeNode;
                        if (structureTreeNode != null)
                        {
                            StringBuilder text = new StringBuilder();

                            Structure structure = structureTreeNode.Item;
                            CreateDefaultStructureValue(text, structure);
                            EditionTextBox.SelectedText = text.ToString();
                        }
                        else
                        {
                            EditionTextBox.SelectedText = StripUseless(sourceNode.Model.FullName, WritingContext());
                        }
                    }
                }

                OLVListItem item = data as OLVListItem;
                if (item != null)
                {
                    Variable variable = item.RowObject as Variable;
                    if (variable != null)
                    {
                        EditionTextBox.SelectedText = SetVariable(variable);
                    }
                }
            }
        }

        /// <summary>
        ///     Edits a value expression and provides the edited expression after user has performed his changes
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression EditExpression(Expression expression)
        {
            Expression retVal = expression;

            if (expression != null)
            {
                bool silentMode = ModelElement.BeSilent;
                try
                {
                    ModelElement.BeSilent = true;

                    InterpretationContext context = new InterpretationContext(Model);
                    context.UseDefaultValue = false;
                    IValue value = expression.GetValue(context, null);
                    if (value != null)
                    {
                        Window window = new Window();
                        window.SetModel(value);
                        window.ShowDialog();

                        string newExpression = value.ToExpressionWithDefault();
                        const bool doSemanticalAnalysis = true;
                        const bool silent = true;
                        retVal = EFSSystem.INSTANCE.Parser.Expression(expression.Root, newExpression,
                            AllMatches.INSTANCE, doSemanticalAnalysis, null, silent);
                    }
                }
                finally
                {
                    ModelElement.BeSilent = silentMode;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Browses through the expression to find the value to edit
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression VisitExpression(Expression expression)
        {
            Expression retVal = expression;

            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                binaryExpression.Left = VisitExpression(binaryExpression.Left);
                binaryExpression.Right = VisitExpression(binaryExpression.Right);
            }

            UnaryExpression unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                if (unaryExpression.Expression != null)
                {
                    unaryExpression.Expression = VisitExpression(unaryExpression.Expression);
                }
                else if (unaryExpression.Term != null)
                {
                    visitTerm(unaryExpression.Term);
                }
            }

            StructExpression structExpression = expression as StructExpression;
            if (structExpression != null)
            {
                retVal = EditExpression(structExpression);
            }

            Call call = expression as Call;
            if (call != null)
            {
                foreach (Expression subExpression in call.AllParameters)
                {
                    VisitExpression(subExpression);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Browse through the Term to find the value to edit
        /// </summary>
        /// <param name="term"></param>
        private void visitTerm(Term term)
        {
            if (term.LiteralValue != null)
            {
                term.LiteralValue = EditExpression(term.LiteralValue);
            }
        }

        /// <summary>
        ///     Browses through the statement to find the structures to edit
        /// </summary>
        /// <param name="statement"></param>
        private Statement VisitStatement(Statement statement)
        {
            Statement retVal = statement;

            VariableUpdateStatement variableUpdateStatement = statement as VariableUpdateStatement;
            if (variableUpdateStatement != null)
            {
                variableUpdateStatement.Expression = VisitExpression(variableUpdateStatement.Expression);
            }

            return retVal;
        }

        private void openStructureEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool dialogShown = false;
            ExplanationPart part = Instance as ExplanationPart;
            if (part != null)
            {
                IValue value = part.Namable as IValue;
                if (value != null)
                {
                    Window window = new Window();
                    window.SetModel(value);
                    window.ShowDialog();
                    dialogShown = true;
                }

                Action action = part.Element as Action;
                if (!dialogShown && action != null)
                {
                    VisitStatement(action.Statement);
                    dialogShown = true;
                }

                Expectation expectation = part.Element as Expectation;
                if (!dialogShown && expectation != null)
                {
                    VisitExpression(expectation.Expression);
                    dialogShown = true;
                }
            }

            if (!dialogShown)
            {
                const bool doSemanticalAnalysis = true;
                const bool silent = true;
                ModelElement root = Instance as ModelElement;
                if (root == null)
                {
                    root = EFSSystem.INSTANCE.Dictionaries[0];
                }

                string text = EditionTextBox.Text;
                Expression expression = EFSSystem.INSTANCE.Parser.Expression(root, text, AllMatches.INSTANCE,
                    doSemanticalAnalysis, null, silent);
                if (expression != null)
                {
                    expression = VisitExpression(expression);
                    EditionTextBox.Text = expression.ToString();
                }

                Statement statement = EFSSystem.INSTANCE.Parser.Statement(root, text, silent);
                if (statement != null)
                {
                    statement = VisitStatement(statement);
                    EditionTextBox.Text = statement.ToString();
                }
            }
        }
    }
}