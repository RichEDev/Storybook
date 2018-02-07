<%@ Page language="c#" Inherits="expenses.admin.printout" MasterPageFile="~/exptemplate.master" Codebehind="printout.aspx.cs"%>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <script language="javascript" type="text/javascript">
        function selectedFields(fields) {
            for(var i=0; i < fields.length; i++) {
                $("input:checkbox[value='" + fields[i] +"']").attr("checked", true);
            }
        }

        function save() {
            var fieldids = [];
            var allcheckedboxes = $('input:checkbox:checked');
            for (var i = 0; i < allcheckedboxes.length; i++) {
                fieldids.push(allcheckedboxes[i].value);
            }
            PageMethods.SaveSelectedFields(accountid, fieldids);
            window.location = "/tailoringmenu.aspx";
        }

        $(document).ready(function() {
            PageMethods.GetSelectedFields(accountid, selectedFields);
        });

    </script>
    
	<div class="inputpanel">&nbsp;
		<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">By default when an employee prints their claim form, the Claim Name, their name and today's date will be printed at the top of the form.</asp:Label>
	</div>
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <a href="javascript:save()"><img alt="save" src="/shared/images/buttons/btn_save.png"/></a>&nbsp;&nbsp;
            <asp:ImageButton id="ImageButton2" runat="server" ImageUrl="../buttons/cancel_up.gif" OnClick="ImageButton2_Click" meta:resourcekey="ImageButton2Resource1"></asp:ImageButton>          
        </div>
    </div>    
    </asp:Content>

