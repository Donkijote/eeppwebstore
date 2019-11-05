using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace WebStore.Validators
{
    public class MinimumFileSizeValidator
        : ValidationAttribute, IClientValidatable
    {        
        private string _errorMessageMB = "El archivo no puede pesar menos de {1} MG";
        private string _errorMessageKB = "El archivo no puede pesar menos de {1} KB";

        /// <summary>
        /// Minimum file size in MB
        /// </summary>
        public double MinimumFileSize { get; private set; }

        /// <param name="minimumFileSize">MinimumFileSize file size in MB</param>
        public MinimumFileSizeValidator(
           double minimumFileSize)
            : base()
        {
            MinimumFileSize = minimumFileSize;
        }

        public override bool IsValid(
             object value)
        {
            if (value == null)
            {
                return true;
            }

            if (!IsValidMinimumFileSize((value as HttpPostedFileBase).ContentLength))
            {
                if (MinimumFileSize < 1.0)
                {
                    double MinimumFileSizeKB = MinimumFileSize * 10;
                    ErrorMessage = String.Format(_errorMessageKB, "{0}", MinimumFileSizeKB);
                }
                else
                {
                    ErrorMessage = String.Format(_errorMessageMB, "{0}", MinimumFileSize);
                }
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(
            string name)
        {
            if (MinimumFileSize < 1.0)
            {
                double MinimumFileSizeKB = MinimumFileSize * 10;
                return String.Format(_errorMessageKB, name, MinimumFileSizeKB);
            }
            else
            {
                return String.Format(_errorMessageMB, name, MinimumFileSize);
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
              ModelMetadata metadata
            , ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage   = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "minimumfilesize"
            };

            clientValidationRule.ValidationParameters.Add("size", MinimumFileSize);

            return new[] { clientValidationRule };
        }

        private bool IsValidMinimumFileSize(
            int fileSize)
        {
            if(MinimumFileSize < 1.0)
            {
                return ConvertBytesToKilobytes(fileSize) >= MinimumFileSize;
            }
            else
            {
                return ConvertBytesToMegabytes(fileSize) >= MinimumFileSize;
            }
            
        }

        private double ConvertBytesToMegabytes(
            int bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        private double ConvertBytesToKilobytes(
            int bytes)
        {
            return bytes / 1024f;
        }
    }
}