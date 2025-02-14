namespace gec.Server.Identidad
{
    public class Constants
    {
        public static class claims
        {
            public const string Nomina = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/NAM_SAMAccountName";
            public const string NombreCompleto = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/fullName";
            public const string CampusAlumno = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/ITESMStdProfCveCampus";
            public const string CampusEmpleado = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/ITESMProfSubdivision";
            public const string ClavePrograma = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/ITESMStdProfCveProgramaAcad";
            public const string PeriodoAcademico = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/ITESMStdProfCveEjercicioAcad";
        }
    }
}
