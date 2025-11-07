using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using ProjetoESB.Infra.Conectores;
using ProjetoESB.Dominio.Conectores;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    //private readonly ConectorRest _rest;
    private readonly IConectorFactory _factory;
    //public TestController(ConectorRest rest) => _rest = rest;

    public TestController(IConectorFactory factory)
    {
        _factory = factory;
    }

    // [HttpGet("call")]
    [HttpGet("http")]
    public async Task<IActionResult> TestarHttpConector()
    {
        // Pega o conector "HTTP" via factory
        var conector = _factory.Resolve("HTTP");

        var requisicao = new ConectorRequisicao
        {
            Endpoint = "https://httpbin.org/get",
            Metodo = HttpMethod.Get
        };

        var resultado = await conector.ExecutarAsync(requisicao);
        return Ok(resultado);
    }
}
    //public async Task<IActionResult> Call()
    //{
    //    var requisicao = new ConectorRequisicao
    //    {
    //        Endpoint = "https://httpbin.org/post",
    //        Metodo = HttpMethod.Post,
    //        Body = "{\"hello\":\"world\"}",
    //        CorrelationId = Guid.NewGuid().ToString()
    //    };

    //    var r = await _rest.ExecutarAsync(requisicao);
    //    return StatusCode(r.StatusCode, r);
    //}
//}
