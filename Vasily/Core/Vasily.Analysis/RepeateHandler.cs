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
            _model.ColFunction = (item) => { return Column(item); };
        }

        public string Repeate()
        {
            _model.LoadMembers(_handler.Members<NoRepeateAttribute>());
            return _template.Repeate(_model);
        }
    }
}
