using FpsServer.Api.Validators;
using FpsServer.Application.Matchmaking.UseCases;
using FpsServer.Infrastructure.Matchmaking;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// SignalR 등록
builder.Services.AddSignalR();

// FluentValidation 등록
builder.Services.AddValidatorsFromAssemblyContaining<JoinMatchmakingRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Infrastructure 레이어 서비스 등록
builder.Services.AddMatchmakingInfrastructure();

// UseCase 등록
builder.Services.AddScoped<JoinMatchmakingQueueUseCase>();
builder.Services.AddScoped<CancelMatchmakingUseCase>();

// CORS 설정 (로컬 테스트용)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// CORS 미들웨어
app.UseCors();

app.UseAuthorization();

app.MapControllers();

// SignalR Hub 엔드포인트 등록
app.MapHub<MatchmakingHub>("/matchmaking-hub");

app.Run();
