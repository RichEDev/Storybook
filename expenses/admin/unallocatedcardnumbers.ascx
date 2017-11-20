<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="unallocatedcardnumbers.ascx.cs" Inherits="expenses.admin.unallocatedcardnumbers" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<script type="text/javascript" language="javascript">
    function match(cardnumber)
    {
        
        var modal = $find('<%= modunallocated.ClientID %>');
        modal.show();
        var txt = document.getElementById('<%= txtcardnumber.ClientID %>');
        txt.value = cardnumber;
    }
</script>

<asp:UpdatePanel ID="upnlcards" runat="server">
<ContentTemplate>
<div><asp:Literal runat="server" ID="litMessage"></asp:Literal></div>
<igtbl:UltraWebGrid ID="gridunallocatednumbers" runat="server" 
                    SkinID="gridskin" 
    OnInitializeLayout="gridunallocatednumbers_InitializeLayout"
    oninitializerow="gridunallocatednumbers_InitializeRow">
</igtbl:UltraWebGrid>
                </ContentTemplate>
</asp:UpdatePanel>
<asp:Panel ID="pnlunallocated" runat="server" CssClass="modalpanel">
    <asp:UpdatePanel ID="upnlmatching" runat="server">
        <ContentTemplate>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="viewsearch" runat="server">
                    <div class="inputpanel">
        <div class="inputpaneltitle">Match unallocated card number</div>
       
            <div class="inputpanel">
            <table>
            <tr>
					<td class="labeltd">
						<asp:Label id="lblsurname" runat="server" meta:resourcekey="lblsurnameResource1">Enter surname of employee (or lead characters)</asp:Label>
					</td>
					<td class="inputtd">
						<asp:TextBox id="txtsurname" runat="server" meta:resourcekey="txtsurnameResource1"></asp:TextBox>
					</td>
				</tr>
							    <tr>
			        <td class="labeltd">
			            <asp:Label id="lblusername" runat="server" meta:resourcekey="lblusernameResource1">Username:</asp:Label>
			        </td>
                    <td class="inputtd">
						<asp:TextBox id="txtusername" runat="server" meta:resourcekey="txtusernameResource1"></asp:TextBox>
					</td>
			    </tr>
			</table>
			</div>
			<div class="inputpanel">
			<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" 
                    meta:resourcekey="cmdokResource1" onclick="cmdok_Click"></asp:ImageButton>&nbsp;&nbsp;<asp:ImageButton
                        ID="cmdcancel" runat="server" ImageUrl="~/buttons/cancel_up.gif" />
		</div>
                </asp:View>
                <asp:View ID="viewresults" runat="server">
                <div class="inputpanel">
            <igtbl:UltraWebGrid ID="gridemployees" runat="server" SkinID="gridskin" 
                   oninitializelayout="gridemployees_InitializeLayout" oninitializerow="gridemployees_InitializeRow" 
                   >
                    </igtbl:UltraWebGrid>
                    <asp:Label ID="lblmsg" runat="server" Text="Label" Visible="false" ForeColor="Red"></asp:Label>   
           </div>
           <div class="inputpanel">
               <asp:ImageButton ID="cmdallocate" runat="server" onclick="cmdallocate_Click" ImageUrl="~/shared/images/buttons/btn_save.png" />&nbsp;&nbsp;<asp:ImageButton
                   ID="cmdallocatecancel" runat="server" ImageUrl="~/buttons/cancel_up.gif" 
                   onclick="cmdallocatecancel_Click" />
           </div>
                </asp:View>
            </asp:MultiView>
    
           
        
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
<cc1:ModalPopupExtender ID="modunallocated" runat="server" TargetControlID="lnkunallocated" PopupControlID="pnlunallocated" BackgroundCssClass="modalBackground" CancelControlID="cmdcancel">
</cc1:ModalPopupExtender>
<asp:LinkButton ID="lnkunallocated" runat="server" style="display: none;">LinkButton</asp:LinkButton>
<asp:TextBox ID="txtcardnumber" runat="server" style="display:none;"></asp:TextBox>