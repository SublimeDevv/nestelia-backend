public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("Acceso prohibido. Token inválido o expirado.") { }

    public ForbiddenAccessException(string message)
        : base(message) { }

    public ForbiddenAccessException(string message, Exception innerException)
        : base(message, innerException) { }
}
