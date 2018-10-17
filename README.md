## Vasily

------

- ### 项目背景
  ​

  ​       由于本人工作原因，经常被小工具、小任务、小接口骚扰，因此想封装一个类库方便数据库方面的操作。在经过Mellivora项目过后，对Dapper项目有了一个大致的权衡，Dapper实体类映射的缓存方法的性能已经接近极限，有些地方考虑到不同数据库的实现以及兼容性，Dapper做出了平衡。因此采用Dapper作为底层操作库。

  ​


- ### 项目介绍
  ​

  ​	该项目主要是针对实体类进行解析，动态生成静态的SQL缓存，方便对Dapper的封装操作。
  
  ​


- ### 引擎结构
  ​
  ![VasilyEngine](https://github.com/NMSAzulX/Vasily/blob/master/Image/VasilyEngine.png)

  1. 引擎的入口VasilyRunner会扫描整个项目，挑选出实现IVasilyNormal接口或实现IVasilyRelation接口的类。

  2. 将扫描结果切片并发交由SqlPackage处理。SqlPackage是引擎外壳，其核心部分是由不同的Handler组成的，包括SelectHandler,UpdateHandler,InsertHandler,RepeateHandler,DeleteHandler,RelationHandler.

  3. SqlPackage通过区分接口来决定扫描结果交由哪个Handler类处理。

     实现IVasilyNormal接口的实体类将被Select、Update、Insert、Delete、Repeate5个Handler处理。

     实现IVasilyRelation接口的实体类将被RelationHandler单独处理。

  4. Handler拿到实体类之后，将由两部分进行处理。即父类BaseHandler,以及SQL生成模板Template.

  5. BaseHandler接过实体类之后将会拆解、过滤、重组成MakerModel,  同时进行静态化处理, 生成MakerModel<TEntityType>.

  6. Template用上一步得到的MakerModel自动生成SQL语句。由于MakerModel和Template均可以在重用，因此并没有直接集成在Handler中，而是解耦出来。至于RelationHandler是没有Template的，因为目前还没有看到重用的价值。

- ### 使用简介


  1. #### 实体类注解

     |    注解名    |      参数      |                           参数说明                           | 注解对象 |                           注解说明                           | 解析接口                       |
     | :----------: | :------------: | :----------------------------------------------------------: | :------: | :----------------------------------------------------------: | ------------------------------ |
     |    Table     | string,SqlType |           第一个参数为表名；第二个参数为数据库类型           |    类    |                      该实体类属于哪个表                      | IVasilyNormal                  |
     |   Primary    |       ——       |                              ——                              |   成员   |                       标识该成员为主键                       | IVasilyNormal                  |
     |    Colunm    |     string     |                    成员到数据库的列名映射                    |   成员   |   使用该注解的成员将用参数作为成员名参与SQL的自动生成过程    | IVasilyNormal                  |
     |  NoRepeate   |       ——       |                              ——                              |   成员   |            使用该注解将成员标记为查重所需要的成员            | IVasilyNormal                  |
     |   Relation   |  Type,string   | 第一个参数为当前列与哪个实体类相关联；第二个参数为所关联实体类的成员名 |   成员   |              使用该注解我们可以创建关系封装操作              | IVasilyRelation                |
     |    Ignore    |       ——       |                              ——                              |   成员   |         使用该注解表示该成员不会参与SQL自动生成过程          | IVasilyNormal，IVasilyRelation |
     | InsertIgnore |       ——       |                              ——                              |   成员   | 使用该注解的成员生成SQL之后，将不会出现在Sql<T>.Insert中,但会出现在Sql<T>.InsertAll中 | IVasilyNormal                  |
     | SelectIgnore |       ——       |                              ——                              |   成员   |   使用该注解的成员生成SQL之后，不会在带有‘All’的SQL语句中    | IVasilyNormal                  |
     | UpdateIgnore |       ——       |                              ——                              |   成员   |   使用该注解的成员生成SQL之后，不会在带有‘All’的SQL语句中    | IVasilyNormal                  |



 ```c#
 namespace VasilyWebDemo.Entity
 {
     [Table("tb_test",SqlType.MySql)]
     public class TestMember : IVasilyNormal
     {
         [PrimaryKey]
         public int tid { get; set; }
 
         [Column("cl.name")] 
         //数据库的列名程序中不一定能用，所以需要映射
         public string name { get; set; }
         
         public string description { get; set; }
     
         public string company{get;set;}
         
         [NoRepeate]
         public string workid{get;set;}
         
         public DateTime UpdateTime{get;set;}
         
         [UpdateIgnore]
         public DateTime CreateTime{get;set;}
         
         [Ignore]
         public string CompanyName{
             get{
 				return company+name;
             }
         }
     }
 }
 ```


 ```c#
 namespace VasilyWebDemo.Entity
 {
 	//假设有国家实体类并有主键country.id
 	[Table("tb_country",SqlType.MySql)]
     public class TestCountry:IVasilyNormal{
     	[Primary]
     	[Column("country.id")]
 		public int countryid;
     }
     //假设有公司实体类并有主键company_id
     [Table("tb_country",SqlType.MySql)]
     public class TestCompany:IVasilyNormal{
     	[Primary]
 		public int company_id;
     }
     
     //假设有公司实体类并有主键department_id,并有个最顶级部门的标识did
     [Table("tb_country",SqlType.MySql)]
     public class TestCompany:IVasilyNormal{
     	[Primary]
 		public int department_id;
 		public int did;
     }
     
     //假设数据库有如下关系成员tid、部门did、公司主键id，国家的主键id为关联成员则建实体类如下
     [Table("tb_member_mapping",SqlType.MySql)]
     public class TestRelation : IVasilyNormal,IVasilyRelation
     {
         [PrimaryKey]
         [Relation(typeof(TestRelation))]
         public int mccdid { get; set; }
 
         [Relation(typeof(TestMember),"tid"/null)]
         public int member_id { get; set; }
         
         [Relation(typeof(TestDepartment),"did")]
         public int department_id { get; set; }
         
     	 [Relation(typeof(TestCompany))]
         public int the_company_id{get;set;}
         
 		 [Relation(typeof(TestCountry),"country.id")]
         public int country_id{get;set;}
         
         public DateTime UpdateTime{get;set;}
         
         [UpdateIgnore]
         public DateTime CreateTime{get;set;}
         
     }
 }
 ```







  2. #### 初始化配置

     ```C#
      VasilyService service = new VasilyService();
      service.AddVasily((option) =>
      {
          option.SqlSplite = "  ";

      }).AddVasilyConnectionCache((option) =>
      {
          option.Key("Mysql").AddConnection<MySqlConnection>("Database=xxx;Data Source=192.168.1.225;User Id=sa;Password=sa;CharSet=utf8;port=3306;SslMode=None;");
          option.Key("Mssql").AddConnection<SqlConnection>("Data Source=192.168.1.125;Initial Catalog=xxx; uid = sa; pwd = sa; Max Pool Size = 512; ");

      }).AddVasilySqlCache((o) =>
      {
          /*o.Key("GetData").SelectConditionCache<Entity>(
              SqlCondition.EMPTY.EQU("year"),
              SqlCondition.AND.EQU("is_delete")
          );*/
          //select * from table where year=@year and is_delete=@is_delete

          o.Key("GetData").SelectConditionCache<Entity>(
              SqlCondition.EMPTY.EQU("year"),
              SqlCondition.AND.EQU("is_delete")
          );
      });
     ```

     ​

  3. #### Dapper封装-VasilyDapper<<EntityType>>

     - Add、Modify、Get、Delete、IsRepeat五中操作,并支持批量操作。

     - Execute、ExecuteScalar常规操作。

     - ExecuteCache、GetCache执行/查询缓存字符串操作。

       ```C#
       VasilyDapper<Entity> sqlHandler = new VasilyDapper<Entity>("KEY");

       Entity entity = new Entity();
       entity.Name="test";
       entity.Description = "just for fun";

       sqlHandler.Add(entity);
       sqlHandler.Delete(entity);
       ```


4. #### Http

- 提供基础Controller，封装了VasilyDapper

- 增加了ReturnResult返回结果，方便快速搭建WebApi.

- 支持IServiceCollection扩展方法。

  ```c#
  return Result(SqlHandler.Get());

  Result:
  {
    "msg": null,
    "data": [
      {
        "tid": 4,
        "name": "首页",
        "description": "aaaaaaaaaaaa"
      },
      {
        "tid": 5,
        "name": "首页5",
        "description": "哈哈哈哈"
      }
    ],
    "status": 0
  }
  ```

  ```c#
  if (ModelState.IsValid)
  {
      return Result(SqlHandler.Modify(value));
  }
  else
  {
       return Result();
  }

  Result:
  {
    "msg": "名字不能为空！",
    "data": null,
    "status": 2
  }
  ```

  ​



- ### 项目计划

   - [x] 将支持并发解析操作

   - [ ] 将跟随.NET Core2.1特性进行性能修改

   - [ ] 进一步封装Sql相关的操作

   - [ ] 支持增加之后自动获取主键ID操作

     ​

- ### 更新日志

   - 2018-02-26：正式发布1.0.0版本.
   - 2018-02-26：发布1.0.1版本，修改部分备注信息，增加单元测试，优化部分逻辑.
   - 2018-02-27：发布1.0.2版本，修改部分命名空间，修改Nuget标签信息，增加HttpDemo, 完善Github ReadMe文档.
   - 2018-03-24：支持并发操作，改EString为StringBuilder操作，从而支持Core2.1的性能提升.

~~~

~~~
