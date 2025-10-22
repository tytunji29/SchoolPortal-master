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
public class SchoolController : ControllerBase
{
    private readonly IAllRepository<School> _repo;
    private readonly IAllRepository<Login> _repoLg;
    private readonly IRegisterRepository _register;
    public SchoolController(IAllRepository<School> repo, IRegisterRepository register, IAllRepository<Login> repoLg)
    {
        _repo = repo;
        _register = register;
        _repoLg = repoLg;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("Add")]
    public async Task<IActionResult> Add([FromForm] SchoolRequestModel obj)
    {
        try
        {
            string defaultPassword = "Welcom@123";
            var r = new ReturnObject();
            var existingItem = _repoLg.GetAll().Data.FirstOrDefault(o => o.Email == obj.Email);
            if (existingItem != null)
            {
                r.Status = "Error";
                r.Message = "School Already Exist";
                return Ok(r);
            }
            var docId = _register.AddDoc(obj.Logo,"0");
            var objRec = obj.Adapt<School>();
            objRec.DocId = $"{docId.Data}";
            var res = _repo.Insert(objRec);

            await _register.Register(obj.Email, defaultPassword, "Admin", $"Admin{res.Data}-1");
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
    public IActionResult UpdateById([FromRoute] int id, [FromForm] SchoolRequestModel obj)
    {
        var extRec = _repo.Get(id).Data.FirstOrDefault();

        var docId = _register.AddDoc(obj.Logo, extRec.DocId);
        extRec.MobileNumber = obj.MobileNumber;
        extRec.Name = obj.Name;
        extRec.Email = obj.Email;
        extRec.Address = obj.Address;
        extRec.SubscriptionType=obj.SubscriptionType;
        extRec.SubscriptionTypeFee = obj.SubscriptionTypeFee;
        extRec.Website=obj.Website;
        extRec.PinKey=obj.PinKey;
        extRec.ThemeColour=obj.ThemeColour;
        return Ok(_repo.Update(extRec));
    }
}
