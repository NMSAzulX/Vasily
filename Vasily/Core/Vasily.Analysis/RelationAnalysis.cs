using System;
using System.Collections.Generic;
using System.Text;

namespace Vasily.Core
{
    public class RelationAnalysis
    {
        public RelationAnalysis(Type type, string splite = null)
        {
            var model = SqlRelationMaker.Create(type, splite);
            GsOperator gs = new GsOperator(type);
            gs["Table"] = model.RelationModel.TableName;
            gs["Primary"] = model.RelationModel.PrimaryKey;
            gs["SourceConditions"]= model.Sources;
            gs["TableConditions"] = model.Tables;
         


            MemberGetter[] getters = new MemberGetter[model.Sources.Length];
            for (int j = 0; j < model.Sources.Length; j += 1)
            {
                var instance = model.AttrMapping[j].Instance;
                getters[j] = MebOperator.Getter(instance.RelationType, instance.ColumnName);
            }
            gs["Getters"] = getters;


            RelationSelectTemplate select = new RelationSelectTemplate();
            //public static string CountFromTable
            gs["CountFromTable"]=select.SelectCountTable(model);
            //public static string GetFromTable;
            gs["GetFromTable"]= select.SelectEntitesTable(model);

            //public static string CountFromSource
            gs["CountFromSource"] = select.SelectCountSource(model);
            //public static string GetFromSource;
            gs["GetFromSource"] = select.SelectEntitesSource(model);

            RelationUpdateTemplate update = new RelationUpdateTemplate();
            //public static string ModifyFromTable;
            gs["ModifyFromTable"]= update.UpdateTable(model);
            gs["ModifyFromSource"] = update.UpdateSource(model);
            
            RelationInsertTemplate insert = new RelationInsertTemplate();
            //public static string AddFromTable;
            gs["AddFromTable"]= insert.InsertTable(model);
            gs["AddFromSource"] = insert.InsertSource(model);



            RelationDeleteTemplate delete = new RelationDeleteTemplate();
            //public static string DeleteFromTable;
            gs["DeletePreFromTable"] = delete.DeletePreTable(model);
            gs["DeleteAftFromTable"] = delete.DeleteAftTable(model);

            //public static string DeleteFromSource;
            gs["DeletePreFromSource"]= delete.DeletePreSource(model);
            gs["DeleteAftFromSource"]= delete.DeleteAftSource(model);

        }
    }
}
