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
using System.ComponentModel;
using System.Windows.Forms;
using DataDictionary.Types;

namespace GUI.DataDictionaryView
{
    public class StructureTreeNode : TypeTreeNode<Structure>
    {
        private class ItemEditor : TypeEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            /// Indicates that the req related item does not need to be attached to a requirement
            /// </summary>
            [Category("Description")]
            public bool IsAbstract
            {
                get { return Item.IsAbstract; }
                set { Item.IsAbstract = value; }
            }
        }

        private NameSpaceTreeNode types
        {
            get { return Parent.Parent as NameSpaceTreeNode; }
        }

        private InterfacesTreeNode interfaces;
        private StructureElementsTreeNode elements;
        private StructureProceduresTreeNode procedures;
        private StructureStateMachinesTreeNode stateMachines;
        private RulesTreeNode rules;
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public StructureTreeNode(Structure item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public StructureTreeNode(Structure item, bool buildSubNodes, string name, bool isFolder, bool addRequirements)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
        {
        }

        /// <summary>
        /// Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates that subnodes of the nodes built should also </param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            interfaces = new InterfacesTreeNode(Item, buildSubNodes);
            elements = new StructureElementsTreeNode(Item, buildSubNodes);
            procedures = new StructureProceduresTreeNode(Item, buildSubNodes);
            stateMachines = new StructureStateMachinesTreeNode(Item, buildSubNodes);
            rules = new RulesTreeNode(Item, buildSubNodes);
            
            Nodes.Add(interfaces);
            Nodes.Add(elements);
            Nodes.Add(procedures);
            Nodes.Add(stateMachines);
            Nodes.Add(rules);
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        private void AddInterfaceHandler(object sender, EventArgs args)
        {
            if (interfaces != null)
            {
                interfaces.AddHandler(sender, args);
            }
        }

        private void AddStructureElementHandler(object sender, EventArgs args)
        {
            if (elements != null)
            {
                elements.AddHandler(sender, args);
            }
        }

        private void AddProcedureHandler(object sender, EventArgs args)
        {
            if (procedures != null)
            {
                procedures.AddHandler(sender, args);
            }
        }

        private void AddStateMachineHandler(object sender, EventArgs args)
        {
            if (stateMachines != null)
            {
                stateMachines.AddHandler(sender, args);
            }
        }

        private void GenerateInheritedFieldsHandler(object sender, EventArgs args)
        {
            Item.GenerateInheritedFields();
            interfaces.Nodes.Clear();
            interfaces.BuildSubNodes(false);
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Interface", new EventHandler(AddInterfaceHandler)));
            newItem.MenuItems.Add(new MenuItem("Structure element", new EventHandler(AddStructureElementHandler)));
            newItem.MenuItems.Add(new MenuItem("Procedure", new EventHandler(AddProcedureHandler)));
            newItem.MenuItems.Add(new MenuItem("State machine", new EventHandler(AddStateMachineHandler)));
            retVal.Add(newItem);
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            if (Item.Interfaces.Count > 0)
            {
                retVal.Add(new MenuItem("Generate inherited fields", new EventHandler(GenerateInheritedFieldsHandler)));
            }
            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }
    }
}