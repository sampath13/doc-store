using DocStore.Analyzer.Net.Reader;
using Microsoft.AspNetCore.Mvc;

namespace DocStore.Api.Controller;

[ApiController] 
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string Get() => "Hello Sampath";
    
    
}