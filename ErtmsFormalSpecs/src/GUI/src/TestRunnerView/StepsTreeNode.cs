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
using DataDictionary.Tests;

namespace GUI.TestRunnerView
{
    public class StepsTreeNode : ModelElementTreeNode<TestCase>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StepsTreeNode(TestCase item, bool buildSubNodes)
            : base(item, buildSubNodes, "Steps", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            foreach (Step step in Item.Steps)
            {
                subNodes.Add(new StepTreeNode(step, recursive));
            }
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendSteps(Step.CreateDefault(Item.Steps));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add", AddHandler)};

            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }

        /// <summary>
        ///     Handles the drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            if (sourceNode is StepTreeNode)
            {
                StepTreeNode step = sourceNode as StepTreeNode;
                step.Delete();
                Item.appendSteps(step.Item);
            }
        }
    }
}