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
using System.Diagnostics;
using System.Reflection;
using ErtmsSolutions.SiUnits;
using log4net;

namespace ErtmsSolutions.Etcs.Subset26.BrakingCurves
{
    /**@brief A one-dimensional curve, where X and Y are SiUnits. The CurveSegment S parameter can be a ConstantSegment or a QuadraticSegment */

    public class Curve<S, XUnit, YUnit>
        where S : CurveSegment<XUnit, YUnit>
        where XUnit : ISiUnit<XUnit>
        where YUnit : ISiUnit<YUnit>
    {
        /************************************************************/
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<S> mySegments;

        /************************************************************/

        public Curve()
        {
            mySegments = new List<S>();
        }

        /************************************************************/

        public void AddSegment(S aSegment)
        {
            mySegments.Add(aSegment);

            mySegments.Sort(CompareSegments);
        }

        /**@brief dor sorting purposes */

        public static int CompareSegments(S a, S b)
        {
            if (a.X.X0.ToUnits() < b.X.X0.ToUnits())
            {
                return -1;
            }
            else if (a.X.X0.ToUnits() > b.X.X0.ToUnits())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /**@brief Public access to the segments for plotting, ...*/

        public int SegmentCount
        {
            get { return mySegments.Count; }
        }

        /**@brief Public access to the segments for plotting, ...*/

        public S this[int idx]
        {
            get { return mySegments[idx]; }
        }

        /**@brief Return the value Y at any position X inside the curve segments. Throws if x is outside curve domain.*/

        public YUnit GetValueAt(XUnit x, BrakingCurveDirectionEnum dir)
        {
            S theSegment = GetSegmentAt(x, dir);
            if (theSegment != null)
            {
                return theSegment.Get(x);
            }
            else
            {
                Debugger.Launch();
            }
            throw new ArgumentException();
        }

        /**@brief Return segment that contains X or null */

        public S GetSegmentAt(XUnit x, BrakingCurveDirectionEnum dir)
        {
            switch (dir)
            {
                case BrakingCurveDirectionEnum.Backwards:
                    foreach (S s in mySegments)
                    {
                        if (s.X.Contains(x))
                        {
                            return s;
                        }
                    }
                    break;
                case BrakingCurveDirectionEnum.Forwards:
                    foreach (S s in mySegments)
                    {
                        if (s.X.X0.ToUnits() < x.ToUnits() && x.ToUnits() <= s.X.X1.ToUnits())
                        {
                            return s;
                        }
                    }
                    break;
            }
            return null;
        }


        /************************************************************/

        public void Dump(string Title)
        {
            Log.DebugFormat("**************{0}**************", Title);
            foreach (S s in mySegments)
                Log.DebugFormat(s.ToString());
        }
    }
}