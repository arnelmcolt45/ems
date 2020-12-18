using System.Collections.Generic;
using MvvmHelpers;
using Ems.Models.NavigationMenu;

namespace Ems.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}