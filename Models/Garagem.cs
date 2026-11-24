namespace ProjetoTransporte.Models;

public class Garagem
{
    public string Nome { get; }
    private readonly Stack<Veiculo> _veiculos = new();

    public Garagem(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da garagem é obrigatório.");

        Nome = nome.Trim();
    }

    public int QuantidadeVeiculos => _veiculos.Count;
    public int PotencialTransporte => _veiculos.Sum(v => v.Capacidade);

    public void Estacionar(Veiculo veiculo) => _veiculos.Push(veiculo);

    public Veiculo? RetirarUltimo() =>
        _veiculos.Count > 0 ? _veiculos.Pop() : null;

    public Veiculo? ConsultarTopo() =>
        _veiculos.Count > 0 ? _veiculos.Peek() : null;

    public IEnumerable<Veiculo> ListarVeiculos() => _veiculos.ToArray();

    public void Limpar() => _veiculos.Clear();
}
