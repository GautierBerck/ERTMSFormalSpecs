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
using DataDictionary;

namespace GUI
{
    public abstract class ReqRelatedTreeNode<T> : ReferencesParagraphTreeNode<T>
        where T : ReqRelated
    {
        /// <summary>
        /// The editor for message variables
        /// </summary>
        protected class ReqRelatedEditor : ReferencesParagraphEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            protected ReqRelatedEditor()
                : base()
            {
            }

            /// <summary>
            /// Indicates if the item is completely implemented
            /// </summary>
            [Category("Meta data")]
            public virtual bool Implemented
            {
                get { return Item.getImplemented(); }
                set { Item.setImplemented(value); }
            }

            /// <summary>
            /// Indicates if the item is completely verified
            /// </summary>
            [Category("Meta data")]
            public virtual bool Verified
            {
                get { return Item.getVerified(); }
                set { Item.setVerified(value); }
            }

            /// <summary>
            /// Indicates that the req related item does not need to be attached to a requirement
            /// </summary>
            [Category("Meta data")]
            public bool NeedsRequirement
            {
                get { return Item.getNeedsRequirement(); }
                set { Item.setNeedsRequirement(value); }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        protected ReqRelatedTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false, bool addRequirements = true)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
        {
        }

        /// <summary>
        /// Mark the item as implemented
        /// </summary>
        public void ImplementedHandler(object sender, EventArgs args)
        {
            Item.setImplemented(true);
        }

        /// <summary>
        /// Mark the item as verified
        /// </summary>
        public void VerifiedHandler(object sender, EventArgs args)
        {
            Item.setVerified(true);
        }

        /// <summary>
        /// Deletes the selected item
        /// </summary>
        public override void DeleteHandler(object sender, EventArgs args)
        {
            base.DeleteHandler(sender, args);
            Item.setVerified(false);
        }

        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            MenuItem newItem = new MenuItem("Mark as...");
            newItem.MenuItems.Add(new MenuItem("Implemented", new EventHandler(ImplementedHandler)));
            newItem.MenuItems.Add(new MenuItem("Verified", new EventHandler(VerifiedHandler)));
            retVal.Insert(2, newItem);

            return retVal;
        }
    }
}