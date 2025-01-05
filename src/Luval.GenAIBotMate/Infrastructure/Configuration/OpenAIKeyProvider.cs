using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Configuration
{
    /// <summary>
    /// The OpenAIKeyProvider class is responsible for storing and providing the OpenAI API key.
    /// This class ensures that the key is securely stored and can be accessed by other classes
    /// that require it to interact with the OpenAI API.
    /// </summary>
    public class OpenAIKeyProvider
    {
        private readonly string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIKeyProvider"/> class with the specified API key.
        /// </summary>
        /// <param name="key">The OpenAI API key.</param>
        public OpenAIKeyProvider(string key)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            _key = key;
        }

        /// <summary>
        /// Gets the OpenAI API key.
        /// </summary>
        /// <returns>The OpenAI API key.</returns>
        public string GetKey()
        {
            return _key;
        }
    }
}
