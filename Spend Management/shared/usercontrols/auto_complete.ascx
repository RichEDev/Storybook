<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="auto_complete.ascx.cs" Inherits="Spend_Management.auto_complete" %>

<asp:ScriptManagerProxy ID="smpAutoComplete" runat="server">
<Services>
<asp:ServiceReference Path="~/shared/webServices/svcAutoComplete.asmx"  />
</Services>
<Scripts>
<asp:ScriptReference Path="~/shared/javaScript/auto_complete.js" />
</Scripts>
</asp:ScriptManagerProxy>
<script language="javascript" type="text/javascript">

    function autoComplete(searchElementID, popupPanelID, fieldID, searchType, minimumCharacters, onCompleteMethod)
    {
        var searchBox = document.getElementById(searchElementID);
        
        if(popupPanelID == null) 
        {
            popupPanelID = '<% = pnlAutoComplete.ClientID %>';
        }
        
        if(isNaN(minimumCharacters) == true || minimumCharacters == null)
        {
            minimumCharacters = 3;
        }
        
        if(searchBox.value.length >= minimumCharacters)
        {
            if(onCompleteMethod == null) 
            {
                onCompleteMethod = "onAutoComplete";
            }
            Spend_Management.svcAutoComplete.AutoComplete(searchBox.value, fieldID, searchElementID, popupPanelID, searchType, AutoCompleteDisplay, errorGettingString);
        }
        return;
    }

    function AutoCompleteDisplay(results) {
        var searchBox = results[0];
        var popupElement = results[1];
        var searchResults = results[2];
        var popupContents = "";

        for (var x = 0; x < searchResults.length; x++) {
            popupContents += "<div onclick=\"autoCompleteResult('" + searchBox + "', '" + searchResults[x] + "');\">" + searchResults[x] + "</div>";
        }

        document.getElementById(popupElement).innerHTML = popupContents;

        $find('<% = pceAutoComplete.ClientID %>')._popupBehavior._parentElement = document.getElementById(searchBox);
        $find('<% = pceAutoComplete.ClientID %>').showPopup();

        return;
    }

    function autoCompleteResult(searchBox, autoCompleteValue) {
        document.getElementById(searchBox).value = autoCompleteValue;
        $find('<% = pceAutoComplete.ClientID %>').hidePopup();
        return;
    }
    
    
    

</script>

<asp:TextBox ID="txtAutoComplete" runat="server" style="visibility: hidden;"></asp:TextBox>
<asp:Panel ID="pnlAutoComplete" runat="server" style="background-color: #cccccc;"></asp:Panel>
<asp:HyperLink ID="lnkAutoComplete" runat="server" NavigateUrl="javascript:void(0);" Text="&nbsp;" style="visibility: hidden;"></asp:HyperLink>
<cc1:PopupControlExtender ID="pceAutoComplete" runat="server" TargetControlID="lnkAutoComplete" PopupControlID="pnlAutoComplete" OffsetY="20"></cc1:PopupControlExtender>