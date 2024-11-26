using PIMFazendaUrbanaRadzen.Services;
using PIMFazendaUrbanaRadzen.Components;
using Radzen;
using PIMFazendaUrbanaAPI.DTOs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Adicionando servi�os ao container
builder.Services.AddRazorComponents()
      .AddInteractiveServerComponents()
      .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024); // Aumenta o tamanho m�ximo da mensagem

builder.Services.AddControllers(); // Adiciona suporte a Controllers
builder.Services.AddRadzenComponents(); // Adiciona servi�os Radzen

// Servi�o de tema usando cookies
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MyApplicationTheme";
    options.Duration = TimeSpan.FromDays(365); // Tempo de dura��o do cookie de tema
});

// Configurar cultura padr�o
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddHttpClient(); // Registra o HttpClient para chamadas HTTP

var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://localhost:7079/api";

builder.Services.AddScoped<DialogService>(); // Para DialogService

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Registra o servi�o de autentica��o
builder.Services.AddScoped<AuthService>(provider =>
    new AuthService(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/auth/login",
        provider.GetRequiredService<ILocalStorageService>(),
        provider.GetRequiredService<CustomAuthenticationStateProvider>() // Passando o provedor aqui
    ));

// Registra as roles/claims
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Funcion�rio", policy =>
        policy.RequireClaim("role", "Funcion�rio", "Gerente")); // Funcion�rio e Gerente t�m acesso

    options.AddPolicy("Gerente", policy =>
        policy.RequireClaim("role", "Gerente")); // Somente Gerente
});



// Registra o servi�o CepApiService
builder.Services.AddScoped<CepApiService>(provider =>
    new CepApiService(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/cep"
    ));

builder.Services.AddScoped(provider =>
    new ClienteApiService<ClienteDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/cliente"
    ));

builder.Services.AddScoped(provider =>
    new FornecedorApiService<FornecedorDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/fornecedor"
    ));

builder.Services.AddScoped(provider =>
    new FuncionarioApiService<FuncionarioDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/funcionario"
    ));

builder.Services.AddScoped(provider =>
    new CultivoApiService<CultivoDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/cultivo"
    ));

builder.Services.AddScoped(provider =>
    new ProducaoApiService<ProducaoDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/producao"
    ));

builder.Services.AddScoped(provider =>
    new InsumoApiService<InsumoDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/insumo"
    ));

builder.Services.AddScoped(provider =>
    new InsumoApiService<SaidaInsumoDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/insumo"
    ));

builder.Services.AddScoped(provider =>
    new RecomendacaoApiService<CultivoDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/recomendacao"
    ));

builder.Services.AddScoped(provider =>
    new EstoqueProdutoApiService<EstoqueProdutoDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/estoqueproduto"
    ));

builder.Services.AddScoped(provider =>
    new CompraApiService<PedidoCompraItemDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/compra"
    ));

builder.Services.AddScoped(provider =>
    new VendaApiService<PedidoVendaItemDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/venda"
    ));

builder.Services.AddScoped(provider =>
    new VendaApiService<PedidoVendaDTO>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/venda"
    ));

builder.Services.AddScoped(provider =>
    new ExportacaoApiService<object>(
        provider.GetRequiredService<HttpClient>(),
        $"{apiBaseUrl}/exportacao"
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Request.Path.Value.Contains("_framework"))
    {
        context.Response.Redirect("/not-found");
    }
});


//app.MapBlazorHub(); // Configura��o necess�ria
//app.MapFallbackToPage("/not-found");

app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.MapControllers(); // Mapeia as controllers
app.UseStaticFiles(); // Habilita o uso de arquivos est�ticos
app.UseAntiforgery(); // Protege contra CSRF (Cross-Site Request Forgery)

// Mapeia os componentes interativos do Blazor Server
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode(); // Renderiza��o interativa do Blazor Server

app.Run();