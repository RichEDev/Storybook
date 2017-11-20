<%@ Page language="c#" Inherits="Spend_Management.aecar" MasterPageFile="~/masters/smForm.master" Codebehind="aecar.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register tagPrefix="aeCars" tagName="aeCar" src="~/shared/usercontrols/aeCars.ascx" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

<aeCars:aeCar ID="aeCar" runat="server" />


    </asp:Content>
