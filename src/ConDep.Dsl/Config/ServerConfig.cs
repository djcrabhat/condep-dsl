using System;
using System.Collections.Generic;
using ConDep.Dsl.Operations.LoadBalancer;
using Newtonsoft.Json;

namespace ConDep.Dsl.Config
{
    [Serializable]
    public class ServerConfig
    {
        private DeploymentUserConfig _deploymentUserRemote;
        private ServerInfo _serverInfo = new ServerInfo();

        public string Name { get; set; }
        public bool StopServer { get; set; }
        public IList<WebSiteConfig> WebSites { get; set; }
        public DeploymentUserConfig DeploymentUser 
        { 
            get { return _deploymentUserRemote ?? (_deploymentUserRemote = new DeploymentUserConfig()); }
            set { _deploymentUserRemote = value; }
        }

        public string LoadBalancerFarm { get; set; }
        internal LoadBalanceState? LoadBalancerState { get; set; }
        internal bool PreventDeployment { get; set; }
        internal bool KeepOffline { get; set; }

        public ServerInfo GetServerInfo()
        {
            return _serverInfo;
        }
    }
}