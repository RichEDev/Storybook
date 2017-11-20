/* <summary>Vehicle Engine Type methods</summary> */
(function()
{
    var scriptName = "VehicleEngineType";

    function execute()
    {
        SEL.registerNamespace("SEL.VehicleEngineType");
        SEL.Vet =
        {
            CONTENT_MAIN: '',
            REDIRECT_URL: '',
            VET_ID: null,

            HELP:
            {
                CODE: '5A5A49E3-6D1D-4DCA-B326-BE824C23267E',
                NAME: 'DA651C02-0D81-4BA0-ABEA-A35A301B0654'
            },

            Save: function()
            {
                $('.inputvalidatorfield').html('');
                SEL.Ajax.Service("/shared/webServices/svcVehicleEngineType.asmx", "Save", new SEL.Vet.VehicleEngineType(),
                    function (data)
                    {
                        var $response = data.d;
                        if ($response.Success)
                        {
                            document.location = SEL.Vet.REDIRECT_URL;
                        }
                        else
                        {
                            SEL.Vet.ShowError($response.Message, $response.Controls);
                        }
                    },
                    function (jqXhr, textStatus, errorThrown)
                    {
                        SEL.Vet.ShowError("Error: " + textStatus + ", " + errorThrown);
                    });
            },

            Delete: function(vehicleEngineTypeId)
            {
                if (!confirm('Are you sure you want to delete this vehicle engine type?'))
                {
                    return;
                }

                SEL.Ajax.Service("/shared/webServices/svcVehicleEngineType.asmx", "Delete", { "vehicleEngineTypeId": vehicleEngineTypeId },
                    function (data)
                    {
                        var $response = data.d;
                        if ($response.Success)
                        {
                            SEL.Grid.deleteGridRow('gridVets', vehicleEngineTypeId);
                        }
                        else
                        {
                            SEL.Vet.ShowError($response.Message);
                        }
                    },
                    function (jqXhr, textStatus, errorThrown)
                    {
                        SEL.Vet.ShowError("Error: " + textStatus + ", " + errorThrown);
                    });
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

            VehicleEngineType: function ()
            {
                this.vehicleEngineType =
                {
                    VehicleEngineTypeId: SEL.Vet.VET_ID,
                    Name: $('#' + SEL.Vet.CONTENT_MAIN + 'txtName').val() || null,
                    Code: $('#' + SEL.Vet.CONTENT_MAIN + 'txtCode').val() || null
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
