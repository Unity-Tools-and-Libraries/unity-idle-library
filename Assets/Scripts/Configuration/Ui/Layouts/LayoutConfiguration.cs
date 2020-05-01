using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.Configuration.UI
{
    public interface LayoutConfigurationBuilder
    {
        LayoutDirection Direction { get; }
        int LeftPadding { get; }
        int RightPadding { get; }
        int TopPadding { get; }
        int BottomPadding { get; }
        int Spacing { get; }
        ChildAxes ControlChildSizeOnAxes { get; }
        ChildAxes ControlChildScaleOnAxes { get; }
        ChildAxes ForceExpandChildrenonAxes { get; }
        ChildAlignment ChildAlignemnt { get; }
    }

    public enum LayoutDirection
    {
        VERTICAL,
        HORIZONTAL,
        NONE
    }

    public enum ChildAlignment
    {
        UPPER_LEFT,
        UPPER_CENTER,
        UPPER_RIGHT,
        MIDDLE_LEFT,
        MIDDLE_CENTER,
        MIDDLE_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT
    }

    public enum ChildAxes
    {
        NONE,
        WIDTH,
        HEIGHT,
        WIDTH_AND_HEIGHT
    }
}