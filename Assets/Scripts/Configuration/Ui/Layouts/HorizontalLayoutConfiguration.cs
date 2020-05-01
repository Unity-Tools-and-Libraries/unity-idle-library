namespace IdleFramework.Configuration.UI
{
    public class HorizontalLayoutConfiguration : LayoutConfigurationBuilder
    {
        private readonly int leftPadding;
        private readonly int rightPadding;
        private readonly int topPadding;
        private readonly int bottomPadding;
        private readonly int spacing;
        private readonly ChildAxes childSize;
        private readonly ChildAxes childScale;
        private readonly ChildAxes childExpand;
        private readonly ChildAlignment childAlignemtn;
        public LayoutDirection Direction => LayoutDirection.HORIZONTAL;

        public int LeftPadding => leftPadding;

        public int RightPadding => rightPadding;

        public int TopPadding => topPadding;

        public int BottomPadding => bottomPadding;

        public int Spacing => spacing;

        public ChildAxes ControlChildSizeOnAxes => throw new System.NotImplementedException();

        public ChildAxes ControlChildScaleOnAxes => throw new System.NotImplementedException();

        public ChildAxes ForceExpandChildrenonAxes => throw new System.NotImplementedException();

        public ChildAlignment ChildAlignemnt => throw new System.NotImplementedException();
    }
}