<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="fields_selector.ascx.cs" Inherits="Spend_Management.fields_selector" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
<Services><asp:ServiceReference InlineScript="false" Path="~/shared/webservices/svcWorkflows.asmx" /></Services>
</asp:ScriptManagerProxy>
<%@ Register TagName="autoComplete" TagPrefix="autoComplete" Src="~/shared/usercontrols/auto_complete.ascx" %>

<style type="text/css">
.fieldstbl 
{
        height: 25px;
}
    
table 
{
    width: 400px;
}    

.datatbl select  
{
    width: 150px;
}

</style>
<script type="text/javascript" language="javascript">
    var criteriaMode = '<%=sAction%>';
    var focusedFieldDD = "";
    var focusedTableDD = "";
    var focusedConditionDD;
    var focusedValueSpan;
    var focusedFieldID;
    var globalRowIndex = 0;    
</script>
<div class="hiddendiv" id="fieldSelDiv">
<div id="fieldselectordiv">
<table id="tbl" class="datatbl" style="width: 710px;" border="1">
<thead><tr>
<th width="20"><img src="/shared/images/icons/delete2.gif" height="16" width="16" alt="X" title="X" /></th>
<th style="width: 150px;">Element</th>
<th style="width: 150px;">Field</th>
<th style="width: 150px;">Operator</th>
<th style="width: 290px;">Value 1</th>
<th style="width: 145px;">Value 2</th>
<th style="width: 40px;">Runtime</th></tr></thead>
<tbody></tbody></table>
</div>
</div>
<autoComplete:autoComplete ID="autoComplete1" runat="server" />

