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
using GUI.SpecificationView;
using Paragraph = DataDictionary.Specification.Paragraph;
using Procedure = DataDictionary.Functions.Procedure;
using ReqRef = DataDictionary.ReqRef;
using Rule = DataDictionary.Rules.Rule;

namespace GUI.DataDictionaryView
{
    public class ProcedureRulesTreeNode : ModelElementTreeNode<Procedure>
    {
        /// <summary>
        /// The editor for message variables
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
        /// <param name="name"></param>
        public ProcedureRulesTreeNode(Procedure item, bool buildSubNodes)
            : base(item, buildSubNodes, "Rules", true)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Rule rule in Item.Rules)
            {
                Nodes.Add(new RuleTreeNode(rule, buildSubNodes));
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

        public void AddHandler(object sender, EventArgs args)
        {
            Rule rule = (Rule) acceptor.getFactory().createRule();
            rule.Name = "<Rule" + (GetNodeCount(false) + 1) + ">";
            AddRule(rule);
        }

        /// <summary>
        /// Adds a rule in the corresponding namespace
        /// </summary>
        /// <param name="variable"></param>
        public RuleTreeNode AddRule(Rule rule)
        {
            Item.appendRules(rule);
            RuleTreeNode retVal = new RuleTreeNode(rule, true);
            Nodes.Add(retVal);

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

            return retVal;
        }

        /// <summary>
        /// Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is RuleTreeNode)
            {
                RuleTreeNode ruleTreeNode = SourceNode as RuleTreeNode;
                Rule rule = ruleTreeNode.Item;

                ruleTreeNode.Delete();
                AddRule(rule);
            }
            else if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = SourceNode as ParagraphTreeNode;
                Paragraph paragaph = node.Item;

                Rule rule = (Rule) acceptor.getFactory().createRule();
                rule.Name = paragaph.Name;

                ReqRef reqRef = (ReqRef) acceptor.getFactory().createReqRef();
                reqRef.Name = paragaph.FullId;
                rule.appendRequirements(reqRef);
                AddRule(rule);
            }
        }
    }
}