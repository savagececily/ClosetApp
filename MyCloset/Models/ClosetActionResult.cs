using System;
using System.Net;

namespace MyCloset.Models
{
    public class ClosetActionResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
