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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Constants;
using DataDictionary.Variables;
using GUI.BoxArrowDiagram;
using DataDictionary.Rules;
using DataDictionary.Types;

namespace GUI.FunctionalView
{
    public partial class FunctionalBlockControl : BoxControl<NameSpace, ProcedureOrFunctionCall>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FunctionalBlockControl()
            : base()
        {
            BoxMode = BoxModeEnum.RoundedCorners;
            BackColor = System.Drawing.Color.Transparent;
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public FunctionalBlockControl(IContainer container)
            : base(container)
        {
            BoxMode = BoxModeEnum.RoundedCorners;
            BackColor = System.Drawing.Color.Transparent;
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
        }

        /// <summary>
        /// Handles a double click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleMouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectBox();

            FunctionalAnalysisPanel panel = (FunctionalAnalysisPanel)Panel;
            if (panel != null)
            {
                FunctionalAnalysisWindow window = new FunctionalAnalysisWindow();
                panel.MDIWindow.AddChildWindow(window);
                window.SetNameSpaceContainer(Model);
                window.Text = Model.Name + " functional analysis";
            }
        }
    }
}
