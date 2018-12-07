using System;

namespace Vasily.Core
{
    public class NormalAnalysis<T> : NormalAnalysis
    {
        public NormalAnalysis(string splite = null) : base(typeof(T), splite) { }

    }

    public class NormalAnalysis
    {
        public NormalAnalysis(Type type,string splite=null)
        {
            var model = SqlMaker.Create(type);
            
            GsOperator gs = new GsOperator(typeof(Sql<>), type);
            gs["SetPrimary"] = MebOperator.Setter(type, model.PrimaryKey);
            gs["Table"] = model.TableName;
            gs["Primary"] = model.PrimaryKey;

            CountTemplate count = new CountTemplate();
            gs["SelectCount"] = count.SelectCount(model);
            gs["SelectCountWhere"] = count.SelectCountWhere(model);


            SelectTemplate select = new SelectTemplate();
            gs["SelectAll"] = select.SelectAll(model);
            gs["SelectAllWhere"] = select.SelectAllWhere(model);
            gs["SelectAllByPrimary"] = select.SelectAllByPrimary(model);
            gs["SelectAllIn"] = select.SelectAllIn(model);

            var selectModel = model.ModelWithoutAttr<SelectIgnoreAttribute>();
            gs["Select"] = select.Select(selectModel);
            gs["SelectWhere"] = select.SelectWhere(selectModel);
            gs["SelectByPrimary"] = select.SelectByPrimary(selectModel);
            gs["SelectIn"] = select.SelectIn(selectModel);


            UpdateTemplate update = new UpdateTemplate();
           
            gs["UpdateAllWhere"] = update.UpdateWhere(model);
            gs["UpdateAllByPrimary"] = update.UpdateByPrimary(model);

            var updateModel = model.ModelWithoutAttr<UpdateIgnoreAttribute>();
            gs["UpdateWhere"] = update.UpdateWhere(updateModel);
            gs["UpdateByPrimary"] = update.UpdateByPrimary(updateModel);


            InsertTemplate insert = new InsertTemplate();
            gs["InsertAll"] = insert.Insert(model);

            var insertModel = model.ModelWithoutAttr<InsertIgnoreAttribute>();
            gs["Insert"] = insert.Insert(insertModel);

            DeleteTemplate delete = new DeleteTemplate();
            gs["DeleteWhere"] = delete.DeleteWhere(model);
            gs["DeleteByPrimary"] = delete.DeleteByPrimary(model);

            RepeateTemplate repeate = new RepeateTemplate();
            var repeateModel = model.ModelWithAttr<NoRepeateAttribute>();
            gs["RepeateCount"] = repeate.RepeateCount(model);
            gs["RepeateId"] = repeate.RepeateId(model);
            gs["RepeateEntities"] = repeate.RepeateEntities(model);

        }
    }
}
