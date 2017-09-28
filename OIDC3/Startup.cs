using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace OIDC3
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Added by me

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var options = new OpenIdConnectOptions()
            {

                // AuthenticationScheme = "oidc",

                Authority = "https://preprod.signicat.com/oidc",
                //RequireHttpsMetadata = false, //added as test
                //SaveTokens = true,


                ClientId = "demo-preprod-postsecret", // "demo-preprod",
                ClientSecret = "mqZ-_75-f2wNsiQTONb7On4aAZ7zc218mrRVk1oufa8",
                CallbackPath = "/redirect",
                SignInScheme = "Cookies",
                // SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                ResponseType = "code", //"id_token token",  
                




                //TokenValidationParameters = new TokenValidationParameters
                //{
                //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mqZ-_75-f2wNsiQTONb7On4aAZ7zc218mrRVk1oufa8"))
                //},

                Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = context =>
                    {

                        context.ProtocolMessage.AcrValues = "urn:signicat:oidc:method:sbid";
                        return Task.FromResult(0);
                    },
                    OnRemoteFailure = context =>
                    {

                        //  context.Response.Redirect("/Home/Error/");
                        context.Response.Redirect("/Home/Error?error=" + context.Failure.Message);

                        context.HandleResponse();
                        return Task.FromResult(0);
                    },

                    //            OnAuthenticationFailed = notification =>
                    //            {
                    //                if (string.Equals(notification.ProtocolMessage.Error, "access_denied", System.StringComparison.Ordinal))
                    //                {
                    //                    notification.HandleResponse();

                    //                    notification.Response.Redirect("/");
                    //                }

                    //                return Task.FromResult<object>(null);
                    //            }

                }
            };






            ////Configuration = new OpenIdConnectConfiguration
            //{
            //    AuthorizationEndpoint = "https://preprod.signicat.com/oidc/authorize",
            //    TokenEndpoint = "https://preprod.signicat.com/oidc/token",
            //    UserInfoEndpoint = "https://preprod.signicat.com/oidc/userinfo",
            //},


            //        return Task.FromResult(0);
            //    },


            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            
            app.UseOpenIdConnectAuthentication(options);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
