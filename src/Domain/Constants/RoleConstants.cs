namespace PTJ.Domain.Constants;

public static class RoleConstants
{
    public const string Admin = "ADMIN";
    public const string Employer = "EMPLOYER";
    public const string Student = "STUDENT";

    public static readonly string[] AllRoles = { Admin, Employer, Student };
}
