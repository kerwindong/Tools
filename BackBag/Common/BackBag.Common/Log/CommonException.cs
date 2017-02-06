using System;
using System.Collections.Generic;

namespace BackBag.Common.Log
{
    public class CommonException 
    {
        public string Title { private set; get; }

        public Exception Exception { private set; get; }

        public CommonException(string title, Exception exception, Dictionary<string, string> customTags = default(Dictionary<string, string>))
        {
            Title = title;

            Exception = exception;
        }
    }

    
}
