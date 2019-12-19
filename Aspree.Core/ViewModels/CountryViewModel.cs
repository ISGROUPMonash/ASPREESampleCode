using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    /// <summary>
    /// Country Model
    /// </summary>
    public class CountryViewModel
    {
        /// <summary>
        /// Id of a country
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Name of the country
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Abbreviation of the country name
        /// </summary>
        public string Abbr { get; set; }
        /// <summary>
        /// Guid of a country
        /// </summary>
        public Guid Guid { get; set; }
    }

    /// <summary>
    /// New Country Model
    /// </summary>
    public class NewCountryViewModel
    {
        /// <summary>
        /// Name of the Country
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Abbreviation of the country name
        /// </summary>
        [Required]
        public string Abbr { get; set; }
    }

    /// <summary>
    /// Edit Country Model
    /// </summary>
    public class EditCountryViewModel
    {
        //[Required]
        //public Guid Guid { get; set; }

        /// <summary>
        /// Name of the country
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Abbreviation of the country name
        /// </summary>
        [Required]
        public string Abbr { get; set; }
    }

    public class GetAllCountryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<CountryViewModel>()
            {
                new CountryViewModel
                {
                    Id = 1,
                    Name = "Example Name",
                    Abbr = "Example IND",
                    Guid = Guid.NewGuid()

                }
            };
        }
    }
    public class CountryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
           return new CountryViewModel
            {
                Id = 1,
                Name = "Example Name",
                Abbr = "Example IND",
                Guid = Guid.NewGuid()

            };
           
        }
    }


    public class NewCountryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewCountryViewModel()
            {
                Name = "Example Name",
                Abbr = "Example IND",
            };
        }
    }

    public class EditCountryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditCountryViewModel()
            {
                Name = "Example Name",
                Abbr = "Example IND",
            };
        }
    }

}
