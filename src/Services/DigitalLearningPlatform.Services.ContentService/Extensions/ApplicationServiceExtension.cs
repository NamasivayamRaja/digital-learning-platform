using Azure.Storage.Blobs;
using DigitalLearningPlatform.ContentService.Repositories;
using DigitalLearningPlatform.Services.ContentService.Application.EventHandlers;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Application.Services;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
namespace DigitalLearningPlatform.Services.ContentService.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddOpenApi();
            services.AddMapster();
            services.AddDbContext<CourseDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton<IStorageService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
                return new AzureStorageService(blobServiceClient);
            });

            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ISectionFileRepository, SectionFileRepository>();
            services.AddScoped<ICourseFileUploadRepository, CourseFileUploadRepository>();
            services.AddScoped<IAuthorLookUpRepository, AuthorLookUpRepository>();
            services.AddScoped<IContentQueryRepository, ContentQueryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<ICourseContentService, CourseContentService>();

            // Event Handlers
            services.AddScoped<InstructorRegisteredIntegrationEventHandler>();

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToke) => {
                    document.Servers = [];
                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
