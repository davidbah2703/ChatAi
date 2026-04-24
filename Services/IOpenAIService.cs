using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatAI.Models;

namespace ChatAI.Services
{
    public interface IOpenAIService
    {
        Task<string> SendMessageAsync(
            IEnumerable<Message> history,
            CancellationToken cancellationToken = default);
    }
}