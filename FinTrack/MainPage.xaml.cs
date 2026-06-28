using FinTrack.Services;
using FinTrack.Models;
using System.Text.Json;
using System.Web;

namespace FinTrack;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MainPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;

        AppWebView.Navigating += OnWebViewNavigating;
        AppWebView.Navigated += OnWebViewNavigated;
    }

    private async void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
    {
        // Garante que os dados sejam carregados assim que a página estiver pronta
        await CarregarDadosDoBanco();
    }

    private async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("fintrack://"))
        {
            e.Cancel = true;

            try
            {
                string jsonUrl = e.Url.Replace("fintrack://", "");
                string jsonRecebido = HttpUtility.UrlDecode(jsonUrl);
                
                using var doc = JsonDocument.Parse(jsonRecebido);
                var root = doc.RootElement;
                string tipo = root.GetProperty("tipo").GetString();

                if (tipo == "ativo")
                {
                    await _databaseService.SalvarAtivoAsync(new Ativo
                    {
                        Nome = root.GetProperty("nome").GetString() ?? "Ativo",
                        Valor = root.GetProperty("valor").GetDouble()
                    });
                }
                else if (tipo == "despesa")
                {
                    await _databaseService.SalvarDespesaAsync(new Despesa
                    {
                        Descricao = root.GetProperty("nome").GetString() ?? "Despesa",
                        Valor = root.GetProperty("valor").GetDouble()
                    });
                }

                await CarregarDadosDoBanco();
            }
            catch (Exception ex)
            {
                // Mostra o erro na UI para sabermos o que aconteceu
                await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Erro", ex.Message, "OK"));
            }
        }
    }

    private async Task CarregarDadosDoBanco()
    {
        var ativos = await _databaseService.GetAtivosAsync();
        var despesas = await _databaseService.GetDespesasAsync();

        var dadosDoBanco = new { ativos = ativos, despesas = despesas };
        
        // CORREÇÃO DE SEGURANÇA: Escape para evitar que aspas no texto quebrem o JS
        string jsonParaEnviar = JsonSerializer.Serialize(dadosDoBanco);
        string script = $"receberDadosDoCSharp({JsonSerializer.Serialize(jsonParaEnviar)})";

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await AppWebView.EvaluateJavaScriptAsync(script);
        });
    }
}