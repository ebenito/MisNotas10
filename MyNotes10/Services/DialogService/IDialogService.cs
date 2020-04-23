using System.Threading.Tasks;

namespace MyNotes10.Services.DialogService
{
    public interface IDialogService 
    {
        void ShowMessage(string message, string header);
        Task<bool> ShowMessageYesNo(string message);
       // bool ShowMessageYesNo(string message);
    }
}
