namespace Vasily.Standard
{
    public interface IInsert
    {
        /// <summary>
        /// 根据model信息生成 INSERT INTO [TableName] ([member1],[member2]...) VALUES (@member1,@member2...)
        /// </summary>
        /// <param name="model">载有生成信息的Model</param>
        /// <returns>插入字符串的结果</returns>
        string Insert(MakerModel model);
    }
}
