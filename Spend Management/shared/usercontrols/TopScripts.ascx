<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopScripts.ascx.cs" Inherits="Spend_Management.shared.usercontrols.TopScripts" EnableViewState="false" %>
<%@ Import Namespace="SpendManagementLibrary" %>
<!-- Loads all the global Css -->
<!-- Tooltip core css for customization -->
 <link href="<%=GlobalVariables.StaticContentLibrary%>/styles/expense/css/jquery.qtip.min.css" rel="stylesheet" type="text/css" />
<!-- Bootstrap Core CSS -->
    <link href="<%=GlobalVariables.StaticContentLibrary%>/styles/expense/css/AdminLTE.css" rel="stylesheet" type="text/css" />    
    <!-- Theme style -->
    <link href="<%=GlobalVariables.StaticContentLibrary%>/styles/expense/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <!-- FontAwesome 4.3.0 -->
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css" rel="stylesheet" type="text/css" /> 
   
 <!-- Custom CSS -->
    <link href="<%=GlobalVariables.StaticContentLibrary%>/styles/expense/css/simple-sidebar.css" rel="stylesheet" type="text/css" />
    
     <link href="<%=GlobalVariables.StaticContentLibrary%>/styles/expense/css/style.css" rel="stylesheet" type="text/css" />

  <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
  <style type="text/css">
      .reportsErrorMessage {
          margin-top: 10px;
      }
       .originalLogoSize {
           width: 100%;
           max-height: 60px;
       }

      .shrinkLogo {
          max-width: 190px;
          max-height: 40px;
          text-align: center;
          margin-left: 50px;
          margin-top: 10px;
      }

      .hideElement {
          display: none;
      }
      .bodyScrollDisable {
          overflow: hidden;
      }
      .udf_tooltips {
          padding-left: 0px;
      }

      .passengersdialog .ui-dialog-titlebar {
          margin-top: 5px;
          margin-bottom: 25px;
      }

      #instructions a {
           text-decoration: underline;
           color: #003768;
       }

      .formpanel a {
          color: #003768;
          text-decoration: underline;
      }

      #ctl00_contentmain_cmbfilter, #ctl00_contentmain_lstFilter {
          margin-left: 103px;
          width: 173px;
      }

      .cgridnew-filter input {
          width: 169px;
      }

      .checkbox, .radio {
          margin-top: 0px;
          margin-bottom: 15px;
      }

      .ajax__tab_inner a:focus {
          outline: none;
      }

      .tooltipCustomClass {
          background-color: #004990;
          width: auto;
          font-size: 14px;
      }

      .tooltipCustomClass .qtip-content {
          color: #FFF;
      }

      .hideTooltip {
          display: none !important;
      }

      .ajax__calendar_body {
          height: 148px !important;
      }

      .ajax__calendar_footer {
          margin-top: 10px !important;
      }

      .ajax__calendar_day {
          height: 20px !important;
      }

      html[data-useragent*='MSIE 10.0'] .ajax__tab_xp .ajax__tab_header .ajax__tab_active .ajax__tab_tab:focus {
          outline: none;
      }

      html[data-useragent*='MSIE 10.0'] .datatbl INPUT[type="checkbox"], html[data-useragent*='MSIE 10.0'] .datatbl INPUT[type="radio"] {
          border: none;
      }

      html[data-useragent*='MSIE 10.0'] input[type="checkbox"], html[data-useragent*='MSIE 10.0'] input[type="radio"] {
          border: none;
      }

      html[data-useragent*='MSIE 10.0'] .formpanel .fillspan {
          margin-top: -5px;
      }

      @-ms-viewport {
          width: device-width;
      }  

      th {
          text-align: center !important;
      }
      
      td input[type="checkbox"] {
          float: left;
          margin: 0 auto;
          width: 100%;
      }

      .breadcrumb_arrow {
          padding-left: 3px;
          padding-right: 5px;
      }

      .inputs {
          display: inline-block;
      }

      .formpanel .onecolumnsmall .inputs, .formpanel .onecolumn .inputs {
          display: inline-block;
      }

      .ellide div {
          display: inline-block !important;
      }

      .ctl00contentmainwizreportusrcolumnsWebGroupBox1ctl {
          margin-left: 15px;
      }

      html[data-useragent*='MSIE 10.0'] .inputtd input[type="checkbox"] {
          border: none;
      }

      .inputpanel a {
          color: #003768;
          text-decoration: underline;
      }

      .mandatory {
          font-weight: 600 !important;
      }

       .ui-widget-content a {
            text-decoration: underline;
            color: #003768;
        }

      #btnFieldSave, #btnFieldCancel, #btnItemRoleSave, #btnItemRoleCancel, #btnExpenseItemSave, #btnExpenseItemCancel {
                padding-left: 7px;
                padding-right: 7px;
                background-image: url('/static/icons/16/new-icons/button.gif');
                color: #ffffff;
                background-repeat: no-repeat;
                height: 27px;
                line-height: 25px;
                display: inline-block;
                font-weight: normal;
                font-family: arial, sans-serif;
                font-size: 13px;
                text-decoration: none;
                border: 0px;
                    }

        td a {
           color: #003768;
           text-decoration: underline;
       }

     .twocolumn a {
            color: #003768;
            text-decoration: underline;
        }

      .inputs a {
         text-decoration: underline;
         color: #003768;
     }

      .cGrid a {
         color: #003768;
          text-decoration: underline;
     }

      .sm_panel a {
            text-decoration: underline;
            color: #003768;
        }

      .row1 input[type="radio"], .row2 input[type="radio"] {
          margin-right: -5px !important;
      }

      #btnOkSessionTimeOut {
          background: transparent;
          border: none;
          margin-right: 0px;
          outline-width: 0px !important;
          outline: none;
      }

      .claimView .ui-dialog-buttonpane button, .hideBackGroundUiModal{
          background: transparent;
          border: none;
          margin-right: 0px;
          outline-width: 0px !important;
      }

      .expenseList .ui-dialog-buttonpane button {
          background: transparent;
          border: none;
          margin-right: 0px;
          outline-width: 0px !important;
      }

      @media all and (-ms-high-contrast: none), (-ms-high-contrast: active) {
          .claimView .formpanel_padding {
              padding-right: 20px;
          } 
         .inputs .AutoComboselect { max-width: 166px;}
         .twocolumn .AutoComboMargin { margin-top: 3px}
      } 

      #divMasterPopup {
          max-height: 500px;
          overflow-y: auto;
      }

      html[data-useragent*='MSIE 10.0'] td input[type=radio]{
          vertical-align: middle;
      }

      html[data-useragent*='MSIE 10.0'] .inputradio{
          width:175px;
      }

      html[data-useragent*='MSIE 10.0'] .inputradio td input[type=radio]{
          margin-right:0px;
          margin-left:3px!important;
      }

     html[data-useragent*='MSIE 10.0'] #criteriaListHeader li{
        padding-left: 13px!important;
    }

    html[data-useragent*='MSIE 10.0'] .groupIcon{
        margin-left:5px;
    }
  </style>

<!--[if IE]>
<style>

    .frameworkApp .formpanel .twocolumn .inputs input{
        padding-right:4px;
    }

    .formpanel .fillspan{
        margin-top:-5px;
    }

    .datatbl input[type="checkbox"], .datatbl input[type="radio"], input[type="checkbox"] {
        border:none;
    }

    .inputtd input[type="checkbox"] {
        border:none;
        outline: none;
    }
    .frameworkApp.twocolumn > span > img, .twocolumn span.inputicon input[type='image'] {
        margin-top: 0;
    }

    td input[type=radio]{
        vertical-align: middle;
    }

    .inputradio{
        width:175px;
    }

    .inputradio td input[type=radio]{
        margin-right:0px;
        margin-left:3px!important;
    }

    #criteriaListHeader li{
    padding-left: 13px!important;
    }

    .groupIcon{
    margin-left:5px;
    }
    
    .main-content-area-padding{
     padding-bottom: 40px !important;
    }
</style>
<![endif]-->

<!--[if IE 7]>
<style>
    #sidebar-wrapper {
        width: 230px;
    }
    .main-content-area-ie7 {
        margin-left: -10px;
    }
    .ajax__tab_xp .ajax__tab_body {
        margin-top: -6px;
    }

    .ajax__tab_xp .ajax__tab_header {
        background: none !important;
        display: inline-block;
    }

    .ajax__tab_xp .ajax__tab_header .ajax__tab_outer {
        background: none !important;
        height: 34px !important;
    }

    .ajax__tab_xp .ajax__tab_body {
        border-top: #999999 1px solid !important;
        margin-top: -6px;
    }

    .leftmain-manu ul li {
        padding-bottom: 0;
        margin: 5px 0 0 0;
        padding-left: 0;
}
    .inputradio td input[type=radio]{
         margin-right:0px;
        margin-left:3px!important;
    }

    .leftmain-manu ul li p {
        cursor: pointer;
    }
    .user-login p {
        margin-top: 14px;
        margin-left: 65px;
    }
    .user-login {
        width: 230px;
    }
    .nav {
        left: 0px;
    }
    .datatbl input[type="checkbox"], .datatbl input[type="radio"], input[type="checkbox"] {
        border:none;
    }   
    .left-menu-click-back{
        left:16px;
        top:276px;
    
    }
    .inputs {
        display: inline !important;
    }
    .formpanel .onecolumnsmall, .formpanel .onecolumn, .formbuttons  {
        display: inline-block;
    }
     
    .ajax__tab_xp .ajax__tab_header .ajax__tab_tab{
        padding:5px !important;
    
    }

    .ajax__tab_xp .ajax__tab_header .ajax__tab_tab SPAN{
        font-weight:normal;
    }


    .datatbl INPUT{
        border:none;
    }

    .top-header-nav{
        left: 0px;
        margin-top: 5px;
    }

    .top-header-nav img{
        margin-top: 3px;
    }

    .ellide DIV{
        display:inline !important;
        white-space:nowrap;
    }
    #wrapper {
        padding-left: 240px;
    }
    #wrapper.toggled {
        padding-left: 0px;  
    }

    #wrapper.toggled #sidebar-wrapper {
        width: 0px;
    }

    .submenuholder{
        position:static;
        top:0px;
        float:left;
        margin-top:0px;
    }
    .pull-right{
        width:100%;
    }
    .pull-right img{
        float:right;
    }
    .breadcrumb > .active {
        margin-left: 74px;
        margin-top: -16px;

    }
    #maindiv{
        position:relative;
        left:0px;
       
    }
        
    .breadcrumb{
        margin-left:0px;
    }
   
    .well {
        min-height: 20px;
        padding: 19px;
        min-height: 80px;
        margin-bottom: 20px;
        background-color: #fff;
        border: 1px solid #e3e3e3;
        border-radius: 4px;
        -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.05);
        box-shadow: inset 0 1px 1px rgba(0,0,0,.05);
    }
    .col-md-6{
        float:left;
        width:48%;
        height:150px;

    }
    .col-md-6 a{
        cursor:hand;

    }
    .media-body{
        margin-top:12px;
    }
    .top-header{
        height:62px;
        width:100%;
    }

    .footer-menu-master img {
        max-width: 190px;
        max-height: 40px;
        margin-top: -45px;
        margin-right: 20px;
    }

    .main-footer img {
        max-width: 190px;
        max-height: 40px;
        margin-top: -45px;
        margin-right: 20px;
    }
    .footer-menu-master {
        position: static!important;
    }

    #wrapper {
        padding-left: 240px;
    }
    #wrapper.toggled {
        padding-left: 73px;  
    }

    #wrapper.toggled #sidebar-wrapper {
        width: 60px;
        overflow: hidden;
    }
    #sidebar-wrapper {
        width: 240px;  
    }
    .user-login {
        width: 210px;
        margin-bottom: 10px;
    } 

    .leftmain-manu ul li{
      padding-bottom: 0;
        margin:0px;
    }

     .leftmain-manu{
    width: 190px;
    }

</style>
<![endif]-->

<!--[if IE 8]>
<style>
     .main-content-area-padding {
     padding-bottom: 40px !important;
    }

    .datatbl input[type="checkbox"], .datatbl input[type="radio"], input[type="checkbox"] {
        border:none;
    }   
          #ctl00_contentmain_tabsGeneralOptions_tabGeneral_lblonlycashcredit {
              margin-left: -7px;   
      }
    #img330, #imgtooltip511 {
        margin-left: 4px;
    }
    .user-login {
        width: 210px;
    }
    .ellide DIV{
        display:inline !important;
        white-space:nowrap;
    
    }
    .ellide SPAN{
        white-space:nowrap;
        width:140px;
    }
    #wrapper {
        padding-left: 240px;
    }
    #wrapper.toggled {
        padding-left: 73px;  
    }

    #wrapper.toggled #sidebar-wrapper {
        width: 60px;
        overflow: hidden;
    }
    #sidebar-wrapper {
        width: 230px;  
    }
    
    .well{
        min-height:20px;
    }
    .media-body{
        margin-top:12px;
    }
    .breadcrumb > .active {
        margin-left: 0px;
        margin-top: -16px;

    }
    #maindiv{
        position:relative;
        left:0px;
    }
    .breadcrumb{
        margin-left:0px;
    }
   
    .well {
        min-height: 20px;
        padding: 19px;
        min-height: 80px;
        margin-bottom: 20px;
        background-color: #fff;
        border: 1px solid #e3e3e3;
        border-radius: 4px;
        -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.05);
        box-shadow: inset 0 1px 1px rgba(0,0,0,.05);
    }
    .col-md-6{
        float:left;
        width:48%;
        height:150px;

    }
   
    .top-header{
        height:62px;
        width:100%;
    }

    .footer-menu-master img {
        max-height: 30px;
    }

    .top-header-nav {
        margin-top: 5px;
    }

    
</style>
<![endif]-->



