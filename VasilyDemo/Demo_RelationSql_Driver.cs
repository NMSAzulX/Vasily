using System;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    class Demo_RelationSql_Driver
    {
        public static void Start()
        {
            One one = new One();
            Two two = new Two();
            Three three = new Three();
            Two_Parent parent = new Two_Parent();

            //以实体类进行操作
            DapperWrapper<Three, Two, One, Two_Parent> dapper1 = new DapperWrapper<Three, Two, One, Two_Parent>("key");

            dapper1.SourceGet(one, two);
            dapper1.SourceGets(one, two);
            dapper1.SourceInsert(three, one, two);
            dapper1.SourcePreDelete(three);
            dapper1.SourceAftDelete(one, two);
            dapper1.SourceUpdate(three, one, two);

            dapper1.TableGet(one.oid, two.rid);

            //以值进行操作

            DapperWrapper<Three, Two, One> dapper2 = new DapperWrapper<Three, Two, One>("key");
            dapper2.TableGet(one.oid);
            dapper2.TableGets(one.oid);
            dapper2.TableInsert(three.tid,one.oid);
            dapper2.TablePreDelete(three.tid);
            dapper2.TableAftDelete(one.oid);
            dapper2.TableUpdate(three.tid, one.oid);
           
        }
    }
}
