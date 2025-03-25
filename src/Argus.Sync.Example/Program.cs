using Argus.Sync.Data.Models;
using Argus.Sync.Example.Data.Models;
using Argus.Sync.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCardanoIndexer<OrderBookDbContext>(builder.Configuration);
builder.Services.AddReducers<OrderBookDbContext, IReducerModel>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();
