using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThreadsProject.Bussiness;
using ThreadsProject.Bussiness.GlobalException;
using ThreadsProject.Bussiness.Profilies;
using ThreadsProject.Core.Entities;
using ThreadsProject.Data.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ThreadsContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric=false;
    opt.User.RequireUniqueEmail = true;
    
}).AddEntityFrameworkStores<ThreadsContext>().AddDefaultTokenProviders();
builder.Services.AddServices();
builder.Services.AddAutoMapper(typeof(UserMapProfilies));


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
//}
//else
//{
//    app.UseHsts();
//}


app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
