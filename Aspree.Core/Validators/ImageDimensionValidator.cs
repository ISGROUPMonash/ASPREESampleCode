using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Aspree.Core.Validators.ExtentionMethods;

namespace Aspree.Core.Validators
{
    public class ImageDimensionValidator
         : ValidationAttribute, IClientValidatable
    {
        private string _errorMessage = "Maximum image dimension is allowed - {0}X{1} Pixel";

        /// <summary>
        /// Maximum file size in MB
        /// </summary>
        public int MaximumHeight { get; private set; }
        public int MaximumWidth { get; private set; }

        /// <param name="maximumFileSize">Maximum file size in MB</param>
        public ImageDimensionValidator(
            int maximumHeight, int maximumWidth)
        {
            MaximumHeight = maximumHeight;
            MaximumWidth = maximumWidth;
        }

        public override bool IsValid(
            object value)
        {
            if (value == null)
            {
                return true;
            }

            if (!IsValidMaximumDimension((value as HttpPostedFileBase).InputStream))
            {
                ErrorMessage = String.Format(_errorMessage, MaximumWidth, MaximumHeight);
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(
            string name)
        {
            return String.Format(_errorMessage, MaximumWidth, MaximumHeight);
        }

        public System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules(
              ModelMetadata metadata
            , ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "imagedimension"
            };

            clientValidationRule.ValidationParameters.Add("width", MaximumWidth);
            clientValidationRule.ValidationParameters.Add("height", MaximumHeight);

            return new[] { clientValidationRule };
        }

        private bool IsValidMaximumDimension(
            System.IO.Stream fileStream)
        {
            var image = System.Drawing.Image.FromStream(fileStream);

            return !(image.Height > MaximumHeight || image.Width > MaximumWidth);
        }

        
    }

}