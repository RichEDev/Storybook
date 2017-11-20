<%@ Page Language="C#" MasterPageFile="~/menu.Master" AutoEventWireup="true" Inherits="restricted" Title="Untitled Page" CodeBehind="restricted.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/menu.master" %>

<asp:Content runat="server" ID="customStyles" ContentPlaceHolderID="head">
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


