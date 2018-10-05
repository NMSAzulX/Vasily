using System.Reflection;

namespace Vasily.Standard
{
    public interface IUpdate
    {
        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>更新字符串结果</returns>
        string UpdateByCondition(MakerModel model);

        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE PrimaryKey=@PrimaryKe
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>更新字符串结果</returns>
        string UpdateByPrimary(MakerModel model);


        /// <summary>
        /// 根据model信息生成 UPDATE [TableName] SET([member1]=@member1,[member2]...=@member2...) WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>更新字符串结果</returns>
        string UpdateWithCondition(MakerModel model, params MemberInfo[] conditions);
    }
}
