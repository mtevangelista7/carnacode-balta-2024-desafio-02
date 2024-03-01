using Blazored.LocalStorage;
using Imc.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Imc.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] ILocalStorageService localStorage { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected string emailUsuario;
        protected string senhaUsuario;
        protected bool naoSouRobo = false;
        protected bool bMostrarConteudo = false;

        protected override async Task OnInitializedAsync()
        {
            var usuarioLocal = await localStorage.GetItemAsync<string>("usuarioLogado");

            if (usuarioLocal != null)
            {
                NavigationManager.NavigateTo("calcula-imc");
                return;
            }

            await Task.Delay(2000);

            bMostrarConteudo = true;

            StateHasChanged();  
        }

        protected void OnCheckboxChange(ChangeEventArgs e)
        {
            naoSouRobo = (bool)e.Value;
        }

        protected async Task OnClickFazerLogin()
        {
            if (!await ValidaCampos())
            {
                return;
            }

            var usuarioLocal = await localStorage.GetItemAsync<string>("usuariosLogados");

            if (usuarioLocal != null)
            {
                var usuarios = JsonSerializer.Deserialize<List<Usuario>>(usuarioLocal) ?? new List<Usuario>();

                var usuario = usuarios.FirstOrDefault(x => x.Email == emailUsuario && x.Senha == senhaUsuario);

                if (usuario != null)
                {
                    await localStorage.SetItemAsync("usuarioLogado", JsonSerializer.Serialize(usuario));
                    await JSRuntime.InvokeVoidAsync("alert", "Login efetuado com sucesso");
                    await JSRuntime.InvokeVoidAsync("location.reload");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Usuário ou senha inválidos");
                }
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Usuário ou senha inválidos");
            }
        }

        private async Task<bool> ValidaCampos()
        {
            if (string.IsNullOrEmpty(emailUsuario))
            {
                await JSRuntime.InvokeVoidAsync("alert", "Informe o e-mail");
                return false;
            }
            if (string.IsNullOrEmpty(senhaUsuario))
            {
                await JSRuntime.InvokeVoidAsync("alert", "Informe a senha");
                return false;
            }
            if (!naoSouRobo)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Marque a opção 'Não sou um robô'");
                return false;
            }

            return true;
        }

        protected void OnClickCadastrar()
        {
            NavigationManager.NavigateTo("cadastro");
        }
    }
}
