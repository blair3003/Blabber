using Blabber.Api.Data;
using Blabber.Api.Repositories;
using Blabber.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add logging to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBlabRepository, BlabRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IBlabService, BlabService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICommentService, CommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/api/account").MapIdentityApi<ApplicationUser>();
app.MapControllers();

app.Run();
