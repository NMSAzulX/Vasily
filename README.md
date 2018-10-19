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

> 1. 引擎的入口VasilyRunner会扫描整个项目，挑选出实现IVasilyNormal接口或实现IVasilyRelation接口的类。

> 2. 将扫描结果切片并发交由SqlPackage处理。SqlPackage是引擎外壳，其核心部分是由不同的Handler组成的，包括SelectHandler,UpdateHandler,InsertHandler,RepeateHandler,DeleteHandler,RelationHandler.

> 3. SqlPackage通过区分接口来决定扫描结果交由哪个Handler类处理。

>&emsp;&emsp;     实现IVasilyNormal接口的实体类将被Select、Update、Insert、Delete、Repeate5个Handler处理。

>&emsp;&emsp;     实现IVasilyRelation接口的实体类将被RelationHandler单独处理。

> 4. Handler拿到实体类之后，将由两部分进行处理。即父类BaseHandler,以及SQL生成模板Template.

> 5. BaseHandler接过实体类之后将会拆解、过滤、重组成MakerModel,  同时进行静态化处理, 生成MakerModel&lt;TEntityType&gt;.

> 6. Template用上一步得到的MakerModel自动生成SQL语句。由于MakerModel和Template均可以在重用，因此并没有直接内聚在Handler中，而是解耦出来。至于RelationHandler是没有Template的，因为目前还没有看到重用的价值。  
 
  * MakerModel注释
  ```c#
    /// <summary>
    /// 当初始化MakerModel的时候，会产生静态泛型副本，以便后续直接用模板处理
    /// 其中Left,Right,TableName,PrimaryKey,Members会被复制
    /// Left,Right为SQL内置关键字分隔符
    /// TableName为表名
    /// PrimaryKey主键名
    /// Members成员信息
    /// ColFunction 为组建SQL语句时用到的过滤函数
    /// FilterFunction 为组建@参数时用到的过滤函数
    /// ColumnMapping 为Column的映射的缓存
    /// </summary>
 ```  
 
  ​


 * 流程与产出表

|    序号    |    流程    |    中间产出物    |    可用产出物    | 
| :----: | :-----------------------------: | :-----------: | :--------------------------: |
| 0 |	—— | —— |	Template |
| 1 |	VasilyRunner + IVasilyNorml/IVasilyRelation |	TEntityType |	—— |
| 2 |	TEntityType + BaseHandler | MakerModel |	MakerModel&lt;TEntityType&gt; |
| 3 |	MakerModel + Tempalte |	—— | Sql&lt;TEntityType&gt; / DapperWrapper&lt;TEntityType&gt; |
| 4 |	MakerModel + RelationHandler | ——	| RelationSql&lt;T,R,S....&gt; / DapperWrapper&lt;T,R,S....&gt; |


 
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
 

  >先看生成结果：  
  >> 1、select student_id from [table] where class_id=@class_id  
  >> 2、select student_id from [table] where class_id=@cid

>>第一种@class_id就是关系表本身的字段，对应的API操作为TableGets、TableUpdate等等
这类函数传参直接传值，如TableGets(1)

>>第二种@cid, 明显关系表中没有这个cid, 实际上它的来源只有一种就是Class这个类里面有个字段是cid，而且被标记成了[PrimaryKey],这种是隐式的操作.  
>>还有一种显式的操作：[Relation(typeof(Class),"cid")] 直接传入。
它们对应的API操作为SourceGet,SourceXXX等等
这类函数传参直接传对象，如SourceGets(myClassInstance); 
这里myClassInstane会通过emit缓存方法获取cid的值。 
   
  ​
   
 ------  
  ​

   
在RelationHandler中，该实体类被扫描处理成一个排列树(以两个元素为最低标准)，上面的类结果如下：  
 
 * A32 = 3!/(3-2)! = 6  
>A32 ： [Student,Class] 、 [Class,Student] 、[Student,TestRelation]、[TestRelation,Student]、[Class,TestRelation]、[TestRelation,Class]  

 * A33 = 3!/0!=6  
>A33 :   [Student,Class,TestRelation]、 [Class,Student,TestRelation] 、[Student,TestRelation,Class]、[TestRelation,Student,Class]、[Class,TestRelation,Student]、[TestRelation,Class,Student]  
    
    ​

  ------  
    
   ​

      

一共12种，为此Vasily将缓存有12种操作关系的静态类。  

> RelationSql<Student,TestRelation,Class> 代表属于TestRelation类中的[Student,Class]关系; 
>>业务上来讲，是通过class获取studnet。  

> RelationSql<Student,TestRelation,Class,TestRelation> 代表属于TestRelation类中的[Student,Class,TestRelation]三者之间的关系; 
>>业务上来讲，是通过Class和TestRelation来获取Student.  


总结:  

 * 第一个泛型代表了最终需要获取的对象;  
 
 * 第二个泛型代表了关系所在的类;  
 
 * 第三个泛型以后代表了条件;  
 
   ​

------  
  ​  
  
  
下面我们看一下以上实体的处理结果：  
```
RelationSql<Student,TestRelation,Class>.GetFromTable  = SELECT `student_id` FROM `relation_table` WHERE `class_id`=@class_id
RelationSql<Student,TestRelation,Class>.GetFromSource = SELECT `student_id` FROM `relation_table` WHERE `class_id`=@cid
```  

```
RelationSql<Student,TestRelation,Class>.ModifyFromTable  = UPDATE `relation_table` SET `student_id`=@student_id WHERE `class_id`=@class_id
RelationSql<Student,TestRelation,Class>ModifyFromSource   = UPDATE `relation_table` SET `student_id`=@sid WHERE`class_id`=@cid
```  


```
RelationSql<Student, Relation, Class>.AddFromTable = INSERT INTO `relation_table` (`student_id`,`class_id`)VALUES(@student_id,@class_id)
RelationSql<Student, Relation, Class>.AddFromSource = INSERT INTO `relation_table` (`student_id`,`class_id`)VALUES(@sid,@cid)
```  

```
//前置删除
RelationSql<Student, Relation, Class>.DeletePreFromTable = "DELETE FROM `relation_table` WHERE `StudentId`=@StudentId"
//后置删除
RelationSql<Student, Relation, Class, Relation>.DeleteAftFromSource = "DELETE FROM `relation_table` WHERE`class_id`=@cid AND `id`=@id
```  

其他更多的例子可以看看UT测试的代码  

  ​  
  
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

从结果`RelationSql<TestRelation,TestRelation,TestRelation_Luzishen>.GetFromSource= SELECT rid FROM relation_table WHERE parent_id=@rid`
可以看到parent_id=@rid，父id与本类的主键建立起了关系。

当然了，如果我们想有个健康的爸爸也可以这样写：
```c#
public class TestRelation_万寿无疆{
	[PrimaryKey]
	public int rid;
}

[Relation(typeof(TestRelation_万寿无疆))]
public int parent_id{get;set;}
``` 

```c#
var children = DapperWrapper<TestRelation,TestRelation,TestRelation_万寿无疆>.UseKey("sqlkey").SourceGets(father);

``` 



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
