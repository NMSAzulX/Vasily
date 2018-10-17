using System;
using Vasily.Standard;

namespace Vasily.Core
{
    public class RepeateHandler : HandlerBase
    {
        private IRepeate _template;
        public RepeateHandler(string splites, Type entity_type) : base(splites, entity_type)
        {
            _template = new RepeateTemplate();
            _model.ColFunction = (item) => { return _model.Column(item); };
            _model.LoadMembers(_handler.Members<NoRepeateAttribute>());
        }

        public string RepeateCount()
        {
            return _template.RepeateCount(_model);
        }

        public string RepeateId()
        {
            return _template.RepeateId(_model);
        }
        public string RepeateEntities()
        {
            return _template.RepeateEntities(_model);
        }
    }
}
