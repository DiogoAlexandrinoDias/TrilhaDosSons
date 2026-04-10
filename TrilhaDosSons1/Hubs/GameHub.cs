using Microsoft.AspNetCore.SignalR;

namespace TrilhaDosSons.Hubs;

// ============================================================
// Modelos
// ============================================================
public class Jogador
{
    public string Id      { get; set; } = "";
    public string Nome    { get; set; } = "";
    public int    Pos     { get; set; } = 0;
    public string Cor     { get; set; } = "";
}

public class Pergunta
{
    public string   Audio   { get; set; } = "";
    public string[] Opcoes  { get; set; } = [];
    public int      Correta { get; set; }
    public string   Dica    { get; set; } = "";
    public int      Idx     { get; set; }
}

// ============================================================
// Hub
// ============================================================
public class GameHub : Hub
{
    // ── Estado global (singleton simples) ──
    private static readonly List<Jogador> Jogadores  = [];
    private static readonly List<int>     Usadas     = [];
    private static int     TurnoAtual   = 0;
    private static Pergunta? PerguntaAtual = null;
    private static bool    EmAndamento  = false;
    private static Jogador? Vencedor    = null;

    private const int META = 20;

    private static readonly string[] Cores =
    [
        "#e74c3c","#3498db","#2ecc71","#f39c12","#9b59b6",
        "#1abc9c","#e67e22","#e91e63","#00bcd4","#ff5722"
    ];

    private static readonly Pergunta[] Perguntas =
    [
        new(){ Audio="sons/cachorro.mp3", Opcoes=["Miau","Au Au","Muuuu"],         Correta=1, Dica="Animal doméstico" },
        new(){ Audio="sons/gato.mp3",     Opcoes=["Miau","Au Au","Cocoricó"],      Correta=0, Dica="Animal doméstico" },
        new(){ Audio="sons/vaca.mp3",     Opcoes=["Beeee","Muuuu","Oink"],         Correta=1, Dica="Animal da fazenda" },
        new(){ Audio="sons/galo.mp3",     Opcoes=["Cocoricó","Quac quac","Au Au"], Correta=0, Dica="Ave da fazenda" },
        new(){ Audio="sons/pato.mp3",     Opcoes=["Miau","Cocoricó","Quac quac"],  Correta=2, Dica="Ave aquática" },
        new(){ Audio="sons/ovelha.mp3",   Opcoes=["Beeee","Muuuu","Oink"],         Correta=0, Dica="Animal de lã" },
        new(){ Audio="sons/porco.mp3",    Opcoes=["Au Au","Oink","Beeee"],         Correta=1, Dica="Animal da fazenda" },
        new(){ Audio="sons/sapo.mp3",     Opcoes=["Croá croá","Piar","Zurrar"],    Correta=0, Dica="Anfíbio" },
        new(){ Audio="sons/passaro.mp3",  Opcoes=["Latir","Piar","Mugir"],         Correta=1, Dica="Ave" },
        new(){ Audio="sons/cavalo.mp3",   Opcoes=["Zurrar","Mugir","Cacarejar"],   Correta=0, Dica="Animal de montaria" },
        new(){ Audio="sons/elefante.mp3", Opcoes=["Rosnar","Latir","Barrir"],      Correta=2, Dica="O maior animal terrestre" },
        new(){ Audio="sons/leao.mp3",     Opcoes=["Rugir","Miar","Piar"],          Correta=0, Dica="Rei da selva" },
        new(){ Audio="sons/relogio.mp3",  Opcoes=["Din din","Tic tac","Zum zum"],  Correta=1, Dica="Mede o tempo" },
        new(){ Audio="sons/sino.mp3",     Opcoes=["Tic tac","Zum zum","Din din"],  Correta=2, Dica="Instrumento percussivo" },
        new(){ Audio="sons/abelha.mp3",   Opcoes=["Tum tum","Zum zum","Piu piu"],  Correta=1, Dica="Inseto que faz mel" },
        new(){ Audio="sons/trem.mp3",     Opcoes=["Tchuuu","Biiip","Vrumm"],       Correta=0, Dica="Meio de transporte sobre trilhos" },
        new(){ Audio="sons/piano.mp3",    Opcoes=["Violino","Guitarra","Piano"],   Correta=2, Dica="Instrumento de teclas" },
        new(){ Audio="sons/flauta.mp3",   Opcoes=["Flauta","Tambor","Trompete"],   Correta=0, Dica="Instrumento de sopro" },
        new(){ Audio="sons/tambor.mp3",   Opcoes=["Piano","Tambor","Violino"],     Correta=1, Dica="Instrumento de percussão" },
        new(){ Audio="sons/telefone.mp3", Opcoes=["Campainha","Sirene","Telefone"],Correta=2, Dica="Aparelho de comunicação" },
    ];

    // ── Helpers ──
    private static Pergunta SortearPergunta()
    {
        var rnd = new Random();
        var disponiveis = Enumerable.Range(0, Perguntas.Length).Where(i => !Usadas.Contains(i)).ToList();
        if (disponiveis.Count == 0) { Usadas.Clear(); disponiveis = Enumerable.Range(0, Perguntas.Length).ToList(); }
        var idx = disponiveis[rnd.Next(disponiveis.Count)];
        Usadas.Add(idx);
        var p = Perguntas[idx];
        p.Idx = idx;
        return p;
    }

    // ── Conexão / Desconexão ──
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var idx = Jogadores.FindIndex(j => j.Id == Context.ConnectionId);
        if (idx != -1)
        {
            Jogadores.RemoveAt(idx);
            if (TurnoAtual >= Jogadores.Count) TurnoAtual = 0;
            await Clients.Group("sala").SendAsync("jogadorSaiu", new { jogadores = Jogadores });
            if (Jogadores.Count == 0) { EmAndamento = false; Vencedor = null; Usadas.Clear(); }
        }
        await base.OnDisconnectedAsync(exception);
    }

    // ── Métodos chamados pelo cliente ──
    public async Task NovoJogador(string nome)
    {
        nome = nome.Trim();
        if (string.IsNullOrEmpty(nome)) return;
        if (EmAndamento)  { await Clients.Caller.SendAsync("erro", "Jogo em andamento, aguarde!"); return; }
        if (Jogadores.Count >= 10) { await Clients.Caller.SendAsync("erro", "Sala cheia (max 10)"); return; }

        var j = new Jogador
        {
            Id   = Context.ConnectionId,
            Nome = nome.Length > 18 ? nome[..18] : nome,
            Pos  = 0,
            Cor  = Cores[Jogadores.Count % Cores.Length]
        };
        Jogadores.Add(j);
        await Groups.AddToGroupAsync(Context.ConnectionId, "sala");
        await Clients.Caller.SendAsync("estadoJogo", new
        {
            jogadores = Jogadores, turnoAtual = TurnoAtual,
            meuId = Context.ConnectionId, meta = META
        });
        await Clients.Group("sala").SendAsync("jogadorEntrou", new { jogadores = Jogadores });
    }

    public async Task IniciarJogo()
    {
        if (EmAndamento || Jogadores.Count == 0) return;
        EmAndamento = true;
        await ProximoTurno();
    }

    public async Task Resposta(int opcaoIdx)
    {
        if (!EmAndamento || Vencedor != null) return;
        var j = Jogadores[TurnoAtual];
        if (j.Id != Context.ConnectionId) return;

        var acertou = opcaoIdx == PerguntaAtual!.Correta;
        j.Pos = acertou ? Math.Min(META, j.Pos + 1) : Math.Max(0, j.Pos - 1);

        await Clients.Group("sala").SendAsync("resultadoResposta", new
        {
            jogador             = j,
            acertou,
            jogadores           = Jogadores,
            respostaCorreta     = PerguntaAtual.Correta,
            respostaCorretaTexto= PerguntaAtual.Opcoes[PerguntaAtual.Correta],
            opcaoEscolhida      = opcaoIdx,
        });

        if (j.Pos >= META)
        {
            Vencedor    = j;
            EmAndamento = false;
            await Task.Delay(800);
            await Clients.Group("sala").SendAsync("vitoria", new { vencedor = j, jogadores = Jogadores });
            return;
        }

        await Task.Delay(2500);
        TurnoAtual = (TurnoAtual + 1) % Jogadores.Count;
        await ProximoTurno();
    }

    public async Task ReiniciarJogo()
    {
        Jogadores.ForEach(j => j.Pos = 0);
        TurnoAtual  = 0;
        Usadas.Clear();
        Vencedor    = null;
        EmAndamento = true;
        await ProximoTurno();
    }

    private async Task ProximoTurno()
    {
        if (!EmAndamento || Vencedor != null || Jogadores.Count == 0) return;
        PerguntaAtual = SortearPergunta();
        await Clients.Group("sala").SendAsync("proximoTurno", new
        {
            turnoAtual    = TurnoAtual,
            jogadorDaVez  = Jogadores[TurnoAtual],
            pergunta      = PerguntaAtual,
            jogadores     = Jogadores,
            meta          = META,
        });
    }
}
