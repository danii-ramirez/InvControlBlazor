using InvControl.Client.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InvControl.Client.Services
{
    public class RadzenCustom
    {
        [Inject]
        DialogService DialogService { get; set; }

        public RadzenCustom(DialogService dialogService)
        {
            DialogService = dialogService;
        }

        public void OpenBusyWithLoader(string text = "Guardando...")
        {
            DialogService.Open("",
               ds =>
               {
                   void content(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b)
                   {
                       b.OpenComponent(0, typeof(Busy));
                       b.AddAttribute(1, "Text", text);
                       b.CloseComponent();
                   }
                   return content;
               },
               new DialogOptions()
               {
                   ShowTitle = false,
                   Style = "min-height: auto; min-width: auto; width: auto",
                   CloseDialogOnEsc = false
               });
        }

        public void OpenBusy(string message)
        {
            DialogService.Open("",
                ds =>
                {
                    RenderFragment content = b =>
                    {
                        b.OpenElement(0, "RadzenRow");
                        b.OpenElement(1, "RadzenColumn");
                        b.AddAttribute(2, "Size", "12");
                        b.AddContent(3, message);
                        b.CloseElement();
                        b.CloseElement();
                    };

                    return content;
                },
                new DialogOptions()
                {
                    ShowTitle = false,
                    Style = "min-height: auto; min-width: auto; width: auto;",
                    CloseDialogOnEsc = false
                });
        }

        public void ShowErrors(List<string> errors, string width = "500px")
        {
            DialogService.Open("Compruebe los datos ingresados",
                ds =>
                {
                    void content(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b)
                    {
                        b.OpenComponent(0, typeof(ShowErrors));
                        b.AddAttribute(1, "Errors", errors);
                        b.CloseComponent();
                    }
                    return content;
                },
                new DialogOptions()
                {
                    Width = width,
                    Height = "auto",
                    Style = "max-height: 600px;",
                    Resizable = false,
                    Draggable = false
                });
        }

        public void CloseDialog()
        {
            DialogService.Close();
        }
    }
}
