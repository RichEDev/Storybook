namespace SpendManagementLibrary
{
    using System;
    [Serializable]
    public abstract class Chart
    {
        public enum ChartType
        {
            Area = 0,
            Bar = 1,
            Column = 2,
            Donut = 3,
            Dot = 4,
            Line = 5,
            Pie = 6,
            Funnel = 7
        }

        public enum Position
        {
            None = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 3,
            BottomRight = 4
        }

        protected Chart()
        {
        }

        protected Chart(ChartType displayType, string chartTitle, bool showLegend, int xAxis, int yAxis, int groupBy, bool cumulative, int chartTitleFont, string chartTitleColour, int textFont, string textFontColour, string textBackgroundColour, byte yAxisRange, Position legendPosition, bool showLabels, bool showValues, bool showPercent, bool enableHover, byte size, int combineOthersPercentage)
        {
            DisplayType = displayType;
            ChartTitle = chartTitle;
            ShowLegend = showLegend;
            XAxis = xAxis;
            YAxis = yAxis;
            GroupBy = groupBy;
            Cumulative = cumulative;
            ChartTitleFont = chartTitleFont;
            ChartTitleColour = chartTitleColour;
            TextFont = textFont;
            TextFontColour = textFontColour;
            TextBackgroundColour = textBackgroundColour;
            YAxisRange = yAxisRange;
            LegendPosition = legendPosition;
            ShowLabels = showLabels;
            ShowValues = showValues;
            ShowPercent = showPercent;
            EnableHover = enableHover;
            Size = size;
            CombineOthersPercentage = combineOthersPercentage;
        }

        public ChartType DisplayType { get; set; }
        public string ChartTitle { get; set; }
        public bool ShowLegend { get; set; }
        public int XAxis { get; set; }
        public int YAxis { get; set; }
        public int GroupBy { get; set; }
        public bool Cumulative { get; set; }
        public int ChartTitleFont { get; set; }
        public string ChartTitleColour { get; set; }
        public int TextFont { get; set; }
        public string TextFontColour { get; set; }
        public string TextBackgroundColour { get; set; }
        public byte YAxisRange { get; set; }
        public Position LegendPosition { get; set; }
        public bool ShowLabels { get; set; }
        public bool ShowValues { get; set; }
        public bool ShowPercent { get; set; }
        public bool EnableHover { get; set; }
        public byte Size { get; set; }
        public int CombineOthersPercentage { get; set; }
    }
}