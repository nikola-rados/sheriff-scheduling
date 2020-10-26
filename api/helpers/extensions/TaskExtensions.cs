using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.Api.helpers.extensions
{
    public static class TaskExtensions
    {
        public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return await Task.WhenAll(tasks);
        }
    }
}
