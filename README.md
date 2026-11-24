# Projeto Transporte — Pilhas em C#

Aplicação Console para controle de uma frota de vans entre garagens, utilizando `Stack<Veiculo>` para representar cada estacionamento.

## Requisitos
- .NET 8 SDK
- Visual Studio Code (ou Visual Studio)

## Como executar
No terminal, dentro da pasta do projeto:

```bash
dotnet restore
dotnet run
```

## Dados iniciais
O programa inicia com:
- 8 veículos numerados de 1 a 8;
- garagem Congonhas;
- garagem Guarulhos;
- capacidade inicial de 15 passageiros por van.

**Observação:** o enunciado não informa a capacidade das vans. Por isso, foi adotado provisoriamente o valor 15 para a frota inicial. Novos veículos permitem informar a própria capacidade.

## Regras implementadas
- Cada garagem possui uma pilha de veículos (`Stack<Veiculo>`).
- O último veículo estacionado é o primeiro a sair (LIFO).
- A jornada precisa ser iniciada antes de liberar viagens.
- No início da jornada, os veículos são distribuídos alternadamente entre as garagens.
- Cadastros de veículos e garagens só são permitidos com a jornada encerrada.
- A opção de liberar viagem registra passageiros na van do topo da pilha. A viagem só é realizada quando a lotação fica completa.
- Ao viajar, o veículo sai da pilha de origem e entra no topo da pilha de destino.
- Ao encerrar a jornada, é exibido o total transportado por veículo e os registros da jornada são limpos.

## Diagrama de classes

```mermaid
classDiagram
    class Veiculo {
        +int Numero
        +int Capacidade
        +int PassageirosNaViagem
        +int TotalPassageirosJornada
        +AdicionarPassageiros(quantidade) bool
        +ConcluirViagem() int
        +EncerrarJornada()
    }

    class Garagem {
        +string Nome
        -Stack~Veiculo~ veiculos
        +QuantidadeVeiculos int
        +PotencialTransporte int
        +Estacionar(veiculo)
        +RetirarUltimo() Veiculo
        +ConsultarTopo() Veiculo
        +ListarVeiculos()
    }

    class Viagem {
        +int NumeroVeiculo
        +string Origem
        +string Destino
        +int Passageiros
        +DateTime DataHora
    }

    class ControleTransporte {
        -List~Veiculo~ veiculos
        -List~Garagem~ garagens
        -List~Viagem~ viagens
        -bool jornadaIniciada
        +CarregarDadosIniciais()
        +Executar()
        +IniciarJornada()
        +EncerrarJornada()
        +LiberarViagem()
    }

    Garagem "1" o-- "0..*" Veiculo : pilha de veículos
    ControleTransporte "1" o-- "*" Garagem
    ControleTransporte "1" o-- "*" Veiculo
    ControleTransporte "1" o-- "*" Viagem
    Viagem --> Veiculo : identifica
```

## Observação sobre o fluxo
O enunciado não apresenta uma opção separada para cadastrar passageiros. Para atender à regra de lotação completa, a opção **5 — Liberar viagem** pergunta quantos passageiros embarcam. Se a capacidade ainda não for atingida, a van permanece no topo da pilha aguardando novos passageiros. Quando completa, a viagem é registrada e o veículo é transferido para a garagem de destino.
