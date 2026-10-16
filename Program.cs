var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseSession();          // before MapControllers
app.UseAuthorization();    // keep this if the template added it
app.MapControllers();

app.Run();


public partial class Program { }