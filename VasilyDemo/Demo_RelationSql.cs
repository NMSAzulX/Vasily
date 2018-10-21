using System;
using System.Collections.Generic;
using System.Text;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    class Demo_RelationSql
    {
        public static void Start()
        {
            VasilyRunner.Run();

            Console.WriteLine(RelationSql<Two, Two, One, Two_Parent>.Table);
            //table_relation

            Console.WriteLine(RelationSql<Two, Two, One, Two_Parent>.Primary);
            //rid


            Console.WriteLine(RelationSql<Two,Two,One,Two_Parent>.AddFromSource);
            //INSERT INTO [table_relation] ([rid],[one_id],[parent_id])VALUES(@rid, @oid, @rid)

            Console.WriteLine(RelationSql<Two, Two, One, Two_Parent>.AddFromTable);
            //INSERT INTO [table_relation] ([rid],[one_id],[parent_id])VALUES(@rid, @one_id, @parent_id)

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.DeleteAftFromSource);
            //DELETE FROM [table_relation] WHERE [one_id] = @oid AND [parent_id] = @rid

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.DeleteAftFromTable);
            //DELETE FROM [table_relation] WHERE [one_id] = @one_id AND [parent_id] = @parent_id

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.DeletePreFromSource);
            //DELETE FROM [table_relation] WHERE [three_id] = @tid

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.DeletePreFromTable);
            //DELETE FROM [table_relation] WHERE [three_id] = @three_id


            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.GetFromSource);
            //SELECT [three_id] FROM [table_relation] WHERE [one_id] = @oid AND [parent_id] = @rid

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.GetFromTable);
            //SELECT [three_id] FROM [table_relation] WHERE [one_id] = @one_id AND [parent_id] = @parent_id

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.ModifyFromSource);
            //UPDATE [table_relation] SET [three_id] = @tid WHERE [one_id] = @oid AND [parent_id] = @rid

            Console.WriteLine(RelationSql<Three, Two, One, Two_Parent>.ModifyFromTable);
            //UPDATE [table_relation] SET [three_id] = @three_id WHERE [one_id] = @one_id AND [parent_id] = @parent_id


        }
    }
}
