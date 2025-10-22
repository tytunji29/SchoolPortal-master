using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Model;
using SchoolPortal.Repository;
using Swashbuckle.AspNetCore.Annotations;

namespace SchoolPortal.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ParentController : ControllerBase
{
    private readonly IAllRepository<Parent> _repo;
    private readonly IRegisterRepository _register;
    private readonly SchoolDbContext _repPo;
    public ParentController(IAllRepository<Parent> repo, SchoolDbContext repPo, IRegisterRepository register)
    {
        _repo = repo;
        _repPo = repPo;
        _register = register;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("Add")]
    public async Task<IActionResult> Add([FromForm] ParentFormmodel obj)
    {
        try
        {
            var r = new ReturnObject();
            var existingItem = _repo.GetAll().Data.FirstOrDefault(o => o.Email == obj.Email);
            if (existingItem != null)
            {
                r.Status = "Error";
                r.Message = "Parent Already Exist";
                return Ok(r);
            };
            var docId = _register.AddDoc(obj.DisplayPicture,"0");
            var rec = obj.Adapt<Parent>();
            rec.DocId = $"{docId.Data}";
           var res=_repo.Insert(rec);
            await _register.Register(obj.Email, "Welcom@123", "Parent", $"Parent{res.Data}-1");
            return Ok(res);

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
    [HttpPut]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("UpdateById/{id}")]
    public IActionResult UpdateById([FromRoute] int id, [FromForm] ParentFormmodel obj)
    {
        Parent extRec = _repo.Get(id).Data.FirstOrDefault();
        var docId = _register.AddDoc(obj.DisplayPicture, extRec.DocId);
        var students = _repPo.Students.Where(x => x.ParentMobileNumber == extRec.MobileNumber).
            ExecuteUpdate(s => s.SetProperty(y => y.ParentMobileNumber ,obj.MobileNumber));
        extRec.Tribe = obj.Tribe;
        extRec.Gender=obj.Gender;
        extRec.MobileNumber=obj.MobileNumber;
        extRec.Name=obj.Name;
        extRec.DOB= obj.DOB;
        extRec.Email= obj.Email;
        extRec.Address= obj.Address;
        extRec.Religion= obj.Religion;
        return Ok(_repo.Update(extRec));
    }

}
