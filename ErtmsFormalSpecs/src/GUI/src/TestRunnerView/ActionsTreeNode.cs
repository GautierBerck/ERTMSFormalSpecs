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
using System.Windows.Forms;

namespace GUI.TestRunnerView
{
    public class ActionsTreeNode : ModelElementTreeNode<DataDictionary.Tests.SubStep>
    {
        /// <summary>
        /// The value editor
        /// </summary>
        private class ItemEditor : NamedEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public ActionsTreeNode(DataDictionary.Tests.SubStep item, bool buildSubNodes)
            : base(item, buildSubNodes, "Actions", true)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (DataDictionary.Rules.Action action in Item.Actions)
            {
                Nodes.Add(new DataDictionaryView.ActionTreeNode(action, buildSubNodes));
            }
            SortSubNodes();
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        /// Adds the given action to the list of actions
        /// </summary>
        /// <param name="action"></param>
        public void addAction(DataDictionary.Rules.Action action)
        {
            action.Enclosing = Item;
            DataDictionaryView.ActionTreeNode actionNode = new DataDictionaryView.ActionTreeNode(action, true);
            Item.appendActions(action);
            Nodes.Add(actionNode);
            SortSubNodes();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            DataDictionary.Rules.Action action = (DataDictionary.Rules.Action)DataDictionary.Generated.acceptor.getFactory().createAction();
            action.Expression = "";
            addAction(action);
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));

            return retVal;
        }

        /// <summary>
        /// Handles the drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);
            if (SourceNode is DataDictionaryView.ActionTreeNode)
            {
                DataDictionaryView.ActionTreeNode action = SourceNode as DataDictionaryView.ActionTreeNode;
                action.Delete();
                addAction(action.Item);
            }
        }

        /// <summary>
        /// Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);
            if (Item.Translation != null)
            {
                if (BaseTreeView != null && BaseTreeView.RefreshNodeContent)
                {
                    IBaseForm baseForm = BaseForm;
                    if (baseForm != null)
                    {
                        if (baseForm.RequirementsTextBox != null)
                        {
                            baseForm.RequirementsTextBox.Text = Item.Translation.getSourceTextExplain();
                        }
                    }
                }
            }
        }
    }
}
