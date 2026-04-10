namespace TrilhaDosSons.Models;

public class Jogador
{
    public string Id      { get; set; } = "";
    public string Nome    { get; set; } = "";
    public int    Pos     { get; set; } = 0;
    public string Cor     { get; set; } = "#22c55e";
}

public class Pergunta
{
    public string   Audio  { get; set; } = "";
    public string[] Opcoes { get; set; } = [];
    public int      Correta { get; set; }
    public string   Dica   { get; set; } = "";
    public int      Idx    { get; set; }
}

public class EstadoJogo
{
    public List<Jogador> Jogadores   { get; set; } = [];
    public int           TurnoAtual  { get; set; } = 0;
    public Pergunta?     PerguntaAtual { get; set; }
    public List<int>     Usadas      { get; set; } = [];
    public bool          EmAndamento { get; set; } = false;
    public Jogador?      Vencedor    { get; set; }
}
