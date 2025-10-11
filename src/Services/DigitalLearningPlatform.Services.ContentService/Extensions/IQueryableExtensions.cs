using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
namespace DigitalLearningPlatform.Services.ContentService.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}
