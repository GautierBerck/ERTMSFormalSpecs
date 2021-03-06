﻿// ------------------------------------------------------------------------------
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

using System.ComponentModel;
using DataDictionary;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;

namespace GUI.FunctionalView
{
    /// <summary>
    ///     A box editor
    /// </summary>
    public class NameSpaceEditor : BoxEditor<IEnclosesNameSpaces, NameSpace, AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="control"></param>
        public NameSpaceEditor(BoxControl<IEnclosesNameSpaces, NameSpace, AccessMode> control)
            : base(control)
        {
        }

        [Category("Description")]
        [ReadOnly(true)]
        public override string Name
        {
            get { return Control.TypedModel.GraphicalName; }
        }
    }
}