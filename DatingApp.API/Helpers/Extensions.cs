using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add(HttpResponseHeadersKey.AppliationError, message);
            response.Headers.Add(HttpResponseHeadersKey.AccessControlExposeHeaders, HttpResponseHeadersKey.AppliationError);
            response.Headers.Add(HttpResponseHeadersKey.AccessControlAllowOrigin, "*");
        }

        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var painationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add(HttpResponseHeadersKey.Pagination, JsonConvert.SerializeObject(painationHeader, camelCaseFormatter));
            response.Headers.Add(HttpResponseHeadersKey.AccessControlExposeHeaders, HttpResponseHeadersKey.Pagination);
        }

        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }
    }
}