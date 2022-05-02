using AgileIM.IM.Helper;
using AgileIM.Service.OAuth;
using AgileIM.Service.OAuth.Configs;
using AgileIM.Service.Services;
using AgileIM.Service.Services.UserService;
using AgileIM.Shared.EFCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IVerifyService, VerifyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services
    .AddIdentityServer()
    .AddDeveloperSigningCredential(true, "tempkey.jwk")
    // �ͻ���������ӵ��ڴ���
    .AddInMemoryClients(Ide4Config.GetApiClients)
    .AddInMemoryApiScopes(Ide4Config.GetApiScopes)
    // ��Ӷ�OpenID Connect��֧��
    .AddInMemoryIdentityResources(Ide4Config.GetIdentityResources)
    //���ܱ�����Api��Դ��ӵ��ڴ���
    .AddInMemoryApiResources(Ide4Config.GetApiResource)
    // �û���֤
    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddProfileService<ProfileService>();


builder.Services.AddDbContext<AgileImDbContext>(options =>
{
    var sqlServerConnStr = builder.Configuration["SqlServerConnStr"];
    options.UseSqlServer(sqlServerConnStr);
});

builder.WebHost.UseUrls("http://*:9659");
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseImServer();

app.UseAuthorization().UseAuthentication();

app.MapControllers();

IdentityModelEventSource.ShowPII = true;

app.UseIdentityServer();

app.Run();

