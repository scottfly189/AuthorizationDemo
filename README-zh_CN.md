jwt鉴权的使用 ([English](README.md)）
========

一、jwt的使用思路：
-----------

* 用户用密码、帐号登录后，向前端发放jwt;
* 前端按照约定将jwt放在请求头中，向服务器后端发送请求;
* 服务器接收到请求后，[asp.net](http://asp.net) core通过配置服务与中间件解析请求，并且自动鉴权;

## 二、一个简单使用示例:

* ### 前置条件
  
  ```
  dotnet add package Microsoft.AspCore.Authentication.JwtBearer
  dotnet add package System.IdentityModel.Tokens.Jwt
  ```

* ### 使用步骤
1. ### 配置jwt鉴权的信息，在appsettings.json中配置
   
   

```
  "Jwt": {
    "Key": "3F025D682370B0126BBAE7A93D9B66CE3F025D682370B0126BBAE7A93D9B66CE",
    "Issuer": "DemoIssuer",
    "Audience": "your_audience"
  }
```

> 注意：.net8对于key有长度有特殊要求，不能太短，一般要求32个字符;

2. ### 登录成功后发放jwt,核心代码如下：

```
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            ...here check the user,emal and passwrd.
            var token = _tokenService.CreateToken(demoUser);
            await Task.CompletedTask;
            return Ok(new AuthResponse { Token = token, UserId = demoUser.Id.ToString(), UserName = demoUser.UserName });
        }
```

```
        public string CreateToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = System.Text.Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                      //这里声明存放在前端并且后期需要使用的内容。
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
```

3. ### 前端根据后端传回的内容在本地保存token,并且随后每次请求都携带这个token,当然如果过期，要刷新或者重新获取token.

4. ### 在asp.net core webapi项目中配置服务与中间件，目的是为了jwt鉴权
   
   ```
   //配置依赖注入服务，注意这里的配置项与发放的token的配置要保持一致。
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
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
   
   ```
   app.UseAuthorization();
   ```

5. ### 如果需要Swagger支持鉴权，则做下面的swagger配置
   
   ```
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
   ```

6. ### 使用
* 在控制器或者action中加入Authorize或者 AllowAnonymous(如果不需要鉴权);
* 在Controller - action中或者HttpContext中使用this.User可以取到jwt中token中的内容;
