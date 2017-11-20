/* <summary>Form Methods</summary> */
(function ()
{
    var scriptName = "Forms";
    
    function execute()
    {
        SEL.registerNamespace("SEL.Forms");
        
        SEL.Forms =
        {
            /* <summary>When applied to a controls event it will block any enter key presses and exec functionToExec</summary> */
            RunOnEnter: function (e, functionToExec)
            {
                if (typeof functionToExec !== "function")
                {
                    SEL.Errors.InvalidArgument("functionToExec must be 'typeof' function");
                    return false;
                }
                
                var nav = window.event ? true : false;
                
                if (nav)
                {
                    if (event.keyCode === 13 && event.srcElement.type !== "textarea" && event.srcElement.type !== "submit")
                    {
                        functionToExec();
                        SEL.Common.stopPropagation();
                        return false;
                    }
                }
                else
                {
                    if (e.which === 13 && e.target.type !== "textarea" && e.target.type !== "submit")
                    {
                        functionToExec();
                        SEL.Common.stopPropagation();
                        return false;
                    }
                }
                
                return true;
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