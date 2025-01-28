using UnityEngine;
using System;

public static class PointsOnLine
{
    public static void GetPosition(Vector3 anchorL, Vector3 anchorR, 
                                      int i, int numberOfPoints, float usableRange,
                                      out Vector3 position)
    {
        // Complicated math. When called with different 0-1 values of progress,
        // returns a set of points that span arcSize portion of a circle. 
        //
        // The arc is bounded by anchorL and anchorR, and passes through 
        // anchorC. The range is further constrained by the 0-1 proportion
        // usableRange. The set of points will always be centered halfway
        // through the arc.
        // 
        // Derived from:
        // https://stackoverflow.com/a/16544330
        // ================

        float lerpIndex;

        if (numberOfPoints == 1) {
            lerpIndex = 0.5f;
        } else {
            lerpIndex = i/(float)(numberOfPoints-1);
        }

        GetPosition(anchorL, anchorR, lerpIndex, usableRange, out position);
    }

    public static void GetPosition(Vector3 anchorL, Vector3 anchorR, 
                                      float progress, float usableRange,
                                      out Vector3 position)
    {
        // Complicated math. When called with different 0-1 values of progress,
        // returns a set of points that span arcSize portion of a circle. 
        //
        // The arc is bounded by anchorL and anchorR, and passes through 
        // anchorC. The range is further constrained by the 0-1 proportion
        // usableRange. The set of points will always be centered halfway
        // through the arc.
        // 
        // Derived from:
        // https://stackoverflow.com/a/16544330
        // ================



        // Arc Distance ===============

        // The amount, from 0-1, we need to shift forward along our arc to have our cards
        // be centered after accounting for usableRange.
        float usableRange_arcOffset = 0.5f*(1-usableRange);
        // How far, from 0-1, this card should be placed on our arc. 
        float distanceOnArc = usableRange*progress + usableRange_arcOffset;

        position = Vector3.Lerp(anchorL, anchorR, distanceOnArc);
    }
}
