using System;
using Vasily.Standard;

namespace Vasily.Core
{
    public class UpdateHandler : HandlerBase
    {
        private IUpdate _template;
        public UpdateHandler(string splites, Type entity_type) : base(splites, entity_type)
        {
            _template = new UpdateTemplate();
            _model.ColFunction = (item) => { return _model.Column(item); };
            _model.AddIgnores(_primary_member);
        }

        public string UpdateByCondition()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<UpdateIgnoreAttribute>());
            return _template.UpdateByCondition(model);
        }

        public string UpdateByPrimary()
        {
            var model = _model.CopyInstance();
            model.AddIgnores(_handler.Members<UpdateIgnoreAttribute>());
            return _template.UpdateByPrimary(model);
        }

        public string UpdateAllByCondition()
        {
            return _template.UpdateByCondition(_model);
        }

        public string UpdateAllByPrimary()
        {
            return _template.UpdateByPrimary(_model);
        }
    }
}
