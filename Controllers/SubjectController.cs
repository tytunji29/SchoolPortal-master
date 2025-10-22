using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Model;
using SchoolPortal.Repository;
using Swashbuckle.AspNetCore.Annotations;

namespace SchoolPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        public IHttpContextAccessor _httpContextAccessor;
        private readonly IAllRepository<Subjects> _repo;
        private readonly IAllRepository<Map_Subjects_Teacher_Class> _repoMap;
        private readonly IAllRepository<ClassTeacherHistory> _repoH;
        private readonly IAllRepository<TeacherTimetable> _repoTT;
        private readonly SchoolDbContext _db;
        string ClientId = "", UserId = "";
        public SubjectController(IAllRepository<Subjects> repo, IAllRepository<TeacherTimetable> repoTT, IHttpContextAccessor httpContextAccessor, IAllRepository<Map_Subjects_Teacher_Class> repoMap, SchoolDbContext db, IAllRepository<ClassTeacherHistory> repoH)
        {
            _httpContextAccessor = httpContextAccessor;
            var k = _httpContextAccessor.HttpContext.User.Claims.ToList();
            if (k.Any())
            {
                 ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value : "";
                 UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            }
            _repo = repo;
            _repoMap = repoMap;
            _repoTT = repoTT;
            _db = db;
            _repoH = repoH;
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("MapToSubject")]
        public async Task<IActionResult> MapToSubject([FromBody] MapClassToTeacher obj)
        {
            try
            {
                var r = new ReturnObject();
                
                var rec = obj.Adapt<Map_Subjects_Teacher_Class>();
                return Ok(_repoMap.Insert(rec));

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
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] GeneralForFormModel obj)
        {
            try
            {
                var r = new ReturnObject();
                var existingItem = _repo.GetAll().Data.FirstOrDefault(o => o.Name == obj.Name);
                if (existingItem != null)
                {
                    r.Status = "Error";
                    r.Message = "Subjects Already Exist";
                    return Ok(r);
                };
                var rec = obj.Adapt<Subjects>();
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
        [Route("GetAllForTeacher")]
        public IActionResult GetAllForTeacher()
        {
            // Create a new response object
            var r = new ReturnObject();

            // Fetch the data from the database
            var recc = (from a in _db.Map_Subjects_Teacher_Class
                        where a.TeacherId == Convert.ToInt32(UserId)
                        join b in _db.Classes on a.ClassId equals b.Id
                        join c in _db.Subjects on a.SubjectId equals c.Id
                        select new
                        {
                            Id = a.Id,
                            subjectId=c.Id,
                            classId=b.Id,
                            ClassName = b.Name,
                            Subject = c.Name
                        }).ToList();

            // Populate the response object
            if (recc.Any())
            {
                r.Status = "success";
                r.Message = "Records found successfully";
                r.Data = recc;
            }
            else
            {
                r.Status = "error";
                r.Message = "No records found";
                r.Data = null;  // or empty list if you prefer
            }

            // Return the response as an HTTP 200 OK
            return Ok(r);

        } 
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetMyTimetable")]
        public IActionResult GetMyTimetable()
        {
            // Create a new response object
            var r = new ReturnObject();

            // Fetch the data from the database
            var recc = (from a in _db.TeacherTimetable
                        where a.TeacherId == Convert.ToInt32(UserId)
                        join b in _db.Classes on a.ClassId equals b.Id
                        join c in _db.Subjects on a.SubjectId equals c.Id
                        select new
                        {
                            Id = a.Id,
                            subjectId=c.Id,
                            classId=b.Id,
                            ClassName = b.Name,
                            Subject = c.Name,
                            Monday = a.Monday,
                            Tuesday = a.Tuesday,
                            Wednesday=a.Wednessday,
                            Thursday = a.Thursday,
                            Friday = a.Friday
                        }).ToList();

            // Populate the response object
            if (recc.Any())
            {
                r.Status = "success";
                r.Message = "Records found successfully";
                r.Data = recc;
            }
            else
            {
                r.Status = "error";
                r.Message = "No records found";
                r.Data = null;  // or empty list if you prefer
            }

            // Return the response as an HTTP 200 OK
            return Ok(r);

        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GenerateMyTimetable")]
        public async Task<IActionResult> GenerateMyTimetable([FromBody] TimetableRequest obj)
        {
            try
            {
                var r = new ReturnObject();
                var recToadd = new TeacherTimetable();
                recToadd.Monday = $"{obj.timetable.Monday.startTime} -{obj.timetable.Monday.endTime}";
                recToadd.Tuesday = $"{obj.timetable.Tuesday.startTime} -{obj.timetable.Tuesday.endTime}";
                recToadd.Wednessday = $"{obj.timetable.Wednesday.startTime} -{obj.timetable.Wednesday.endTime}";
                recToadd.Thursday = $"{obj.timetable.Thursday.startTime} -{obj.timetable.Thursday.endTime}";
                recToadd.Friday = $"{obj.timetable.Friday.startTime} -{obj.timetable.Friday.endTime}";
                recToadd.TeacherId = Convert.ToInt32(UserId);
                recToadd.SubjectId = obj.subjectId;
                recToadd.ClassId = obj.classId;

                var res = _repoTT.Insert(recToadd);
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

        //[HttpGet]
        //[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        //[Route("GetAllByClassId/{classId}")]
        //public IActionResult GetAllByClassId([FromRoute] int classId)
        //{
        //    var r = new ReturnObject();
        //    r.Status = "Error";
        //    r.Message = "No Class Found For This Teacher";
        //    r.Data = null;
        //    var tesaWeReLookingFor = _repo.GetAll().Data.Where(o => o.ClassId == classId).ToList();
        //    if (tesaWeReLookingFor.Count() > 0)
        //    {
        //        r.Status = "success";
        //        r.Message = "Subjects Found Successfully";
        //        r.Data = tesaWeReLookingFor;
        //    }
        //    return Ok(r);
        //}

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
        public IActionResult UpdateById([FromRoute] int id, [FromBody] GeneralForFormModel obj)
        {
            var extRec = _repo.Get(id).Data.FirstOrDefault();


            extRec.Name = obj.Name;
            return Ok(_repo.Update(extRec));
        }
    }
}