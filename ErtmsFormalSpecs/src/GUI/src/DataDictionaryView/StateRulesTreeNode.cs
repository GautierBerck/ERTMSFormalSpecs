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
using DataDictionary.Generated;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;

namespace GUI.DataDictionaryView
{
    public class StateRulesTreeNode : ReqRelatedTreeNode<State>
    {
        private class ItemEditor : ReqRelatedEditor
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
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Rule rule in Item.StateMachine.Rules)
            {
                Nodes.Add(new RuleTreeNode(rule, buildSubNodes));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public StateRulesTreeNode(State item, bool buildSubNodes)
            : base(item, buildSubNodes, "Rules", true, false)
        {
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
        /// Create structure based on the subsystem structure
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is RuleTreeNode)
            {
                RuleTreeNode node = SourceNode as RuleTreeNode;
                Rule rule = node.Item;
                node.Delete();
                AddRule(rule);
            }
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Rule rule = (Rule) acceptor.getFactory().createRule();
            rule.Name = "<Rule" + (GetNodeCount(false) + 1) + ">";

            RuleCondition condition = (RuleCondition) acceptor.getFactory().createRuleCondition();
            condition.Name = "<Condition1>";
            rule.appendConditions(condition);

            RuleTreeNode node = AddRule(rule);
        }

        /// <summary>
        /// Adds a new rule to the model
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public RuleTreeNode AddRule(Rule rule)
        {
            RuleTreeNode retVal = new RuleTreeNode(rule, true);

            Item.StateMachine.appendRules(rule);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}