﻿using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.SemanticModel;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations
{
    public abstract class RemoteOperation : IOperateRemote
    {
        public abstract void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token);
        public abstract string Name { get; }
        public abstract bool IsValid(Notification notification);
        public void DryRun() { }
    }
}