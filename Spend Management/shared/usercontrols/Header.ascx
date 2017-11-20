<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="Spend_Management.shared.usercontrols.Header" %>
<%@ Import Namespace="SpendManagementLibrary" %>

<div class="top-header">
    <div class="top-header-logo"><a href="/"><asp:Literal ID="litlogo" runat="server"></asp:Literal><%--<img src="<%=GlobalVariables.StaticContentLibrary%>/images/expense/EXP152-wp.png" alt="Software qurope" />--%></a></div>
    
    <div class="nav" id="menu-toggle">
        <div class="r_nav nav_click"><span></span></div>
        

    </div>
</div>