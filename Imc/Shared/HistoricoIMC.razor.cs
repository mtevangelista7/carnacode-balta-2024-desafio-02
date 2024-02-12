using Blazored.LocalStorage;
using Imc.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Imc.Shared
{
    public class HistoricoIMCBase : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; }
        protected List<HistoricoImcUsuario> ListIMC = null;
        protected string campoPesquisar;

        protected override async Task OnInitializedAsync()
        {
            await CarregaDados();
        }

        public async Task LimparHistorico()
        {
            await localStorage.RemoveItemAsync("listIMC");
            ListIMC = null;

            StateHasChanged();
        }

        public async Task CarregaDados()
        {
            var listValoresIMC = await localStorage.GetItemAsync<string>("listIMC");

            if (listValoresIMC != null)
            {
                ListIMC = JsonSerializer.Deserialize<List<HistoricoImcUsuario>>(listValoresIMC);

                ListIMC = [.. ListIMC.OrderByDescending(x => x.DataImc)];
            }

            StateHasChanged();
        }

        protected string FormatarData(DateTime data)
        {
            TimeSpan diferenca = DateTime.Now - data;

            if (diferenca.TotalMinutes < 1)
                return "agora";
            else if (diferenca.TotalMinutes < 60)
                return $"{(int)diferenca.TotalMinutes}m atrás";
            else if (diferenca.TotalHours < 24 && data.Date == DateTime.Today)
                return $"{(int)diferenca.TotalHours}h atrás";
            else
                return data.ToString("dd/MM/yyyy");
        }

        protected string ConvertStringFiltro(string valor)
        {
            return valor.Trim().ToLower();
        }
    }
}
