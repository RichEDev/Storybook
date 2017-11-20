<%@ Page language="c#" Inherits="expenses.setupview" MasterPageFile="~/exptemplate.master" Codebehind="setupview.aspx.cs" %>
<%@ Register TagPrefix="ignav" Namespace="Infragistics.WebUI.UltraWebNavigator" Assembly="Infragistics4.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="styles">
    <style type="text/css">
        td input[type="checkbox"]{
            width:12px!important;
            float:none!important;
            margin:2px;
        }

    </style>
    </asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
		<div class="valdiv">
			<asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" meta:resourcekey="lblmsgResource1">Label</asp:Label>
		</div>
		<div class="inputpanel">
			<div class="inputpaneltitle">
                <asp:Label ID="lblfields" runat="server" Text="Fields" meta:resourcekey="lblfieldsResource1"></asp:Label>
			</div>
			<TABLE style="width:500px;">
				<TR>
					<TD rowspan="4">
                        <asp:UpdatePanel ID="upnlfields" runat="server" RenderMode="Inline">
                            <ContentTemplate>
						<ignav:UltraWebTree id="lstavailable" runat="server" Width="250px"  
                           BorderStyle=Outset Height="400px"
							BorderWidth="1px" CheckBoxes="True" DefaultImage="" HiliteClass="" HoverClass="" 
                            meta:resourcekey="lstavailableResource1" LoadOnDemand="Manual" 
                            ondemandload="lstavailable_DemandLoad"></ignav:UltraWebTree>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                            </TD>
					<TD></TD>
					<TD rowspan="4">
						<asp:ListBox style="border: 1px solid #ccc;" id="lstselected" runat="server" Width="250px" Height="400px" meta:resourcekey="lstselectedResource1"></asp:ListBox></TD>
					<td>
						<asp:ImageButton id="cmdup" runat="server" ImageUrl="~/shared/images/new-buttons/b_ctrl_up.gif" meta:resourcekey="cmdupResource1"></asp:ImageButton></td>
				</TR>
				<TR>
					<TD>
						<asp:ImageButton id="cmdright" runat="server" ImageUrl="~/shared/images/new-buttons/right_up.gif" meta:resourcekey="cmdrightResource1"></asp:ImageButton></TD>
					<td rowspan="3" valign="top">
						<asp:ImageButton id="cmddown" runat="server" ImageUrl="~/shared/images/new-buttons/b_ctrl_down.gif" meta:resourcekey="cmddownResource1"></asp:ImageButton></td>
				</TR>
				<TR>
					<TD>
						<asp:ImageButton id="cmdleft" runat="server" ImageUrl="~/shared/images/new-buttons/left_up.gif" meta:resourcekey="cmdleftResource1"></asp:ImageButton></TD>
				</TR>
				<TR>
					<TD>
						<asp:ImageButton id="cmddleft" runat="server" ImageUrl="~/shared/images/new-buttons/dleft_up.gif" meta:resourcekey="cmddleftResource1"></asp:ImageButton></TD>
				</TR>
			</TABLE>
		</div>
		<div class="inputpanel">
			<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
			<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="~/buttons/cancel_up.gif" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
		<asp:TextBox id="txtviewid" runat="server" Visible="False" meta:resourcekey="txtviewidResource1"></asp:TextBox>
            </asp:Content>
	

