using System;
using System.Collections.Generic;
using System.Text;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    class Demo_Sql
    {
        public static void Start()
        {
            VasilyRunner.Run();

            Console.WriteLine(Sql<One>.Table);
            //table_one

            Console.WriteLine(Sql<One>.Primary);
            //oid

            Console.WriteLine(Sql<One>.SelectAll);
            //SELECT * FROM [table_one]

            Console.WriteLine(Sql<One>.SelectAllByPrimary);
            //SELECT * FROM [table_one] WHERE [oid] = @oid

            Console.WriteLine(Sql<One>.SelectAllIn);
            //SELECT * FROM [table_one] WHERE [oid] IN @keys

            Console.WriteLine(Sql<One>.SelectAllWhere);
            //SELECT * FROM [table_one] WHERE


            Console.WriteLine(Sql<One>.Select);
            //SELECT [oid],[name],[create_time],[update_time],[age],[student_id] FROM [table_one]

            Console.WriteLine(Sql<One>.SelectByPrimary);
            //SELECT [oid],[name],[create_time],[update_time],[age],[student_id] FROM [table_one] WHERE [oid]=@oid

            Console.WriteLine(Sql<One>.SelectIn);
            //SELECT [oid],[name],[create_time],[update_time],[age],[student_id] FROM [table_one] WHERE [oid] IN @keys

            Console.WriteLine(Sql<One>.SelectWhere);
            //SELECT [oid],[name],[create_time],[update_time],[age],[student_id] FROM [table_one] WHERE

            Console.WriteLine(Sql<One>.UpdateAllByPrimary);
            //UPDATE [table_one] SET [name]=@name,[create_time]=@create_time,[update_time]=@update_time,[age]=@age,[student_id]=@student_id WHERE [oid]=@oid

            Console.WriteLine(Sql<One>.UpdateAllWhere);
            //UPDATE [table_one] SET [name]=@name,[create_time]=@create_time,[update_time]=@update_time,[age]=@age,[student_id]=@student_id WHERE

            Console.WriteLine(Sql<One>.UpdateByPrimary);
            //UPDATE [table_one] SET [name]=@name,[update_time]=@update_time,[age]=@age,[student_id]=@student_id WHERE [oid]=@oid

            Console.WriteLine(Sql<One>.UpdateWhere);
            //UPDATE [table_one] SET [name]=@name,[update_time]=@update_time,[age]=@age,[student_id]=@student_id WHERE

            Console.WriteLine(Sql<One>.DeleteByPrimary);
            //DELETE FROM [table_one] WHERE [oid] = @oid

            Console.WriteLine(Sql<One>.DeleteWhere);
            //DELETE FROM [table_one] WHERE


            Console.WriteLine(Sql<One>.Insert);
            //INSERT INTO [table_one] ([name],[create_time],[update_time],[age],[student_id])VALUES(@name, @create_time, @update_time, @age, @student_id)

            Console.WriteLine(Sql<One>.InsertAll);
            //INSERT INTO [table_one] ([name],[create_time],[update_time],[age],[student_id])VALUES(@name, @create_time, @update_time, @age, @student_id)

            Console.WriteLine(Sql<One>.RepeateCount);
            //SELECT COUNT(*) FROM [table_one] WHERE [student_id] = @student_id

            Console.WriteLine(Sql<One>.RepeateEntities);
            //SELECT * FROM [table_one] WHERE [student_id] = @student_id
            Console.WriteLine(Sql<One>.RepeateId);
            //SELECT [oid] FROM [table_one] WHERE [student_id] = @student_id

            Console.WriteLine(Sql<One>.SelectCount);
            //SELECT Count(*) FROM [table_one]
            Console.WriteLine(Sql<One>.SelectCountWhere);
            //SELECT  Count(*) FROM [table_one] WHERE


            //操作对应的是DapperWrapper<One>
            One one = new One();
            DapperWrapper<One> dapper = new DapperWrapper<One>("key");
            dapper.IsRepeat(one);
        }

    }
}
