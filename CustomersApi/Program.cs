using CustomersApi.Data;
using CustomersApi.Infrastructure;
using CustomersApi.Models;
using Microsoft.EntityFrameworkCore;
using OrderApi.Infrastructure;
using SharedModels;
using MessageListener = CustomersApi.Infrastructure.MessageListener;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string rabbitmqConnectionString = "host=rabbitmq";

builder.Services.AddDbContext<CustomerApiContext>(opt => opt.UseInMemoryDatabase("CustomerDb"));

builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();

builder.Services.AddTransient<IDbInitializer, DbInitializer>();

builder.Services.AddSingleton<IConverter<Customer, CustomerDto>, CustomerConverter>();
builder.Services.AddSingleton<IMessagePublisher>(new MessagePublisher(rabbitmqConnectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Initialize the database.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<CustomerApiContext>();
    var dbInitializer = services.GetService<IDbInitializer>();
    dbInitializer.Initialize(dbContext);
}

Task.Factory.StartNew(() => new MessageListener(app.Services, rabbitmqConnectionString).Start());

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();