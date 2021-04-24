using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace U_Mod.Extensions
{
    public class MyWebClient : WebClient
    {
        /// <summary>
        /// Default constructor (30000 ms timeout)
        /// NOTE: timeout can be changed later on using the [Timeout] property.
        /// </summary>
        public MyWebClient() : this(30000) { }

        /// <summary>
        /// Constructor with customizable timeout
        /// </summary>
        /// <param name="timeout">
        /// Web request timeout (in milliseconds)
        /// </param>
        public MyWebClient(int timeout)
        {
            Timeout = timeout;
        }

        #region Methods
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = Timeout;
            ((HttpWebRequest)w).ReadWriteTimeout = Timeout;
            return w;
        }
        #endregion
        
        /// <summary>
        /// Web request timeout (in milliseconds)
        /// </summary>
        public int Timeout { get; set; }
    }
}
