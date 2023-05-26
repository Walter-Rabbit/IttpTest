using IttpTest.Core;
using IttpTest.Data;
using IttpTest.Web.Tools;
using IttpTest.Web.Tools.Handlers;
using IttpTest.Web.Tools.Requirements;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.Name = HeaderNames.Cookie;
        opt.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        opt.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => { policy.Requirements.Add(new RoleRequirement("Admin")); });
    opt.InvokeHandlersAfterFailure = false;
});

builder.Services.AddSingleton<IAuthorizationHandler, RoleHandler>();
builder.Services.AddControllers(opt => opt.Filters.Add<ExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddData(configuration);
builder.Services.AddCore();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();