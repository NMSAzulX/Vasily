using Vasily.Core;

namespace System
{

    public static class RelationSql<T, Relation, C1>
    {
        static RelationSql()
        {
            RelationTriggerHandler template = new RelationTriggerHandler(typeof(RelationSql<T, Relation, C1>));
            var filter = template._filter;
            var handler = template.Handler;
            Table = template._model.TableName;
            Primary = template._model.PrimaryKey;
            SourceConditions = template.Sources;
            TableConditions = template.Tables;
            Getters = template.Getters;

           

            CountFromTable = handler.SelectCountByCondition();
            GetFromTable = handler.SelectString();
            ModifyFromTable = handler.UpdateString();
            DeletePreFromTable = handler.DeletePreString();
            DeleteAftFromTable = handler.DeleteAftString();
            AddFromTable = handler.InsertString();

            CountFromSource = handler.SelectCountByCondition(filter);
            GetFromSource = handler.SelectString(filter);
            ModifyFromSource = handler.UpdateString(filter);
            DeletePreFromSource = handler.DeletePreString(filter);
            DeleteAftFromSource = handler.DeleteAftString(filter);
            AddFromSource = handler.InsertString(filter);

        }

        public static MemberGetter[] Getters;

        public static string Table;
        public static string Primary;

        

        public static string[] SourceConditions;
        public static string[] TableConditions;

        public static string CountFromSource;
        public static string GetFromSource;
        public static string ModifyFromSource;
        public static string DeletePreFromSource;
        public static string DeleteAftFromSource;
        public static string AddFromSource;

        public static string CountFromTable;
        public static string GetFromTable;
        public static string ModifyFromTable;
        public static string DeletePreFromTable;
        public static string DeleteAftFromTable;
        public static string AddFromTable;



    }
    public static class RelationSql<T, Relation, C1, C2>
    {
        static RelationSql()
        {
            RelationTriggerHandler template = new RelationTriggerHandler(typeof(RelationSql<T, Relation, C1,C2>));
            var filter = template._filter;
            var handler = template.Handler;
            Table = template._model.TableName;
            Primary = template._model.PrimaryKey;
            SourceConditions = template.Sources;
            TableConditions = template.Tables;
            Getters = template.Getters;



            CountFromTable = handler.SelectCountByCondition();
            GetFromTable = handler.SelectString();
            ModifyFromTable = handler.UpdateString();
            DeletePreFromTable = handler.DeletePreString();
            DeleteAftFromTable = handler.DeleteAftString();
            AddFromTable = handler.InsertString();

            CountFromSource = handler.SelectCountByCondition(filter);
            GetFromSource = handler.SelectString(filter);
            ModifyFromSource = handler.UpdateString(filter);
            DeletePreFromSource = handler.DeletePreString(filter);
            DeleteAftFromSource = handler.DeleteAftString(filter);
            AddFromSource = handler.InsertString(filter);
        }
        public static MemberGetter[] Getters;

        public static string Table;
        public static string Primary;

        public static string[] SourceConditions;
        public static string[] TableConditions;

        public static string CountFromSource;
        public static string GetFromSource;
        public static string ModifyFromSource;
        public static string DeletePreFromSource;
        public static string DeleteAftFromSource;
        public static string AddFromSource;

        public static string CountFromTable;
        public static string GetFromTable;
        public static string ModifyFromTable;
        public static string DeletePreFromTable;
        public static string DeleteAftFromTable;
        public static string AddFromTable;

    }
    public static class RelationSql<T, Relation, C1, C2, C3>
    {
        static RelationSql()
        {
            RelationTriggerHandler template = new RelationTriggerHandler(typeof(RelationSql<T, Relation, C1, C2, C3>));
            var filter = template._filter;
            var handler = template.Handler;
            Table = template._model.TableName;
            Primary = template._model.PrimaryKey;
            SourceConditions = template.Sources;
            TableConditions = template.Tables;
            Getters = template.Getters;



            CountFromTable = handler.SelectCountByCondition();
            GetFromTable = handler.SelectString();
            ModifyFromTable = handler.UpdateString();
            DeletePreFromTable = handler.DeletePreString();
            DeleteAftFromTable = handler.DeleteAftString();
            AddFromTable = handler.InsertString();

            CountFromSource = handler.SelectCountByCondition(filter);
            GetFromSource = handler.SelectString(filter);
            ModifyFromSource = handler.UpdateString(filter);
            DeletePreFromSource = handler.DeletePreString(filter);
            DeleteAftFromSource = handler.DeleteAftString(filter);
            AddFromSource = handler.InsertString(filter);
        }
        public static MemberGetter[] Getters;

        public static string Table;
        public static string Primary;

        public static string[] SourceConditions;
        public static string[] TableConditions;

        public static string CountFromSource;
        public static string GetFromSource;
        public static string ModifyFromSource;
        public static string DeletePreFromSource;
        public static string DeleteAftFromSource;
        public static string AddFromSource;

        public static string CountFromTable;
        public static string GetFromTable;
        public static string ModifyFromTable;
        public static string DeletePreFromTable;
        public static string DeleteAftFromTable;
        public static string AddFromTable;
    }
    public static class RelationSql<T, Relation, C1, C2, C3, C4>
    {
        static RelationSql()
        {
            RelationTriggerHandler template = new RelationTriggerHandler(typeof(RelationSql<T, Relation, C1, C2, C3, C4>));
            var filter = template._filter;
            var handler = template.Handler;
            Table = template._model.TableName;
            Primary = template._model.PrimaryKey;
            SourceConditions = template.Sources;
            TableConditions = template.Tables;
            Getters = template.Getters;



            CountFromTable = handler.SelectCountByCondition();
            GetFromTable = handler.SelectString();
            ModifyFromTable = handler.UpdateString();
            DeletePreFromTable = handler.DeletePreString();
            DeleteAftFromTable = handler.DeleteAftString();
            AddFromTable = handler.InsertString();

            CountFromSource = handler.SelectCountByCondition(filter);
            GetFromSource = handler.SelectString(filter);
            ModifyFromSource = handler.UpdateString(filter);
            DeletePreFromSource = handler.DeletePreString(filter);
            DeleteAftFromSource = handler.DeleteAftString(filter);
            AddFromSource = handler.InsertString(filter);
        }
        public static MemberGetter[] Getters;

        public static string Table;
        public static string Primary;

        public static string[] SourceConditions;
        public static string[] TableConditions;

        public static string CountFromSource;
        public static string GetFromSource;
        public static string ModifyFromSource;
        public static string DeletePreFromSource;
        public static string DeleteAftFromSource;
        public static string AddFromSource;

        public static string CountFromTable;
        public static string GetFromTable;
        public static string ModifyFromTable;
        public static string DeletePreFromTable;
        public static string DeleteAftFromTable;
        public static string AddFromTable;
    }
    public class RelationSql<T, Relation, C1, C2, C3, C4, C5>
    {
        static RelationSql()
        {
            RelationTriggerHandler template = new RelationTriggerHandler(typeof(RelationSql<T, Relation, C1, C2, C3, C4, C5>));
            var filter = template._filter;
            var handler = template.Handler;
            Table = template._model.TableName;
            Primary = template._model.PrimaryKey;
            SourceConditions = template.Sources;
            TableConditions = template.Tables;
            Getters = template.Getters;



            CountFromTable = handler.SelectCountByCondition();
            GetFromTable = handler.SelectString();
            ModifyFromTable = handler.UpdateString();
            DeletePreFromTable = handler.DeletePreString();
            DeleteAftFromTable = handler.DeleteAftString();
            AddFromTable = handler.InsertString();

            CountFromSource = handler.SelectCountByCondition(filter);
            GetFromSource = handler.SelectString(filter);
            ModifyFromSource = handler.UpdateString(filter);
            DeletePreFromSource = handler.DeletePreString(filter);
            DeleteAftFromSource = handler.DeleteAftString(filter);
            AddFromSource = handler.InsertString(filter);
        }
        public static MemberGetter[] Getters;

        public static string Table;
        public static string Primary;


        public static string[] SourceConditions;
        public static string[] TableConditions;

        public static string CountFromSource;
        public static string GetFromSource;
        public static string ModifyFromSource;
        public static string DeletePreFromSource;
        public static string DeleteAftFromSource;
        public static string AddFromSource;

        public static string CountFromTable;
        public static string GetFromTable;
        public static string ModifyFromTable;
        public static string DeletePreFromTable;
        public static string DeleteAftFromTable;
        public static string AddFromTable;
    }
    public class RelationSql<T, Relation, C1, C2, C3, C4, C5, C6>
    {
        static RelationSql()
        {
            RelationTriggerHandler template = new RelationTriggerHandler(typeof(RelationSql<T, Relation, C1, C2, C3, C4, C5, C6>));
            var filter = template._filter;
            var handler = template.Handler;
            Table = template._model.TableName;
            Primary = template._model.PrimaryKey;
            SourceConditions = template.Sources;
            TableConditions = template.Tables;
            Getters = template.Getters;



            CountFromTable = handler.SelectCountByCondition();
            GetFromTable = handler.SelectString();
            ModifyFromTable = handler.UpdateString();
            DeletePreFromTable = handler.DeletePreString();
            DeleteAftFromTable = handler.DeleteAftString();
            AddFromTable = handler.InsertString();

            CountFromSource = handler.SelectCountByCondition(filter);
            GetFromSource = handler.SelectString(filter);
            ModifyFromSource = handler.UpdateString(filter);
            DeletePreFromSource = handler.DeletePreString(filter);
            DeleteAftFromSource = handler.DeleteAftString(filter);
            AddFromSource = handler.InsertString(filter);
        }
        public static MemberGetter[] Getters;

        public static string Table;
        public static string Primary;

        public static string[] SourceConditions;
        public static string[] TableConditions;

        public static string CountFromSource;
        public static string GetFromSource;
        public static string ModifyFromSource;
        public static string DeletePreFromSource;
        public static string DeleteAftFromSource;
        public static string AddFromSource;

        public static string CountFromTable;
        public static string GetFromTable;
        public static string ModifyFromTable;
        public static string DeletePreFromTable;
        public static string DeleteAftFromTable;
        public static string AddFromTable;
    }
}
