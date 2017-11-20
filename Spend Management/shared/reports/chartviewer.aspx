<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_chartviewer" MasterPageFile="~/masters/smForm.master" Codebehind="chartviewer.aspx.cs" %>




<%@ Register TagPrefix="igchart" Namespace="Infragistics.WebUI.UltraWebChart" Assembly="Infragistics4.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
     <div>
        <asp:DropDownList ID="cmdcharttype" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmdcharttype_SelectedIndexChanged">
            <asp:ListItem>Area Chart</asp:ListItem>
            <asp:ListItem>Bar Chart</asp:ListItem>
            <asp:ListItem>Box Chart</asp:ListItem>
            <asp:ListItem Selected="True">Column Chart</asp:ListItem>
            <asp:ListItem>Doughnut Chart</asp:ListItem>
            <asp:ListItem>Funnel Chart</asp:ListItem>
            <asp:ListItem>Line Chart</asp:ListItem>
            <asp:ListItem>Pie Chart</asp:ListItem>
            <asp:ListItem>Pyramid Chart</asp:ListItem>
            <asp:ListItem>Scatter Chart</asp:ListItem>
            <asp:ListItem>Spline Chart</asp:ListItem>
        </asp:DropDownList>
        <igchart:UltraChart ID="chartrpt" runat="server" BackgroundImageFileName="" BorderWidth="0px" EmptyChartText="" Height="600px" Version="6.3" Width="1000px">
            <Axis>
                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Z>
                <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                        Orientation="Horizontal" VerticalAlign="Center" ItemFormatString="&lt;DATA_VALUE:00.##&gt;">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y2>
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X>
                <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y>
                <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                        Orientation="VerticalLeftFacing" VerticalAlign="Center" ItemFormatString="&lt;ITEM_LABEL&gt;">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X2>
                <PE ElementType="None" Fill="Cornsilk" />
                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Z2>
            </Axis>
            <Effects>
                <Effects>
                    <igchartprop:GradientEffect>
                    </igchartprop:GradientEffect>
                </Effects>
            </Effects>
            <ColorModel AlphaLevel="150" Scaling="Oscillating">
            </ColorModel>
            <Legend Visible="True"></Legend>
        </igchart:UltraChart>
        <br />
        <br />
        <asp:CheckBox ID="chk3d" runat="server" AutoPostBack="True" OnCheckedChanged="chk3d_CheckedChanged"
            Text="3D" />
        <asp:CheckBoxList ID="chkcolumns" runat="server" AutoPostBack="True" OnSelectedIndexChanged="chkcolumns_SelectedIndexChanged">
        </asp:CheckBoxList></div>
 </asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentleft">
        <div class="panel">
            <div class="paneltitle">
            Chart Options
            </div>
        </div>
        
        <div class="framework"></div>
    </asp:Content>

