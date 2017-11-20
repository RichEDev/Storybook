<%@ Page language="c#" Inherits="expenses.information.faqs" MasterPageFile="~/exptemplate.master" Codebehind="faqs.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<%@ Register tagprefix="igtbl" namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<div class="inputpanel">
<p>Please click on a category to expand it.</p>
<cc1:Accordion ID="accordionFaqs" runat="server" HeaderCssClass="accordianheader" RequireOpenedPane="false" EnableViewState="false" SelectedIndex="-1" SuppressHeaderPostbacks="false" >
    
</cc1:Accordion></div>
</asp:Content>
			
		
	
