## Vasily

------

详情请转到![wiki](https://github.com/NMSAzulX/Vasily/wiki)

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
     AcceptInstance:{ id:10000, name:"小明" },
     Script:"c>id & c==name ^c - id ^(3,10)"
}
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
   
   - [x] 支持Link自定义操作
   
   - [x] 路由支持query-page-vp、query-vp路由自定义查询。
   
   - [x] 路由支持all-modify、all-add、all-delete整个实体类操作。
   
   - [x] 路由支持accurate-get、accurate-gets、accurate-page-gets、accurate-modify、accurate-add、accurate-delete、accurate-repeate路由精确操作。
   

   - [ ]  考虑未来是否只支持.NETStandard2.1 

   - [ ]  添加常用运行时脚本解析，重构底层映射，替换dapper.



- ### 注意事项

   - 由于dapper的API封装不够灵活，为保证性能以及减少实体类的侵入性，分页语句没有进行Ad-hoc优化，即参数化查询，因此需要手动控制计划缓存的数据库版本，还需要用户注意配置优化选项。

   - 由于dapper的API封装不够灵活，部分参数化操作采用DynamicParameters进行顺序添加，如关系操作的TableAPI系列。



~~~

~~~
