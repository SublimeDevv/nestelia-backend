using Microsoft.AspNetCore.Mvc;
using Nestelia.Application.Interfaces.AuditLogs;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Nestelia.WebAPI.Controllers.AuditLogs
{
    [ApiExplorerSettings(IgnoreApi = true)] 

    public class MonitoringController(IAuditLogService service) : ControllerBase
    {
        private readonly IAuditLogService _service = service;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [Route("/getLogs")]
        public async Task GetLogs([FromQuery] int? level = null, [FromQuery] int? httpMethod = null, int offset = 0, int pageSize = 1000)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                while (webSocket.State == WebSocketState.Open)
                {
                    var data = await _service.GetAuditLogs(level, httpMethod, offset, pageSize);
                    var json = JsonSerializer.Serialize(data, _jsonOptions);

                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(json),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );

                    await Task.Delay(1000);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [Route("/getCountLogs")]
        public async Task GetCountLogs(int level = 0, int httpMethod = 0)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                while (webSocket.State == WebSocketState.Open)
                {
                    var data = await _service.GetCountLogs(level, httpMethod);
                    var json = JsonSerializer.Serialize(data, _jsonOptions);
                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(json),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                    await Task.Delay(1000);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [Route("/getEntitiesLogs")]
        public async Task GetEntitiesLogs()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                while (webSocket.State == WebSocketState.Open)
                {
                    var data = await _service.GetAuditEntities();
                    var json = JsonSerializer.Serialize(data, _jsonOptions);
                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(json),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                    await Task.Delay(1000);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [Route("/getLogsCount")]
        public async Task GetLogsCount()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                while (webSocket.State == WebSocketState.Open)
                {
                    var data = await _service.GetAuditLogsCount();
                    var json = JsonSerializer.Serialize(data, _jsonOptions);
                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(json),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                    await Task.Delay(1000);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

    }
}
