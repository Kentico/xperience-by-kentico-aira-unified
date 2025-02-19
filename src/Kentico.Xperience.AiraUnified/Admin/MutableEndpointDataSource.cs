using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Kentico.Xperience.AiraUnified.Admin;

internal abstract class MutableEndpointDataSource : EndpointDataSource
{
    private readonly object endpointLock = new();
    private IReadOnlyList<Endpoint> endpoints = [];
    private CancellationTokenSource cancellationTokenSource;
    private IChangeToken changeToken;

    protected MutableEndpointDataSource(CancellationTokenSource cancellationTokenSource, IChangeToken changeToken)
    {
        SetEndpoints(endpoints);
        this.cancellationTokenSource = cancellationTokenSource;
        this.changeToken = changeToken;
    }

    public override IChangeToken GetChangeToken() => changeToken;

    public override IReadOnlyList<Endpoint> Endpoints => endpoints;

    protected void SetEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        lock (endpointLock)
        {
            var oldCancellationTokenSource = cancellationTokenSource;
            this.endpoints = endpoints;
            cancellationTokenSource = new CancellationTokenSource();
            changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
            oldCancellationTokenSource?.Cancel();
        }
    }
}

