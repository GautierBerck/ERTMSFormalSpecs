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
using Enum = DataDictionary.Types.Enum;
using EnumValue = DataDictionary.Constants.EnumValue;

namespace GUI.DataDictionaryView
{
    public class EnumerationValuesTreeNode : EnumerationTreeNode
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public EnumerationValuesTreeNode(Enum item, bool buildSubNodes)
            : base(item, buildSubNodes, "Values", true, false)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            Nodes.Clear();

            foreach (EnumValue value in Item.Values)
            {
                Nodes.Add(new EnumerationValueTreeNode(value, buildSubNodes));
            }
            SortSubNodes();
            SubNodesBuilt = true;
        }

        public void AddEnumValueHandler(object sender, EventArgs args)
        {
            EnumValue value = (EnumValue) acceptor.getFactory().createEnumValue();
            AddValue(value);
        }

        public void AddValue(EnumValue value)
        {
            value.Name = "<EnumValue" + (GetNodeCount(false) + 1) + ">";
            value.setValue("");
            Item.appendValues(value);
            Nodes.Add(new EnumerationValueTreeNode(value, true));
            SortSubNodes();
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddEnumValueHandler)));

            return retVal;
        }
    }
}