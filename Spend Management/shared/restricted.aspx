<%@ Page Language="C#" MasterPageFile="~/masters/smMenu.Master" AutoEventWireup="true" Inherits="restrictedPage" Title="Restricted" Codebehind="restricted.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/masters/smMenu.master" %>

<asp:Content runat="server" ID="customStyles" ContentPlaceHolderID="Styles">
    <style type="text/css">
        .main-content-area {
            width: 100% !important;
            background: none !important;
        }

        .inputpanel {
            padding-left: 26px;
        }

            .inputpanel > label {
                font-size: 18px;
                line-height: 35px;
                margin-bottom: 50px;
            }

            .inputpanel a {
                color: #003768;
                text-decoration: underline;
                font-weight: bold;
                 text-transform: lowercase;
            }
    </style>
</asp:Content>


