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
using DataDictionary.Specification;
using DataDictionary.Types;
using GUI.SpecificationView;

namespace GUI.DataDictionaryView
{
    public class StructuresTreeNode : ModelElementTreeNode<NameSpace>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public StructuresTreeNode(NameSpace item, bool buildSubNodes)
            : base(item, buildSubNodes, "Structures", true)
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

            foreach (Structure structure in Item.Structures)
            {
                if (!structure.IsAbstract)
                {
                    subNodes.Add(new StructureTreeNode(structure, recursive));
                }
            }

            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        ///     Adds a structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddHandler(object sender, EventArgs args)
        {
            Item.appendStructures(Structure.CreateDefault(Item.Structures, false));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> { new MenuItem("Add", AddHandler) };

            return retVal;
        }

        /// <summary>
        ///     Accepts drop of a tree node, in a drag & drop operation
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);

            if (sourceNode is StructureTreeNode)
            {
                StructureTreeNode structureTreeNode = sourceNode as StructureTreeNode;
                Structure structure = structureTreeNode.Item;

                structureTreeNode.Delete();
                Item.appendStructures(structure);
            }
            else if (sourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode node = sourceNode as ParagraphTreeNode;
                Paragraph paragraph = node.Item;

                Structure structure = Structure.CreateDefault(Item.Structures, false);
                Item.appendStructures(structure);
                structure.FindOrCreateReqRef(paragraph);
            }
        }
    }
}