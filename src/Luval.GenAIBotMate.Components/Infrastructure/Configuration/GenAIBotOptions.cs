using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Components.Infrastructure.Configuration
{
    /// <summary>
    /// Represents the configuration options for the GenAIBot
    /// </summary>
    public class GenAIBotOptions
    {
        /// <summary>
        /// The temperature to use for the OpenAI model.
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// The model to use for the OpenAI chat completion.
        /// </summary>
        public string Model { get; set; } = OpenAIModels.GPT4o;
    }

    public class OpenAIModels
    {
        public const string GPT4o = "gpt-4o";
        public const string GPT4o_Mini = "gpt-4o-mini";
        public const string GPT35_Turbo = "gpt-3.5-turbo-0125";
    }
}
