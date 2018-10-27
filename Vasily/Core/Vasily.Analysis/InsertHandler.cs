using System;
using Vasily.Standard;

namespace Vasily.Core
{
    public class InsertHandler : BaseHandler
    {
        private IInsert _template;
        public InsertHandler(string splites, Type entity_type) : base(splites, entity_type)
        {
            _template = new InsertTemplate();
            _model.ColFunction = (item) => { return _model.Column(item); };
            if (!_primary_manually)
            {
                _model.AddIgnores(_primary_member);
            }
        }

        public string Insert()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<InsertIgnoreAttribute>());
            return _template.Insert(model);
        }

        public string InsertAll()
        {
            return _template.Insert(_model);
        }
    }
}
