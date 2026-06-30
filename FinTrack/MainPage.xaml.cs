using System.Collections.ObjectModel;
using FinTrack.Models;
using FinTrack.Services;
using Microcharts;
using SkiaSharp;

namespace FinTrack;

public partial class MainPage : ContentPage
{
    private readonly FinanceService _financeService = new();
    public ObservableCollection<Transaction> TransacoesList { get; set; } = new();

    public MainPage()
    {
        InitializeComponent();
        ListaGeral.ItemsSource = TransacoesList;
        CarregarDados();
    }

    private void CarregarDados()
    {
        TransacoesList.Clear();
        var bdDados = _financeService.GetAll();
        foreach (var item in bdDados)
        {
            TransacoesList.Add(item);
        }
        AtualizarDashboard();
    }

    private void OnAdicionarAtivoClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntradaNomeAtivo.Text) || !decimal.TryParse(EntradaValorAtivo.Text, out decimal valor)) return;
        _financeService.Add(EntradaNomeAtivo.Text, valor, false);
        EntradaNomeAtivo.Text = string.Empty;
        EntradaValorAtivo.Text = string.Empty;
        CarregarDados();
    }

    private void OnAdicionarDespesaClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntradaNomeDespesa.Text) || !decimal.TryParse(EntradaValorDespesa.Text, out decimal valor)) return;
        _financeService.Add(EntradaNomeDespesa.Text, valor, true);
        EntradaNomeDespesa.Text = string.Empty;
        EntradaValorDespesa.Text = string.Empty;
        CarregarDados();
    }

    private void OnExcluirClicked(object? sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is int id)
        {
            _financeService.Delete(id);
            CarregarDados();
        }
    }

    private void AtualizarDashboard()
    {
        decimal totalAtivos = _financeService.GetTotal(false);
        decimal totalDespesas = _financeService.GetTotal(true);
        decimal saldo = _financeService.GetBalance();

        lblSaldo.Text = $"R$ {saldo:F2}";

        if (totalAtivos > 0)
        {
            decimal pct = (totalDespesas / totalAtivos) * 100;
            lblComprometimento.Text = $"{pct:F0}%";
            lblComprometimento.TextColor = pct > 70 ? Color.FromArgb("#dc3545") : Color.FromArgb("#198754");
        }
        else
        {
            lblComprometimento.Text = "0%";
            lblComprometimento.TextColor = Color.FromArgb("#198754");
        }

        if (totalDespesas > 0)
        {
            lblProporcao.Text = $"{(totalAtivos / totalDespesas):F1}x";
        }
        else if (totalAtivos > 0)
        {
            lblProporcao.Text = $"{totalAtivos:F1}x";
        }
        else
        {
            lblProporcao.Text = "0x";
        }

        // Renderização do Gráfico
        var entries = new[]
        {
            new ChartEntry((float)totalAtivos)
            {
                Label = "Ativos",
                ValueLabel = totalAtivos.ToString("C"),
                Color = SKColor.Parse("#198754")
            },
            new ChartEntry((float)totalDespesas)
            {
                Label = "Despesas",
                ValueLabel = totalDespesas.ToString("C"),
                Color = SKColor.Parse("#dc3545")
            }
        };

        graficoBarras.Chart = new BarChart 
        { 
            Entries = entries, 
            LabelTextSize = 30f, 
            BackgroundColor = SKColors.Transparent 
        };
    }
}