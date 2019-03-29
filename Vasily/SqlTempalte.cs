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



        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE  + condition
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>

        public static string CustomerSelect(SqlModel model,string condition)
        {
            return Select.SelectWhere(model) + condition;
        }


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [PrimaryKey] IN @keys
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        public static string SelectIn(SqlModel model)
        {
            return Select.SelectIn(model);
        }


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>查询字符串结果</returns>
        public static string SelectWithCondition(SqlModel model, params string[] conditions)
        {
            return Select.SelectWithCondition(model,conditions);
        }


        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition">查询字符串</param>
        /// <returns>更新字符串结果</returns>
        public static string CustomerUpdate(SqlModel model,string condition)
        {
            return Update.UpdateWhere(model) + condition;
        }



        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>更新字符串结果</returns>
        public static string UpdateWithCondition(SqlModel model, params string[] conditions)
        {
            return Update.UpdateWithCondition(model,conditions);
        }


        /// <summary>
        /// 生成 DELETE FROM [TableName] WHERE [condition1]=@condition1 AND [condition2]=@condition2
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="conditions">需要匹配的成员集合</param>
        /// <returns>删除字符串结果</returns>
        public static string DeleteWithCondition(SqlModel model, params string[] conditions)
        {
            return Delete.DeleteWithCondition(model,conditions);
        }

        /// <summary>
        /// 生成 DELETE FROM [TableName] WHERE [condition1]=@condition1 AND [condition2]=@condition2
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition">查询字符串</param>
        /// <returns>删除字符串结果</returns>
        public static string CustomerDelete(SqlModel model, string condition)
        {
            return Delete.DeleteWhere(model)+condition;
        }


        /// <summary>
        /// 根据model信息生成 SELECT COUNT(*) FROM [TableName] WHERE [Member1]=@Member1 AND [Member2]=@Member2 ....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查重字符串结果</returns>
        public static string RepeateCount(SqlModel model)
        {
            return Repeate.RepeateCount(model);
        }
    }
}
