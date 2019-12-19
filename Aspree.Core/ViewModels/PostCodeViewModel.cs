using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class PostCodeViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Postal Code
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Guid of Suburb
        /// </summary>
        public Guid SuburbId { get; set; }
        /// <summary>
        /// Guid of City
        /// </summary>
        public Guid CityId { get; set; }
        /// <summary>
        /// Guid of state
        /// </summary>
        public Guid StateId { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public Guid Guid { get; set; }
    }

    public class NewPostCodeViewModel
    {
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public Guid SuburbId { get; set; }
        [Required]
        public Guid CityId { get; set; }
        [Required]
        public Guid StateId { get; set; }
    }

    public class EditPostCodeViewModel
    {
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public Guid SuburbId { get; set; }
        [Required]
        public Guid CityId { get; set; }
        [Required]
        public Guid StateId { get; set; }
    }

    public class GetAllPostCodeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<PostCodeViewModel>()
            {
                new PostCodeViewModel
                {
                CityId = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                Id = 1,
                PostalCode = "Example postal code",
                StateId = Guid.NewGuid(),
                SuburbId = Guid.NewGuid(),
                }
                
            };
        }
    }
    public class PostCodeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PostCodeViewModel
            {
                CityId = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                Id = 1,
                PostalCode = "Example postal code",
                StateId = Guid.NewGuid(),
                SuburbId = Guid.NewGuid(),
            };
        }
    }

    public class NewPostCodeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewPostCodeViewModel
            {
                CityId = Guid.NewGuid(),
                PostalCode = "Example postal code",
                StateId = Guid.NewGuid(),
                SuburbId = Guid.NewGuid(),
            };
        }
    }

    public class EditPostCodeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditPostCodeViewModel
            {
                CityId = Guid.NewGuid(),
                PostalCode = "Example postal code",
                StateId = Guid.NewGuid(),
                SuburbId = Guid.NewGuid(),
            };
        }
    }
}
