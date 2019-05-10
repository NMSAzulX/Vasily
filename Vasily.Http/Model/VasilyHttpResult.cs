namespace Microsoft.AspNetCore.Mvc
{
    public ref struct ReturnPageResult
    {
        public string message;
        public object data;
        public int code;
        public int totle;
        public string description;

        public ReturnPageResult Ok()
        {
            code = 0;
            return this;
        }

        public ReturnPageResult Error(int error = 1)
        {
            code = error;
            return this;
        }
    }
    public ref struct ReturnResult
    {
        public string message;
        public string description;
        public object data;
        public int code;

        public ReturnResult Ok()
        {
           code = 0;
           return this;
        }

        public ReturnResult Error(int error=1)
        {
            code = error;
            return this;
        }
    }

   
}

