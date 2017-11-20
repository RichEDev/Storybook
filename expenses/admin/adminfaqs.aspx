
<%@ Page language="c#" Inherits="expenses.admin.adminfaqs" MasterPageFile="~/exptemplate.master" Codebehind="adminfaqs.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>



	
				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aefaqcategory.aspx" class="submenuitem"><asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Add FAQ Category</asp:Label></a>
				
				<asp:LinkButton id="cmdaddfaq" runat="server" CssClass="submenuitem" onclick="cmdaddfaq_Click" meta:resourcekey="cmdaddfaqResource1"><asp:Label id="Label4" runat="server" meta:resourcekey="Label4Resource1">Add FAQ</asp:Label></asp:LinkButton>
				
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
<script language="javascript">
				
				function deleteFAQ(faqid)
				{
					if (confirm('Are you sure you wish to delete the selected FAQ?'))
					{
						
						url = "adminfaqs.aspx?action=3";
						doCallBack(url,"faqid=" + faqid);
						document.getElementById('faqs').deleteRow(getIndex('faqs',faqid));
					}
				}
				
				function deleteFAQCategory(faqcategoryid)
				{
					if (confirm('Are you sure you wish to delete the selected FAQ category?'))
					{

						url = "adminfaqs.aspx?action=4";
						doCallBack(url,"faqcategoryid=" + faqcategoryid);
						document.getElementById('faqcategories').deleteRow(getIndex('faqcategories',faqcategoryid));
					}
				}
		
</script>
<script language="javascript" src="../callback.js"></script>			
	<div class="valdiv">
		<asp:Label id="lblcatmsg" runat="server" Visible="False" ForeColor="Red" meta:resourcekey="lblcatmsgResource1">Label</asp:Label>
		<asp:Label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Frequently Asked Question Categories</asp:Label></div>
	</div>
	<div class="inputpanel"><asp:Literal id="litcategories" runat="server" EnableViewState="False" meta:resourcekey="litcategoriesResource1"></asp:Literal></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Frequently Asked Questions</asp:Label></div>
		<asp:Literal id="litfaqs" runat="server" EnableViewState="False" meta:resourcekey="litfaqsResource1"></asp:Literal>
	</div>

    </asp:Content>


