using ProjetoTransporte.Models;

namespace ProjetoTransporte.Services;

public class ControleTransporte
{
    private readonly List<Veiculo> _veiculos = new();
    private readonly List<Garagem> _garagens = new();
    private readonly List<Viagem> _viagens = new();
    private bool _jornadaIniciada;
    private int _proximoNumeroVeiculo = 1;

    public void CarregarDadosIniciais()
    {
        _garagens.Add(new Garagem("Congonhas"));
        _garagens.Add(new Garagem("Guarulhos"));

        // A capacidade não foi definida no enunciado. Foi adotado 15 como padrão inicial.
        for (int i = 1; i <= 8; i++)
            _veiculos.Add(new Veiculo(i, 15));

        _proximoNumeroVeiculo = 9;
        Console.WriteLine("Dados iniciais carregados: 8 vans (capacidade padrão de 15 passageiros),");
        Console.WriteLine("garagens Congonhas e Guarulhos.");
    }

    public void Executar()
    {
        int opcao;
        do
        {
            ExibirMenu();
            opcao = LerInteiro("Escolha uma opção: ", 0, 9);

            try
            {
                switch (opcao)
                {
                    case 0: Console.WriteLine("Programa finalizado."); break;
                    case 1: CadastrarVeiculo(); break;
                    case 2: CadastrarGaragem(); break;
                    case 3: IniciarJornada(); break;
                    case 4: EncerrarJornada(); break;
                    case 5: LiberarViagem(); break;
                    case 6: ListarVeiculosGaragem(); break;
                    case 7: InformarQuantidadeViagens(); break;
                    case 8: ListarViagens(); break;
                    case 9: InformarPassageirosTransportados(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível concluir a operação: {ex.Message}");
            }

            if (opcao != 0) Pausar();
        } while (opcao != 0);
    }

    private static void ExibirMenu()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("        SISTEMA DE TRANSPORTE");
        Console.WriteLine("====================================");
        Console.WriteLine("0 - Finalizar");
        Console.WriteLine("1 - Cadastrar veículo");
        Console.WriteLine("2 - Cadastrar garagem");
        Console.WriteLine("3 - Iniciar jornada");
        Console.WriteLine("4 - Encerrar jornada");
        Console.WriteLine("5 - Liberar viagem");
        Console.WriteLine("6 - Listar veículos de uma garagem");
        Console.WriteLine("7 - Quantidade de viagens por rota");
        Console.WriteLine("8 - Listar viagens por rota");
        Console.WriteLine("9 - Passageiros transportados por rota");
        Console.WriteLine("------------------------------------");
        Console.WriteLine($"Jornada: {(_jornadaIniciada ? "INICIADA" : "ENCERRADA")}");
    }

    private void CadastrarVeiculo()
    {
        ExigirJornadaEncerrada();
        Console.WriteLine("\n--- Cadastro de veículo ---");
        int capacidade = LerInteiro("Capacidade de passageiros: ", 1, 200);
        var veiculo = new Veiculo(_proximoNumeroVeiculo++, capacidade);
        _veiculos.Add(veiculo);
        Console.WriteLine($"Veículo {veiculo.Numero} cadastrado com sucesso.");
    }

    private void CadastrarGaragem()
    {
        ExigirJornadaEncerrada();
        Console.WriteLine("\n--- Cadastro de garagem ---");
        string nome = LerTexto("Nome da nova garagem: ");
        if (_garagens.Any(g => g.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Já existe uma garagem com esse nome.");
            return;
        }
        _garagens.Add(new Garagem(nome));
        Console.WriteLine("Garagem cadastrada com sucesso.");
    }

    private void IniciarJornada()
    {
        if (_jornadaIniciada)
        {
            Console.WriteLine("A jornada já está iniciada.");
            return;
        }
        if (_garagens.Count < 2)
        {
            Console.WriteLine("Cadastre pelo menos duas garagens para iniciar a jornada.");
            return;
        }

        // Reorganiza a frota alternadamente entre as garagens cadastradas.
        foreach (var garagem in _garagens) garagem.Limpar();

        for (int i = 0; i < _veiculos.Count; i++)
            _garagens[i % _garagens.Count].Estacionar(_veiculos[i]);

        _viagens.Clear();
        foreach (var veiculo in _veiculos) veiculo.EncerrarJornada();

        _jornadaIniciada = true;
        Console.WriteLine("Jornada iniciada. Veículos distribuídos alternadamente entre as garagens.");
        MostrarDistribuicao();
    }

    private void EncerrarJornada()
    {
        if (!_jornadaIniciada)
        {
            Console.WriteLine("Não há jornada em andamento.");
            return;
        }

        Console.WriteLine("\n========== RELATÓRIO DA JORNADA ==========");
        foreach (var veiculo in _veiculos.OrderBy(v => v.Numero))
            Console.WriteLine($"Veículo {veiculo.Numero}: {veiculo.TotalPassageirosJornada} passageiros transportados");

        Console.WriteLine($"Total de viagens: {_viagens.Count}");
        Console.WriteLine("As ocorrências da jornada serão limpas.");
        _viagens.Clear();
        foreach (var veiculo in _veiculos) veiculo.EncerrarJornada();
        _jornadaIniciada = false;
        Console.WriteLine("Jornada encerrada.");
    }

    private void LiberarViagem()
    {
        ExigirJornadaIniciada();
        Console.WriteLine("\n--- Liberar viagem ---");
        Garagem? origem = SelecionarGaragem("Origem: ");
        if (origem is null) return;
        Garagem? destino = SelecionarGaragem("Destino: ");
        if (destino is null) return;
        if (ReferenceEquals(origem, destino))
        {
            Console.WriteLine("A origem e o destino devem ser diferentes.");
            return;
        }

        Veiculo? veiculo = origem.ConsultarTopo();
        if (veiculo is null)
        {
            Console.WriteLine($"A garagem {origem.Nome} está vazia. Aguarde um veículo retornar.");
            return;
        }

        Console.WriteLine($"Veículo disponível no topo da pilha: Van {veiculo.Numero} (capacidade {veiculo.Capacidade}).");
        Console.WriteLine($"Passageiros já registrados nesta van: {veiculo.PassageirosNaViagem}.");
        int vagas = veiculo.Capacidade - veiculo.PassageirosNaViagem;
        int novosPassageiros = LerInteiro($"Quantidade de passageiros que embarcarão (1 a {vagas}): ", 1, vagas);

        if (!veiculo.AdicionarPassageiros(novosPassageiros))
        {
            Console.WriteLine("Quantidade inválida.");
            return;
        }

        if (veiculo.PassageirosNaViagem < veiculo.Capacidade)
        {
            Console.WriteLine($"Lotação ainda não atingida ({veiculo.PassageirosNaViagem}/{veiculo.Capacidade}).");
            Console.WriteLine("A van permanece na garagem e poderá receber mais passageiros.");
            return;
        }

        origem.RetirarUltimo();
        int passageiros = veiculo.ConcluirViagem();
        destino.Estacionar(veiculo);
        _viagens.Add(new Viagem(veiculo.Numero, origem.Nome, destino.Nome, passageiros));
        Console.WriteLine($"Viagem liberada e concluída: Van {veiculo.Numero}, {origem.Nome} -> {destino.Nome}.");
        Console.WriteLine($"A van chegou à garagem {destino.Nome} e foi estacionada.");
    }

    private void ListarVeiculosGaragem()
    {
        Garagem? garagem = SelecionarGaragem("Garagem para consulta: ");
        if (garagem is null) return;

        Console.WriteLine($"\nGaragem: {garagem.Nome}");
        Console.WriteLine($"Quantidade de veículos: {garagem.QuantidadeVeiculos}");
        Console.WriteLine($"Potencial total de transporte: {garagem.PotencialTransporte} passageiros");
        Console.WriteLine("Veículos (do topo da pilha para a base):");
        if (garagem.QuantidadeVeiculos == 0) Console.WriteLine("Nenhum veículo estacionado.");
        foreach (var veiculo in garagem.ListarVeiculos())
            Console.WriteLine($"  {veiculo}");
    }

    private void InformarQuantidadeViagens()
    {
        (string origem, string destino) = LerRota();
        int quantidade = _viagens.Count(v => RotaIgual(v, origem, destino));
        Console.WriteLine($"Quantidade de viagens de {origem} para {destino}: {quantidade}");
    }

    private void ListarViagens()
    {
        (string origem, string destino) = LerRota();
        var viagens = _viagens.Where(v => RotaIgual(v, origem, destino)).ToList();
        Console.WriteLine($"\nViagens de {origem} para {destino}:");
        if (viagens.Count == 0) Console.WriteLine("Nenhuma viagem registrada nessa rota.");
        else foreach (var viagem in viagens) Console.WriteLine(viagem);
    }

    private void InformarPassageirosTransportados()
    {
        (string origem, string destino) = LerRota();
        int total = _viagens.Where(v => RotaIgual(v, origem, destino)).Sum(v => v.Passageiros);
        Console.WriteLine($"Passageiros transportados de {origem} para {destino}: {total}");
    }

    private (string origem, string destino) LerRota()
    {
        Console.WriteLine("\n--- Consulta por rota ---");
        Garagem? origem = SelecionarGaragem("Origem: ");
        Garagem? destino = SelecionarGaragem("Destino: ");
        return (origem?.Nome ?? "", destino?.Nome ?? "");
    }

    private Garagem? SelecionarGaragem(string mensagem)
    {
        Console.WriteLine(mensagem);
        for (int i = 0; i < _garagens.Count; i++)
            Console.WriteLine($"{i + 1} - {_garagens[i].Nome}");

        int indice = LerInteiro("Selecione: ", 1, _garagens.Count) - 1;
        return _garagens[indice];
    }

    private void MostrarDistribuicao()
    {
        foreach (var garagem in _garagens)
            Console.WriteLine($"{garagem.Nome}: {string.Join(", ", garagem.ListarVeiculos().Select(v => v.Numero))}");
    }

    private void ExigirJornadaIniciada()
    {
        if (!_jornadaIniciada)
            throw new InvalidOperationException("Inicie a jornada antes de liberar viagens.");
    }

    private void ExigirJornadaEncerrada()
    {
        if (_jornadaIniciada)
            throw new InvalidOperationException("Cadastros só podem ser realizados com a jornada encerrada.");
    }

    private static bool RotaIgual(Viagem viagem, string origem, string destino) =>
        viagem.Origem.Equals(origem, StringComparison.OrdinalIgnoreCase) &&
        viagem.Destino.Equals(destino, StringComparison.OrdinalIgnoreCase);

    private static int LerInteiro(string mensagem, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensagem);
            if (int.TryParse(Console.ReadLine(), out int valor) && valor >= minimo && valor <= maximo)
                return valor;
            Console.WriteLine($"Digite um número entre {minimo} e {maximo}.");
        }
    }

    private static string LerTexto(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string? valor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(valor)) return valor.Trim();
            Console.WriteLine("O campo não pode ficar vazio.");
        }
    }

    private static void Pausar()
    {
        Console.WriteLine("\nPressione ENTER para continuar...");
        Console.ReadLine();
    }
}
