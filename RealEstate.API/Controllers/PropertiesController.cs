using Microsoft.AspNetCore.Mvc;
using RealEstate.API.DTOs;
using RealEstate.API.Services;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _service;

    public PropertiesController(IPropertyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties()
    {
        var properties = await _service.GetAllPropertiesAsync();
        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyDto>> GetProperty(int id)
    {
        var property = await _service.GetPropertyByIdAsync(id);

        if (property == null)
        {
            return NotFound();
        }

        return property;
    }

    [HttpPost]
    public async Task<ActionResult<PropertyDto>> CreateProperty(CreatePropertyDto propertyDto)
    {
        var createdProperty = await _service.CreatePropertyAsync(propertyDto);
        return CreatedAtAction(nameof(GetProperty), new { id = createdProperty.Id }, createdProperty);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(int id, UpdatePropertyDto propertyDto)
    {
        var result = await _service.UpdatePropertyAsync(id, propertyDto);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var result = await _service.DeletePropertyAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
