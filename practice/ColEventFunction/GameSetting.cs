using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSetting
{

    public enum ColEventType
    {
        None = 0,
        A = 1,
        B = 2,
        C = 3
    }

    public enum ColEventCase
    {
        None = 0,
        AB = 1,
        BC = 2,
        AC = 3
    }

    class EventTypeSetting
    {
        public static ColEventCase GetColEventCase(ColEventType a, ColEventType b)
        {
            if ((a == ColEventType.A && b == ColEventType.B)
                || (a == ColEventType.B && b == ColEventType.A))
                return ColEventCase.AB;


            if ((a == ColEventType.B && b == ColEventType.C)
                || (a == ColEventType.C && b == ColEventType.B))
                return ColEventCase.BC;

            if ((a == ColEventType.A && b == ColEventType.C)
               || (a == ColEventType.C && b == ColEventType.A))
                return ColEventCase.AC;

            return ColEventCase.None;
        }
    }

}

