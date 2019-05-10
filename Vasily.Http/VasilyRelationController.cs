using Vasily;
using Vasily.Http.Standard;

namespace Microsoft.AspNetCore.Mvc
{

    public class VasilyController<T, R, S1> : VasilyRelationResultController<T> where T : class
    {

        public VasilyController()
        {

        }

        public VasilyController(string key) : base()
        {
            driver = new DapperWrapper<T, R, S1>(key);

        }
        public VasilyController(string reader, string writter) : base()
        {
            driver = new DapperWrapper<T, R, S1>(reader, writter);
        }


    }
    public class VasilyController<T, R, S1, S2> : VasilyRelationResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2>(reader, writter);
        }

    }

    public class VasilyController<T, R, S1, S2, S3> : VasilyRelationResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3>(reader, writter);
        }

    }
    public class VasilyController<T, R, S1, S2, S3, S4> : VasilyRelationResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4>(reader, writter);
        }

    }

    public class VasilyController<T, R, S1, S2, S3, S4, S5> : VasilyRelationResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5>(reader, writter);
        }

    }
    public class VasilyController<T, R, S1, S2, S3, S4, S5, S6> : VasilyRelationResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5, S6>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5, S6>(reader, writter);
        }
    }
}
