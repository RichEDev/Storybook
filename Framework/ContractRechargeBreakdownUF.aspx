<%@ Page Title="" Language="C#" MasterPageFile="~/FWMaster.master" AutoEventWireup="true" CodeFile="ContractRechargeBreakdownUF.aspx.cs" Inherits="ContractRechargeBreakdownUF" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<script language="javascript" type="text/javascript" src="callback.js"></script>
<%-- <asp:Panel runat="server" ID="UFPanel"></asp:Panel>--%>
<asp:PlaceHolder runat="server" ID="phRAUFields"></asp:PlaceHolder>
<div class="inputpanel">
<asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/buttons/update.gif" 
        onclick="cmdUpdate_Click" />
<asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/buttons/cancel.gif" 
        CausesValidation="false" onclick="cmdCancel_Click" />
</div>
<asp:HiddenField runat="server" ID="returnURL" />
</asp:Content>

