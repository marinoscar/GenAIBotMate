using Markdig;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Extensions.Tables;

namespace Luval.GenAIBotMate.Components
{
    public partial class ChatMessageContent : ComponentBase
    {
        private readonly MarkdownPipeline _pipeline;
        public ChatMessageContent()
        {
            _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions()
                .UsePipeTables()
                .UseMathematics()
                .UseDiagrams()
                .UseFigures()
                .UseAutoLinks()
                .UseBootstrap()
                .Build();
        }

        [Parameter]
        public string AgentResponse { get; set; } = "";

        [Parameter]
        public string UserMessage { get; set; } = "";

        public MarkupString GetHtmlFromMD(string md)
        {
            return new MarkupString(Markdown.ToHtml(md ?? "", _pipeline));
        }
    }
}
