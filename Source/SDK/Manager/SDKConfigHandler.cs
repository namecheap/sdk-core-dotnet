using System.Configuration;

namespace PayPal.Manager
{
    /// <summary>
    /// Custom handler for SDK configuration section as defined in configuration files
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// API Username
        /// </summary>
        string APIUserName { get; set; }

        /// <summary>
        /// API password
        /// </summary>
        string APIPassword { get; set; }

        /// <summary>
        /// Application Id
        /// </summary>
        string ApplicationId { get; set; }

        /// <summary>
        /// API signature
        /// </summary>
        string APISignature { get; set; }

        /// <summary>
        /// Client certificate for SSL authentication
        /// </summary>
        string APICertificate { get; set; }

        /// <summary>
        /// Private key password for SSL authentication
        /// </summary>
        string PrivateKeyPassword { get; set; }

        /// <summary>
        /// Signature Subject
        /// </summary>
        string SignatureSubject { get; set; }

        /// <summary>
        /// Certificate Subject
        /// </summary>
        string CertificateSubject { get; set; }
    }

#if NETSTANDARD || NETSTANDARD2_0
    /// <summary>
    /// Custom handler for SDK configuration section as defined in configuration files
    /// </summary>
    public class Account : IAccount
    {
        /// <summary>
        /// API Username
        /// </summary>
        public string APIUserName { get; set; }

        /// <summary>
        /// API password
        /// </summary>
        public string APIPassword { get; set; }

        /// <summary>
        /// Application Id
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// API signature
        /// </summary>
        public string APISignature { get; set; }

        /// <summary>
        /// Client certificate for SSL authentication
        /// </summary>
        public string APICertificate { get; set; }

        /// <summary>
        /// Private key password for SSL authentication
        /// </summary>
        public string PrivateKeyPassword { get; set; }

        /// <summary>
        /// Signature Subject
        /// </summary>
        public string SignatureSubject { get; set; }

        /// <summary>
        /// Certificate Subject
        /// </summary>
        public string CertificateSubject { get; set; }
    }
#else

    /// <summary>
    /// Custom handler for SDK configuration section as defined in App.Config or Web.Config files
    /// </summary>    
    public class SDKConfigHandler : ConfigurationSection
    {  
        private static readonly ConfigurationProperty accountsElement =
             new ConfigurationProperty("accounts", typeof(AccountCollection), null, ConfigurationPropertyOptions.IsRequired);

        /// <summary>
        /// Explicit default constructor
        /// </summary>
        public SDKConfigHandler() { }

        /// <summary>
        /// Accounts Collection
        /// </summary>
        [ConfigurationProperty("accounts", IsRequired = true)]
        public AccountCollection Accounts
        {
            get { return (AccountCollection)this[accountsElement]; }
        }

        [ConfigurationProperty("settings", IsRequired = true)]
        public NameValueConfigurationCollection Settings
        {
            get { return (NameValueConfigurationCollection)this["settings"]; }
        }

        public string Setting(string name)
        {
            NameValueConfigurationElement config = Settings[name];
            return ((config == null) ? null : config.Value);
        }
    }    

    [ConfigurationCollection(typeof(Account), AddItemName = "account", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class AccountCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Account();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Account)element).APIUserName;
        }

        public Account Account(int index)
        {
            return (Account)BaseGet(index);
        }

        public Account Account(string value)
        {
            return (Account)BaseGet(value);
        }

        public new  Account this[string name]
        {
            get { return (Account)BaseGet(name); }
        }

        public Account this[int index]
        {
            get { return (Account)BaseGet(index); }
        }
    }

    /// <summary>
    /// Class holds the <Account> element
    /// </summary>
    public class Account : ConfigurationElement, IAccount
    {
        private static readonly ConfigurationProperty apiUserName =
            new ConfigurationProperty(AccountFieldNames.ApiUsername, typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty apiPassword =
            new ConfigurationProperty(AccountFieldNames.ApiPassword, typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty appId =
            new ConfigurationProperty(AccountFieldNames.ApplicationId, typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty apiSignature =
            new ConfigurationProperty(AccountFieldNames.ApiSignature, typeof(string), string.Empty);

        private static readonly ConfigurationProperty apiCertificate =
            new ConfigurationProperty(AccountFieldNames.ApiCertificate, typeof(string), string.Empty);

        private static readonly ConfigurationProperty privateKeyPassword =
            new ConfigurationProperty(AccountFieldNames.PrivateKeyPassword, typeof(string), string.Empty);

        private static readonly ConfigurationProperty signSubject =
           new ConfigurationProperty(AccountFieldNames.SignatureSubject, typeof(string), string.Empty);

        private static readonly ConfigurationProperty certifySubject =
           new ConfigurationProperty(AccountFieldNames.CertificateSubject, typeof(string), string.Empty);

        public Account()
        {
            base.Properties.Add(apiUserName);
            base.Properties.Add(apiPassword);
            base.Properties.Add(appId);
            base.Properties.Add(apiSignature);
            base.Properties.Add(apiCertificate);
            base.Properties.Add(privateKeyPassword);
            base.Properties.Add(signSubject);
            base.Properties.Add(certifySubject);
        }

        /// <summary>
        /// API Username
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.ApiUsername, IsRequired = true)]
        public string APIUserName
        {
            get { return (string)this[apiUserName]; }
            set { this[apiUserName] = value; }
        }

        /// <summary>
        /// API password
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.ApiPassword, IsRequired = true)]
        public string APIPassword
        {
            get { return (string)this[apiPassword]; }
            set { this[apiPassword] = value; }
        }

        /// <summary>
        /// Application Id
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.ApplicationId)]
        public string ApplicationId
        {
            get { return (string)this[appId]; }
            set { this[appId] = value; }
        }

        /// <summary>
        /// API signature
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.ApiSignature)]
        public string APISignature
        {
            get { return (string)this[apiSignature]; }
            set { this[apiSignature] = value; }
        }

        /// <summary>
        /// Client certificate for SSL authentication
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.ApiCertificate)]
        public string APICertificate
        {
            get { return (string)this[apiCertificate]; }
            set { this[apiCertificate] = value; }
        }

        /// <summary>
        /// Private key password for SSL authentication
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.PrivateKeyPassword)]
        public string PrivateKeyPassword
        {
            get { return (string)this[privateKeyPassword]; }
            set { this[privateKeyPassword] = value; }
        }
      
        /// <summary>
        /// Signature Subject
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.SignatureSubject)]
        public string SignatureSubject
        {
            get { return (string)this[signSubject]; }
            set { this[signSubject] = value; }
        }

        /// <summary>
        /// Certificate Subject
        /// </summary>
        [ConfigurationProperty(AccountFieldNames.CertificateSubject)]
        public string CertificateSubject
        {
            get { return (string)this[certifySubject]; }
            set { this[certifySubject] = value; }
        } 
    } 

#endif
}
