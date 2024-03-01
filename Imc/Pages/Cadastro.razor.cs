using Blazored.LocalStorage;
using Imc.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Imc.Pages
{
    public class CadastroBase : ComponentBase
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] ILocalStorageService localStorage { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected string nomeUsuario;
        protected string emailUsuario;
        protected string senhaUsuario;
        protected string confirmacaoSenhaUsuario;
        protected bool naoSouRobo = false;
        protected bool bMostrarConteudo = false;

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(2000);

            bMostrarConteudo = true;

            StateHasChanged();
        }

        protected async Task OnClickCadastrar()
        {
            if (!await ValidaCampos())
            {
                return;
            }

            var usuarioLocal = await localStorage.GetItemAsync<string>("usuariosLogados");

            if (usuarioLocal != null)
            {
                var usuarios = JsonSerializer.Deserialize<List<Usuario>>(usuarioLocal) ?? new List<Usuario>();

                if (usuarios.Any(x => x.Email == emailUsuario))
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Já existe um usuário cadastrado com este e-mail");
                    return;
                }

                var usuarioAux = new Usuario
                {
                    UserName = nomeUsuario,
                    Email = emailUsuario,
                    Senha = senhaUsuario
                };

                usuarios.Add(usuarioAux);

                await localStorage.SetItemAsync("usuarioLogado", JsonSerializer.Serialize(usuarioAux));
                await localStorage.SetItemAsync("usuariosLogados", JsonSerializer.Serialize(usuarios));
            }
            else
            {
                var usuarioAux = new Usuario
                {
                    UserName = nomeUsuario,
                    Email = emailUsuario,
                    Senha = senhaUsuario
                };

                await localStorage.SetItemAsync("usuarioLogado", JsonSerializer.Serialize(usuarioAux));
                await localStorage.SetItemAsync("usuariosLogados", JsonSerializer.Serialize(new List<Usuario>{usuarioAux}));
            }

            await JSRuntime.InvokeVoidAsync("alert", "Cadastro efetuado com sucesso");
            NavigationManager.NavigateTo("calcula-imc");
        }

        private async Task<bool> ValidaCampos()
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                await JSRuntime.InvokeVoidAsync("alert", "O campo Username é obrigatório");
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailUsuario))
            {
                await JSRuntime.InvokeVoidAsync("alert", "O campo E-mail é obrigatório");
                return false;
            }

            if (string.IsNullOrWhiteSpace(senhaUsuario))
            {
                await JSRuntime.InvokeVoidAsync("alert", "O campo Senha é obrigatório");
                return false;
            }

            if (string.IsNullOrWhiteSpace(confirmacaoSenhaUsuario))
            {
                await JSRuntime.InvokeVoidAsync("alert", "O campo Confirmação de Senha é obrigatório");
                return false;
            }

            if (senhaUsuario != confirmacaoSenhaUsuario)
            {
                await JSRuntime.InvokeVoidAsync("alert", "O campo Senha está diferente do campo Confirmação de Senha");
                return false;
            }
            if (!naoSouRobo)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Marque a opção 'Não sou um robô'");
                return false;
            }

            return true;
        }

        protected void OnCheckboxChange(ChangeEventArgs e)
        {
            naoSouRobo = (bool)e.Value;
        }

        protected void OnClickFazerLogin()
        {
            NavigationManager.NavigateTo("");
        }
    }
}
