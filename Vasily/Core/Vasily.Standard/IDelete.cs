using System.Reflection;

namespace Vasily.Standard
{
    public interface IDelete
    {
        /// <summary>
        /// 根据model信息生成 DELETE FROM [TableName] WHERE 
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>删除字符串的结果</returns>
        string DeleteByCondition(MakerModel model);

        /// <summary>
        /// 根据model信息生成 DELETE FROM [TableName] WHERE [PrimaryKey] =@PrimaryKey
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>删除字符串的结果</returns>
        string DeleteByPrimary(MakerModel model);

        /// <summary>
        /// 生成 DELETE FROM [TableName] WHERE [condition1]=@condition1 AND [condition2]=@condition2
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="conditions">需要匹配的成员集合</param>
        /// <returns>删除字符串结果</returns>
        string DeleteWithCondition(MakerModel model, params MemberInfo[] conditions);
    }
}
