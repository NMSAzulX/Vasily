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
  ![VasilyEngine](https://github.com/NMSAzulX/Vasily/blob/master/Image/Runtime.png)

> 1. 引擎的入口VasilyRunner会扫描整个项目，挑选出实现IVasilyNormal接口的类。

> 2. 将扫描结果切片并发交由NormalAnalysis类处理。NormalAnalysis核心部分是由SqlMaker以及Template组成，包括SelectTemplate,UpdateTemplate,InsertTemplate,RepeateTemplate,DeleteTemplate,RelationTemplate,CountTemplate.

> 3. NormalAnalysis通过SqlMaker将实体类解析为SqlModel以便Template生成逻辑，SqlModel 和 Template 可以在运行时复用。

> 4. 关系操作采用静态泛型调用，例如SqlRelation<T,R,C1,C2>.. 第一次静态调用将触发RelationAnalysis,同NormalAnalysis一样，核心部分由一个Maker和各种Template构成，产物SqlRelationModel以及Template可复用.

> 5. SQL语句生成完毕后，将被DapperWrapper封装，与Dapper结合使用.

 
  ​


 * 流程与产出表

|    序号    |    流程    |    中间产出物    |    可用产出物    | 
| :----: | :-----------------------------: | :-----------: | :--------------------------: |
| 0 |	—— | —— |	Template |
| 1 |	VasilyRunner + IVasilyNorml |	TEntityType |	—— |
| 2 |	TEntityType + NormalAnalaysis | SqlModel |	SqlEntity&lt;TEntityType&gt; |
| 4 |	REntityType + RelationAnalaysis | SqlRelationModel	| SqlRelation&lt;T,REntityTypeR,S....&gt; / DapperWrapper&lt;T,REntityType,S....&gt; |


 
  ​


- ### 使用简介

  #### 实体类注解
 
  ​  
  
|    注解名    |      参数      |                           参数说明                           |   注解对象   |                           注解说明                           |           解析接口           |
| :---------: | :-----------: | :----------------------------------------------------------: | :----------: | :----------------------------------------------------------: | :-----------------------------: |
|    Table     | string,SqlType |           第一个参数为表名；第二个参数为数据库类型           |    类    |                      该实体类属于哪个表                      | IVasilyNormal                  |
|   Primary    |       ——       |                              ——                              |   成员   |                       标识该成员为主键                       | IVasilyNormal                  |
|    Colunm    |     string     |                    成员到数据库的列名映射                    |   成员   |   使用该注解的成员将用参数作为成员名参与SQL的自动生成过程    | IVasilyNormal                  |
|  NoRepeate   |       ——       |                              ——                              |   成员   |            使用该注解将成员标记为查重所需要的成员            | IVasilyNormal                  |
|   Relation   |  Type,string   | 第一个参数为当前列与哪个实体类相关联；第二个参数为所关联实体类的成员名 |   成员   |              使用该注解我们可以创建关系封装操作              | IVasilyRelation                |
|    Ignore    |       ——       |                              ——                              |   成员   |         使用该注解表示该成员不会参与SQL自动生成过程          | IVasilyNormal，IVasilyRelation |
| InsertIgnore |       ——       |                              ——                              |   成员   | 使用该注解的成员生成SQL之后，将不会出现在Sql&lt;T&gt;.Insert中,但会出现在Sql&lt;T&gt;.InsertAll中 | IVasilyNormal                  |
| SelectIgnore |       ——       |                              ——                              |   成员   |   使用该注解的成员生成SQL之后，不会在带有‘All’的SQL语句中    | IVasilyNormal                  |
| UpdateIgnore |       ——       |                              ——                              |   成员   |   使用该注解的成员生成SQL之后，不会在带有‘All’的SQL语句中    | IVasilyNormal                  |

----------

#### 关系实战
```c#
//创建实体类
[Table("relation_table")]
public class TestRelation:IVasilyRelation
{
	[PrimaryKey]
	[Relation(typeof(TestRelation))]
	public int rid{get;set;}

	[Relation(typeof(Student))]
	public int student_id{get;set;}

	[Relation(typeof(Class))]
	public int class_id{get;set;}
}
```  

注解RelationAttribute,两个参数:  

 * Parameter1: 是该外联字段所属的类;  
 
 * Parameter2: 参数是为了区分操作，Vasily提供了两种关系操作;  
 

  >先看其中一项的结果： 

  - 第一种

  ```sql
         select * from 


      stuTable  as  V_stuTable_TA         inner join 
relation_table  as  V_relation_table_TB   ON  


    V_stuTable_TA.[student_id]
                = 
    V_relation_table_TB.[student_id] 


               and 


    V_relation_table_TB.[class_id]
                =
           [@class_id]   <----跟下面不一样的地方
```  
  - 第二种

```sql
         select * from 


      stuTable  as  V_stuTable_TA         inner join 
relation_table  as  V_relation_table_TB   ON  


    V_stuTable_TA.[student_id]
                = 
    V_relation_table_TB.[student_id] 


               and 


    V_relation_table_TB.[class_id]
                =
              [@cid]   <----跟上面不一样的地方
```

第一种，后面查询条件为 @class_id，该字段属于表本身的字段，对应的封装dapper后的操作为操作为TableGets、TableUpdate等等

>用法：是直接传值，如TableGets(1)

第二种，后面查询条件为 @cid, 这个是[Relation(typeof(Class))]中，Class类里面的字段, 而且cid被标记成了[PrimaryKey], 这种属于隐式的操作.  还有一种显式的操作：[Relation(typeof(Class),"cid")] 直接传入。它们对应的API操作为SourceGet,SourceXXX等等

>用法：这类函数传参直接传对象，如SourceGets(myClassInstance); 

>>>这里myClassInstane会通过emit缓存方法获取cid的值。 
   
  ​
 ------  
  ​
 起初Vasily采用了排列树进行预热操作，考虑到排列书在关系复杂的时候占用的空间较多，现在已经改为触发式生成缓存，也就是只有当用的时候才会生成缓存。

> RelationSql<Student,TestRelation,Class> 代表属于TestRelation类中的[Student,Class]关系; 
>>业务上来讲，是通过class获取studnet。  

> RelationSql<Student,TestRelation,Class,TestRelation> 代表属于TestRelation类中的[Student,Class,TestRelation]三者之间的关系; 
>>业务上来讲，是通过Class和TestRelation来获取Student.  


总结:  

 * 第一个泛型代表了最终需要获取的对象;  
 
 * 第二个泛型代表了关系所在的类;  
 
 * 第三个泛型以后代表了条件;  
 
   ​
>>其他更多的例子可以看看UT测试的代码  

------  
  ​  

#### 关系拓展

 * 找儿子模型

```c#
[Table("relation_table")]
public class TestRelation:IVasilyRelation
{
	[PrimaryKey]
	public int rid{get;set;}

	[Relation(typeof(TestRelation))]
	public int parent_id{get;set;}
}
``` 

> 看这个实体，根据我们上述的实战来看，成关系必须至少是两个实体之间，而这个类里面仅仅有一个关系注解而且还是指向自身的。
> 从业务的角度上很容易看清楚这是个常见的撸自身设计，在前端很有可能是个树形展示，接下来我们使用relation扩展解析来解决这个关系操作。

```c#
//新建一个类
public class TestRelation_Luzishen{}
//原来的类改为：
[Table("relation_table")]
public class TestRelation:IVasilyRelation
{
	[PrimaryKey]
	[Relation(typeof(TestRelation))]
	public int rid{get;set;}

	[Relation(typeof(TestRelation_Luzishen),"rid")]
	public int parent_id{get;set;}
}
``` 

首先我们以TestRelation_为前缀创建一个类，当Vasily在解析`[Relation(typeof(TestRelation_Luzishen),"rid")]`的时候，会按照TestRelation类，rid字段生成EMIT映射操作，另外也让RelationSql<>的关系更加清晰。

当然了，也可以这样写：
```c#
public class TestRelation_AnyName{
	[PrimaryKey]
	public int rid;
}

[Relation(typeof(TestRelation_AnyName))]
public int parent_id{get;set;}
``` 

```c#
var children = DapperWrapper<TestRelation,TestRelation,TestRelation_AnyName>.UseKey("sqlkey").SourceGets(father);

``` 

 ### 语法封装以及脚本查询

 - #### SqlCondition 语法封装：

```c#

SqlCondition<TEntity> c = new SqlCondition<TEntity>();


//普通操作符
c>"id"  ==> id>@id 如果采用泛型操作 id可以根据Column注解进行数据库字段的映射
c!="id" ==> id<>@id


//与或操作符
c>"id" & (c!="id" | c<"id")  ==>  (id>@id AND (id!=@id OR id<@id))


//排序操作符
c +"id" - "age" ==> ORDER BY id ASC, age DESC


//分页操作符
c ^ (2,10) ==> 分页语句，兼容MySql，SqlServer2012以后，PgSql，SqlLite


//组合
c>"id" ^ c -"id" ^ (current_page, size)  ==> id>@id ORDER BY id DESC +分页查询


//Vasily可根据语法树解析字符串脚本进而生成SQL语句，如下：
"c>id ^ c-id ^(2,10)" => id>@id ORDER BY id DESC +分页查询
```


- #### VasilyProtocal(VP,瓦西里查询协议):

Vasiy 提供了一个基于语法和参数化的查询协议

为了方便操作，封装了3种隐式转换：
VasilyProtocal<Entity> vp = script;
VasilyProtocal<Entity> vp = SqlCondition<TEntity>
VasilyProtocal<Entity> vp = condition=>condition.....

为了方便前端进行交互，AJAX自定义查询传送格式定位如下

```c#
//可以隐式转换为vp,进而适配vasily进行查询
{
     Instance:{ id:10000, name:"小明" },
     Script:"c>id & c==name ^c - id ^(3,10)"
}

//VasilyController中增加了两个默认API：
//api/[controller]/query-page-vp
//api/[controoler]/query-vp
//参数传vp格式即可

```

  sql 已经进行了防注入检测，参数也采用参数化处理

- ### 项目计划

   - [x] 支持并发解析操作

   - [x] 使用standard兼容

   - [x] 支持触发式解析关系实体操作，触发生成静态关系SQL缓存
   
   - [x] 支持自动解析普通实体操作，生成静态无竞争常用SQL语句

   - [x] 支持唯一约束安全插入，并获取主键ID
   
   - [x] 支持手动控制主键是否参与到更新插入等操作
   
   - [x] 支持条件查询、分页查询、排序查询语法及脚本解析
   
   - [x] 支持HTTP，自动分页查询返回
   
   - [ ] 支持索引优化分页
   
   - [x] 支持语法树解析
   
   - [x] 支持前端SqlVP格式安全请求分页数据
   
   - [x] 支持数据库集合操作，包括Union,Intersect,Except,UnionAll
   
   - [x] 支持多模糊查询语法及脚本解析
   
   - [x] 支持雪花算法生成唯一ID

   - [ ]  考虑未来是否只支持.NETStandard2.1 

   - [ ]  添加常用运行时脚本解析，重构底层映射，替换dapper.



- ### 注意事项

   - 由于dapper的API封装不够灵活，为保证性能以及减少实体类的侵入性，分页语句没有进行Ad-hoc优化，即参数化查询，因此需要手动控制计划缓存的数据库版本，还需要用户注意配置优化选项。

   - 由于dapper的API封装不够灵活，部分参数化操作采用DynamicParameters进行顺序添加，如关系操作的TableAPI系列。


- ### 更新日志

   - ~~2018-02-26：正式发布1.0.0版本.~~
   - ~~2018-02-26：发布1.0.1版本，修改部分备注信息，增加单元测试，优化部分逻辑.~~
   - ~~2018-02-27：发布1.0.2版本，修改部分命名空间，修改Nuget标签信息，增加HttpDemo, 完善Github ReadMe文档.~~
   - ~~2018-03-24：支持并发操作，改EString为StringBuilder操作，从而支持Core2.1的性能提升.~~
   - 2018-10-19：重构Vasily,优化解析引擎，采用排列树支持关系操作，增强注解，解耦解析模板，优化操作体验.
   - 2018-10-25：支持语法运算生成SQL,支持脚本解析生成SQL.
   - 2018-10-26：增加Union支持查询、更新、删除操作.
   - 2018-11-02：修复Union逻辑封装，优化运行时运算符重载逻辑，增加模糊查询，运行时%符号重载，以及脚本解析.
   - 2018-11-21：增加Union、Intersect、Except、UnionAll集合查询.
   - 2018-12-07：重构关系操作，采用INNER JOIN, 优化引擎架构，提高复用性。
   - 2018-12-28：重构VP协议格式，使其更规范，消去对object的扩展。

~~~

~~~
