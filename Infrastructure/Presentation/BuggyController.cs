using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation;
[ApiController]
[Route("api/[controller]")]
public class BuggyController : ControllerBase
{
    [HttpGet("notfound")]
    public IActionResult GetNotFoundRequest()
    {
        // Code
        return NotFound();
    }

    [HttpGet("servererror")]
    public IActionResult GetServerErrorRequest()
    {
        throw new Exception();
        return Ok();
    }


    [HttpGet("badrequest")]
    public IActionResult GetNotBadRequest()
    {
        // Code
        return BadRequest();
    }


    [HttpGet("badrequest/{id}/{age}")]
    public IActionResult GetNotBadRequest(int id , int age) // validation error
    {
        // Code
        return BadRequest();
    }


    [HttpGet("unauthorized")]
    public IActionResult GetNotUnauthorizedRequest(int id) // validation error
    {
        // Code
        return Unauthorized();
    }


}
