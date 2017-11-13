using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBHWA.Models
{
    public class Query
    {
        public Query()
        {
            FieldName = "";
            FieldValue = "";
            QueryString = "";
        }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string QueryString { get; set; }
    }
}