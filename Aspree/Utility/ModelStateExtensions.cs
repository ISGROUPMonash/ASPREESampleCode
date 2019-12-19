using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http.ModelBinding;
using System.Web.ModelBinding;

namespace Aspree.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static IEnumerable<object> AllErrors(this ModelStateDictionary modelState)
        {
            var result = new List<object>();
            var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any())
                                            .Select(x => new { x.Key, x.Value.Errors });

            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors
                                   .Select(error => new { Key = fieldKey.Split('.')[1] , Message = error.ErrorMessage });
                result.AddRange(fieldErrors);
            }

            return result;
        }
    }
}
