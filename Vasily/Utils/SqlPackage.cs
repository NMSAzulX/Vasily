using System;
using Vasily.Core;
using Vasily.Utils;

namespace Vasily
{
    public class SqlPackage<T> : SqlPackage
    {
        public SqlPackage(string splites = null) : base(typeof(T), splites)
        {

        }
    }

    public class SqlPackage
    {
        private Type _entity_type;
        internal string _splites;
        private string _primary;
        //private bool IsRelation;
        private bool IsNormal;
        public SqlPackage(Type type, string splites = null)
        {
            _entity_type = type;
            AttrOperator attr = new AttrOperator(_entity_type);
            _splites = SqlSpliter.GetSpliter(attr);
            _primary = attr.Mapping<PrimaryKeyAttribute>().Member.Name;
            //IsRelation = type.GetInterface("IVasilyRelation") != null;
            IsNormal = type.GetInterface("IVasilyNormal") != null;
            Analysis();
        }
        
        public void Analysis()
        {
            //if (IsRelation)
            //{
               // RelationHandler relation = new RelationHandler(_splites, _entity_type);
            //}
            if (IsNormal)
            {
                GsOperator gs = new GsOperator(typeof(Sql<>), _entity_type);
                gs["SetPrimary"] = MebOperator.Setter(_entity_type, _primary);

                SelectHandler select = new SelectHandler(_splites, _entity_type);
                gs["Table"] = select._model.TableName;
                gs["Primary"] = _primary;
                gs["SelectCount"] = select.SelectCount();
                gs["SelectCountByCondition"] = select.SelectCountByCondition();
                gs["SelectAll"] = select.SelectAll();
                gs["SelectAllByCondition"] = select.SelectAllByCondition();
                gs["SelectAllByPrimary"] = select.SelectAllByPrimary();
                gs["SelectAllIn"] = select.SelectAllIn();
                gs["Select"] = select.Select();
                gs["SelectByCondition"] = select.SelectByCondition();
                gs["SelectByPrimary"] = select.SelectByPrimary();
                gs["SelectIn"] = select.SelectIn();


                UpdateHandler update = new UpdateHandler(_splites, _entity_type);
                gs["UpdateByCondition"] = update.UpdateByCondition();
                gs["UpdateByPrimary"] = update.UpdateByPrimary();
                gs["UpdateAllByCondition"] = update.UpdateAllByCondition();
                gs["UpdateAllByPrimary"] = update.UpdateAllByPrimary();


                InsertHandler insert = new InsertHandler(_splites, _entity_type);
                gs["InsertAll"] = insert.InsertAll();
                gs["Insert"] = insert.Insert();


                DeleteHandler delete = new DeleteHandler(_splites, _entity_type);
                gs["DeleteByCondition"] = delete.DeleteByCondition();
                gs["DeleteByPrimary"] = delete.DeleteByPrimary();

                RepeateHandler repeate = new RepeateHandler(_splites, _entity_type);
                gs["RepeateCount"] = repeate.RepeateCount();
                gs["RepeateId"] = repeate.RepeateId();
                gs["RepeateEntities"] = repeate.RepeateEntities();
            }
        }


    }
}
