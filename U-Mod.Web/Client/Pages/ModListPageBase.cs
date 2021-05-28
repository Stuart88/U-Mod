using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using U_Mod.Shared.Helpers;
using U_Mod.Shared.Models;

namespace U_Mod.Web.Client.Pages
{
    public class ModListPageBase : LayoutComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }
        public MasterList MasterList { get; set; }
       

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                BasicHttpResponse<MasterList> resp = await Http.GetFromJsonAsync<BasicHttpResponse<MasterList>>($"Mod/Masterlist");

                this.MasterList = resp.Data;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(StringHelpers.ErrorMessage(e));
            }

        }
    }
}
