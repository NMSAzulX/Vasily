using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    class Demo_Sql
    {
        public static void Start()
        {
            VasilyRunner.Run();

            Console.WriteLine(SqlEntity<One>.Table);
            //table_one

            Console.WriteLine(SqlEntity<One>.Primary);
            //oid

            Console.WriteLine(SqlEntity<One>.SelectAll);
            //SELECT * FROM [table_one]

            Console.WriteLine(SqlEntity<One>.SelectAllByPrimary);
            //SELECT * FROM [table_one] WHERE [oid] = @oid

            Console.WriteLine(SqlEntity<One>.SelectAllIn);
            //SELECT * FROM [table_one] WHERE [oid] IN @keys

            Console.WriteLine(SqlEntity<One>.SelectAllWhere);
            //SELECT * FROM [table_one] WHERE


            Console.WriteLine(SqlEntity<One>.UpdateAllByPrimary);
            //UPDATE [table_one] SET [name]=@name,[create_time]=@create_time,[update_time]=@update_time,[age]=@age,[student_id]=@student_id WHERE [oid]=@oid

            Console.WriteLine(SqlEntity<One>.UpdateAllWhere);
            //UPDATE [table_one] SET [name]=@name,[create_time]=@create_time,[update_time]=@update_time,[age]=@age,[student_id]=@student_id WHERE


            Console.WriteLine(SqlEntity<One>.DeleteByPrimary);
            //DELETE FROM [table_one] WHERE [oid] = @oid

            Console.WriteLine(SqlEntity<One>.DeleteWhere);
            //DELETE FROM [table_one] WHERE


            Console.WriteLine(SqlEntity<One>.InsertAll);
            //INSERT INTO [table_one] ([name],[create_time],[update_time],[age],[student_id])VALUES(@name, @create_time, @update_time, @age, @student_id)

            Console.WriteLine(SqlEntity<One>.RepeateCount);
            //SELECT COUNT(*) FROM [table_one] WHERE [student_id] = @student_id

            Console.WriteLine(SqlEntity<One>.RepeateEntities);
            //SELECT * FROM [table_one] WHERE [student_id] = @student_id
            Console.WriteLine(SqlEntity<One>.RepeateId);
            //SELECT [oid] FROM [table_one] WHERE [student_id] = @student_id

            Console.WriteLine(SqlEntity<One>.SelectCount);
            //SELECT Count(*) FROM [table_one]
            Console.WriteLine(SqlEntity<One>.SelectCountWhere);
            //SELECT  Count(*) FROM [table_one] WHERE


            //操作对应的是DapperWrapper<One>
            One one = new One();
            DapperWrapper<One> dapper = new DapperWrapper<One>("key");
            dapper.IsRepeat(one);
        }

    }
}
