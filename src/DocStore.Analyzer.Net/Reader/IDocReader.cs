using DocStore.Analyzer.Net.Models;

namespace DocStore.Analyzer.Net.Reader;

public interface IDocReader : IDisposable
{
    PageData ReadPage(int pageNumber);
    DocumentData ReadDocument(string documentName, Guid documentId);
}