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
using System.Drawing.Design;
using System.ComponentModel;

namespace GUI.DataDictionaryView
{
    public class ActionTreeNode : ModelElementTreeNode<DataDictionary.Rules.Action>
    {
        private class ItemEditor : Editor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description")]
            [System.ComponentModel.Editor(typeof(Converters.ExpressionableUITypedEditor), typeof(UITypeEditor))]
            [System.ComponentModel.TypeConverter(typeof(Converters.ExpressionableUITypeConverter))]
            public DataDictionary.Rules.Action Expression
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Description")]
            [System.ComponentModel.Editor(typeof(Converters.CommentableUITypedEditor), typeof(UITypeEditor))]
            [System.ComponentModel.TypeConverter(typeof(Converters.CommentableUITypeConverter))]
            public DataDictionary.Rules.Action Comment
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            /// Sets the verified flag of the the enclosing rule 
            /// </summary>
            /// <param name="val"></param>
            private void SetRuleAsVerified(bool val)
            {
                if (Item.Rule != null)
                {
                    Item.Rule.setVerified(val);
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public ActionTreeNode(DataDictionary.Rules.Action item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
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

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(4, new MenuItem("-"));
            retVal.Insert(5, new MenuItem("Split", new EventHandler(SplitHandler)));

            return retVal;
        }

        /// <summary>
        /// Splits the selected action to several sub-actions
        /// </summary>
        public virtual void SplitHandler(object sender, EventArgs args)
        {
            DataDictionary.Interpreter.Statement.Statement statement = Item.EFSSystem.Parser.Statement(Item, Item.ExpressionText);
            DataDictionary.Interpreter.Statement.VariableUpdateStatement variableUpdateStatement = statement as DataDictionary.Interpreter.Statement.VariableUpdateStatement;
            if (variableUpdateStatement != null)
            {
                DataDictionary.Interpreter.Expression expression = variableUpdateStatement.Expression;
                DataDictionary.Interpreter.StructExpression structExpression = expression as DataDictionary.Interpreter.StructExpression;
                if (structExpression != null)
                {
                    Dictionary<string, DataDictionary.Interpreter.Expression> associations = structExpression.Associations;
                    foreach (KeyValuePair<string, DataDictionary.Interpreter.Expression> value in associations)
                    {
                        DataDictionary.Rules.Action action = (DataDictionary.Rules.Action)DataDictionary.Generated.acceptor.getFactory().createAction();
                        action.ExpressionText = structExpression.Structure.ToString() + "." + value.Key + " <- " + value.Value.ToString();
                        string aString = value.Value.ToString();
                        ActionTreeNode actionTreeNode = new ActionTreeNode(action, true);

                        BaseTreeNode parent = Parent as BaseTreeNode;
                        if ((parent != null) && (parent.Nodes != null))
                        {
                            DataDictionary.Rules.RuleCondition ruleCondition = Item.Enclosing as DataDictionary.Rules.RuleCondition;
                            if (ruleCondition != null)
                            {
                                ruleCondition.appendActions(action);
                            }
                            else
                            {
                                DataDictionary.Tests.SubStep subStep = Item.Enclosing as DataDictionary.Tests.SubStep;
                                if (subStep != null)
                                {
                                    subStep.appendActions(action);
                                }
                            }
                            parent.Nodes.Add(actionTreeNode);
                        }
                    }
                }
            }
            Delete();
            SortSubNodes();
        }
    }
}
