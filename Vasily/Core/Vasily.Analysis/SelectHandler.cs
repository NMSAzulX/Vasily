using System;
using Vasily.Standard;

namespace Vasily.Core
{
    public class SelectHandler : BaseHandler
    {
        private ISelect _template;
        public SelectHandler(string splites, Type entity_type) : base(splites, entity_type)
        {
            _template = new SelectTemplate();
        }
        public string SelectAll()
        {
            return _template.SelectAll(_model);
        }

        public string SelectAllByCondition()
        {
            return _template.SelectAllByCondition(_model);
        }

        public string SelectAllByPrimary()
        {
            return _template.SelectAllByPrimary(_model);
        }

        public string SelectAllIn()
        {
            return _template.SelectAllIn(_model);
        }


        public string Select()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<SelectIgnoreAttribute>());
            return _template.Select(model);
        }

        public string SelectByCondition()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<SelectIgnoreAttribute>());
            return _template.SelectByCondition(model);
        }

        public string SelectByPrimary()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<SelectIgnoreAttribute>());
            return _template.SelectByPrimary(model);
        }

        public string SelectIn()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<SelectIgnoreAttribute>());
            return _template.SelectIn(model);
        }
    }
}
