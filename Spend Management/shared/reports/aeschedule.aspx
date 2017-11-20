<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="reports_aeschedule" Title="Untitled Page" Codebehind="aeschedule.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register TagPrefix="reports" TagName="criteria" Src="runtimecriteria.ascx" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
    
<div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblgeneral" runat="server" Text="General Details" meta:resourcekey="lblgeneralResource1"></asp:Label></div>
    <table>
        <tr><td class="labeltd">
            <asp:Label ID="lbloutput" runat="server" Text="Output Type" meta:resourcekey="lbloutputResource1"></asp:Label>:</td><td class="inputtd">
            <asp:DropDownList ID="cmboutputtype" runat="server" meta:resourcekey="cmboutputtypeResource1">
                <asp:ListItem Value="2" meta:resourcekey="ListItemResource1">Excel</asp:ListItem>
                <asp:ListItem Value="3" meta:resourcekey="ListItemResource2">CSV</asp:ListItem>
                <asp:ListItem Value="4" meta:resourcekey="ListItemResource15">Flat File</asp:ListItem>
            </asp:DropDownList></td></tr>
            <tr><td class="labeltd">
                <asp:Label ID="lbldelivery" runat="server" Text="Delivery Method" meta:resourcekey="lbldeliveryResource1"></asp:Label>:</td><td class="inputtd">
                <asp:DropDownList ID="cmbdeliverymethod" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbdeliverymethod_SelectedIndexChanged" meta:resourcekey="cmbdeliverymethodResource1">
                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource3">Send to my e-mail Address</asp:ListItem>
                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource4">Send to multiple e-mail address</asp:ListItem>
                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource5">Send to FTP Server</asp:ListItem>
                </asp:DropDownList></td></tr>
           <tr>
                        <td class="labeltd">
                            Email message body
                        </td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtEmailBody" TextMode="MultiLine" Rows="6"></asp:TextBox>
                        </td>
                                                <td></td>
                    </tr>
              <asp:MultiView ID="deliveryview" runat="server">
                  <asp:View ID="View4" runat="server">
                  
                    <tr><td class="labeltd">
                        <asp:Label ID="lblemailaddresses" runat="server" Text="Label">E-mail Addresses (Separate with a semicolon):</asp:Label></td><td class="inputtd">
                        <asp:TextBox ID="txtmultipleemail" runat="server"></asp:TextBox></td><td>
                            <asp:RequiredFieldValidator ID="reqemails" ControlToValidate="txtmultipleemail" Enabled=false runat="server" ErrorMessage="Please enter the e-mail addresses in the box provided" Text="*"></asp:RequiredFieldValidator></td></tr>
                 
                  </asp:View>
                  <asp:View ID="View5" runat="server">
                    
                        <tr><td class="labeltd">
                            <asp:Label ID="lblftp" runat="server" Text="Label">FTP Address:</asp:Label></td><td class="inputtd">
                            <asp:TextBox ID="txtftpaddress" runat="server"></asp:TextBox></td><td>
                                <asp:RequiredFieldValidator ID="reqftpaddress" ControlToValidate="txtftpaddress" Enabled="false" runat="server" ErrorMessage="Please enter the address of the FTP server in the box provided" Text="*"></asp:RequiredFieldValidator></td></tr>
                        <tr><td class="labeltd">
                            <asp:Label ID="lblusername" runat="server" Text="Label">Username:</asp:Label></td><td class="inputtd">
                            <asp:TextBox ID="txtftpusername" runat="server"></asp:TextBox></td><td>
                                <asp:RequiredFieldValidator ID="reqftpusername" Enabled="false" ControlToValidate="txtftpusername" runat="server" ErrorMessage="Please enter the username to use to logon to the FTP server in the box provided" Text="*"></asp:RequiredFieldValidator></td></tr>
                        <tr><td class="labeltd">
                            <asp:Label ID="lblpassword" runat="server" Text="Label">Password:</asp:Label></td><td class="inputtd">
                            <asp:TextBox ID="txtftppassword" runat="server" TextMode="Password"></asp:TextBox></td><td>
                                <asp:RequiredFieldValidator ID="reqftppassword" ControlToValidate="txtftppassword" Enabled="false" runat="server" ErrorMessage="Please enter the password to use to logon to the FTP server in the box provided" Text="*"></asp:RequiredFieldValidator></td></tr>
                        <tr><td class="labeltd"><asp:Label runat="server" ID="lblusessl">Use Secure Connection (SSL)?</asp:Label></td><td class="inputtd"><asp:CheckBox runat="server" ID="chkusessl" /></td></tr>
                  </asp:View>
              </asp:MultiView>    
        <tr><td class="labeltd">
            <asp:Label ID="lblstartdate" runat="server" Text="Start Date:" meta:resourcekey="lblstartdateResource1"></asp:Label></td><td class="inputtd">
            <asp:TextBox ID="txtstartdate" runat="server" meta:resourcekey="txtstartdateResource1"></asp:TextBox>
            
                <asp:CompareValidator ID="cvStartDate" runat="server" ErrorMessage="Start Date must be a valid date" Text="*" Type="Date" ControlToValidate="txtstartdate" Operator="DataTypeCheck" ></asp:CompareValidator> 
                <asp:CompareValidator ID="compStartMin" runat="server" ErrorMessage="The start date you have entered is invalid. Date must be later than 01/01/1900" Text="*" Type="Date" ControlToValidate="txtstartdate" Operator="GreaterThanEqual" ValueToCompare="01/01/1900" ></asp:CompareValidator>
                <asp:CompareValidator ID="compStartMax" runat="server" ErrorMessage="The start date you have entered is invalid. Date must be less than 31/12/3000" Text="*" Type="Date" ControlToValidate="txtstartdate" Operator="LessThan" ValueToCompare="31/12/3000" ></asp:CompareValidator>  
            </td></tr>
        <tr><td class="labeltd">
            <asp:Label ID="lblenddate" runat="server" Text="End Date:" meta:resourcekey="lblenddateResource1"></asp:Label></td><td class="inputtd">
            <asp:TextBox ID="txtenddate" runat="server" meta:resourcekey="txtenddateResource1"></asp:TextBox>
            
            <asp:CompareValidator ID="compEndDate" runat="server" ErrorMessage="End Date must be a valid date" Text="*" Type="Date" ControlToValidate="txtenddate" Operator="DataTypeCheck"></asp:CompareValidator>
            <asp:CompareValidator ID="compEndMin" runat="server" ErrorMessage="The end date you have entered is invalid. Date must be later than 01/01/1900" Text="*" Type="Date" ControlToValidate="txtenddate" Operator="GreaterThanEqual" ValueToCompare="01/01/1900" ></asp:CompareValidator>
            <asp:CompareValidator ID="compEndMax" runat="server" ErrorMessage="The end date you have entered is invalid. Date must be less than 31/12/3000" Text="*" Type="Date" ControlToValidate="txtenddate" Operator="LessThan" ValueToCompare="31/12/3000" ></asp:CompareValidator>  
            
            </td></tr>
    </table>
</div>
<div class="inputpanel">
<div class="inputpaneltitle">
    <asp:Label ID="lblschedule" runat="server" Text="Schedule" meta:resourcekey="lblscheduleResource1"></asp:Label></div>
    <asp:ListBox ID="lstscheduletype" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstfrequency_SelectedIndexChanged" meta:resourcekey="lstscheduletypeResource1">
        <asp:ListItem Value="1" meta:resourcekey="ListItemResource6">Day</asp:ListItem>
        <asp:ListItem Value="2" meta:resourcekey="ListItemResource7">Week</asp:ListItem>
        <asp:ListItem Value="3" meta:resourcekey="ListItemResource8">Month</asp:ListItem>
        <asp:ListItem Value="4" meta:resourcekey="ListItemResource9">Once</asp:ListItem>
    </asp:ListBox>
    <asp:MultiView ID="scheduleview" runat="server">
        <asp:View ID="View1" runat="server">
        <div class="inputpanel">
        <asp:RadioButton ID="optdays" runat="server" Text="On the following days:" GroupName="days" Checked="True" meta:resourcekey="optdaysResource1" />
        <table>
            
                <tr><td>
                    <asp:CheckBox ID="chkdayssunday" runat="server" Text="Sunday" meta:resourcekey="chkdayssundayResource1" /></td>
                    <td>
                        <asp:CheckBox ID="chkdaysmonday" runat="server" Text="Monday" meta:resourcekey="chkdaysmondayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkdaystuesday" runat="server" Text="Tuesday" meta:resourcekey="chkdaystuesdayResource1" /></td>
                        <td>
                        <asp:CheckBox ID="chkdayswednesday" runat="server" Text="Wednesday" meta:resourcekey="chkdayswednesdayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkdaysthursday" runat="server" Text="Thursday" meta:resourcekey="chkdaysthursdayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkdaysfriday" runat="server" Text="Friday" meta:resourcekey="chkdaysfridayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkdayssaturday" runat="server" Text="Saturday" meta:resourcekey="chkdayssaturdayResource1" /></td>
                    </tr>
        </table>
        </div>
        
        <div class="inputpanel">
            <asp:RadioButton ID="optweekdays" runat="server" Text="Every Weekday" GroupName="days" meta:resourcekey="optweekdaysResource1"/></div>
            <div class="inputpanel">
                <asp:RadioButton ID="optdaysnumber" runat="server" Text="Repeat after this number of days:" GroupName="days" meta:resourcekey="optdaysnumberResource1"/>&nbsp;&nbsp;<asp:TextBox
                    ID="txtnumdays" runat="server" Width="20px" meta:resourcekey="txtnumdaysResource1"></asp:TextBox>
            </div>
            <div class="inputpanel">
                <asp:Label ID="lbldaysstarttime" runat="server" Text="Start Time:" meta:resourcekey="lbldaysstarttimeResource1"></asp:Label> 
                <asp:TextBox ID="txtdayshour" runat="server" Width="25px" meta:resourcekey="txtdayshourResource1"></asp:TextBox><asp:RequiredFieldValidator
                    ID="reqDaysHour" runat="server"  ErrorMessage="Please enter the hours" ControlToValidate="txtdayshour" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeDaysHour" runat="server" ErrorMessage="Please enter a value between 0 and 23 for the hour" ControlToValidate="txtdayshour" Type="Integer" MinimumValue="0" MaximumValue="23" Text="*"></asp:RangeValidator>&nbsp;&nbsp;<asp:TextBox ID="txtdaysminutes"
                    runat="server" Width="25px" meta:resourcekey="txtdaysminutesResource1"></asp:TextBox><asp:RequiredFieldValidator
                        ID="reqDaysMinutes" runat="server" ErrorMessage="Please enter the minutes" ControlToValidate="txtdaysminutes" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeDaysMinutes" runat="server" ErrorMessage="Please enter a value between 0 and 59 for the minute" ControlToValidate="txtdaysminutes" Type="Integer" MinimumValue="0" MaximumValue="59" Text="*"></asp:RangeValidator></div>
        </asp:View>
        <asp:View ID="View2" runat="server">
        <div class="inputpanel">
            <asp:Label ID="lblrepeat" runat="server" Text="Repeat after this number of weeks:" meta:resourcekey="lblrepeatResource1"></asp:Label> 
            <asp:TextBox ID="txtnumweeks" runat="server" Width="25px" meta:resourcekey="txtnumweeksResource1"></asp:TextBox>
        </div>
        <div class="inputpanel">
            <table>
            
                <tr><td>
                    <asp:CheckBox ID="chkweeksunday" runat="server" Text="Sunday" meta:resourcekey="chkweeksundayResource1" /></td>
                    <td>
                        <asp:CheckBox ID="chkweekmonday" runat="server" Text="Monday" meta:resourcekey="chkweekmondayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkweektuesday" runat="server" Text="Tuesday" meta:resourcekey="chkweektuesdayResource1" /></td>
                        <td>
                        <asp:CheckBox ID="chkweekwednesday" runat="server" Text="Wednesday" meta:resourcekey="chkweekwednesdayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkweekthursday" runat="server" Text="Thursday" meta:resourcekey="chkweekthursdayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkweekfriday" runat="server" Text="Friday" meta:resourcekey="chkweekfridayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkweeksaturday" runat="server" Text="Saturday" meta:resourcekey="chkweeksaturdayResource1" /></td>
                    </tr>
        </table>
        </div>
        <div class="inputpanel">
            <asp:Label ID="lblweekstarttime" runat="server" Text="Start Time:" meta:resourcekey="lblweekstarttimeResource1"></asp:Label> 
                <asp:TextBox ID="txtweekhour" runat="server" Width="25px" meta:resourcekey="txtweekhourResource1"></asp:TextBox><asp:RequiredFieldValidator
                    ID="reqWeekHour" runat="server"  ErrorMessage="Please enter the hours" ControlToValidate="txtweekhour" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeWeekHour" runat="server" ErrorMessage="Please enter a value between 0 and 23 for the hour" ControlToValidate="txtweekhour" Type="Integer" MinimumValue="0" MaximumValue="23" Text="*"></asp:RangeValidator>&nbsp;&nbsp;<asp:TextBox ID="txtweekminutes"
                    runat="server" Width="25px" meta:resourcekey="txtweekminutesResource1"></asp:TextBox><asp:RequiredFieldValidator
                        ID="reqWeekMinutes" runat="server" ErrorMessage="Please enter the minutes" ControlToValidate="txtweekminutes" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeWeekMinutes" runat="server" ErrorMessage="Please enter a value between 0 and 59 for the minute" ControlToValidate="txtweekminutes" Type="Integer" MinimumValue="0" MaximumValue="59" Text="*"></asp:RangeValidator></div>
        </asp:View>
        <asp:View ID="View3" runat="server">
        <div class="inputpanel">
        Months:
        <table>
            <tr><td style="height: 35px">
                <asp:CheckBox ID="chkmonthjan" runat="server" Text="Jan" meta:resourcekey="chkmonthjanResource1" /></td><td style="height: 35px">
                    <asp:CheckBox ID="chkmonthapril" runat="server" Text="Apr" meta:resourcekey="chkmonthaprilResource1" /></td><td style="height: 35px">
                        <asp:CheckBox ID="chkmonthjuly" runat="server" Text="Jul" meta:resourcekey="chkmonthjulyResource1" /></td><td style="height: 35px">
                            <asp:CheckBox ID="chkmonthoct" runat="server" Text="Oct" meta:resourcekey="chkmonthoctResource1" /></td></tr>
            <tr><td style="height: 22px">
                <asp:CheckBox ID="chkmonthfeb" runat="server" Text="Feb" meta:resourcekey="chkmonthfebResource1" /></td><td style="height: 22px">
                    <asp:CheckBox ID="chkmonthmay" runat="server" Text="May" meta:resourcekey="chkmonthmayResource1" /></td><td style="height: 22px"><asp:CheckBox ID="chkmonthaugust" runat="server" Text="Aug" meta:resourcekey="chkmonthaugustResource1" /></td><td style="height: 22px">
                        <asp:CheckBox ID="chkmonthnov" runat="server" Text="Nov" meta:resourcekey="chkmonthnovResource1" /></td></tr>
            <tr><td>
                <asp:CheckBox ID="chkmonthmarch" runat="server" Text="Mar" meta:resourcekey="chkmonthmarchResource1" /></td><td>
                    <asp:CheckBox ID="chkmonthjune" runat="server" Text="Jun" meta:resourcekey="chkmonthjuneResource1" /></td><td>
                        <asp:CheckBox ID="chkmonthsep" runat="server" Text="Sep" meta:resourcekey="chkmonthsepResource1" /></td><td>
                            <asp:CheckBox ID="chkmonthdec" runat="server" Text="Dec" meta:resourcekey="chkmonthdecResource1" /></td></tr>
        </table>
        </div>
        <div class="inputpanel">
        <table>
            <tr><td>
                <asp:RadioButton ID="optmonthweek" runat="server" Text="On week of month:" Checked="True" GroupName="month" meta:resourcekey="optmonthweekResource1" /></td><td>
                    <asp:DropDownList ID="cmbmonthweek" runat="server" meta:resourcekey="cmbmonthweekResource1">
                    <asp:ListItem Value="1" Text="1st" meta:resourcekey="ListItemResource10"></asp:ListItem>
                    <asp:ListItem Value="2" Text="2nd" meta:resourcekey="ListItemResource11"></asp:ListItem>
                    <asp:ListItem Value="3" Text="3rd" meta:resourcekey="ListItemResource12"></asp:ListItem>
                    <asp:ListItem Value="4" Text="4th" meta:resourcekey="ListItemResource13"></asp:ListItem>
                    <asp:ListItem Value="5" Text="Last" meta:resourcekey="ListItemResource14"></asp:ListItem>
                    </asp:DropDownList></td></tr>
        </table>
        <table>
            
                <tr><td>
                    <asp:Label ID="lbldayofweek" runat="server" Text="On day of week:" meta:resourcekey="lbldayofweekResource1"></asp:Label></td><td>
                    <asp:CheckBox ID="chkmonthsunday" runat="server" Text="Sunday" meta:resourcekey="chkmonthsundayResource1" /></td>
                    <td>
                        <asp:CheckBox ID="chkmonthmonday" runat="server" Text="Monday" meta:resourcekey="chkmonthmondayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkmonthtuesday" runat="server" Text="Tuesday" meta:resourcekey="chkmonthtuesdayResource1" /></td>
                        <td>
                        <asp:CheckBox ID="chkmonthwednesday" runat="server" Text="Wednesday" meta:resourcekey="chkmonthwednesdayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkmonththursday" runat="server" Text="Thursday" meta:resourcekey="chkmonththursdayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkmonthfriday" runat="server" Text="Friday" meta:resourcekey="chkmonthfridayResource1"/></td>
                        <td>
                        <asp:CheckBox ID="chkmonthsaturday" runat="server" Text="Saturday" meta:resourcekey="chkmonthsaturdayResource1" /></td>
                    </tr>
        </table>
        
        </div>
        <div class="inputpanel">
            <table>
                <tr><td>
                    <asp:RadioButton ID="optmonthcalendar" runat="server" Text="On Calendar Day(s):" GroupName="month" meta:resourcekey="optmonthcalendarResource1"/></td><td>
                        <asp:TextBox ID="txtcalendardays" runat="server" meta:resourcekey="txtcalendardaysResource1"></asp:TextBox></td></tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:Label ID="lblmonthstarttime" runat="server" Text="Start Time:" meta:resourcekey="lblmonthstarttimeResource1"></asp:Label> 
                <asp:TextBox ID="txtmonthhour" runat="server" Width="25px" meta:resourcekey="txtmonthhourResource1"></asp:TextBox><asp:RequiredFieldValidator
                    ID="reqMonthHour" runat="server"  ErrorMessage="Please enter the hours" ControlToValidate="txtmonthhour" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeMonthHour" runat="server" ErrorMessage="Please enter a value between 0 and 23 for the hour" ControlToValidate="txtmonthhour" Type="Integer" MinimumValue="0" MaximumValue="23" Text="*"></asp:RangeValidator>&nbsp;&nbsp;<asp:TextBox ID="txtmonthminutes"
                    runat="server" Width="25px" meta:resourcekey="txtmonthminutesResource1"></asp:TextBox><asp:RequiredFieldValidator
                        ID="reqMonthMinutes" runat="server" ErrorMessage="Please enter the minutes" ControlToValidate="txtmonthminutes" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeMonthMinutes" runat="server" ErrorMessage="Please enter a value between 0 and 59 for the minute" ControlToValidate="txtmonthminutes" Type="Integer" MinimumValue="0" MaximumValue="59" Text="*"></asp:RangeValidator></div>
        </asp:View>
        <asp:View ID="View6" runat="server">
        <div class="inputpanel">
            <asp:Label ID="lbloncestarttime" runat="server" Text="Start Time:" meta:resourcekey="lbloncestarttimeResource1"></asp:Label> 
                <asp:TextBox ID="txtoncehour" runat="server" Width="25px" meta:resourcekey="txtoncehourResource1"></asp:TextBox><asp:RequiredFieldValidator
                    ID="reqOnceHour" runat="server"  ErrorMessage="Please enter the hours" ControlToValidate="txtoncehour" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeOnceHour" runat="server" ErrorMessage="Please enter a value between 0 and 23 for the hour" ControlToValidate="txtoncehour" Type="Integer" MinimumValue="0" MaximumValue="23" Text="*"></asp:RangeValidator>&nbsp;&nbsp;<asp:TextBox ID="txtonceminutes"
                    runat="server" Width="25px" meta:resourcekey="txtonceminutesResource1"></asp:TextBox><asp:RequiredFieldValidator
                        ID="reqOnceMinutes" runat="server" ErrorMessage="Please enter the minutes" ControlToValidate="txtonceminutes" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                        ID="rangeOnceMinutes" runat="server" ErrorMessage="Please enter a value between 0 and 59 for the minute" ControlToValidate="txtonceminutes" Type="Integer" MinimumValue="0" MaximumValue="59" Text="*"></asp:RangeValidator> Please allow 15 minutes for your schedule to be processed</div>
        </asp:View>
    </asp:MultiView>
    </div>
    
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblcriteria" runat="server" Text="Report Criteria" meta:resourcekey="lblcriteriaResource1"></asp:Label></div>
        <reports:criteria runat="server" ID="criteria"></reports:criteria>
    </div>
    
    <div class="inputpanel">
        <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" />&nbsp;&nbsp;<asp:ImageButton ID="cmdcancel"
            runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" OnClick="cmdcancel_Click" meta:resourcekey="cmdcancelResource1" /></div>
</asp:Content>

