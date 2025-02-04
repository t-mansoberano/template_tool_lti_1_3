namespace gec.Infrastructure.Lti;

public static class ClaimTypes
{
    public const string Roles = "https://purl.imsglobal.org/spec/lti/claim/roles";
    public const string Custom = "https://purl.imsglobal.org/spec/lti/claim/custom";
    public const string Context = "https://purl.imsglobal.org/spec/lti/claim/context";
    public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    public const string UserId = "user_id";
    public const string CourseId = "course_id";
    public const string Name = "name";
    public const string Picture = "picture";
    public const string Label = "label";
    public const string Title = "title";
    public const string Type = "type";
    public const string RoleInstructor = "http://purl.imsglobal.org/vocab/lis/v2/institution/person#Instructor";
    public const string RoleStudent = "http://purl.imsglobal.org/vocab/lis/v2/institution/person#Student";
}