﻿namespace MT.Web.Models;

public class ResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Result { get; set; }
}
