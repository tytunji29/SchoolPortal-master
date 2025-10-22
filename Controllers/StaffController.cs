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
public class StaffController : ControllerBase
{
    private readonly IAllRepository<Staff> _repo;
    private readonly IRegisterRepository _register;
    public StaffController(IAllRepository<Staff> repo, IRegisterRepository register)
    {
        _repo = repo;
        _register = register;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("Add")]
    public async Task<IActionResult> Add([FromForm] StaffRequestModel obj)
    {
        try
        {
            var r = new ReturnObject();
            var existingItem = _repo.GetAll().Data.FirstOrDefault(o => o.Email == obj.Email);
            if (existingItem != null)
            {
                r.Status = "Error";
                r.Message = "Staff Already Exist";
                return Ok(r);
            };
            var docId = _register.AddDoc(obj.DisplayPicture, "0");
            var rec = obj.Adapt<Staff>();
            var res = _repo.Insert(rec);
            rec.DocId = $"{docId.Data}"; await _register.Register(obj.Email, "Welcom@123", $"{obj.StaffType}", $"Staff{res.Data}-1");
            return Ok();

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
    [Route("GetAllTeacherAndAssist")]
    public IActionResult GetAllTeacherAndAssist()
    {
        var r = new ReturnObject();
        r.Message = "Record Found Successfully";
        r.Status = "success";
        r.Data= _repo.GetAll().Data.Where(o => o.StaffType.ToLower() == "teacher" || o.StaffType.ToLower() == "assistant teacher");
        return Ok(r);
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
    public IActionResult UpdateById([FromRoute] int id, [FromForm] StaffRequestModel obj)
    {
        var extRec = _repo.Get(id).Data.FirstOrDefault();

        var docId = _register.AddDoc(obj.DisplayPicture, extRec.DocId);
        extRec.Tribe = obj.Tribe;
        extRec.Gender = obj.Gender;
        extRec.MobileNumber = obj.MobileNumber;
        extRec.StaffType = obj.StaffType;
        extRec.Name = obj.Name;
        extRec.DOB = obj.DOB;
        extRec.Email = obj.Email;
        extRec.Address = obj.Address;
        extRec.Religion = obj.Religion;
        return Ok(_repo.Update(extRec));
    }

}
