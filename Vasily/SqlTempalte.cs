using Vasily.Core;
using Vasily.Standard;

namespace Vasily
{
    public static class SqlTemplate
    {
        public static ISelect Select;
        public static IUpdate Update;
        public static IDelete Delete;
        public static IInsert Insert;
        public static IRepeate Repeate;
        static SqlTemplate()
        {
            Select = new SelectTemplate();
            Update = new UpdateTemplate();
            Delete = new DeleteTemplate();
            Insert = new InsertTemplate();
            Repeate = new RepeateTemplate();
        }
    }

    

    

}
