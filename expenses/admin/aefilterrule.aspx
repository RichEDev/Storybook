<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aefilterrule.aspx.cs" MasterPageFile="~/expform.master" Inherits="expenses.admin.aefilterrule" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register TagPrefix="ignav" Namespace="Infragistics.WebUI.UltraWebNavigator" Assembly="Infragistics4.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div class="valdiv">
        <asp:ValidationSummary id="ValidationSummary1" runat="server" Width="496px" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
		<asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>
    
    <div class="inputpanel">
	    <div class="inputpaneltitle">
	        <asp:Label id="lblFilterRule" runat="server" meta:resourcekey="lblFilterRuleResource1">Filter Rule</asp:Label>
	    </div>
        <asp:Table ID="tblFilterRule" runat="server">
            <asp:TableRow>
                <asp:TableCell CssClass="labeltd">
                    <asp:Label id="lblParent" runat="server" meta:resourcekey="lblParentResource1">Parent</asp:Label>
                </asp:TableCell>
                <asp:TableCell CssClass="inputtd">
                    <asp:DropDownList id="cmbparent" runat="server" onselectedindexchanged="cmbparent_SelectedIndexChanged" AutoPostBack="True">
			        <asp:ListItem meta:resourcekey="ListItemResource1">Costcode</asp:ListItem>
			        <asp:ListItem meta:resourcekey="ListItemResource2">Department</asp:ListItem>
			        <%--<asp:ListItem meta:resourcekey="ListItemResource3">Address</asp:ListItem>--%>
		            <asp:ListItem>Projectcode</asp:ListItem>
                    <asp:ListItem>Reason</asp:ListItem>
		            </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>&nbsp;&nbsp;&nbsp;<img id="imgtooltip354" onclick="SEL.Tooltip.Show('77f6c652-f892-441f-9f28-758b72da5a86', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/>&nbsp;&nbsp;&nbsp;</asp:TableCell>
                <asp:TableCell CssClass="labeltd">
                    <asp:Label id="lblChild" runat="server" meta:resourcekey="lblChildResource1">Child</asp:Label>
                </asp:TableCell>
                <asp:TableCell CssClass="inputtd">
                    <asp:DropDownList id="cmbchild" runat="server">
			        <asp:ListItem meta:resourcekey="ListItemResource1">Costcode</asp:ListItem>
			        <asp:ListItem meta:resourcekey="ListItemResource2">Department</asp:ListItem>
		            <asp:ListItem>Projectcode</asp:ListItem>
                    <asp:ListItem>Reason</asp:ListItem>
		            </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell><img id="imgtooltip355" onclick="SEL.Tooltip.Show('583df51d-9af2-43ac-b7d9-6dd9b3b2eeba', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></asp:TableCell>
                <asp:TableCell>
                   &nbsp;&nbsp;&nbsp; <asp:ImageButton id="cmdAdd" runat="server" ImageUrl="/shared/images/icons/16/plain/add2.png" meta:resourcekey="cmdAddResource1"></asp:ImageButton>
                </asp:TableCell>
        </asp:TableRow>
	    </asp:Table>
	   
	</div>
	
	<div class="inputpanel">
	    <div class="inputpaneltitle">
	    <asp:Label id="lblRuleVals" runat="server" meta:resourcekey="lblRuleValsResource1" Visible="False">Filter Rule Value Selection</asp:Label></div>
        <asp:UpdatePanel ID="pnlRuleVals" runat="server" Visible="False">
        <ContentTemplate>
            <asp:Table ID="tblRuleVals" runat="server">
                <asp:TableRow>
                    <asp:TableCell CssClass="labeltd">
                        <asp:Label ID="lblParentSearch" runat="server"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="cellcmbpar" CssClass="inputtd">
                        <asp:DropDownList ID="cmbParVals" runat="server">
                        </asp:DropDownList>
                    </asp:TableCell>
                    <asp:TableCell ID="celltxtpar" CssClass="inputtd">
                        <asp:TextBox ID="txtParentSearch" runat="server"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <cc1:AutoCompleteExtender ID="autoCompPar" runat="server" MinimumPrefixLength="1" TargetControlID="txtParentSearch" ServicePath="~/shared/webServices/svcAutocomplete.asmx">
                        </cc1:AutoCompleteExtender>
                    </asp:TableCell>
                    <asp:TableCell CssClass="labeltd">
                        <asp:Label ID="lblChildSearch" runat="server"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="cellcmbchild" CssClass="inputtd">
                        <asp:DropDownList ID="cmbChildVals" runat="server">
                        </asp:DropDownList>
                    </asp:TableCell>
                    <asp:TableCell ID="celltxtchild" CssClass="inputtd">
                        <asp:TextBox ID="txtChildSearch" runat="server"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell>
                        <cc1:AutoCompleteExtender ID="autoCompChild" runat="server" MinimumPrefixLength="1" TargetControlID="txtChildSearch" ServicePath="~/shared/webServices/svcAutocomplete.asmx">
                        </cc1:AutoCompleteExtender>
                    </asp:TableCell>
                    <asp:TableCell>
                    <asp:ImageButton id="cmdAddVal" runat="server" ImageUrl="/shared/images/icons/16/plain/add2.png" meta:resourcekey="cmdAddValResource1"></asp:ImageButton>
                </asp:TableCell>
                </asp:TableRow>
            </asp:Table>&nbsp
            
            <div class="inputpaneltitle">
	        <asp:Label id="lblValues" runat="server">Filter Rule Values</asp:Label></div>
	        
            <ignav:UltraWebTree id="lstFilterRuleVals" runat="server" Width="250px"  BorderStyle="Solid" Height="400px" 
				BorderWidth="1px" CheckBoxes="True" DefaultImage="" HiliteClass="" HoverClass="" 
                meta:resourcekey="lstavailableResource1" LoadOnDemand="Manual" 
                onnodeclicked="lstFilterRuleVals_NodeClicked"></ignav:UltraWebTree>
          
        </ContentTemplate>
        </asp:UpdatePanel>
			
	</div>
	
	<div class="inputpanel"><asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
    
    <script type="text/javascript">
        $(function()
        {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args)
            {
                var ex = args.get_error();
                if (ex != null)
                {
                    SEL.MasterPopup.ShowMasterPopup(ex.message.substr("Sys.WebForms.PageRequestManagerServerErrorException: ".length), 'Message from ' + moduleNameHTML);
                    args.set_errorHandled(true);
                }
            });
        });
    </script>

</asp:Content>