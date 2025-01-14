﻿using emcr_dfa_poc18.Models.AppSettings;
using emcr_dfa_poc18.Services;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.Enums;
using Microsoft.Extensions.Options;
using pdfservice.Utils;
using Stubble.Core.Contexts;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var envAppsettings = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
builder.Configuration.AddJsonFile(envAppsettings, optional: true);

var appWebDefaultSection = builder.Configuration.GetSection("WebDefaults");
builder.Services.Configure<AppWebDefaults>(appWebDefaultSection);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IPdfRequestService, PdfRequestService>();
builder.Services.AddTransient<IPdfServiceWebUtility, PdfServiceWebUtility>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
