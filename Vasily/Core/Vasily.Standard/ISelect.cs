using System;
using System.Reflection;

namespace Vasily.Standard
{
    public interface ISelect
    {
        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName]
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectAll(MakerModel model);

        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectAllByCondition(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE [PrimaryKey] = @PrimaryKey
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectAllByPrimary(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT * FROM [TableName] WHERE [PrimaryKey] IN @keys
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectAllIn(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName]
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string Select(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE 
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectByCondition(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [PrimaryKey]=@PrimaryKey
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectByPrimary(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [PrimaryKey] IN @keys
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>查询字符串结果</returns>
        string SelectIn(MakerModel model);


        /// <summary>
        /// 根据model信息生成 SELECT [member1],[member2]... FROM [TableName] WHERE [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <param name="condition_models">需要匹配的成员集合</param>
        /// <returns>查询字符串结果</returns>
        string SelectWithCondition(MakerModel model, params MemberInfo[] condition_models);
        
    }
}
