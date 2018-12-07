using Vasily.Core;

namespace Vasily
{
    public static class SqlTemplate
    {
        public static SelectTemplate Select;
        public static UpdateTemplate Update;
        public static DeleteTemplate Delete;
        public static InsertTemplate Insert;
        public static RepeateTemplate Repeate;
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
