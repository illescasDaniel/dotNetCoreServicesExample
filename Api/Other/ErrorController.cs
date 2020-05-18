using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace myMicroservice.Api.Other.Controllers
{
    [ApiController]
    [Route("/ErrorController")]
    public class ErrorController : ControllerBase
    {
        //[HttpGet]
        //[HttpPost]
        //[HttpDelete]
        //[HttpPut]
        //[HttpHead]
        //[HttpOptions]
        //[HttpPatch]
        //[AllowAnonymous]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Route("/error-local-development")]
        //public IActionResult ErrorLocalDevelopment()
        //{
        //    var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        //    return Problem(
        //        detail: context.Error.StackTrace,
        //        title: context.Error.Message
        //    );
        //}

        [HttpGet("error")]
        [HttpPost("error")]
        //[HttpDelete]
        //[HttpPut]
        //[HttpHead]
        //[HttpOptions]
        //[HttpPatch]
        //[AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Route("/Error")]
        public IActionResult Error()
        {
            Console.WriteLine("error!");
            return Problem();
        }
    }
}
