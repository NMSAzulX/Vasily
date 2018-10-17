using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vasily.Core
{
    public abstract class HandlerBase
    {
        internal bool _primary_manually;
        internal Type _entity_type;
        internal AttrOperator _handler;
        internal MakerModel _model;
        internal MemberInfo _primary_member;
        public HandlerBase(string spiltes,Type entity_type)
        {
            _model = new MakerModel();
            if (spiltes == null)
            {
                _model.Left = default(char);
                _model.Right = default(char);
            }
            else
            {
                _model.Left = spiltes[0];
                _model.Right = spiltes[1];
            }
           

            if (entity_type==null)
            {
                return;
            }

            _model.PrimaryKey = null;
            _entity_type = entity_type;

            //创建标签Helper
            _handler = new AttrOperator(_entity_type);

            //表名解析
            var table = _handler.ClassInstance<TableAttribute>();
            _model.TableName = table.Name;

            //主键解析
            var primary = _handler.Mapping<PrimaryKeyAttribute>();

            if (primary.Instance!=null)
            {
                _model.PrimaryKey = primary.Member.Name;
                _primary_manually = primary.Instance.IsManually;
                _primary_member = primary.Member;
            }

            //列名映射解析
            Dictionary<MemberInfo, string> _column_mapping = new Dictionary<MemberInfo, string>();
            var mappings = _handler.Mappings<ColumnAttribute>();
            foreach (var item in mappings)
            {
                _column_mapping[item.Member] = item.Instance.Name;
            }
            _model.ColumnMapping = _column_mapping;
            //填充属性
            _model.LoadMembers(_handler._members);

            //忽略掉Ignore标签的成员
            var ignores = _handler.Members<IgnoreAttribute>();
            _model.AddIgnores(ignores);

            //静态sql生成器。例如 MakerModel<Student>
            GsOperator gs = new GsOperator(typeof(MakerModel<>), _entity_type);
            gs.Set("PrimaryKey", _model.PrimaryKey);
            gs.Set("TableName", _model.TableName);
            gs.Set("Left", _model.Left);
            gs.Set("Right", _model.Right);
            gs.Set("Members", _model.Members);
            gs.Set("ColumnMapping", _column_mapping);

        }

        //~HandlerBase()
        //{
        //    Console.WriteLine("释放！");
        //}

    }

}
