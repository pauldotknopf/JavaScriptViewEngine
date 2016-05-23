using System;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// An abstracted render engine
    /// </summary>
    public interface IRenderEngine : IDisposable
    {
        /// <summary>
        /// Perform a render
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="model">The model.</param>
        /// <param name="viewBag">The view bag.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        Task<RenderResult> Render(string path, object model, dynamic viewBag, ViewType viewType);
    }
    
    /// <summary>
    /// The type of full being rendered
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// Full request, including "<html></html>"
        /// </summary>
        Full,
        /// <summary>
        /// A small chunk of html
        /// </summary>
        Partial
    }

    /// <summary>
    /// The render result of a <see cref="IRenderEngine"/>
    /// </summary>
    public class RenderResult
    {
        /// <summary>
        /// The html
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// The response status code. Only used if the ViewType = Full
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Did the JavaScript engine ask us to redirect the user to another location?
        /// </summary>
        public string Redirect { get; set; }
    }
}