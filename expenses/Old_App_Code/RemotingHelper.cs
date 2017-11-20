using System;
using System.Collections;
using System.Runtime.Remoting;

class RemotingHelper
{
    private static bool _isInit;
    private static IDictionary _wellKnownTypes;

    public static Object GetObject(Type type)
    {
        if (!_isInit) InitTypeCache();
        WellKnownClientTypeEntry entr = (WellKnownClientTypeEntry)_wellKnownTypes[type];

        if (entr == null)
        {
            throw new RemotingException("Type not found!");
        }

        return Activator.GetObject(entr.ObjectType, entr.ObjectUrl);
    }

    public static void InitTypeCache()
    {
        _isInit = true;
        _wellKnownTypes = new Hashtable();
        foreach (WellKnownClientTypeEntry entr in
          RemotingConfiguration.GetRegisteredWellKnownClientTypes())
        {

            if (entr.ObjectType == null)
            {
                throw new RemotingException("A configured type could not " +
                  "be found. Please check spelling");
            }
            _wellKnownTypes.Add(entr.ObjectType, entr);
        }
    }
}
