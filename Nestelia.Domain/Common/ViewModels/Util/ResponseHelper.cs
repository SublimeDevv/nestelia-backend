namespace Nestelia.Domain.Common.ViewModels.Util
{
    public class ResponseHelper
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public static ResponseHelper CreateSuccess(string message, object? data = null) => new() { Success = true, Message = message, Data = data };
        public static ResponseHelper CreateFailure(string message, object? data = null) => new() { Success = false, Message = message, Data = data };
    }
}