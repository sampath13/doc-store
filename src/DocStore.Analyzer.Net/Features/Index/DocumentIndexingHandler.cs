using DocStore.Analyzer.Net.Services.Queue;
using DocStore.Data.Context;
using DocStore.Data.Model;
using MediatR;

namespace DocStore.Analyzer.Net.Features.Index;

public sealed class DocumentIndexingHandler(
    IMessageProducer messageProducer,
    AppSettings appSettings,
    DocStoreContext context)
    : IRequestHandler<DocumentIndexingRequest, DocumentIndexingResponse>
{
    public async Task<DocumentIndexingResponse> Handle(DocumentIndexingRequest request, CancellationToken cancellationToken)
    {
        // Write file to temp path
        // TODO change this to writing to Storage Account
        var fileName = $"{request.FileName}{Guid.NewGuid()}";
        var fileNameWithPath = Path.Combine(appSettings.Store.Path, fileName);
        await using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await request.Stream.CopyToAsync(stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);

        var document = new Document
        {
            Name = request.FileName,
            AddedBy = Guid.NewGuid(),
            Status = Status.Draft,
            StorePath = fileNameWithPath,
            AddedDate = DateTimeOffset.UtcNow
        };
        
        context.Documents.Add(document);
        await context.SaveChangesAsync(cancellationToken);
        
        // Send message to Indexer worker
        await messageProducer.SendAsync(new DocStoreMessage<string, string>
        {
            Key = document.Id.ToString(),
            Payload = document.Id.ToString()
        }, cancellationToken);
        
        document.Stages.Add(new DocumentProcessingStage()
        {
            Stage = Stage.InitProcessing
        });
        
        await context.SaveChangesAsync(cancellationToken);
        
        return new DocumentIndexingResponse();
    }
}