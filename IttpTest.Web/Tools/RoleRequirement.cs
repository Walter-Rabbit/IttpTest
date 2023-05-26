﻿using Microsoft.AspNetCore.Authorization;

namespace IttpTest.Web.Tools;

public class RoleRequirement : IAuthorizationRequirement
{
    public RoleRequirement(string role)
    {
        Role = role;
    }

    public string Role { get; }
}