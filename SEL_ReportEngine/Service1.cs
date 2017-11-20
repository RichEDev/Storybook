using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SEL_ReportEngine
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
			SqlDependency.Start(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            ChannelServices.RegisterChannel(new TcpChannel(port), false);
            WellKnownServiceTypeEntry wkste = new WellKnownServiceTypeEntry(typeof(cReportsSvc), "reports.rem", WellKnownObjectMode.SingleCall);
			RemotingConfiguration.ApplicationName = ConfigurationManager.AppSettings["ServiceName"];
            RemotingConfiguration.RegisterWellKnownServiceType(wkste);
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            SqlDependency.Stop(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
        }
    }
}
