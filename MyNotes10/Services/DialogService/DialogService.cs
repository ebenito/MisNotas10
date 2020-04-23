using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace MyNotes10.Services.DialogService
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string header)
        {
            //var messageDialog = new MessageDialog(message, header);
            //var result = messageDialog.ShowAsync();

            //https://social.msdn.microsoft.com/Forums/windowsapps/en-US/d08c0968-65fa-4564-8917-a39836a4b27b/uwp-why-does-messagedialog-show-the-title-text-twice-under-windows-10?forum=wpdevelop
            try
            {
                ContentDialog messageDialog = new ContentDialog()
                {
                    Title = header,
                    Content = message
                };

                messageDialog.PrimaryButtonText = "OK";
                var res = messageDialog.ShowAsync();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en IDialogService: " + ex.Message);
            }
        }

        public async Task<bool> ShowMessageYesNo(string message)
        {

            bool resp = false;

            await Task.Run(() =>
            {
                var messageDialog = new MessageDialog(message);
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Si") { Id = 0 });
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;

                var result = messageDialog.ShowAsync();

                if ((int)result.Id == 0) resp = true;
                else resp = false;
            });

            return resp;
        }
        
    }
}
