using FpsServer.Api.Validators;
using FpsServer.Application.Matchmaking.UseCases;
using FpsServer.Application.Chat.UseCases;
using FpsServer.Infrastructure.Matchmaking;
using FpsServer.Infrastructure.Chat;
using FpsServer.Infrastructure.MatchSession;
using FpsServer.Infrastructure.MMR;
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
builder.Services.AddValidatorsFromAssemblyContaining<StartMatchRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Infrastructure 레이어 서비스 등록
builder.Services.AddMatchmakingInfrastructure();
builder.Services.AddChatInfrastructure();
builder.Services.AddMatchSessionInfrastructure();
builder.Services.AddMMRInfrastructure();

// UseCase 등록
builder.Services.AddScoped<JoinMatchmakingQueueUseCase>();
builder.Services.AddScoped<CancelMatchmakingUseCase>();
builder.Services.AddScoped<SendMessageUseCase>();
builder.Services.AddScoped<GetChatHistoryUseCase>();
builder.Services.AddScoped<FpsServer.Application.MatchSession.UseCases.StartMatchUseCase>();
builder.Services.AddScoped<FpsServer.Application.MatchSession.UseCases.EndMatchUseCase>();
builder.Services.AddScoped<FpsServer.Application.MMR.UseCases.UpdatePlayerMMRUseCase>();

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

// OpenAPI 설정 (.NET 9)
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
app.MapHub<ChatHub>("/chat-hub");

app.Run();
