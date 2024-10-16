# AuthorizationDemo [中文版](README-zh_CN.md)

### Integrating JWT to .NET 8 - this is asp.net core .net8 webapi jwt simple demo,You can learn how to issue and interpret JWT in ASP.NET Core .NET 8 Web API.

I. JWT Usage Strategy:
----------------------

* After users log in with their password and account, JWT is issued to the frontend;
* The frontend places the JWT in the request header as agreed and sends requests to the backend server;
* After receiving the request, [ASP.NET](http://ASP.NET) Core parses the request and automatically authenticates through configured services and middleware;

### Directory description:

1. server: an Asp.net core web api backend, which mainly provides authentication services;

2. web: a vue3 frontend, which sends requests to the backend through axios and displays the results;

3. 

## II. A Simple Usage Example:

* Prerequisites
  
  ```
  dotnet add package Microsoft.AspCore.Authentication.JwtBearer 
  dotnet add package System.IdentityModel.Tokens.Jwt
  ```

* ### Usage Steps
1. Configure JWT authentication information in appsettings.json
   
   ```
     "Jwt": {
       "Key": "3F025D682370B0126BBAE7A93D9B66CE3F025D682370B0126BBAE7A93D9B66CE",
       "Issuer": "DemoIssuer",
       "Audience": "your_audience"
     }
   ```

> Note: NET 8 has specific length requirements for the key, it can't be too short, generally requiring 32 characters;

1. Issue JWT after successful login, core code as follows:
   
            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginRequest request)
            {
                ...here check the user,emal and passwrd.
                var token = _tokenService.CreateToken(demoUser);
                await Task.CompletedTask;
                return Ok(new AuthResponse { Token = token, UserId = demoUser.Id.ToString(), UserName = demoUser.UserName });
            }
       
            public string CreateToken(User user)
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = System.Text.Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                          //Here declare the content to be stored in the frontend and needed later.
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(ClaimTypes.Role,"test"),
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"], 
                    Audience = _configuration["Jwt:Audience"],
                };
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

2. The frontend saves the token locally based on the content returned by the backend, and carries this token with each subsequent request. Of course, if it expires, the token needs to be refreshed or re-obtained.

3. Configure services and middleware in the [ASP.NET](http://ASP.NET) Core WebAPI project
   
        //Configure dependency injection services, note that the configuration here should be consistent with the token issuance configuration. 
       
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ClockSkew = TimeSpan.Zero
            };
        });
   
   ```
   app.UseAuthorization();
   ```

4. If Swagger support for authentication is needed, make the following Swagger configuration
   
       builder.Services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new() { Title = "Pappa's API", Version = "v1" });
           c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
           {
               Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
               Name = "Authorization",
               In = ParameterLocation.Header,
               Type = SecuritySchemeType.ApiKey,
               Scheme = "Bearer"
           });
           c.AddSecurityRequirement(new OpenApiSecurityRequirement()
           {
               {
                   new OpenApiSecurityScheme
                   {
                       Reference = new OpenApiReference
                       {
                           Type = ReferenceType.SecurityScheme,
                           Id = "Bearer"
                       },
                       Scheme = "oauth2",
                       Name = "Bearer",
                       In = ParameterLocation.Header,
                   },
                   new List<string>()
               }
           });
       });

5. ### Usage
* Add [Authorize] or [AllowAnonymous] (if authentication is not required) to the controller or action;
* Use this.User in the Controller - action or HttpContext to retrieve the content from the JWT token;
  
  

# I wish you a happy swim in the ocean of freedom!
