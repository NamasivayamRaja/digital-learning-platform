using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Extensions
{
    public static class HttpExtensions
    {
        public static HttpResponse AddPaginationResponseHeader<T>(this HttpResponse response, PagedList<T> header)
        {
            var paginationHeader = new PaginationHeader(header.CurrentPage, header.PageSize, header.TotalPages, header.TotalCount);

            var jsonOptions = new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var serializedPageHeader = JsonSerializer.Serialize(paginationHeader, jsonOptions);

            response.Headers.Append("Pagination", serializedPageHeader);
            
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");

            return response;
        }
    }
}
