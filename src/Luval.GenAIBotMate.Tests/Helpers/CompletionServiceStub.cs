using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Tests.Helpers
{
    public class CompletionServiceStub : IChatCompletionService
    {
        private List<StreamingChatMessageContent> _streaming;

        public void Initialize(string jsonWithResponse)
        {
            _streaming = JsonSerializer.Deserialize<List<StreamingChatMessageContent>>(jsonWithResponse) ?? new List<StreamingChatMessageContent>();
        }

        public void Initialize()
        {
            Initialize(DefaultStreamingValue());
        }

        public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

        public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            if (_streaming == null) throw new InvalidOperationException("Class not initialize call the Initialize method for executing this method");
            if (!_streaming.Any()) throw new InvalidOperationException("Class was not initialized properly");

            return _streaming.ToAsyncEnumerable();
        }

        public static string DefaultStreamingValue()
        {
            return @"
[
  {
    ""Content"": """",
    ""Role"": {
      ""Label"": ""Assistant""
    },
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": ""Why"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" don\u0027t"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" skeleton"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": ""s"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" fight"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" each"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" other"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": ""?\n\n"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": ""They"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" don\u0027t"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" have"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" the"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": "" guts"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": ""!"",
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": null
    }
  },
  {
    ""Content"": null,
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": null,
      ""FinishReason"": ""Stop""
    }
  },
  {
    ""Content"": null,
    ""Role"": null,
    ""ChoiceIndex"": 0,
    ""ModelId"": ""gpt-4o"",
    ""Metadata"": {
      ""CompletionId"": ""chatcmpl-AiYM3R7A0KoiwwFxhhekRU5RGww78"",
      ""CreatedAt"": ""2024-12-26T02:57:35+00:00"",
      ""SystemFingerprint"": ""fp_5f20662549"",
      ""RefusalUpdate"": null,
      ""Usage"": {
        ""OutputTokenCount"": 14,
        ""InputTokenCount"": 26,
        ""TotalTokenCount"": 40,
        ""OutputTokenDetails"": {
          ""ReasoningTokenCount"": 0,
          ""AudioTokenCount"": 0
        },
        ""InputTokenDetails"": {
          ""AudioTokenCount"": 0,
          ""CachedTokenCount"": 0
        }
      },
      ""FinishReason"": null
    }
  }
]
";
        }
    }
}
