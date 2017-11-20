/* <summary>Type validation for inputs Methods</summary> */
(function ($get, $find, $create)
{
    var scriptName = "Input";
    
    function execute()
    {
        SEL.registerNamespace("SEL.Input");
        SEL.Input = {
            /// <summary>
            /// Attaches an AjaxControlToolkit filtered textbox extender to the specified control id formulated to only allow certain value-type related characters
            /// </summary>
            Filter:
            {
                Decimal: function (elementIdentifier)
                {
                    $create(window.Sys.Extended.UI.FilteredTextBoxBehavior, { "ValidChars": "0123456789.", "id": elementIdentifier }, null, null, $get(elementIdentifier));
                    $("#" + elementIdentifier).on("remove", function (jQueryEvent)
                    {
                        $find(jQueryEvent.target.id).dispose();
                    });
                }
            }
        };
    }

    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}($get, $find, $create));