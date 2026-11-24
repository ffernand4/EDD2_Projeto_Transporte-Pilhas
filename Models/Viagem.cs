namespace ProjetoTransporte.Models;

public class Viagem
{
    public int NumeroVeiculo { get; }
    public string Origem { get; }
    public string Destino { get; }
    public int Passageiros { get; }
    public DateTime DataHora { get; }

    public Viagem(int numeroVeiculo, string origem, string destino, int passageiros)
    {
        NumeroVeiculo = numeroVeiculo;
        Origem = origem;
        Destino = destino;
        Passageiros = passageiros;
        DataHora = DateTime.Now;
    }

    public override string ToString() =>
        $"{DataHora:dd/MM/yyyy HH:mm:ss} | Van {NumeroVeiculo} | {Origem} -> {Destino} | {Passageiros} passageiros";
}
