<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" EnableTheming="true" AutoEventWireup="false" 
    Inherits="Framework2006.Attachments" CodeFile="Attachments.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript">
            function OpenAttachment()
            {
                var hiddenID;
                var IDField;
                var params;
                var att_path;
                var curpos;
                
                params = window.location.search;

                curpos = params.indexOf('action=open',0);
                if(curpos == -1)
                {
                    //alert('action= not found');
                    return;
                }
                else
                {
                    curpos = params.indexOf('path=',0);
                    //alert('curpos = ' + curpos);
                    if(curpos == -1)
                    {
                        return;
                    }
                    att_path = params.substring(curpos+5);
                    //alert('att_path= ' + att_path);
                    window.open('file://' + att_path);
                    return;
                }
            }
    </script>

    <div class="inputpanel">
        <div class="inputpaneltitle">
            Attachments&nbsp;
            <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
        <div>
            <asp:Label ID="lblStatusMsg" runat="server" ForeColor="red"></asp:Label></div>
    </div>
    <div class="inputpanel">
        <asp:Literal runat="server" ID="litAttachments"></asp:Literal></div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif"
            CausesValidation="false" />
    </div>
    <asp:Panel runat="server" ID="AddPanel" Visible="false">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Attachment Details</div>
            <table>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="att_area" runat="server" Visible="False" ReadOnly="True"></asp:TextBox>
                        <asp:TextBox ID="editID" runat="server" Visible="False">0</asp:TextBox>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="uploadFilePanel">
                    <tr>
                        <td class="labeltd">
                            Attachment Type
                        </td>
                        <td class="inputtd">
                            <asp:DropDownList runat="server" ID="lstAttachmentType">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblFileToAttach" runat="server">file to attach</asp:Label>
                        </td>
                        <td class="inputtd_wide">
                            <asp:FileUpload ID="attbrowser" runat="server" />
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblDescription" runat="server">description</asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtDescription" runat="server" Width="250px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblStorageDir" runat="server">storage dir</asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstStorageDir" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblNewSubDir" runat="server" Visible="False">create a subdir</asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtNewSubDir" runat="server" Visible="False"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <li><b>NOTE: </b>A <asp:Label runat="server" ID="lblMaxUploadValue"></asp:Label>Mb attachment size is enforced on this system.</li>
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/buttons/update.gif" />&nbsp;&nbsp;
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/buttons/cancel.gif" CausesValidation="false" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="HyperlinkPanel" Visible="false">
                <div class="inputpanel">
                    <div class="inputpaneltitle">
                        Attachment Details</div>
                    <table>
                        <tr>
                            <td colspan="3">
                                <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rdoLinkType"
                                    RepeatLayout="Flow" RepeatColumns="2" AutoPostBack="True" Width="400px">
                                    <asp:ListItem Text="Web Link" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="File Link" Value="1"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <asp:Panel runat="server" ID="panelWebLink">
                            <tr>
                                <td class="labeltd">
                                    Web Link
                                </td>
                                <td class="inputtd_wide">
                                    <asp:TextBox runat="server" ID="txtWebLink"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator runat="server" ID="reqWebLink" ControlToValidate="txtWebLink"
                                        Text="*" ErrorMessage="Field cannot be blank" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <cc1:ValidatorCalloutExtender runat="server" ID="reqexWebLink" TargetControlID="reqWebLink">
                                    </cc1:ValidatorCalloutExtender>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="panelFileLink" Visible="false">
                            <tr>
                                <td class="labeltd">
                                    Current File Link
                                </td>
                                <td class="inputtd_wide">
                                    <asp:TextBox runat="server" ReadOnly="true" ID="txtCurFileLink"></asp:TextBox>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td class="labeltd">
                                    Link URL to Attachment
                                </td>
                                <td class="inputtd_wide">
                                    <asp:FileUpload runat="server" ID="lnkFileSelect" />
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="reqLinkURL" runat="server" ControlToValidate="lnkFileSelect"
                                        ErrorMessage="Field cannot be blank" SetFocusOnError="True" Text="*"></asp:RequiredFieldValidator>
                                    <cc1:ValidatorCalloutExtender runat="server" ID="reqexLinkURL" TargetControlID="reqLinkURL">
                                    </cc1:ValidatorCalloutExtender>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td class="labeltd">
                                Description:
                            </td>
                            <td class="inputtd_wide">
                                <asp:TextBox runat="server" ID="txtLinkDescription"></asp:TextBox>
                            </td>
                            <td>
                                <cc1:ValidatorCalloutExtender ID="reqexWebLinkDesc" runat="server" TargetControlID="reqLinkDescription">
                                </cc1:ValidatorCalloutExtender>
                                <asp:RequiredFieldValidator ID="reqLinkDescription" runat="server" ControlToValidate="txtLinkDescription"
                                    ErrorMessage="Field cannot be blank" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <li>NOTE: The full URL must be specified (e.g. http://&lt;full web address link&gt;)</li>
                            </td>
                        </tr>
                    </table>
                </div>
       
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdLinkUpdate" ImageUrl="~/buttons/update.gif" />&nbsp;&nbsp;
            <asp:ImageButton runat="server" ID="cmdLinkCancel" ImageUrl="~/buttons/cancel.gif"
                CausesValidation="false" />
        </div>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="returnURL" />
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="lnkAdd" CssClass="submenuitem" ToolTip="Upload file attachment to the server"
        Visible="False">Add Attachment</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkLink" CssClass="submenuitem" Visible="false"
        ToolTip="Provide URL only. No upload takes place">Link Attachment</asp:LinkButton>
        <asp:Literal runat="server" ID="litHelp"></asp:Literal>
</asp:Content>
