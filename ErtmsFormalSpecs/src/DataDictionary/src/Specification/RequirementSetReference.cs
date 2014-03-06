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

namespace DataDictionary.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a reference to a requirement set
    /// </summary>
    public class RequirementSetReference : Generated.RequirementSetReference
    {
        public override System.Collections.ArrayList EnclosingCollection
        {
            get
            {
                Paragraph paragraph = (Paragraph)Enclosing;

                return paragraph.allRequirementSets();
            }
        }

        /// <summary>
        /// Provides the requirement set referenced by this requirement set reference
        /// </summary>
        public RequirementSet Ref
        {
            get
            {
                return EFSSystem.findRequirementSet(Name);
            }
        }
    }
}
