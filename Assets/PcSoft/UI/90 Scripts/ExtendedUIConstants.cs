﻿namespace PcSoft.UI._90_Scripts
{
    internal static class ExtendedUIConstants
    {
        public const string Root = "Extended UI";

        public static class Menus
        {
            private const string ComponentMenu = Root + "/Component";
            
            public static class Components
            {
                private const string UiMenu = ComponentMenu + "/UI";

                public static class Ui
                {
                    public const string ComponentMenu = UiMenu + "/Components";
                    public const string WindowMenu = UiMenu + "/Window";
                }
            }
        }
    }
}
