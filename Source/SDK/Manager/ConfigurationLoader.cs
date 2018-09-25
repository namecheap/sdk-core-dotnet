using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using PayPal.Exception;

namespace PayPal.Manager
{
    internal class ConfigurationLoader
    {
        private static readonly string[] AccountPropertyNames =
        {
            AccountFieldNames.ApiUsername, AccountFieldNames.ApiPassword, AccountFieldNames.ApplicationId,
            AccountFieldNames.ApiCertificate, AccountFieldNames.ApiSignature, AccountFieldNames.PrivateKeyPassword,
            AccountFieldNames.CertificateSubject, AccountFieldNames.SignatureSubject
        };

        public Dictionary<string, string> LoadFromJsonFile(string fileName, bool throwExceptionIfNotLoaded)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(fileName);

                var configuration = builder.Build();

                return LoadFromConfigurationRoot(configuration);
            }
            catch (System.Exception ex)
            {
                if (throwExceptionIfNotLoaded)
                {
                    throw new ConfigException($"Unable to load 'paypal' section from {fileName} file: " + ex.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// Loads settings Configuration root
        /// </summary>
        /// <param name="configurationRoot">Loaded Configuraton root</param>
        public Dictionary<string, string> LoadFromConfigurationRoot(IConfigurationRoot configurationRoot)
        {
            var paypalConfigSection = configurationRoot.GetSection("paypal");

            if (paypalConfigSection == null)
            {
                throw new ConfigException($"Cannot load configuration section. Ensure you have configured the 'paypal' section correctly.");
            }

            var configValues = paypalConfigSection.GetSection("settings").GetChildren().ToDictionary(s => s.Key, s => s.Value);
            var accountsCount = paypalConfigSection.GetSection("accounts").GetChildren().Count();

            for (int index = 0; index < accountsCount; index++)
            {
                foreach (var accountPropertyName in AccountPropertyNames)
                {
                    var value = paypalConfigSection[$"accounts:{index}:{accountPropertyName}"];

                    if (!string.IsNullOrEmpty(value))
                    {
                        configValues.Add($"account{index}.{accountPropertyName}", value);
                    }
                }
            }

            return configValues;
        }
    }
}
