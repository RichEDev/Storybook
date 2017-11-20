namespace SELFileTransferService
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Configuration;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Http;
    using SpendManagementLibrary;

    /// <summary>
    /// A class to get an instance of <see cref="IESR"/> for remoting purposes.
    /// </summary>
    public class Remoting
    {
        private readonly IChannel _chan;

        /// <summary>
        /// Create a new instance of <see cref="Remoting"/>
        /// </summary>
        public Remoting()
        {
            var clientProvider = new BinaryClientFormatterSinkProvider();
            var props = new Hashtable {["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full};
            this._chan = new HttpChannel(props, clientProvider, null);
        }

        /// <summary>
        /// Get the remoting Interface instance so the connection can be made to the web application
        /// </summary>
        /// <returns></returns>
        public IESR GetServiceInstance()
        {
            var chanCount = ChannelServices.RegisteredChannels.Count(c => c.ChannelName == this._chan.ChannelName);

            if (chanCount == 0)
            {
                ChannelServices.RegisterChannel(this._chan, false);
            }

            var clsEsr = (IESR)Activator.GetObject(typeof(IESR), ConfigurationManager.AppSettings["ESRService"] + "/ESR.rem");

            return clsEsr;
        }
    }
}
