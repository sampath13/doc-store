using DocStore.Analyzer.Net.Models;
using UglyToad.PdfPig;

namespace DocStore.Analyzer.Net.Reader;

public class PdfDocReader : IDocReader
{
    private readonly PdfDocument _pdfDocument;

    public PdfDocReader(string pathToDoc)
    {
        _pdfDocument = PdfDocument.Open(pathToDoc);
    }
    
    public PdfDocReader(Stream stream)
    {
        _pdfDocument = PdfDocument.Open(stream);
    }
    
    public PageData ReadPage(int pageNumber)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageNumber, _pdfDocument.NumberOfPages);
        var page = _pdfDocument.GetPage(pageNumber);
        var pageContent = page.Text;
        return new PageData
        {
            Content = pageContent,
            PageNumber = pageNumber,
            TotalPages = _pdfDocument.NumberOfPages
        };
    }

    public DocumentData ReadDocument(string documentName, Guid documentId)
    {
        var documentData = new DocumentData
        {
            TotalPages = _pdfDocument.NumberOfPages,
            Pages = new List<PageData>()
        };

        for (var i = 0; i < _pdfDocument.NumberOfPages; i++)
        {
            var  page = ReadPage(i + 1);
            page.DocumentId = documentId;
            page.DocumentName = documentName;
            documentData.Pages.Add(page);
        }

        return documentData;
    }

    public void Dispose() => _pdfDocument.Dispose();
}