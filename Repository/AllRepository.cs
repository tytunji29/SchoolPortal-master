using Microsoft.EntityFrameworkCore;
using SchoolPortal.Model;

namespace SchoolPortal.Repository;
public interface IAllRepository<T> where T : StaticData
{
    // IEnumerable<T> GetAllIsValid();
    ReturnObject<T> GetAll();
    ReturnObject<T> GetAllForLogin();

    ReturnObject<T> Get(int id);

    ReturnObject Insert(T entity);

    ReturnObject Update(T entity);
    //void SoftDelete(T entity);

    ReturnObject<T> Delete(int id);
}
public class AllRepository<T> : IAllRepository<T> where T : StaticData
{
    public IHttpContextAccessor _httpContextAccessor;

    private readonly SchoolDbContext _context;
    private DbSet<T> entities;
    private string errorMessage = string.Empty;
    AuthDto auth = new();

    public AllRepository(SchoolDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        //in case of controller with many repo
        if (auth.ClientId == null)
        {
            _httpContextAccessor = httpContextAccessor;
            var k = _httpContextAccessor.HttpContext.User.Claims.ToList();
            if (k.Any())
            {
                auth.ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value : "";
                auth.UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            }
        }

        _context = context;
        entities = context.Set<T>();
    }

    public ReturnObject<T> GetAllForLogin()
    {
        var r = new ReturnObject<T>();
        try
        {
            //.Where(o => o.ClientId == auth.ClientId)
            var rec = entities.AsEnumerable();
            r.Status = "success";
            r.Message = "Record Found Successfully";
            r.Data = rec;
        }
        catch (Exception ex)
        {
            r.Status = "Error";
            r.Message = ex.Message;
        }
        return r;

    }
    public ReturnObject<T> GetAll()
    {
        var r = new ReturnObject<T>();
        try
        {
            var rec = entities.AsEnumerable().Where(o => o.ClientId == auth.ClientId);
            r.Status = "success";
            r.Message = "Record Found Successfully";
            r.Data = rec;
        }
        catch (Exception ex)
        {
            r.Status = "Error";
            r.Message = ex.Message;
        }
        return r;

    }
    public ReturnObject<T> Get(int id)
    {
        var result = new ReturnObject<T>();

        try
        {
            // Check if entities is null or empty
            if (entities == null)
            {
                result.Status = "Error";
                result.Message = "No data source available.";
                return result;
            }

            // Find the record
            var record = entities.Where(o => o.ClientId == auth.ClientId && o.Id == id);

            if (record != null)
            {
                result.Status = "success";
                result.Message = "Record found successfully.";
                result.Data = record;
            }
            else
            {
                result.Status = "NotFound";
                result.Message = "Record not found.";
            }
        }
        catch (Exception ex)
        {
            // Log the exception here (if logging is set up)
            result.Status = "Error";
            result.Message = $"An error occurred while fetching the record: {ex.Message}";
        }

        return result;
    }


    public ReturnObject Insert(T entity)
    {
        var r = new ReturnObject();
        try
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            entity.ClientId = entity.ClientId ?? auth.ClientId ?? "-1";
            entity.CreatedBy = entity.CreatedBy ?? auth.UserId ?? "-1";
            entity.ModifiedBy = entity.CreatedBy ?? auth.UserId ?? "-1";
            entity.CreatedAt = DateTime.UtcNow;
            entity.ModifiedAt = DateTime.UtcNow;
            entities.Add(entity);
            _context.SaveChanges();
            r.Status = "success";
            r.Message = "Record Saved Successfully";
            r.Data = entity.Id;
        }
        catch (Exception ex)
        {
            r.Status = "Error";
            r.Message = ex.Message;
        }
        return r;
    }

    public ReturnObject Update(T entity)
    {
        var r = new ReturnObject();
        try
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            entity.ModifiedBy = auth.UserId != null ? auth.UserId : "-1";
            entity.ModifiedAt = DateTime.UtcNow;
            _context.SaveChanges();
            r.Status = "success";
            r.Message = "Record Updated Successfully";
            r.Data = null;
        }
        catch (Exception ex)
        {
            r.Status = "Error";
            r.Message = ex.Message;
        }
        return r;
    }

    public ReturnObject<T> Delete(int id)
    {
        var r = new ReturnObject<T>();
        try
        {
            if (id == 0)
            {
                throw new ArgumentNullException("entity");
            }
            var record = entities.FirstOrDefault(o => o.ClientId == auth.ClientId && o.Id == id);

            entities.Remove(record);
            _context.SaveChanges();
            r.Data = null;
        }
        catch (Exception ex)
        {
            r.Status = "Error";
            r.Message = ex.Message;
        }
        return r;
    }

}