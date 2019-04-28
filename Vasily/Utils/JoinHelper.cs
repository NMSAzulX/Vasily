using System;
using System.Collections.Generic;
using System.Text;
using Vasily.Core;

namespace Vasily.Utils
{
    public class JoinHelper
    {
        public static string GetA(SqlModel model)
        {
            return  $"{model.Left}V_{model.TableName}_TA{model.Right}";
        }
        public static string GetA(SqlRelationModel model)
        {
            return $"{model.Left}V_{model.TableName}_TA{model.Right}";
        }
        
        public static string GetB(SqlModel model)
        {
            return $"{model.Left}V_{model.TableName}_TB{model.Right}";
        }
        public static string GetB(SqlRelationModel model)
        {
            return $"{model.Left}V_{model.TableName}_TB{model.Right}";
        }
    }
}
