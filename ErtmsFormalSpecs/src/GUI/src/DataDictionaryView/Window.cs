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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUI.Properties;
using Utils;

namespace GUI.DataDictionaryView
{
    public partial class Window : BaseForm
    {
        public override BaseTreeView TreeView
        {
            get { return dataDictTree; }
        }

        /// <summary>
        ///     The Dictionary handled by this view
        /// </summary>
        private Dictionary _dictionary;

        /// <summary>
        ///     The Dictionary handled by this view
        /// </summary>
        public Dictionary Dictionary
        {
            get { return _dictionary; }
            set
            {
                _dictionary = value;
                dataDictTree.Root = _dictionary;
                Text = _dictionary.Name + @" " + Resources.Window_Dictionary_model_view;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public Window(Dictionary dictionary)
        {
            InitializeComponent();
            Dictionary = dictionary;
        }

        /// <summary>
        ///     Indicates that the model element should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        protected override bool ShouldTrackSelectionChange(IModelElement modelElement)
        {
            bool retVal = base.ShouldTrackSelectionChange(modelElement);

            if (retVal)
            {
                NameSpace nameSpace = EnclosingFinder<NameSpace>.find(modelElement, true);

                retVal = nameSpace != null || modelElement == Dictionary;
            }

            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the selected model changed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if refresh should be performed</returns>
        public override bool HandleSelectionChange(Context.SelectionContext context)
        {
            bool retVal = base.HandleSelectionChange(context);

            if (retVal)
            {
                if ((context.Sender == modelDiagramPanel) || (context.Sender == stateDiagramPanel))
                {
                    bool rebuildRightPart = (context.Criteria & Context.SelectionCriteria.DoubleClick) != 0;
                    UpdateModelView(context, rebuildRightPart);
                }
                else
                {
                    UpdateModelView(context, true);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Updates the model view, according to the element selected in the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rebuildRightPart">Indicates that the right part should be rebuilt</param>
        private void UpdateModelView(Context.SelectionContext context, bool rebuildRightPart)
        {
            StateMachine stateMachine = GetStateMachine(context);
            IModelElement model = DisplayedElementInModelDiagramPanel(context);
            if (stateMachine == null)
            {
                DisplayModel(context, rebuildRightPart, model);
            }
            else
            {
                if (model is NameSpace)
                {
                    DisplayStateMachine(context, rebuildRightPart, stateMachine);
                }
                else if (model is Structure)
                {
                    DisplayStateMachine(context, rebuildRightPart, stateMachine);
                }
                else
                {
                    DisplayModel(context, rebuildRightPart, model);                    
                }
            }
        }

        /// <summary>
        /// Displays the state machine instead of the model
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rebuildRightPart"></param>
        /// <param name="stateMachine"></param>
        private void DisplayStateMachine(Context.SelectionContext context, bool rebuildRightPart, StateMachine stateMachine)
        {
            if (stateMachine != null)
            {
                if (rebuildRightPart)
                {
                    IVariable variable = context.Element as IVariable;
                    modelDiagramPanel.Visible = false;
                    if (variable != null)
                    {
                        stateDiagramPanel.SetStateMachine(variable);
                    }
                    else
                    {
                        stateDiagramPanel.Model = stateMachine;
                    }
                    stateDiagramPanel.Visible = true;
                    stateDiagramPanel.RefreshControl();
                }
                stateDiagramPanel.SelectModel(context.Element);
            }
        }

        /// <summary>
        /// Displays the model instead of the state machine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rebuildRightPart"></param>
        /// <param name="model"></param>
        private void DisplayModel(Context.SelectionContext context, bool rebuildRightPart, IModelElement model)
        {
            if (model != null)
            {
                if (rebuildRightPart)
                {
                    stateDiagramPanel.Visible = false;
                    modelDiagramPanel.Model = model;
                    modelDiagramPanel.Visible = true;
                    modelDiagramPanel.RefreshControl();
                }
                modelDiagramPanel.SelectModel(context.Element);
            }
        }

        /// <summary>
        /// Provides the Namespace or the Dictionary enclosing the element in the context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static IModelElement DisplayedElementInModelDiagramPanel(Context.SelectionContext context)
        {
            IModelElement model = null;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (model == null)
            {
                model = EnclosingFinder<Function>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<RuleCondition>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<Rule>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<Procedure>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<Structure>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<Range>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<DataDictionary.Types.Enum>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<NameSpace>.find(context.Element, true);
            }

            if (model == null)
            {
                model = EnclosingFinder<Dictionary>.find(context.Element, true);
            }
            return model;
        }

        /// <summary>
        /// Provides the state machine from the context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static StateMachine GetStateMachine(Context.SelectionContext context)
        {
            StateMachine stateMachine;

            State state = EnclosingFinder<State>.find(context.Element, true);
            if (state != null && state.StateMachine.countStates() > 0)
            {
                stateMachine = state.StateMachine;
            }
            else
            {
                stateMachine = EnclosingFinder<StateMachine>.find(context.Element, true);
            }

            IVariable variable = context.Element as IVariable;
            if (variable != null)
            {
                stateMachine = variable.Type as StateMachine;
            }

            return stateMachine;
        }

        /// <summary>
        ///     Indicates that a change event should be displayed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns></returns>
        protected override bool ShouldDisplayChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = modelElement == null;

            if (!retVal)
            {
                if (changeKind != Context.ChangeKind.EndOfCycle)
                {
                    Dictionary enclosing = EnclosingFinder<Dictionary>.find(modelElement, true);
                    retVal = (enclosing == Dictionary);
                }
                else
                {
                    retVal = base.ShouldDisplayChange(modelElement, changeKind);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Allows to refresh the view, when the value of a model changed
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind"></param>
        /// <returns>True if the view should be refreshed</returns>
        public override bool HandleValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            bool retVal = base.HandleValueChange(modelElement, changeKind);

            if (retVal)
            {
                if (changeKind != Context.ChangeKind.EndOfCycle)
                {
                    modelDiagramPanel.RefreshControl();
                    if (stateDiagramPanel != null)
                    {
                        stateDiagramPanel.RefreshControl();
                    }

                    Dictionary enclosing = EnclosingFinder<Dictionary>.find(modelElement, true);
                    if (modelElement == null || enclosing == Dictionary)
                    {
                        dataDictTree.RefreshModel(modelElement);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Finds the tree node which corresponds to the model element
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseTreeNode FindNode(IModelElement model)
        {
            return TreeView.FindNode(model, true);
        }

        /// <summary>
        ///     Selects the next node where error message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextErrortoolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Error);
        }

        /// <summary>
        ///     Selects the next node where warning message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextWarningToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Warning);
        }

        /// <summary>
        ///     Selects the next node where info message is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextInfoToolStripButton_Click(object sender, EventArgs e)
        {
            TreeView.SelectNext(ElementLog.LevelEnum.Info);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!EfsSystem.Instance.Markings.SelectPreviousMarking())
            {
                MessageBox.Show(
                    Resources.Window_toolStripButton1_Click_No_more_marking_to_show,
                    Resources.Window_toolStripButton1_Click_No_more_markings,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!EfsSystem.Instance.Markings.SelectNextMarking())
            {
                MessageBox.Show(
                    Resources.Window_toolStripButton1_Click_No_more_marking_to_show,
                    Resources.Window_toolStripButton1_Click_No_more_markings,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}