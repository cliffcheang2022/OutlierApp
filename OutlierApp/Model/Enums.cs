using System.ComponentModel;

namespace OutlierApp.Model
{
    public enum EnumIniKey
    {
        /// <summary>
        /// Backup Path
        /// </summary>
        backuppath,

        /// <summary>
        /// Output Path
        /// </summary>
        outputpath,

        /// <summary>
        /// Input Path
        /// </summary>
        inputpath,

        /// <summary>
        /// Input File Name
        /// </summary>
        inputfile,

        /// <summary>
        /// Output File Name
        /// </summary>
        outputfile,

        /// <summary>
        /// enable function
        /// </summary>
        enable,

        /// <summary>
        /// Server Host
        /// </summary>
        host,

        /// <summary>
        /// Server Port
        /// </summary>
        port,

        /// <summary>
        /// DB Name
        /// </summary>
        dbname,

        /// <summary>
        /// user
        /// </summary>
        user,

        /// <summary>
        /// password
        /// </summary>
        password,

        /// <summary>
        /// Server Channel Key
        /// </summary>
        channelkey,

        /// <summary>
        /// From email address
        /// </summary>
        from,

        /// <summary>
        /// To email address
        /// </summary>
        to,

        /// <summary>
        /// Cc email address
        /// </summary>
        cc,

        /// <summary>
        /// Smtp subject
        /// </summary>
        subject,

        /// <summary>
        /// Smtp content
        /// </summary>
        content
    }

    public enum EnumIniSection
    {
        /// <summary>
        /// Encrypted Section
        /// </summary>
        [Description("Encrypted")]
        EncryptedSection,

        /// <summary>
        /// DB Server Socket Section
        /// </summary>
        [Description("DBServer")]
        DBServerSection,

        /// <summary>
        /// Main Section
        /// </summary>
        [Description("OutLier")]
        MainSection,

        /// <summary>
        /// Alert Section
        /// </summary>
        [Description("Alert")]
        AlertSection
    }
}
