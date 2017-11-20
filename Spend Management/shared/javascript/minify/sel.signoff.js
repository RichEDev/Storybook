/* <summary>Vehicle Engine Type methods</summary> */
(function()
{
    var scriptName = "Signoff";

    function execute()
    {
        SEL.registerNamespace("SEL.Signoff");
        SEL.Signoff =
        {
            CONTENT_MAIN: '',

            HELP:
            {
                PAY_BEFORE_VALIDATE: '7B0B3A55-45EB-41A5-B338-A52BBEFEE69F',
                THRESHOLD: '23D633E6-C777-4CD2-8909-7860F0F705C9'
            },

            ShowError: function(message, invalidControls)
            {
                SEL.MasterPopup.ShowMasterPopup(message.replace(/\n/g, "<br />"), 'Message from ' + moduleNameHTML);
                if (invalidControls != null)
                {
                    for (var i = 0; i <= invalidControls.length; i++)
                    {
                        $('#' + SEL.Vet.CONTENT_MAIN + 'txt' + invalidControls[i]).parent().nextAll('.inputvalidatorfield').first()
                            .append($('<span/>').addClass('invalid').text('*'));
                    }
                }
            },

            ToggleVerificationThreshold: function ()
            {
                var payBeforeValidate = $('#' + SEL.Signoff.CONTENT_MAIN + 'chkPayBeforeValidate').prop('checked');

                if (payBeforeValidate)
                {
                    if ($('#' + SEL.Signoff.CONTENT_MAIN + 'cmbinclude').val() != "1")
                    {
                        __doPostBack('Manual:cmbinclude_SelectedIndexChanged', '1');
                        return;
                    }

                    $('#' + SEL.Signoff.CONTENT_MAIN + 'cmbinvolvement').val(2);
                    $('#' + SEL.Signoff.CONTENT_MAIN + 'chksinglesignoff').prop('checked', false);

                }
            },

            ResizeTableColumns: function()
            {
                var $tds = $('#' + SEL.Signoff.CONTENT_MAIN + 'chkPayBeforeValidate').closest('table').find('tr').first().find('td');
                $('#' + SEL.Signoff.CONTENT_MAIN + 'cmbsignofftype').closest('table').find('tr').first().children('td').each(function (i) {
                    if (i != 2) {
                        $tds.eq(i).width($(this).width());
                    }
                });
            }

        };

        $(function()
        {
            $('#' + SEL.Signoff.CONTENT_MAIN + 'chkPayBeforeValidate').on('change', SEL.Signoff.ToggleVerificationThreshold);
            SEL.Signoff.ResizeTableColumns();
        });

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
