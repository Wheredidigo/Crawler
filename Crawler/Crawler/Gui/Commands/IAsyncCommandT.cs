using System.Threading.Tasks;
using System.Windows.Input;

namespace Crawler.Gui.Commands
{
    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
    {
        ICommand Command { get; }
        Task ExecuteAsync(T obj);
        bool CanExecute(object obj);
    }
}