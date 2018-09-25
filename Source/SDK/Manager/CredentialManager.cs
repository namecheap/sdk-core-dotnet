using System;
using System.Collections.Generic;
using PayPal.Authentication;
using PayPal.Exception;
using PayPal.Log;

namespace PayPal.Manager
{
    /// <summary>
    /// Reads API credentials to be used with the application
    /// </summary>
    public sealed class CredentialManager
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static Logger logger = Logger.GetLogger(typeof(CredentialManager));
        
        private static string accountPrefix = "account";

#if NET_2_0 || NET_3_5
        /// <summary>
        /// Singleton instance of ConnectionManager
        /// </summary>
        private static readonly CredentialManager singletonInstance = new CredentialManager();

        /// <summary>
        /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        /// </summary>
        static CredentialManager() { }

        /// <summary>
        /// Gets the Singleton instance of ConnectionManager
        /// </summary>
        public static CredentialManager Instance
        {
            get
            {
                return singletonInstance;
            }
        }
#elif NET_4_0 || NET_4_5 || NET_4_5_1 || NETSTANDARD || NETSTANDARD2_0
        /// <summary>
        /// System.Lazy type guarantees thread-safe lazy-construction
        /// static holder for instance, need to use lambda to construct since constructor private
        /// </summary>
        private static readonly Lazy<CredentialManager> laze = new Lazy<CredentialManager>(() => new CredentialManager());

        /// <summary>
        /// Accessor for the Singleton instance of ConnectionManager
        /// </summary>
        public static CredentialManager Instance { get { return laze.Value; } }    
#endif

        /// <summary>
        /// Private constructor, private to prevent direct instantiation
        /// </summary>
        private CredentialManager() { }   

        /// <summary>
        /// Returns the default Account Name
        /// </summary>
        /// <returns></returns>
        private Account GetAccount(Dictionary<string, string> config, string apiUserName)
        {                        
            foreach (KeyValuePair<string, string> kvPair in config)
            {
                //logger.Info(kvPair.Key + " " + kvPair.Value);
                if(kvPair.Key.EndsWith("." + AccountFieldNames.ApiUsername))
                {
                    if (apiUserName == null || apiUserName.Equals(kvPair.Value)) 
                    {
                        Account accnt = new Account();

                        string keyPrefix = kvPair.Key.Substring(0, kvPair.Key.IndexOf('.') + 1);

                        if (config.ContainsKey(keyPrefix + AccountFieldNames.ApiUsername)) 
                        {
                            accnt.APIUserName = config[keyPrefix + AccountFieldNames.ApiUsername];
                        }
                        if (config.ContainsKey(keyPrefix + AccountFieldNames.ApiPassword))
                        {
                            accnt.APIPassword = config[keyPrefix + AccountFieldNames.ApiPassword];
                        }
                        if (config.ContainsKey(keyPrefix + AccountFieldNames.ApiSignature)) 
                        {
                            accnt.APISignature = config[keyPrefix + AccountFieldNames.ApiSignature];
                        }
                        if(config.ContainsKey(keyPrefix + AccountFieldNames.ApiCertificate)) 
                        {
                            accnt.APICertificate = config[keyPrefix + AccountFieldNames.ApiCertificate];
                        }
                        if (config.ContainsKey(keyPrefix + AccountFieldNames.PrivateKeyPassword)) 
                        {
                            accnt.PrivateKeyPassword = config[keyPrefix + AccountFieldNames.PrivateKeyPassword];
                        }            
                        if(config.ContainsKey(keyPrefix + AccountFieldNames.CertificateSubject))
                        {
                            accnt.CertificateSubject = config[keyPrefix + AccountFieldNames.CertificateSubject];
                        }
                        if(config.ContainsKey(keyPrefix + AccountFieldNames.ApplicationId))
                        {
                            accnt.ApplicationId = config[keyPrefix + AccountFieldNames.ApplicationId];
                        }
                        return accnt;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the API Credentials
        /// </summary>
        /// <param name="apiUserName"></param>
        /// <returns></returns>
        public ICredential GetCredentials(Dictionary<string, string> config, string apiUserName)
        {
            ICredential credential = null;
            Account accnt = GetAccount(config, apiUserName);
            if (accnt == null)
            {
                throw new MissingCredentialException("Missing credentials for " + apiUserName);
            }
            if (!string.IsNullOrEmpty(accnt.APICertificate))
            {
                CertificateCredential certCredential = new CertificateCredential(accnt.APIUserName, accnt.APIPassword, accnt.APICertificate, accnt.PrivateKeyPassword);
                certCredential.ApplicationId = accnt.ApplicationId;
                if (!string.IsNullOrEmpty(accnt.CertificateSubject))
                {
                    SubjectAuthorization subAuthorization = new SubjectAuthorization(accnt.CertificateSubject);
                    certCredential.ThirdPartyAuthorization = subAuthorization;
                }
                credential = certCredential;
            }
            else
            {
                SignatureCredential signCredential = new SignatureCredential(accnt.APIUserName, accnt.APIPassword, accnt.APISignature);
                signCredential.ApplicationId = accnt.ApplicationId;
                if (!string.IsNullOrEmpty(accnt.SignatureSubject))
                {
                    SubjectAuthorization subjectAuthorization = new SubjectAuthorization(accnt.SignatureSubject);
                    signCredential.ThirdPartyAuthorization = subjectAuthorization;
                }
                if (!string.IsNullOrEmpty(accnt.CertificateSubject))
                {
                    SubjectAuthorization subAuthorization = new SubjectAuthorization(accnt.CertificateSubject);
                    signCredential.ThirdPartyAuthorization = subAuthorization;
                }
                credential = signCredential;
            }
            ValidateCredentials(credential);
            
            return credential;            
        }

        /// <summary>
        /// Validates the API Credentials
        /// </summary>
        /// <param name="apiCredentials"></param>
        private void ValidateCredentials(ICredential apiCredentials)
        {
            if (apiCredentials is SignatureCredential)
            {
                SignatureCredential credential = (SignatureCredential)apiCredentials;
                Validate(credential);
            }
            else if (apiCredentials is CertificateCredential)
            {
                CertificateCredential credential = (CertificateCredential)apiCredentials;
                Validate(credential);
            }
        }

        /// <summary>
        /// Validates the Signature Credentials
        /// </summary>
        /// <param name="apiCredentials"></param>
        private void Validate(SignatureCredential apiCredentials)
        {
            if (string.IsNullOrEmpty(apiCredentials.UserName))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorUserName);
            }
            if (string.IsNullOrEmpty(apiCredentials.Password))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorPassword);
            }
            if (string.IsNullOrEmpty(((SignatureCredential)apiCredentials).Signature))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorSignature);
            }
        }

        /// <summary>
        /// Validates the Certificate Credentials
        /// </summary>
        /// <param name="apiCredentials"></param>
        private void Validate(CertificateCredential apiCredentials)
        {
            if (string.IsNullOrEmpty(apiCredentials.UserName))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorUserName);
            }
            if (string.IsNullOrEmpty(apiCredentials.Password))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorPassword);
            }

            if (string.IsNullOrEmpty(((CertificateCredential)apiCredentials).CertificateFile))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorCertificate);
            }

            if (string.IsNullOrEmpty(((CertificateCredential)apiCredentials).PrivateKeyPassword))
            {
                throw new InvalidCredentialException(BaseConstants.ErrorMessages.ErrorPrivateKeyPassword);
            }
        }      
    }
}
