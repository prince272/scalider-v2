namespace Scalider.AspNetCore.Navigation.Xml
{

    /// <summary>
    /// Options used by the <see cref="XmlNavigationTreeBuilder"/> class.
    /// </summary>
    public class XmlNavigationOptions
    {

        /// <summary>
        /// A value indicating the default file name of the navigation definition file.
        /// </summary>
        public const string DefaultFileName = "navigation.xml";

        /// <summary>
        /// Gets or sets a value indicating the name of the file navigation definition file.
        /// </summary>
        public string FileName { get; set; } = DefaultFileName;

    }

}