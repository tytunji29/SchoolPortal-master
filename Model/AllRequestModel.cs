using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Model;

public class SignUpRequestModel : SignInRequestModel
{
}
public class GeneralFormModel
{
    public string Name { get; set; }
}
public class GeneralForFormModel : GeneralFormModel
{
    public int ClassId { get; set; }
}

public class MapClassToTeacher
{
    public int TeacherId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
}
public class ClassTeacherHistoryFormmodel : GeneralFormModel
{
    public string Teacher { get; set; }
    public string AssistantTeacher { get; set; }
}
public class StudentFormModel : Docm
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string DOB { get; set; }
    public string Tribe { get; set; }
    public string Religion { get; set; }
    public string Gender { get; set; }

    public string ParentMobileNumber { get; set; }
    public string Relationship { get; set; }

}
public class ParentFormmodel : Docm
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string DOB { get; set; }
    public string Tribe { get; set; }
    public string Religion { get; set; }
    public string Gender { get; set; }
}
public class Docm
{
    public IFormFile DisplayPicture { get; set; }

}
public class StaffRequestModel : Docm
{
    public string Name { get; set; }
    public string StaffType { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string DOB { get; set; }
    public string Tribe { get; set; }
    public string Religion { get; set; }
    public string Gender { get; set; }
}

public class TimetableRequest
{
    public int classId { get; set; }
    public Timetable timetable { get; set; }
    public int subjectId { get; set; }
}

public class Timetable
{
    public Monday Monday { get; set; }
    public Tuesday Tuesday { get; set; }
    public Wednesday Wednesday { get; set; }
    public Thursday Thursday { get; set; }
    public Friday Friday { get; set; }
}

public class Monday
{
    public string startTime { get; set; }
    public string endTime { get; set; }
}

public class Tuesday
{
    public string startTime { get; set; }
    public string endTime { get; set; }
}

public class Wednesday
{
    public string startTime { get; set; }
    public string endTime { get; set; }
}

public class Thursday
{
    public string startTime { get; set; }
    public string endTime { get; set; }
}

public class Friday
{
    public string startTime { get; set; }
    public string endTime { get; set; }
}

public class SchoolRequestModel
{
    public string Name { get; set; }
    public string Address { get; set; }
    public IFormFile Logo { get; set; }
    public string ThemeColour { get; set; }
    public string SubscriptionType { get; set; }
    public float SubscriptionTypeFee { get; set; }
    public string Website { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string PinKey { get; set; }
}
public class SignInRequestModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
