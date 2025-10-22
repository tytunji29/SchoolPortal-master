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
public class ClassesController : ControllerBase
{
    private readonly IAllRepository<Classes> _repo;
    private readonly IAllRepository<ClassTeacherHistory> _repoH;
    private readonly SchoolDbContext _db;
    public ClassesController(IAllRepository<Classes> repo, SchoolDbContext db, IAllRepository<ClassTeacherHistory> repoH)
    {
        _repo = repo;
        _db = db;
        _repoH = repoH;
    }
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
    [Route("Add")]
    public async Task<IActionResult> Add([FromBody] ClassTeacherHistoryFormmodel obj)
    {
        try
        {
            var r = new ReturnObject();
            var existingItem = _repo.GetAll().Data.FirstOrDefault(o => o.Name == obj.Name);
            if (existingItem != null)
            {
                r.Status = "Error";
                r.Message = "Classes Already Exist";
                return Ok(r);
            };
            var rec = obj.Adapt<Classes>();
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
    [Route("GetAllByTeacherId/{teacherId}")]
    public IActionResult GetAllByTeacherId([FromRoute] int teacherId)
    {
        var r = new ReturnObject();
        r.Status = "Error";
        r.Message = "No Class Found For This Teacher";
        r.Data = null;
        var tesaWeReLookingFor = _db.Staffs.FirstOrDefault(o => o.Id == teacherId);
        var newrec = _db.Staffs
    .Where(a => teacherId == a.Id)
    .Join(
        _db.Classes,
        a => a.ClientId,
        b => b.ClientId,
        (a, b) => b
    )
    .Where(o => o.Teacher.ToLower() == tesaWeReLookingFor.Name.ToLower())
    .ToList();
        if (newrec.Count() > 0)
        {
            r.Status = "success";
            r.Message = "Classes Found Successfully";
            r.Data = newrec;
        }
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
    public IActionResult UpdateById([FromRoute] int id, [FromBody] ClassTeacherHistoryFormmodel obj)
    {
        var extRec = _repo.Get(id).Data.FirstOrDefault();

        if (extRec.Teacher != obj.Teacher || extRec.AssistantTeacher !=obj.AssistantTeacher)
        {
            var oo = new ClassTeacherHistory();
            oo.ClassName = extRec.Name;
            oo.FormalTeacher = obj.Teacher;
            oo.FormalAssistTeacher = obj.AssistantTeacher;

            _repoH.Insert(oo);
        }
        extRec.Name = obj.Name;
        extRec.Teacher = obj.Teacher;
        extRec.AssistantTeacher = obj.AssistantTeacher;

        return Ok(_repo.Update(extRec));
    }


}
