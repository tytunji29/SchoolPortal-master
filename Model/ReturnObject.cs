using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Model;

public class ReturnObject
{
    public string Status { get; set; }
    public string Message { get; set; }
    public dynamic Data { get; set; }
}
public class ReturnObject<T>
{
    public IEnumerable<T> Data { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}

public class AuthDto
{
    public string email { get; set; }
    public string ClientId { get; set; }
    public string UserId { get; set; }
}
public class StaticData
{
    [Key]
    public int Id { get; set; }
    public string CreatedBy { get; set; }
    public string ClientId { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
