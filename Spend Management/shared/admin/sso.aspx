<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smTemplate.master" CodeBehind="sso.aspx.cs" Inherits="Spend_Management.SsoAdmin" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ContentPlaceHolderID="contentmenu" runat="server">
    <!-- menu items go here -->
</asp:Content>

<asp:Content ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="contentmain" runat="server">
    
    <asp:ScriptManagerProxy runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.sso.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
    <div class="sm_panel singlecolumn" style="float: left; margin-right: 10px">

        <div class="sectiontitle">
            Service Provider
        </div>
        <div class="twocolumn">
            <label>
                SAML version
            </label><span class="inputs padded">
                2.0
            </span><span class="inputicon">
                       
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.SAML_VERSION, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                 
            </span>
        </div>
        <div class="twocolumn">
            <label>
                SSO service URL
            </label><span class="inputs padded">
                 <%=Request.Url.GetLeftPart(UriPartial.Authority)%>/shared/sso.aspx
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.SERVICE_URL, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>
        <div class="twocolumn">
            <label>
                <%=this.ModuleName%> public certificate
            </label><span class="inputs padded">
                <helpers:CSSButton ID="btnSpDownloadCertificate" runat="server" Text="download" UseSubmitBehavior="False" OnClick="btnSpDownloadCertificate_OnClick" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.SEL_CERT, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>

        <div class="sectiontitle">
            Identity Provider
        </div>
        <div class="twocolumn">
            <label class="mandatory">
                Issuer*
            </label><span class="inputs">
                <asp:TextBox runat="server" ID="txtIssuerUri" MaxLength="1000" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.ISSUER_URI, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>
        <div class="twocolumn">
            <label class="mandatory">
                Identity provider public certificate*
            </label><span class="inputs padded">
                <span runat="server" ID="spnIpCertificateInfo" style="display: block; margin-bottom: 6px; line-height: 21px;"></span>
                <span class="buttonContainer">
                    <label id="lblIpPublicCertificate" class="buttonInner" style="width: auto; padding: 0;">
                        <span style="display: block; position: relative; overflow: hidden; width: 45px; height: 100%; padding: 5px 9px 0;">
                            <asp:FileUpload runat="server" ID="fupIpPublicCertificate" style="position: absolute; display: block; top: -2px; right: -4px; width: 100%; height: 29px; opacity: 0; filter: alpha(opacity=0); cursor: pointer;" />
                            change
                        </span>
                    </label>
                </span>
                <br />
                <span id="spnIpPublicCertificateFilename"></span>
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield" style="vertical-align: top;">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.IP_CERT, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>

        <div class="sectiontitle">
            SAML Request
        </div>
        <div class="twocolumn">
            <label>
                Company ID
            </label><span runat="server" id="spnCompanyId" class="inputs padded">
                        
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.COMPANY_ID, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                       
            </span>
        </div>
        <div class="twocolumn">
            <label class="mandatory">
                Company ID attribute*
            </label><span class="inputs">
                <asp:TextBox runat="server" ID="txtCompanyIdAttribute" MaxLength="200" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.COMPANY_ID_ATTRIBUTE, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>
        <div class="twocolumn">
            <label class="mandatory">
                Identifier attribute*
            </label><span class="inputs">
                <asp:TextBox runat="server" ID="txtIdentifierAttribute" MaxLength="200" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.ID_ATTRIBUTE, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>
        <div class="twocolumn">
            <label class="mandatory">
                Identifier lookup field*
            </label><span class="inputs">
                <asp:DropDownList runat="server" ID="ddlIdentifierLookupField">
                    <asp:ListItem Value="00000000-0000-0000-0000-000000000000">[None]</asp:ListItem>
                    <asp:ListItem Value="0F951C3E-29D1-49F0-AC13-4CFCABF21FDA">Email Address</asp:ListItem>
                    <asp:ListItem Value="C23858B8-7730-440E-B481-C43FE8A1DBEF">ESR Assignment Number</asp:ListItem>
                    <asp:ListItem Value="6A76898B-4052-416C-B870-61479CA15AC1">Payroll Number</asp:ListItem>
                    <asp:ListItem Value="1C45B860-DDAA-47DA-9EEC-981F59CCE795" Selected="True">Username</asp:ListItem>
                </asp:DropDownList>
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.ID_LOOKUP, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>

        <div class="sectiontitle">
            Redirection
        </div>
        <div class="twocolumn">
            <label>
                Logon error URL
            </label><span class="inputs">
                <asp:TextBox runat="server" ID="txtLoginErrorUrl" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.LOGIN_ERROR_URL, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>
        <div class="twocolumn">
            <label>
                Session timeout URL
            </label><span class="inputs">
                <asp:TextBox runat="server" ID="txtTimeoutUrl" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.TIMEOUT_URL, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>
        <div class="twocolumn">
            <label>
                Exit URL
            </label><span class="inputs">
                <asp:TextBox runat="server" ID="txtExitUrl" />
            </span><span class="inputicon">
                
            </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Sso.HELP.EXIT_URL, 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="Show help" style="border: none;" />
            </span><span class="inputvalidatorfield">
                
            </span>
        </div>

    </div>
    
    <div id="divHelp" class="comment" style="float: left; width: 300px; min-width: 300px; min-height: 200px; max-height: 720px; overflow-y: auto; display: none;">
        <h2>Example heading</h2>
        <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed id mauris et sem fermentum lobortis. <a href="http://www.google.co.uk?q=Robyn is awesome">Donec vitae scelerisque est</a>. Cras consectetur, lectus quis scelerisque vulputate, diam mauris hendrerit massa, at pellentesque est mi sed nibh. In at lorem ac felis consectetur convallis. Maecenas id massa sit amet nunc lobortis sagittis et a velit. <a href="/">In rutrum faucibus justo</a>, non rhoncus tortor congue quis. Aliquam libero elit, scelerisque nec massa quis, tempus ultrices risus. Mauris eget aliquam ligula, at placerat ex.
        </p>
        <p>
            Nullam dignissim id lectus in tincidunt. Mauris nec dapibus risus. Sed convallis arcu est, eu tincidunt tortor vulputate at. Aenean ultricies consectetur dignissim. Aliquam erat volutpat. Nunc ac eleifend turpis.
        </p>
        <ul>
            <li>Pellentesque fermentum consequat urna vitae consectetur.</li>
            <li>Sed in tincidunt nisi.</li>
            <li>Vestibulum porta pellentesque diam ac gravida.</li>
            <li>Quisque sit amet consectetur nulla. Quisque vel purus magna.</li>
            <li>Pellentesque nec justo auctor, consequat orci vitae, consectetur arcu. Praesent metus neque, tempus eget nisi ut, iaculis mollis dui.</li>
        </ul>
        <h2>Another heading which wraps on to multiple lines</h2>
        <p>
            Nunc dignissim, orci at bibendum fermentum, risus mauris bibendum neque, gravida porttitor urna ipsum id sapien. Phasellus vestibulum metus ullamcorper, finibus ex eu, gravida mauris. Vivamus molestie quis nulla in venenatis. Vestibulum luctus, tellus sit amet molestie auctor, urna mi scelerisque velit, aliquet feugiat tortor dolor at mi. Morbi at rutrum est. Duis vel erat tortor. Mauris dui tellus, tincidunt scelerisque tincidunt varius, aliquet eget mi.
        </p>
        <p style="text-align: center;">
            <img src ="/static/images/errors/missing.jpg" width="62" height="80" />
        </p>
    </div>
    
    <div style="clear: left;">
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSave" runat="server" Text="save" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancel" runat="server" Text="cancel" UseSubmitBehavior="False" />
        </div>
    </div>
    
    <iframe id="saver" name="saver" style="display: none;"></iframe>
    
    <script type="text/javascript">
        SEL.Sso.CONTENT_MAIN = '<%=this.btnSave.Parent.ClientID%>_';
        SEL.Sso.REDIRECT_URL = '<%=this.RedirectUrl%>';

        $(function ()
        {
            $('#' + SEL.Sso.CONTENT_MAIN + 'btnSave')
                .removeAttr("onclick")
                .click(function ()
                {
                    SEL.Sso.Save();
                });

            $('#' + SEL.Sso.CONTENT_MAIN + 'btnCancel')
                .removeAttr("onclick")
                .click(function ()
                {
                    document.location = SEL.Sso.REDIRECT_URL;
                });

            $('form')
                .keypress(function (e)
                {
                    if (e.which == $.ui.keyCode.ENTER)
                    {
                        $('#' + SEL.Sso.CONTENT_MAIN + 'btnSave').click();
                    }
                });

            $('#' + SEL.Sso.CONTENT_MAIN + 'fupIpPublicCertificate')
                .change(function ()
                {
                    this.blur();


                    var filename = SEL.Common.GetFilename($(this).val());
                    if (!/\.(cer|crt)$/i.test(filename))
                    {
                        $('#spnIpPublicCertificateFilename').text('');
                        $('#' + SEL.Sso.CONTENT_MAIN + 'spnIpCertificateInfo').slideDown();
                        SEL.Sso.ShowError('Please select a valid Identity provider certificate. The file must have an extension of .cer or .crt.');

                        var $fupIpPublicCertificate = $('#' + SEL.Sso.CONTENT_MAIN + 'fupIpPublicCertificate');
                        $fupIpPublicCertificate.wrap('<form>');
                        $fupIpPublicCertificate.closest('form')[0].reset();
                        $fupIpPublicCertificate.unwrap();
                    }
                    else
                    {
                        $('#spnIpPublicCertificateFilename').text(filename);
                        $('#' + SEL.Sso.CONTENT_MAIN + 'spnIpCertificateInfo').slideUp();
                    }

                });

            $('form').attr('target', 'saver');
            $('#saver').on('load', function()
            {
                var response = $(this).contents().text();
                if (response == null || response.trim() == '')
                {
                    return;
                }

                var $response = $.parseJSON(response);
                if ($response != null)
                {
                    if ($response.Success)
                    {
                        document.location = SEL.Sso.REDIRECT_URL;
                    }
                    else
                    {
                        SEL.Sso.ShowError($response.Message, $response.Controls);
                    }
                }
            });

            $('#lblIpPublicCertificate').attr('for', SEL.Sso.CONTENT_MAIN + 'fupIpPublicCertificate');
        });
    </script>
    
    <tooltip:tooltip runat="server" />
    
</asp:Content>
