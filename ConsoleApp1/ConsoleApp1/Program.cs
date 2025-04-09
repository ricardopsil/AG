using System;
using System.Collections.Generic;
using System.Linq;

class AG_Roteamento
{
    static Random rand = new Random();
    const int TAMANHO_POPULACAO = 100;
    const int NUM_GERACOES = 50;
    const int TAMANHO_CROMOSSOMO = 9;
    const double TAXA_MUTACAO = 0.1;

    static void Main()
    {
        List<List<int>> populacao = GerarPopulacaoInicial();

        for (int geracao = 0; geracao < NUM_GERACOES; geracao++)
        {
            // Avaliar fitness
            List<(List<int> rota, int fitness)> avaliada = populacao
                .Select(rota => (rota, AvaliarRota(rota)))
                .OrderBy(par => par.Item2) // menor penalidade é melhor
                .ToList();

            // Mostrar a melhor da geração
            Console.WriteLine($"Geração {geracao + 1} - Melhor rota: {string.Join(",", avaliada[0].rota)} | Penalidade: {avaliada[0].fitness}");

            // Elitismo - manter os 10 melhores
            List<List<int>> novaPopulacao = avaliada.Take(10).Select(p => new List<int>(p.rota)).ToList();

            // Gerar nova população via cruzamento
            while (novaPopulacao.Count < TAMANHO_POPULACAO)
            {
                var pai1 = SelecionarPorTorneio(avaliada);
                var pai2 = SelecionarPorTorneio(avaliada);
                var filho = Cruzamento(pai1, pai2);
                Mutar(filho);
                novaPopulacao.Add(filho);
            }

            populacao = novaPopulacao;
        }
    }

    static List<List<int>> GerarPopulacaoInicial()
    {
        List<List<int>> populacao = new List<List<int>>();
        for (int i = 0; i < TAMANHO_POPULACAO; i++)
        {
            List<int> rota = Enumerable.Range(1, TAMANHO_CROMOSSOMO).OrderBy(x => rand.Next()).ToList();
            populacao.Add(rota);
        }
        return populacao;
    }

    static int AvaliarRota(List<int> rota)
    {
        int penalidade = 0;

        // Penalidade por ordem incorreta
        for (int i = 0; i < rota.Count - 1; i++)
        {
            if (rota[i] > rota[i + 1])
            {
                penalidade += 10;
            }
        }

        // Penalidade por duplicatas
        HashSet<int> vistas = new HashSet<int>();
        foreach (int cidade in rota)
        {
            if (!vistas.Add(cidade))
            {
                penalidade += 20;
            }
        }

        return penalidade;
    }

    static List<int> SelecionarPorTorneio(List<(List<int> rota, int fitness)> populacao)
    {
        int torneioTamanho = 5;
        var selecionados = new List<(List<int>, int)>();
        for (int i = 0; i < torneioTamanho; i++)
        {
            var escolhido = populacao[rand.Next(populacao.Count)];
            selecionados.Add(escolhido);
        }
        return selecionados.OrderBy(p => p.Item2).First().Item1;
    }

    static List<int> Cruzamento(List<int> pai1, List<int> pai2)
    {
        int pontoCorte = rand.Next(1, TAMANHO_CROMOSSOMO - 1);
        List<int> filho = pai1.Take(pontoCorte).ToList();
        foreach (int gene in pai2)
        {
            if (!filho.Contains(gene))
                filho.Add(gene);
        }
        return filho;
    }

    static void Mutar(List<int> individuo)
    {
        if (rand.NextDouble() < TAXA_MUTACAO)
        {
            int idx1 = rand.Next(TAMANHO_CROMOSSOMO);
            int idx2 = rand.Next(TAMANHO_CROMOSSOMO);
            int temp = individuo[idx1];
            individuo[idx1] = individuo[idx2];
            individuo[idx2] = temp;
        }
    }
}