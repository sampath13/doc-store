using DocStore.Analyzer.Net.Features.Index;
using DocStore.Analyzer.Net.Features.List;
using DocStore.Analyzer.Net.Features.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocStore.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public sealed class PdfDocController(IMediator mediatr) : ControllerBase
{
    [HttpPost("index")]
    [ProducesResponseType(StatusCodes.Status200OK,  Type = typeof(DocumentIndexingResponse))]
    public async Task<IActionResult> Submit(IFormFile file, CancellationToken ct)
    {
        var request = new DocumentIndexingRequest()
        {
            FileName = file.FileName,
            Stream = file.OpenReadStream()
        };
        var response = await mediatr.Send(request, ct);
        return Ok(response);
    }
    
    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK,  Type = typeof(SearchContentResponse))]
    public async Task<IActionResult> Search([FromBody]SearchContentRequest request, CancellationToken ct)
    {
        var response = await mediatr.Send(request, ct);
        return Ok(response);
    }  
    
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK,  Type = typeof(ListDocumentsResponse))]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var response = await mediatr.Send(new ListDocumentsRequest(), ct);
        return Ok(response);
    }  
}