using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Recipe.Common.Helper
{
    public class JsonApiPagination
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }

    public class JsonApiRequest
    {
        public JsonApiRequest()
        {
            this.Filters = new Dictionary<string, string>();
        }

        public List<string> Include { get; set; }

        public List<string> Sort { get; set; }

        public Dictionary<string, string> Filters { get; set; }

        public JsonApiPagination Pagination { get; set; }
    }
}
