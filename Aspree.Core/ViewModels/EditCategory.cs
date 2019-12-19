using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class EditCategory
    {
        //[Required]
        //public Guid Guid { get; set; }
        /// <summary>
        /// Category string
        /// </summary>
        [Required]
        public string Category { get; set; }
    }

    public class EditCategoryExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditCategory
            {
                Category = "Category example",
            };
        }
    }
}
