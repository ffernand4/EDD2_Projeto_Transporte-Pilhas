using ProjetoTransporte.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var sistema = new ControleTransporte();
sistema.CarregarDadosIniciais();
sistema.Executar();
