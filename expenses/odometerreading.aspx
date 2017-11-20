<%@ Page language="c#" Inherits="expenses.odometerreading" MasterPageFile="~/expform.master" Codebehind="odometerreading.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
		<script language="javascript">
		function ok_onclick()
		{
		
			var i;
			var newamount;
			for (i = 0; i < cars.length; i++)
			{
				newamount = document.getElementById('newodo' + cars[i][0]);
				if (newamount.value == '')
				{
					alert('Please enter the new odometer reading(s) in the box(s) provided');
					return false;
				}
				if (checkInt(newamount.value) == false)
				{
					alert('Please enter a valid value for the new odometer reading.\nValid characters are the numbers 0-9');
					return false;
				}
				if (newamount.value < cars[i][1])
				{
					alert('The new odometer reading must be greater than the current odometer reading');
					return false;
				}
			}
			
			if (document.getElementById("businessmilesno") != null)
			{
			    if (document.getElementById("businessmilesno").checked == false && document.getElementById("businessmilesyes").checked == false)
			    {
			        alert('Please declare whether you have incurred any business mileage since your last reading');
			        return false;
			    }
			}
			return true;
		}
		
		
		</script>

			<div class="inputpanel">
				<div class="inputpaneltitle">
                    <asp:Label ID="lblreadings" runat="server" Text="Odometer Readings" meta:resourcekey="lblreadingsResource1"></asp:Label></div>
                <br>
				<asp:Label id="lblday" runat="server" Font-Size="Small" meta:resourcekey="lbldayResource1">Label</asp:Label><br /><br /><asp:Label id="lblReimburse" runat="server" CssClass="comment" Font-Size="Small" style="display:none" meta:resourcekey="lblReimburseResource1"></asp:Label><br /><br />
				<asp:Literal id="litcars" runat="server" meta:resourcekey="litcarsResource1"></asp:Literal>
			</div>
			<div class="inputpanel">
				<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;<asp:ImageButton 
                    ID="cmdcancel" runat="server" CausesValidation="false" 
                    ImageUrl="~/buttons/cancel_up.gif" onclick="cmdcancel_Click" />
			</div>

    </asp:Content>

