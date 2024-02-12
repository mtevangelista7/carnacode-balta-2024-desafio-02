using Blazored.LocalStorage;
using Imc.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Imc.Shared
{
    public class CalculaIMCBase : ComponentBase
    {
        [Parameter] public EventCallback<string> CarregaHistorico { get; set; }
        [Inject] public IJSRuntime jSRuntime {  get; set; }
        [Inject] ILocalStorageService localStorage {  get; set; }

        protected string? alturaUsuario;
        protected string? pesoUsuario;
        protected string? sexoUsuario;
        protected bool possui65AnosOuMais = false;

        protected async Task OnClickCalcularImc()
        {
            List<HistoricoImcUsuario> listItensImc;
            string jsonHistorico;

            try
            {
                if (!await ValidaCampos())
                {
                    return;
                }

                if (!double.TryParse(alturaUsuario.Replace(".", ","), out double altura))
                {
                    await jSRuntime.InvokeVoidAsync("alert", "O valor do campo Altura é inválido, utilize vírgula como separador");
                    return;
                }

                if (!double.TryParse(pesoUsuario.Replace(".", ","), out double peso))
                {
                    await jSRuntime.InvokeVoidAsync("alert", "O valor do campo Peso é inválido");
                    return;
                }

                var resultadoIMC = peso / Math.Pow(altura, 2);
                await jSRuntime.InvokeVoidAsync("alert", "IMC calculado com sucesso!");

                if (await localStorage.ContainKeyAsync("listIMC"))
                {
                    jsonHistorico = await localStorage.GetItemAsync<string>("listIMC");

                    if (!String.IsNullOrWhiteSpace(jsonHistorico))
                    {
                        listItensImc = JsonSerializer.Deserialize<List<HistoricoImcUsuario>>(jsonHistorico);
                    }
                    else
                    {
                        listItensImc = [];
                    }   
                }
                else
                {
                    listItensImc = [];
                }

                listItensImc.Add(new HistoricoImcUsuario { Classificacao = DefineClassificacaoImc(resultadoIMC), DataImc = DateTime.Now, Imc = resultadoIMC });

                string jsonString = JsonSerializer.Serialize(listItensImc);
                await localStorage.SetItemAsync("listIMC", jsonString);

                await CarregaHistorico.InvokeAsync();

                LimparCampos();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> ValidaCampos()
        {
            string mensagemErro = "Preencha o campo: ";

            if (String.IsNullOrWhiteSpace(alturaUsuario))
            {
                await jSRuntime.InvokeVoidAsync("alert", String.Format("{0} {1}", mensagemErro, "Altura"));
                return false;
            }

            if (String.IsNullOrWhiteSpace(pesoUsuario))
            {
                await jSRuntime.InvokeVoidAsync("alert", String.Format("{0} {1}", mensagemErro, "Peso"));
                return false;
            }

            if (String.IsNullOrWhiteSpace(sexoUsuario))
            {
                await jSRuntime.InvokeVoidAsync("alert", String.Format("{0} {1}", mensagemErro, "Sexo"));
                return false;
            }

            return true;
        }

        private string DefineClassificacaoImc(double valorImc)
        {
            if (possui65AnosOuMais)
            {
                if (valorImc < 22)
                {
                    return "Magreza";
                }
                else if (valorImc >= 22 && valorImc < 27)
                {
                    return "Peso ideal";
                }
                else
                {
                    return "Sobrepeso";
                }
            }
            else
            {
                if (valorImc < 18.5)
                {
                    return "Magreza";
                }
                else if (valorImc >= 18.5 && valorImc < 24.9)
                {
                    return "Peso ideal";
                }
                else if (valorImc >= 24.9 && valorImc < 29.9)
                {
                    return "Sobrepeso";
                }
                else if (valorImc >= 29.9 && valorImc < 34.9)
                {
                    return "Obesidade grau I";
                }
                else if (valorImc >= 34.9 && valorImc < 39.9)
                {
                    return "Obesidade grau II";
                }
                else
                {
                    return "Obesidade grau III";
                }
            }
        }

        protected void LimparCampos()
        {
            alturaUsuario = "";
            pesoUsuario = "";
            sexoUsuario = "";
            possui65AnosOuMais = false;
        }

        protected void OnCheckboxChange(ChangeEventArgs e)
        {
            possui65AnosOuMais = (bool)e.Value;
        }
    }
}
