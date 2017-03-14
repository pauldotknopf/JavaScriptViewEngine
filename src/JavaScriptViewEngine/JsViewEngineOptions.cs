namespace JavaScriptViewEngine
{
    /// <summary>
    /// Options used to configure the JavaScript view engine
    /// </summary>
    public class JsViewEngineOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsViewEngineOptions"/> class.
        /// </summary>
        public JsViewEngineOptions()
        {
            ViewNamePrefix = "js-";
        }

        /// <summary>
        /// The value that view names should be prefixed with if you wish to use a javascript view engine.
        /// If you leave this blank, You must ensure that you remove the razor view engine to prevent searching
        /// it for views to render when you intend to use the javascript view engine.
        /// Also, if you leave the razor view engine, you must ensure this value is valid for file paths,
        /// because the razor view engine will throw exceptions for you when it gets asked to find a view.
        /// </summary>
        public string ViewNamePrefix { get; set; }
    }
}
