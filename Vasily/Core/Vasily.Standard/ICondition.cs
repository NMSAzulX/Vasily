using System.Reflection;

namespace Vasily.Standard
{
    public interface ICondition
    {
        /// <summary>
        /// 根据model信息生成 [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有委托处理的Model</param>
        /// <param name="conditions">需要匹配的成员集合</param>
        /// <returns>条件字符串结果</returns>
        string Condition(MakerModel model, params MemberInfo[] conditions);

        /// <summary>
        /// 根据model信息生成 [condition1]=@condition,[condition2]=@condition2.....
        /// </summary>
        /// <param name="model">载有委托处理的Model</param>
        /// <param name="conditions">需要匹配的字符串集合</param>
        /// <returns>条件字符串结果</returns>
        string Condition(MakerModel model, params string[] conditions);
    }
}
