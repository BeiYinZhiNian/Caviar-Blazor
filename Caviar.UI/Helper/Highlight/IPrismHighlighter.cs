using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caviar.UI.Helper
{
    public interface IPrismHighlighter
    {
        ValueTask<MarkupString> HighlightAsync(string code, string language);

        Task HighlightAllAsync();
    }
}