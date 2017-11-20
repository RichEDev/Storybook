/* <summary>Control Methods</summary> */
(function ()
{
    var scriptName = "Controls";
    
    function execute()
    {
        SEL.registerNamespace("SEL.Controls");
        
        SEL.Controls =
        {
            setCssClass: function (control, cssClassName)
            {
                control.setAttribute("class", cssClassName);
                control.setAttribute("className", cssClassName);
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
}());