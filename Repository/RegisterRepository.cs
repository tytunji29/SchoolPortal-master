using SchoolPortal.Model;
using System.Data;

namespace SchoolPortal.Repository
{
    public interface IRegisterRepository
    {
        ReturnObject AddDoc(IFormFile file, string id);
        string GetDoc(int id);
        Task<ReturnObject> Register(string email, string password, string role, string clientId);
    }
    public class RegisterRepository : IRegisterRepository
    {
        private readonly IAllRepository<Login> _repo;
        private readonly IAllRepository<DocumentHolder> _repoDoc;
        public RegisterRepository(IAllRepository<Login> repo, IAllRepository<DocumentHolder> repoDoc)
        {
            _repo = repo;
            _repoDoc = repoDoc;
        }

        public ReturnObject AddDoc(IFormFile file, string id)
        {
            var r = new ReturnObject();
            var stFile = AllHelper.ConvertIFormFilesToBase64(file);
            if (id != "0")
            {
                var rec = _repoDoc.Get(Convert.ToInt32( id)).Data.FirstOrDefault();
                rec.Document = stFile;
                return _repoDoc.Update(rec);
            }
            else if (id == "0")
                return _repoDoc.Insert(new DocumentHolder
                {
                    CreatedAt = DateTime.Now,
                    Document = stFile,
                    ModifiedAt = DateTime.Now
                });
            r.Status = "error";
            return r;

        }

        public string GetDoc(int id)
        {
            var rec = _repoDoc.Get(id).Data.FirstOrDefault();
            return rec != null ? rec.Document : "";
        }

        public async Task<ReturnObject> Register(string email, string password, string role, string clientId)
        {
            try
            {
                var r = new ReturnObject();
                r.Message = $"{role} added successfully";
                r.Status = "success";
                var passwordService = new AllHelper();
                var existingUser = _repo.GetAllForLogin().Data.FirstOrDefault(o => o.Email == email);
                if (existingUser != null)
                {
                    r.Status = "error";
                    r.Message = "User already exists";
                    return r;
                }
                Login la = new()
                {
                    Email = email,
                    Password = passwordService.HashPassword(password),
                    Role = role,
                    FirstLogin = true,
                    CreatedAt = DateTime.Now,
                    ClientId = role == "SuperAdmin" ? "-1" : clientId,
                    ModifiedAt = DateTime.Now
                };
                _repo.Insert(la);
                return r;
            }
            catch (Exception ex)
            {
                return new ReturnObject
                {
                    Status = "error",
                    Message = ex.Message
                };
            }
        }
    }
}
