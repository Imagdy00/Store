﻿using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation;
[ApiController]
[Route("api/[controller]")]

public class AuthController(IServiceManager serviceManager) : ControllerBase
{
    //Login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var result = await serviceManager.AuthService.LoginAsync(loginDto);
        return Ok(result);
    }

    //Register

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await serviceManager.AuthService.RegisterAsync(registerDto);
        return Ok(result);
    }



}
