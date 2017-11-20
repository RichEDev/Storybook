<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploader.ascx.cs" Inherits="Spend_Management.shared.usercontrols.FileUploader" %>

<script type="text/javascript">
    $(document).ready(function ()
    {
        var guidControlId = '<%= txtGuid.ClientID%>';
        var pathControlId = '<%= txtImageLibraryFilePath.ClientID%>';
        var nameControlId = '<%= txtImageLibraryFileName.ClientID%>';
        var typeControlId = '<%= txtUploadType.ClientID%>';
        var changedControlId = '<%= txtChanged.ClientID%>';
        var attributeId = '<%= this.AttributeId%>';
        var hyperlinkControlId = '<%= this.attachmentLink.ClientID%>';
        var replacementControlId = '<%= this.replacementText.ClientID%>';
        var mandatory = '<%=this.Mandatory%>';
        var uploadFile = '<%=this.fileUpload.ClientID%>';
        var uploadLibrary = '<%=this.ImageLibraryButton.ClientID%>';
        var uploadBoth = '<%=this.UploadButton.ClientID%>';
        var ie9OrLess = '<%=this.Ie9OrLess%>';
        var includeImageLibrary = new Boolean('<%=this.IncludeImageLibrary%>');

        $('#' + guidControlId).css('display', 'none');
        $('#' + pathControlId).css('display', 'none');
        $('#' + nameControlId).css('display', 'none');
        $('#' + typeControlId).css('display', 'none');
        $('#' + changedControlId).css('display', 'none');
        $('#' + uploadFile).css('display', 'none');
        $('#' + uploadLibrary).css('display', 'none');
        $('#' + uploadBoth).css('display', 'none');

        if (ie9OrLess == 'True') {
            $('#' + uploadFile).css('display', '');
            if (includeImageLibrary == true) {
                $('#' + uploadLibrary).css('display', '');
            }
        }
        else {
            $('#' + uploadBoth).css('display', '');
        }

        var deleteIcon = $('img[id*=img' + attributeId + ']');
        if (deleteIcon.length > 0) {
            var fileGuid = $('#' + guidControlId).val();
            if (fileGuid == '' || fileGuid == '00000000-0000-0000-0000-000000000000' || mandatory == 'True')
            {
                deleteIcon.css('display', 'none');
            }

            deleteIcon.click(function () { javascript: ClearAttachment(changedControlId, hyperlinkControlId, replacementControlId, deleteIcon, typeControlId); });
        }
    });
</script>

<asp:FileUpload ID="fileUpload" runat="server" Width="85px" Height="23px" />
<input type="button" runat="server" id="UploadButton" style="display:none; width: 85px; height: 23px;" value="browse ..." />
<input type="button" runat="server" id="ImageLibraryButton" style="display:none; width: 85px; height: 23px;" value="Library..." />
<asp:HyperLink ID="attachmentLink" runat="server" Text="" style="vertical-align: central"></asp:HyperLink>
<span id="replacementText" runat="server" style="vertical-align: text-bottom;"></span>
<asp:RequiredFieldValidator runat="server" ID="reqFileUploader" ControlToValidate="txtUploadType"  ValidationGroup="" Text="*" ErrorMessage="Please select a valid {0}." Display="Dynamic" Enabled="False"></asp:RequiredFieldValidator>
<asp:TextBox runat="server" ID="txtUploadType"></asp:TextBox>
<asp:TextBox runat="server" ID="txtGuid"></asp:TextBox>
<asp:TextBox runat="server" ID="txtImageLibraryFilePath"></asp:TextBox>
<asp:TextBox runat="server" ID="txtImageLibraryFileName"></asp:TextBox>
<asp:TextBox runat="server" ID="txtChanged"></asp:TextBox>
