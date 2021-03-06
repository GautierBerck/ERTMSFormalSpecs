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
using DataDictionary.Rules;

namespace GUI.DataDictionaryView
{
    public class RulePreConditionsTreeNode : RuleConditionTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public RulePreConditionsTreeNode(RuleCondition item, bool buildSubNodes)
            : base(item, buildSubNodes, "Pre conditions", false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            // Do not use the base version
            SubNodesBuilt = true;

            foreach (PreCondition preCondition in Item.PreConditions)
            {
                subNodes.Add(new PreConditionTreeNode(preCondition, recursive));
            }
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendPreConditions(PreCondition.CreateDefault(Item.PreConditions));
            Item.setVerified(false);
        }

        /// <summary>
        ///     Handles a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            PreConditionTreeNode preConditionTreeNode = sourceNode as PreConditionTreeNode;
            if (preConditionTreeNode != null)
            {
                if (
                    MessageBox.Show("Are you sure you want to move the corresponding pre-condition ?",
                        "Move pre-condition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    PreCondition preCondition = preConditionTreeNode.Item;
                    preConditionTreeNode.Delete();
                    Item.appendPreConditions(preCondition);
                    Item.setVerified(false);
                }
            }
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            return retVal;
        }
    }
}