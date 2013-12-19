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

namespace GUI.DataDictionaryView
{
    public class RuleDisablingsTreeNode : ModelElementTreeNode<DataDictionary.Dictionary>
    {
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
        public RuleDisablingsTreeNode(DataDictionary.Dictionary item)
            : base(item, "Disabled rules", true)
        {
        }

        protected override void BuildSubNodes()
        {
            base.BuildSubNodes();

            foreach (DataDictionary.Rules.RuleDisabling ruleDisabling in Item.RuleDisablings)
            {
                Nodes.Add(new RuleDisablingTreeNode(ruleDisabling));
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

        public void AddHandler(object sender, EventArgs args)
        {
            DataDictionary.Rules.RuleDisabling ruleDisabling = (DataDictionary.Rules.RuleDisabling)DataDictionary.Generated.acceptor.getFactory().createRuleDisabling();
            ruleDisabling.Name = "<RuleDisabling" + (GetNodeCount(false) + 1) + ">";
            Item.appendRuleDisablings(ruleDisabling);
            Nodes.Add(new RuleDisablingTreeNode(ruleDisabling));
            SortSubNodes();
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));

            return retVal;
        }

        /// <summary>
        /// Update counts according to the selected folder
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.RuleDisablings.Count + (Item.RuleDisablings.Count > 1 ? " rule disablings " : " rule disabling ") + "selected.");
        }
    }
}
