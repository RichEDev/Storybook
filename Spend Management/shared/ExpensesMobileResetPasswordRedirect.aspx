<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpensesMobileResetPasswordRedirect.aspx.cs" Inherits="Spend_Management.shared.ExpensesMobileResetPasswordRedirect" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Expenses Mobile Forgotten Details</title>
    <link rel="stylesheet" type="text/css" media="screen" href="~/shared/css/ForgottenDetailsRedirect.css" />
    <script id="jquery" type="text/javascript" src="/static/js/jQuery/jquery-1.9.1.min.js"></script>

    <script type="text/javascript">
        function redirectMobile() {

            var authToken = getQueryVariable("key");
            window.location.href = "expensesmobile://authenticate?key=" + authToken;
        }
        function redirectWeb() {

            var authToken = getQueryVariable("key");
            window.location.href = "../shared/authenticate.aspx?key=" + authToken;
        }

        $(document).ready(setTimeout(function () {

          var deviceRegEx = new RegExp('Android|webOS|iPhone|iPad|Windows Phone|Opera Mini|IEMobile|Mobile', 'i');

          //Attempt to redirect for mobile devices.
          if (deviceRegEx.test(navigator.userAgent))
                redirectMobile();
            else
                redirectWeb();       
        }, 300));

        //Attempts to return the value from the query string for the provided variable name.
        function getQueryVariable(variable) {
            var query = window.location.search.substring(1);
            var values = query.split("&");
            for (var i = 0; i < values.length; i++) {
                var pair = values[i].split("=");
                if (pair[0] === variable) { return pair[1]; }
            }
            return (false);
        }
    </script>
</head>
<body>
    <div id="desc">
        <div>
            <img src="/static/images/branding/company/ExpensesLogoWhite.png" alt="ExpensesLogoWhite" />
        </div>
        <br />
        <p class="desc">
            If you are using your mobile device, tap Launch Expenses Mobile to
        <br />
            reset your password. Otherwise, tap Launch Expenses Web, where you
        <br />
            will be directed to the Expenses website to change your password.
        <br />

        </p>
        <br />
        <a id="launch" href="javascript:redirectMobile()">Launch Expenses Mobile </a>
        <span id="buttonSpacing"></span>
        <a id="launch" href="javascript:redirectWeb()">Launch Expenses Web </a>
    </div>
</body>
</html>
