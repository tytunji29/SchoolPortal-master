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
public class StudentController : ControllerBase
{
    private readonly IAllRepository<Student> _repo;
    private readonly IRegisterRepository _register;
    public StudentController(IAllRepository<Student> repo, IRegisterRepository register)
    {
        _repo = repo;
        _register = register;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("Add")]
    public async Task<IActionResult> Add([FromForm] StudentFormModel obj)
    {
        try
        {
            var r = new ReturnObject();
            var existingItem = _repo.GetAll().Data.FirstOrDefault(o => o.Name == obj.Name);
            if (existingItem != null)
            {
                r.Status = "Error";
                r.Message = "Student Already Exist";
                return Ok(r);
            };

            var docId = _register.AddDoc(obj.DisplayPicture,"0");
            var rec = obj.Adapt<Student>();
            rec.DocId =  $"{docId.Data}";
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
        var rec = _repo.Get(id);
        rec.Data.FirstOrDefault().DocId = _register.GetDoc(id);
        return Ok(rec);
    }
    [HttpDelete]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("DeleteById/{id}")]
    public IActionResult DeleteById([FromRoute] int id)
    {
        return Ok(_repo.Delete(id));
    }


}
