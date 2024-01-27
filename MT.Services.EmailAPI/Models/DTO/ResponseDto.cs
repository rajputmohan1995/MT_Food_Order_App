﻿namespace MT.Services.EmailAPI;

public class ResponseDto
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public object? Result { get; set; }
}