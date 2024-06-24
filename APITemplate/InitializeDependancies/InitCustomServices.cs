using APITemplate.Services;

namespace APITemplate.InitializeDependancies
{
    public static class InitCustomServices
    {
        public static void SetupCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService, TaskService>();
        }
    }
}
