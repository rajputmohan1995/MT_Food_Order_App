﻿using System.ComponentModel.DataAnnotations;

namespace MT.Web.Models;

public class LoginDTO
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    public string? ReturnUrl { get; set; }
}