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
    public class CityViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        /// <summary>
        /// Name of city
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Guid of state
        /// </summary>
        public Guid StatedId { get; set; }

        /// <summary>
        /// Abbreviation string
        /// </summary>
        public string Abbr { get; set; }

        /// <summary>
        /// Guid of city
        /// </summary>
        public Guid Guid { get; set; }
    }

    public class NewCityViewModel
    {
        /// <summary>
        /// Name of city
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Guid of state
        /// </summary>
        [Required]
        public Guid StateId { get; set; }

        /// <summary>
        /// Abbreviation string
        /// </summary>
        [Required]
        public string Abbr { get; set; }
    }

    public class EditCityViewModel
    {
        //[Required]
        //public Guid Guid { get; set; }
        /// <summary>
        /// Name of city
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Guid of state
        /// </summary>
        [Required]
        public Guid StateId { get; set; }

        /// <summary>
        /// Abbreviation string
        /// </summary>
        [Required]
        public string Abbr { get; set; }
    }

    public class GetAllCityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<CityViewModel>()

            {
                new CityViewModel
                {
                Id = 1,
                Guid = Guid.NewGuid(),
                Name = "Name example",
                StatedId = Guid.NewGuid(),
                Abbr = "Abbreviation example",

                }
               
            };
        }
    }
    public class CityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new CityViewModel
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                Name = "Name example",
                StatedId = Guid.NewGuid(),
                Abbr = "Abbreviation example",
            };
        }
    }

    public class NewCityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewCityViewModel
            {
                Name = "City example",
                StateId = Guid.NewGuid(),
                Abbr = "Abbreviation example",
            };
        }
    }

    public class EditCityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditCityViewModel
            {
                Name = "City example",
                StateId = Guid.NewGuid(),
                Abbr = "Abbreviation example",
            };
        }
    }
}
