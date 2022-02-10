# Authentication
It´s a library for Authentication by JWT. 
Easy to implement in your project.

### For implement:
In the startup.cs
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthorization(options =>
        {
            options.AddPolicy("LoggingAuthorizationHandler",
            policy => policy.Requirements.Add(new LoggingAuthorizationHandler()));
        });

    services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer();
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthorization();
}
```

In the project.csproj
```
<ItemGroup>
    <Reference Include="Authentication.Lib">
      <HintPath>..\SolutionItems\Authentication.Lib.dll</HintPath>
    </Reference>
  </ItemGroup>
```