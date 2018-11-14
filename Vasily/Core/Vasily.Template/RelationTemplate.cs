using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vasily.Standard;

namespace Vasily.Core
{
    public class RelationTemplate
    {
        private ISelect _select_template;
        private IUpdate _update_template;
        private IDelete _delete_template;
        private IInsert _insert_template;
        private MemberInfo[] _parameters;
        private MakerModel _model;
        public RelationTemplate(MakerModel model, MemberInfo[] parameters)
        {
            _select_template = new SelectTemplate();
            _update_template = new UpdateTemplate();
            _delete_template = new DeleteTemplate();
            _insert_template = new InsertTemplate();
            _model = model;
            _parameters = parameters;
        }

        /// <summary>
        /// 获取SELECT [member1] FROM [TableName] WHERE [member2]=@member2 AND = [member3]=@member3
        /// </summary>
        /// <param name="_parameters">成员集合</param>
        /// <returns>返回条件查询SQL</returns>
        public string SelectString(Func<MemberInfo, string> filter = null)
        {
            var temp_model = _model.CopyInstance();
            temp_model.FilterFunction = filter;
            MemberInfo[] temp = new MemberInfo[_parameters.Length - 1];
            Array.Copy(_parameters, 1, temp, 0, _parameters.Length - 1);
            temp_model.LoadMembers(_parameters[0]);
            return _select_template.SelectWithCondition(temp_model, temp);
        }

        /// <summary>
        /// 获取SELECT Count(*) FROM [TableName] WHERE [member2]=@member2 AND = [member3]=@member3
        /// </summary>
        /// <returns>返回条件查询SQL</returns>
        public string SelectCountByCondition(Func<MemberInfo, string> filter = null)
        {
            var temp_model = _model.CopyInstance();
            temp_model.FilterFunction = filter;
            MemberInfo[] temp = new MemberInfo[_parameters.Length - 1];
            Array.Copy(_parameters, 1, temp, 0, _parameters.Length - 1);
            return _select_template.SelectCountWithCondition(temp_model, temp);
        }


        /// <summary>
        /// 获取更新语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string UpdateString(Func<MemberInfo, string> filter = null)
        {
            var temp_model = _model.CopyInstance();
            temp_model.FilterFunction = filter;
            MemberInfo[] temp = new MemberInfo[_parameters.Length - 1];
            Array.Copy(_parameters, 1, temp, 0, _parameters.Length - 1);
            temp_model.LoadMembers(_parameters[0]);
            return _update_template.UpdateWithCondition(temp_model, temp);
        }

        /// <summary>
        /// 获取删除语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string DeletePreString(Func<MemberInfo, string> filter = null)
        {
            if (filter != null)
            {
                var temp_model = _model.CopyInstance();
                temp_model.FilterFunction = filter;
                return _delete_template.DeleteWithCondition(temp_model, _parameters[0]);
            }
            return _delete_template.DeleteWithCondition(_model, _parameters[0]);
        }
        /// <summary>
        /// 获取删除语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string DeleteAftString(Func<MemberInfo, string> filter = null)
        {

            MemberInfo[] temp = new MemberInfo[_parameters.Length - 1];
            Array.Copy(_parameters, 1, temp, 0, _parameters.Length - 1);

            if (filter != null)
            {
                var temp_model = _model.CopyInstance();
                temp_model.FilterFunction = filter;
                return _delete_template.DeleteWithCondition(temp_model, temp);
            }
            return _delete_template.DeleteWithCondition(_model, temp);
        }

        /// <summary>
        /// 获取插入语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string InsertString(Func<MemberInfo, string> filter = null)
        {
            var temp_model = _model.CopyInstance();
            temp_model.FilterFunction = filter;
            temp_model.LoadMembers(_parameters);
            return _insert_template.Insert(temp_model);

        }
    }
}
