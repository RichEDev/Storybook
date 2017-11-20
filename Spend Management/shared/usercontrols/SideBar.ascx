<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SideBar.ascx.cs" Inherits="Spend_Management.shared.usercontrols.SideBar" EnableViewState="True" %>
<%@ Import Namespace="SpendManagementLibrary" %>

<!-- Sidebar -->
<div id="sidebar-wrapper">
    <aside class="main-sidebar">
        <!-- sidebar: style can be found in sidebar.less -->
        <section class="sidebar">

            <!-- Sidebar user panel -->
            <div class="user-login">
                <img id="userIcon" runat="server" src="/static/images/expense/menu-icons/user2-160x160.jpg" class="img-circle" alt="User Image" />

                <p>
                    <asp:Literal ID="lituser" runat="server" meta:resourcekey="lituserResource1"></asp:Literal>
                </p>
            </div>
            <div class="clear"></div>

            <div class="leftmain-manu">
                <ul class="quickTooltip">
                   <asp:Literal runat="server" ID="ltMenuItems"></asp:Literal>

                </ul>

            </div>
        </section>
        <!-- /.sidebar -->
    </aside>
</div>
