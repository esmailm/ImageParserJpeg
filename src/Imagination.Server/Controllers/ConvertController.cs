using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Imagination.ServicesHandler;

namespace Imagination.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConvertController : ControllerBase
    {
        private readonly IRequestHandler _requestHandler;

        public ConvertController(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using var activity = Program.Telemetry.StartActivity("Recieved Image Convert Request");
            using (MemoryStream responseStream = new MemoryStream())
            {
                await _requestHandler.ExtractAndConvert(Request.Body, responseStream,CancellationToken.None);
                activity?.AddEvent(new ActivityEvent($"Conversion Completed Successfully"));
                return Ok(responseStream.ToArray());
            }

        }
    }
}