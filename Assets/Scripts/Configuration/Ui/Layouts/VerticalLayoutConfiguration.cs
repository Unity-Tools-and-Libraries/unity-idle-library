using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.UI.Layouts
{
    public class VerticalLayoutConfiguration : LayoutConfigurationBuilder
    {
        private readonly int leftPadding;
        private readonly int rightPadding;
        private readonly int topPadding;
        private readonly int bottomPadding;
        private readonly int spacing;
        private readonly ChildAlignment childAlignment;
        private readonly ChildAxes childSize;
        private readonly ChildAxes childScale;
        private readonly ChildAxes childExpand;

        public VerticalLayoutConfiguration() : this(ChildAlignment.MIDDLE_CENTER, ChildAxes.NONE, ChildAxes.NONE, ChildAxes.NONE, 0, 0, 0, 0, 0) { }
        public VerticalLayoutConfiguration(ChildAlignment childAlignment) : this(childAlignment, ChildAxes.NONE, ChildAxes.NONE, ChildAxes.NONE, 0, 0, 0, 0, 0) { }
        public VerticalLayoutConfiguration(ChildAlignment childAlignment, int spacing) : this(childAlignment, ChildAxes.NONE, ChildAxes.NONE, ChildAxes.NONE, spacing, 0, 0, 0, 0) { }
        public VerticalLayoutConfiguration(ChildAlignment childAlignment, ChildAxes childSize, ChildAxes childScale, ChildAxes childExpand, int spacing, int leftPadding, int rightPadding, int topPadding, int bottomPadding)
        {
            this.leftPadding = leftPadding;
            this.rightPadding = rightPadding;
            this.topPadding = topPadding;
            this.bottomPadding = bottomPadding;
            this.spacing = spacing;
            this.childAlignment = childAlignment;
        }

        public LayoutDirection Direction => LayoutDirection.VERTICAL;

        public int LeftPadding => leftPadding;

        public int RightPadding => rightPadding;

        public int TopPadding => topPadding;

        public int BottomPadding => bottomPadding;

        public int Spacing => spacing;

        public ChildAlignment ChildAlignemnt => childAlignment;

        public ChildAxes ControlChildSizeOnAxes => throw new System.NotImplementedException();

        public ChildAxes ControlChildScaleOnAxes => throw new System.NotImplementedException();

        public ChildAxes ForceExpandChildrenonAxes => throw new System.NotImplementedException();
    }
}