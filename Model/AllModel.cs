namespace SchoolPortal.Model;

using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



public class LoginReturn
{
    public string Token { get; set; }
    public string Role { get; set; }
    public string ClientId { get; set; }
    public string UserId { get; set; }
    public bool FirstLogin { get; set; }

}
public class Login : StaticData
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public bool FirstLogin { get; set; }
}
public class DocumentHolder : StaticData
{
    public string Document { get; set; }
}
public class School : StaticData
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string DocId { get; set; }
    public string ThemeColour { get; set; }
    public string SubscriptionType { get; set; }
    public float SubscriptionTypeFee { get; set; }
    public string Website { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string PinKey { get; set; }
}
public class Classes : StaticData
{
    public string Name { get; set; }
    public string Teacher { get; set; }
    public string AssistantTeacher { get; set; }
}
public class Subjects : StaticData
{
    public string Name { get; set; }
}
public class Map_Subjects_Teacher_Class : StaticData
{
    public int TeacherId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
}public class TeacherTimetable : StaticData
{
    public int TeacherId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public string Monday { get; set; }
    public string Tuesday { get; set; }
    public string Wednessday { get; set; }
    public string Thursday { get; set; }
    public string Friday { get; set; }
}

public class ClassesLevel : StaticData
{
    public string Name { get; set; }
}
public class ClassTeacherHistory : StaticData
{
    public string ClassName { get; set; }
    public string FormalTeacher { get; set; }
    public string FormalAssistTeacher { get; set; }
}
public class Staff : StaticData
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string StaffType { get; set; }
    public string DOB { get; set; }
    public string DocId { get; set; }
    public string Tribe { get; set; }
    public string Religion { get; set; }
    public string Gender { get; set; }
}
public class Student : StaticData
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string DOB { get; set; }
    public string Tribe { get; set; }
    public string Religion { get; set; }
    public string DocId { get; set; }
    public string Gender { get; set; }
    public string ParentMobileNumber { get; set; }
    public string Relationship { get; set; }
}
public class Payment : StaticData
{
}
public class Result : StaticData
{
}
public class Parent : StaticData
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string DOB { get; set; }
    public string Tribe { get; set; }
    public string Religion { get; set; }
    public string DocId { get; set; }
    public string Gender { get; set; }
}
