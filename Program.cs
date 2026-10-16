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

// the test client talks plain http, so a redirect to https would turn every response into a 307
if (!app.Environment.IsEnvironment("Testing"))
    app.UseHttpsRedirection();

app.UseSession();          // before MapControllers
app.UseAuthorization();
app.MapControllers();

app.Run();


public partial class Program { }