using System;
using System.Data;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary
{
    
    [Serializable]
    public class ReportChart : Chart
    {
        
        public Guid Reportid { get; set; }

        public ReportChart()
        { }

        public ReportChart(Guid reportid, ChartType displayType, string chartTitle, bool showLegend, int xAxis, int yAxis,
            int groupBy, bool cumulative, int chartTitleFont, string chartTitleColour, int textFont, string textFontColour, string textBackgroundColour, byte yAxisRange, Position legendPosition, bool showLabels, bool showValues, bool showPercent, bool enableHover, byte chartSize, int combineOthersPercentage)
            :base(displayType, chartTitle, showLegend, xAxis, yAxis, groupBy, cumulative, chartTitleFont, chartTitleColour, textFont, textFontColour, textBackgroundColour, yAxisRange, legendPosition, showLabels, showValues, showPercent, enableHover, chartSize, combineOthersPercentage)
        {
            Reportid = reportid;
        }

        public bool Save(ICurrentUserBase user, IDBConnection connection = null)
        {
            if (this.XAxis == -1 && this.YAxis == -1)
            {
                Delete(user);
                return false;
            }

            using (
                var databaseConnection = connection ??
                                         new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string sql = "dbo.SaveReportChart";
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@reportId", this.Reportid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ChartTitle", this.ChartTitle);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@chartType", this.DisplayType);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ShowLegend", this.ShowLegend);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@XAxis", this.XAxis);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@YAxis", this.YAxis);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@GroupBy", this.GroupBy);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@Cumulative", this.Cumulative);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ChartTitleFont", this.ChartTitleFont);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ChartTitleColour", this.ChartTitleColour);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@TextFont", this.TextFont);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@TextFontColour", this.TextFontColour);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@TextBackgroundColour", this.TextBackgroundColour);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@YAxisRange", this.YAxisRange);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@LegendPosition", this.LegendPosition);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ShowLabels", this.ShowLabels);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ShowValues", this.ShowValues);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ShowPercent", this.ShowPercent);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@EnableHover", this.EnableHover);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@ChartSize", this.Size);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@CollectedThresholdPercent", this.CombineOthersPercentage);
                databaseConnection.ExecuteProc(sql);
                
            }

            return true;
        }

        private void Delete(ICurrentUserBase user)
        {
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string sql = "delete from ReportCharts where reportId = @reportId";
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@reportId", this.Reportid);
                databaseConnection.ExecuteSQL(sql);
            }
        }

        public static ReportChart Get(Guid reportId, ICurrentUserBase user, IDBConnection connection = null)
        {
            var result = new ReportChart {Reportid = reportId, XAxis = -1, YAxis = -1};
            using (
                var databaseConnection = connection ??
                                         new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string sql = "dbo.GetReportChart";
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@reportId", reportId);
                using (var reader = databaseConnection.GetReader(sql, CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        result.ChartTitle = reader.GetString(reader.GetOrdinal("chartTitle"));
                        result.Reportid = reader.GetGuid(reader.GetOrdinal("reportId"));
                        result.DisplayType = (ChartType) reader.GetByte(reader.GetOrdinal("chartType"));
                        result.ShowLegend = reader.GetBoolean(reader.GetOrdinal("showLegend"));
                        result.XAxis = reader.GetInt32(reader.GetOrdinal("xAxis"));
                        result.YAxis = reader.GetInt32(reader.GetOrdinal("yAxis"));
                        result.GroupBy = reader.GetInt32(reader.GetOrdinal("groupBy"));
                        result.Cumulative = reader.GetBoolean(reader.GetOrdinal("cumulative"));
                        result.ChartTitleFont = reader.GetInt32(reader.GetOrdinal("chartTitleFont"));
                        result.ChartTitleColour = reader.GetString(reader.GetOrdinal("ChartTitleColour"));
                        result.TextFont = reader.GetInt32(reader.GetOrdinal("textFont"));
                        result.TextFontColour = reader.GetString(reader.GetOrdinal("textFontColour"));
                        result.TextBackgroundColour = reader.GetString(reader.GetOrdinal("textBackgroundColour"));
                        result.YAxisRange = reader.GetByte(reader.GetOrdinal("YAxisRange"));
                        result.LegendPosition = (Position) reader.GetByte(reader.GetOrdinal("LegendPosition"));
                        result.ShowLabels = reader.GetBoolean(reader.GetOrdinal("ShowLabels"));
                        result.ShowValues = reader.GetBoolean(reader.GetOrdinal("ShowValues"));
                        result.ShowPercent = reader.GetBoolean(reader.GetOrdinal("ShowPercent"));
                        result.EnableHover = reader.GetBoolean(reader.GetOrdinal("EnableHover"));
                        result.Size = reader.GetByte(reader.GetOrdinal("ChartSize"));
                        result.CombineOthersPercentage = reader.GetInt32(reader.GetOrdinal("CollectedThresholdPercent"));
                    }
                    reader.Close();
                }
            }

            return result;
        }
    }
}
