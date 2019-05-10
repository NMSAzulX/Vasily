namespace Microsoft.AspNetCore.Mvc
{
    public ref struct ReturnPageResult
    {
        public string message;
        public object data;
        public int code;
        public int totle;
        public string description;
    }
    public ref struct ReturnResult
    {
        public string message;
        public string description;
        public object data;
        public int code;
    }

   
}

