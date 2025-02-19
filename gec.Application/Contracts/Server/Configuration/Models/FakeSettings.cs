namespace gec.Application.Contracts.Server.Configuration.Models;

public class FakeSettings
{
    public const string Key = "Fake";

    public bool UseFakeLti { get; set; } = false;
    public bool UseFakeApiCanvas { get; set; } = false;
    public bool UseFakeFederation { get; set; } = false;
    public string FakeApiCanvasToken { get; set; } = string.Empty;
    public string FakeLtiContextPath { get; set; } = string.Empty;
    public string FakeFederationContextPath { get; set; } = string.Empty;
    public string FakeCompleteEvaluationsViewPath { get; set; } = string.Empty;
    public string FakeStudentCourseEvaluationsViewPath { get; set; } = string.Empty;
}