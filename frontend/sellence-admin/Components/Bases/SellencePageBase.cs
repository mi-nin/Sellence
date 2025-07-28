using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SellenceAdmin.Components.Bases;

internal abstract class SellencePageBase : ComponentBase
{
    [Inject]
    protected ILogger<PageBase> Logger { get; set; } = default!;

    protected bool IsLoading { get; set; }

    protected string? ErrorMessage { get; set; }

    protected abstract Task LoadDataAsync();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.IsLoading = true;
            this.ErrorMessage = null;
            await this.LoadDataAsync();
        }
        catch (Exception)
        {
            this.ErrorMessage = "An error occurred while loading data.";
        }
        finally
        {
            this.IsLoading = false;
        }
    }
}
