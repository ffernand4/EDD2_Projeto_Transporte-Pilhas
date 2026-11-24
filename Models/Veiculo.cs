namespace ProjetoTransporte.Models;

public class Veiculo
{
    public int Numero { get; }
    public int Capacidade { get; }
    public int PassageirosNaViagem { get; private set; }
    public int TotalPassageirosJornada { get; private set; }

    public Veiculo(int numero, int capacidade)
    {
        if (numero <= 0) throw new ArgumentException("O número do veículo deve ser positivo.");
        if (capacidade <= 0) throw new ArgumentException("A capacidade deve ser positiva.");

        Numero = numero;
        Capacidade = capacidade;
    }

    public bool AdicionarPassageiros(int quantidade)
    {
        if (quantidade <= 0 || PassageirosNaViagem + quantidade > Capacidade)
            return false;

        PassageirosNaViagem += quantidade;
        return true;
    }

    public int ConcluirViagem()
    {
        int passageiros = PassageirosNaViagem;
        TotalPassageirosJornada += passageiros;
        PassageirosNaViagem = 0;
        return passageiros;
    }

    public void EncerrarJornada()
    {
        PassageirosNaViagem = 0;
        TotalPassageirosJornada = 0;
    }

    public override string ToString() =>
        $"Van {Numero} | Capacidade: {Capacidade} | Passageiros aguardando: {PassageirosNaViagem}";
}
