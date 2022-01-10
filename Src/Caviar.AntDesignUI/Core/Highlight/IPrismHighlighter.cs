using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Core
{
    public interface IPrismHighlighter
    {
        ValueTask<MarkupString> HighlightAsync(string code, string language);

        Task HighlightAllAsync();
    }
}