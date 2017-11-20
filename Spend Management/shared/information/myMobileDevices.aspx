<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="myMobileDevices.aspx.cs" Inherits="Spend_Management.myMobileDevices" %>
<%@ Register src="~/shared/usercontrols/mobileDevices.ascx" tagName="mdev" tagPrefix="mdev" %>
<%@ Register assembly="SpendManagementHelpers" namespace="SpendManagementHelpers" tagprefix="cc2" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="styles" runat="server">
  <style type="text/css">
      html[data-useragent*='MSIE 10.0']  #ctl00_contentmain_usrMobileDevices_lblDeviceName, #ctl00_contentmain_usrMobileDevices_lblDeviceType {
                margin-top: 5px;
            }
  </style>
    
     <!--[if IE]>
        <style>
            #ctl00_contentmain_usrMobileDevices_lblDeviceName, #ctl00_contentmain_usrMobileDevices_lblDeviceType {
                margin-top: 5px;
            }
        </style>
<![endif]-->

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server"><a href="javascript:SEL.MobileDevices.LoadMobileDeviceModal(SEL.MobileDevices.LoadType.New, null);" class="submenuitem">New Mobile Device</a></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server"></asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">

<div class="formpanel formpanel_padding">
    <div class="sectiontitle">My Mobile Devices</div>
    <mdev:mdev ID="usrMobileDevices" runat="server"></mdev:mdev>
    
    <div class="formbuttons">
            <cc2:CSSButton ID="btnClose" runat="server" Text="close" 
                onclick="btnClose_Click"></cc2:CSSButton></div>
</div>
</asp:Content>
