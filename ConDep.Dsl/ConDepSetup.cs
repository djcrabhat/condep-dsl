using System;
using System.Collections.Generic;
using System.Linq;
using ConDep.Dsl.LoadBalancer;
using ConDep.Dsl.Model.Config;
using ConDep.Dsl.WebDeploy;

namespace ConDep.Dsl
{
    internal class ConDepSetup : ISetupConDep, IValidate, IProvideForSetup
	{
		private readonly List<ConDepOperationBase> _operations = new List<ConDepOperationBase>();
	    private ILoadBalance _loadBalancer;
        private readonly ISetupWebDeploy _webDeploySetup;
        private readonly ConDepConfig _envConfig;
        private readonly ILookupLoadBalancer _loadBalancerLookup;
        private readonly ConDepContext _context;

        public ConDepSetup(ISetupWebDeploy webDeploySetup, ConDepConfig envConfig, ILookupLoadBalancer loadBalancerLookup, ConDepContext context)
        {
            _webDeploySetup = webDeploySetup;
            _envConfig = envConfig;
            _loadBalancerLookup = loadBalancerLookup;
            _context = context;
        }

        public ConDepContext Context { get { return _context; } }

        public ISetupWebDeploy WebDeploySetup
        {
            get { return _webDeploySetup; }
        }

        public ConDepConfig EnvConfig { get { return _envConfig; } }

        public void AddOperation(ConDepOperationBase operation)
	    {
	        CheckLoadBalancerRequirement(operation);
	        _operations.Add(operation);
	    }

	    private void CheckLoadBalancerRequirement(ConDepOperationBase operation)
	    {
	        if (!(operation is IRequireLoadBalancing)) return;
	        
            if(_loadBalancer == null)
	        {
	            _loadBalancer = GetLoadBalancer();
	        }

	        operation.BeforeExecute = _loadBalancer.BringOffline;
	        operation.AfterExecute = _loadBalancer.BringOnline;
	    }

	    private ILoadBalance GetLoadBalancer()
	    {
            return _loadBalancerLookup.GetLoadBalancer();
	    }

	    public bool IsValid(Notification notification)
		{
			return _operations.All(operation => operation.IsValid(notification));
		}

        public WebDeploymentStatus Execute(ConDepOptions options, WebDeploymentStatus webDeploymentStatus)
        {
            if (options.PrintSequence)
            {
                PrintExecutionSequence(options);
                return webDeploymentStatus;
            }

            var operationExecutor = new OperationExecutor(_operations, options, webDeploymentStatus, _context);
            var result = operationExecutor.Execute();
            return result;
        }

        public void PrintExecutionSequence(ConDepOptions options)
        {
            if(options.HasContext())
            {
                foreach (var operation in _operations)
                {
                    if (operation is ConDepContextOperationPlaceHolder)
                    {
                        if(options.Context == ((ConDepContextOperationPlaceHolder)operation).ContextName)
                        {
                            var contextSetup = _context[options.Context];
                            contextSetup.PrintExecutionSequence(options);
                        }
                    }
                    else
                    {
                        operation.PrintExecutionSequence(Console.Out);
                    }
                }
            }
            else
            {
                foreach (var operation in _operations)
                {
                    if (operation is ConDepContextOperationPlaceHolder)
                    {
                        var contextSetup = _context[((ConDepContextOperationPlaceHolder)operation).ContextName];
                        contextSetup.PrintExecutionSequence(options);
                    } 
                    else
                    {
                        operation.PrintExecutionSequence(Console.Out);
                    }
                }
            }
        }
	}
}