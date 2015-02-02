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

namespace HistoricalData
{
    /// <summary>
    /// The object factory
    /// </summary>
    public class Factory : Generated.Factory
    {
        public override Generated.History createHistory()
        {
            History retVal = new History();

            return retVal;
        }

        public override Generated.Commit createCommit()
        {
            Commit retVal = new Commit();

            return retVal;
        }

        public override Generated.Change createChange()
        {
            Change retVal = new Change();

            return retVal;
        }

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static Factory __instance = new Factory();

        /// <summary>
        /// The singleton instance
        /// </summary>
        public static Factory INSTANCE
        {
            get { return __instance; }
        }
    }
}