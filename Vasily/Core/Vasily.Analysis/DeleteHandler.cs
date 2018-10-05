using System;
using Vasily.Standard;

namespace Vasily.Core
{
    public class DeleteHandler : HandlerBase
    {
        private IDelete _template;
        public DeleteHandler(string splites, Type entity_type) : base(splites, entity_type)
        {
            _template = new DeleteTemplate();
        }
        public string DeleteByCondition()
        {
            return _template.DeleteByCondition(_model);
        }

        public string DeleteByPrimary()
        {
            return _template.DeleteByPrimary(_model);
        }
    }
}
