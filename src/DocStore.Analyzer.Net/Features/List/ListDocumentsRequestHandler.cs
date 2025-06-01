using DocStore.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DocStore.Analyzer.Net.Features.List;

public class ListDocumentsRequestHandler(DocStoreContext context) : IRequestHandler<ListDocumentsRequest, ListDocumentsResponse>
{
    public async Task<ListDocumentsResponse> Handle(ListDocumentsRequest request, CancellationToken cancellationToken)
    {
        var response = new ListDocumentsResponse();
        response.Files =
            await context.Documents.AsNoTracking()
                //.Where(x => x.Status == Status.Draft || x.Status == Status.Visible)
                .Select(x => new Document { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);
        return response;
    }
}