/* <summary>Debugging Tools Helper Methods</summary> */
(function ()
{
    var scriptName = "DebugTools";
    function execute()
    {
        SEL.registerNamespace("SEL.DebugTools");
        SEL.DebugTools =
        {
            // <summary>
            //  Format an object in a readable string form (NOT JSON)
            //      ie. for alert(object);
            //      Will only show partial version of large objects (max 10 nested levels and max 20 properties per object/array)
            // </summary>
            // <param="objectToShow"> Needed - the object that will be formatted </summary>
            // <param="showPrototypeProperties"> optional - show the properties inherited from the Object's prototype </summary>
            // <param="prefixFragment"> FOR INTERNAL USE ONLY </summary>
            // <param="iteration"> FOR INTERNAL USE ONLY </summary>
            // <returns> A basic String representation of object </returns>
            ObjectToString: function (objectToShow, showPrototypeProperties, prefixFragment, iteration)
            {
                iteration = (typeof iteration === 'number') ? iteration : 0;
                prefixFragment = (typeof prefixFragment === 'string') ? prefixFragment : '';
                showPrototypeProperties = (typeof showPrototypeProperties === 'boolean') ? showPrototypeProperties : false;

                var message = prefixFragment + "{\n",
                    prefix = prefixFragment + '    ',
                    ownParameter = '',
                    subMessage = '',
                    loop = 0;

                for (var p in objectToShow)
                {
                    if (iteration > 10 || loop > 20) return message + "\nITERATION TOO LARGE\n";

                    if (Object.prototype.hasOwnProperty.call(objectToShow, p))
                    {
                        ownParameter = '';
                    }
                    else
                    {
                        if (showPrototypeProperties) ownParameter = '(from prototype) ';
                        else continue;
                    }

                    if (typeof objectToShow[p] === 'string' || typeof objectToShow[p] === 'number' || typeof objectToShow[p] === 'boolean' || objectToShow[p] === null)
                    {
                        subMessage = objectToShow[p];
                    }
                    else if (typeof objectToShow[p] === 'object')
                    {
                        subMessage = "\n" + SEL.DebugTools.ObjectToString(objectToShow[p], showPrototypeProperties, prefix, iteration++);
                    }
                    else
                    {
                        subMessage = typeof objectToShow[p];
                    }

                    message = message + prefix + ownParameter + p + ' = ' + subMessage + "\n";

                    loop++;
                }

                return message + prefixFragment + '}';
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