/* <summary>Single Sign-on methods</summary> */
(function()
{
    var scriptName = "Sso";

    function execute()
    {
        SEL.registerNamespace("SEL.Sso");
        SEL.Sso =
        {
            CONTENT_MAIN: '',
            REDIRECT_URL: '',

            HELP:
            {
                SAML_VERSION: '870D42CE-8E03-46F4-95CD-D870E8F929D8',
                SERVICE_URL: '68ACB861-DF6E-41C6-823A-5FE91A89DE27',
                SEL_CERT: '1C140727-2681-4470-BE34-1E4B928F77A5',
                ISSUER_URI: 'BB88DC90-12D2-479C-A4C9-B8B6E9129BA9',
                IP_CERT: 'F5255E91-A306-4F67-9C92-21B7392F967C',
                COMPANY_ID: 'D09885CF-A3B1-40F4-89DC-05063072A974',
                COMPANY_ID_ATTRIBUTE: 'FB1478DF-663A-42D3-B528-8570137D1D1C',
                ID_ATTRIBUTE: '9A015DF1-FAFF-4F31-A70F-F1D9D9457905',
                ID_LOOKUP : 'B1F39859-5F6C-4D40-931A-C9ACA546A7C6',
                LOGIN_ERROR_URL: '558BEA0C-6888-42E0-B29F-33B0597F341B',
                TIMEOUT_URL: '5C0A6C50-0E43-4718-AE94-48922975F5CA',
                EXIT_URL: '36C50AE0-5DDA-4C40-BC08-D922C82858ED'
            },

            Save: function()
            {
                $('.inputvalidatorfield').html('');
                $('#__EVENTTARGET').val('btnSave');
                $('form').submit();
            },

            ShowError: function(message, invalidControls)
            {
                SEL.MasterPopup.ShowMasterPopup(message.replace(/\n/g, "<br />"), 'Message from ' + moduleNameHTML);
                if (invalidControls != null)
                {
                    for (var i = 0; i <= invalidControls.length; i++)
                    {
                        $('#' + SEL.Sso.CONTENT_MAIN + invalidControls[i]).closest('.twocolumn').find('.inputvalidatorfield')
                            .append($('<span/>').addClass('invalid').text('*'));
                    }
                }
            }
        };
    }
    if (window.Sys && Sys.loader)
    {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
})();
