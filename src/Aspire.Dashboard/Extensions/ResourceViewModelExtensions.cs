// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Dashboard.Model;

namespace Aspire.Dashboard.Extensions;

internal static class ResourceViewModelExtensions
{
    public static bool IsHiddenState(this ResourceViewModel resource)
    {
        return resource.KnownState == KnownResourceState.Hidden;
    }

    public static bool IsRunningState(this ResourceViewModel resource)
    {
        return resource.KnownState == KnownResourceState.Running;
    }

    public static bool IsFinishedState(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Finished;
    }

    public static bool IsStopped(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Exited or KnownResourceState.Finished or KnownResourceState.FailedToStart;
    }

    public static bool IsStartingOrBuildingOrWaiting(this ResourceViewModel resource)
    {
        return resource.KnownState is KnownResourceState.Starting or KnownResourceState.Building or KnownResourceState.Waiting;
    }

    public static bool HasNoState(this ResourceViewModel resource) => string.IsNullOrEmpty(resource.State);

    // We only care about the readiness state if the resource is running
    public static bool ShowReadinessState(this ResourceViewModel resource) =>
        resource.IsRunningState() && resource.ReadinessState is ReadinessState.NotReady;
}
