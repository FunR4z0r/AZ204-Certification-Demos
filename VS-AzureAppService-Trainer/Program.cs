using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace VS_AzureAppService_Trainer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

            Action<MvcOptions> configureControllers = new Action<MvcOptions>(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // Add Key Vault configuration provider
            bool.TryParse(builder.Configuration["IsConfidentialApplication"], out var isConfidentialApplication);
            var keyVaultConfig = builder.Configuration.GetSection("KeyVaultAuth");
            if (isConfidentialApplication)
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultConfig["VaultUri"]),
                    new ClientSecretCredential(keyVaultConfig["TenantId"], keyVaultConfig["ClientId"], keyVaultConfig["ClientSecret"]));
            else
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultConfig["VaultUri"]),
                    new DefaultAzureCredential());

            bool.TryParse(builder.Configuration["RequireOpenId"], out var requireOpenIdAuth);
            if (requireOpenIdAuth)
                builder.Services.AddControllersWithViews(configureControllers);
            else
                builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages()
                .AddMicrosoftIdentityUI();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}