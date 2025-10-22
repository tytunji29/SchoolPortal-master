using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Model;
using SchoolPortal.Repository;
using Swashbuckle.AspNetCore.Annotations;

namespace SchoolPortal.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClassLevelController : ControllerBase
{
    private readonly IAllRepository<ClassesLevel> _repo;
    public ClassLevelController(IAllRepository<ClassesLevel> repo)
    {
        _repo = repo;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("Add")]
    public async Task<IActionResult> Add([FromBody] GeneralFormModel obj)
    {
        try
        {
            var r = new ReturnObject();
            var existingItem = _repo.GetAll().Data.FirstOrDefault(o => o.Name == obj.Name);
            if (existingItem != null)
            {
                r.Status = "Error";
                r.Message = "ClassLevel Already Exist";
                return Ok(r);
            };
            var rec = obj.Adapt<ClassesLevel>();
            return Ok(_repo.Insert(rec));

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
            {
                Status = "error",
                Message = ex.Message
            });
        }
    }
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("GetAll")]
    public IActionResult GetAll()
        {
        return Ok(_repo.GetAll());
    }
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("GetById/{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        return Ok(_repo.Get(id));
    }
    [HttpDelete]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("DeleteById/{id}")]
    public IActionResult DeleteById([FromRoute] int id)
    {
        return Ok(_repo.Delete(id));
    }
    [HttpPut]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("UpdateById/{id}")]
    public IActionResult UpdateById([FromRoute] int id, [FromBody] GeneralFormModel obj)
    {
        var extRec = _repo.Get(id).Data.FirstOrDefault();
        extRec.Name = obj.Name;
        return Ok(_repo.Update(extRec));
    }

}
