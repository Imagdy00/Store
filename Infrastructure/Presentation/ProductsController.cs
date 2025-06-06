﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Services.Abstractions;
using Shared;
using Shared.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation;
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IServiceManager serviceManager) : ControllerBase 
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK , Type = typeof(PaginationResponse<ProductResultDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError , Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest , Type = typeof(ErrorDetails))]
    [Cache(100)]
    [Authorize]
    public async Task<ActionResult<PaginationResponse<ProductResultDto>>> GetAllProducts([FromQuery] ProductSpecificationParameters specParams )
    {
        var result = await serviceManager.ProductService.GetAllProductsAsync(specParams);

        if (result is null) return BadRequest();

        return Ok(result);

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResultDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<ProductResultDto>> GetProductById(int id)
    {
        var result = await serviceManager.ProductService.GetProductByIdAsync(id);
       

        return Ok(result);
    }

    [HttpGet("brands")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BrandResultDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<BrandResultDto>> GetAllBrands()
    {
        var result = await serviceManager.ProductService.GetAllBrandsAsync();
        if(result is null) return BadRequest();
        return Ok(result);
    }



    [HttpGet("types")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TypeResultDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
    public async Task<ActionResult<TypeResultDto>> GetAllTypes()
    {
        var result = await serviceManager.ProductService.GetAllTypesAsync();
        if (result is null) return BadRequest();
        return Ok(result);
    }



}
