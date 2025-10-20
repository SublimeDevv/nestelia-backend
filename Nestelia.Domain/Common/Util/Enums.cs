namespace Nestelia.Domain.Common.Util
{
    public class Enums
    {
        public enum Sexo
        {
            NO_ESPECIFICADO,
            MASCULINO,
            FEMENINO,
            OTRO
        }
        
        public enum Roles
        {
            Admin,
            User,
        }

        public enum AuditLogLevel
        {
            INFO,
            WARNING,
            ERROR,
            SUCCESS
        }

        public enum HttpMethodLog
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH
        }
    }
}
