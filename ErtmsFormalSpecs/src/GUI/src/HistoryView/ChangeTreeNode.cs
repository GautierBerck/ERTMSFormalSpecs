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
using HistoricalData;

namespace GUI.HistoryView
{
    public class ChangeTreeNode : ModelElementTreeNode<Change>
    {
        /// <summary>
        /// The editor
        /// </summary>
        private class ChangeEditor : Editor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="model"></param>
            public ChangeEditor()
            {
            }

            /// <summary>
            /// The commit author
            /// </summary>
            public string Author { get { return Item.Commit.getCommitter(); } }

            /// <summary>
            /// The commit message
            /// </summary>
            public string Message { get { return Item.Commit.getMessage(); } }

            /// <summary>
            /// The commit date
            /// </summary>
            public DateTime Date { get { return Item.Commit.Date; } }

            /// <summary>
            /// The commit action
            /// </summary>
            [Category("Description"), TypeConverter(typeof(Converters.ChangeActionConverter))]
            public HistoricalData.Generated.acceptor.ChangeOperationEnum Action { get { return Item.Action; } }

            /// <summary>
            /// The commit field change
            /// </summary>
            [Category("Description")]
            public string Field { get { return Item.Field; } }
        }

        /// <summary>
        /// Instanciates the editor
        /// </summary>
        /// <returns></returns>
        protected override ModelElementTreeNode<Change>.Editor createEditor()
        {
            return new ChangeEditor();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public ChangeTreeNode(HistoricalData.Change item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        /// Raised when the selection has changed
        /// </summary>
        public override void SelectionChanged(bool displayStatistics)
        {
            Window window = BaseForm as Window;
            if (window != null)
            {
                window.SelectionChanged(Item);
            }

            base.SelectionChanged(displayStatistics);
        }
    }
}
