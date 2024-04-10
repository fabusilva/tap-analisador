using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ContadorFrequenciaPalavras
{
    class Program
    {
        static void Main(string[] args)
        {
            string diretorioEntrada = "./vikings-first-season";
            string diretorioSaida = "./resultado";

            if (!Directory.Exists(diretorioSaida))
            {
                Directory.CreateDirectory(diretorioSaida);
            }

            string[] arquivos = Directory.GetFiles(diretorioEntrada, "*.srt");

            foreach (string arquivo in arquivos)
            {
                Dictionary<string, int> frequenciaPalavras = CalcularFrequenciaPalavras(arquivo);

                var palavrasOrdenadas = frequenciaPalavras.OrderByDescending(p => p.Value).Select(
                    p => new { palavra = p.Key, frequencia = p.Value }).ToList();
                    
                string nomeArquivoSaida = Path.Combine(diretorioSaida,Path.GetFileNameWithoutExtension(arquivo) + ".json");
                string jsonOutput = JsonConvert.SerializeObject(palavrasOrdenadas,Formatting.Indented);
                File.WriteAllText(nomeArquivoSaida, jsonOutput);
            }

            Console.WriteLine("Processo concluído. Arquivos JSON gerados com sucesso.");
        }

        static Dictionary<string, int> CalcularFrequenciaPalavras(string arquivo)
        {
            string textoCompleto = File.ReadAllText(arquivo);
            textoCompleto = Regex.Replace(textoCompleto, @"\[\d{2}:\d{2}:\d{2}\]", "");
            string[] palavras = Regex.Split(textoCompleto, @"\W+");

            Dictionary<string, int> frequenciaPalavras = palavras
                .Where(palavra => !string.IsNullOrEmpty(palavra) && !palavra.Any(char.IsDigit))
                .Select(palavra => palavra.ToLowerInvariant())
                .GroupBy(palavra => palavra)
                .ToDictionary(group => group.Key, group => group.Count());

            return frequenciaPalavras;
        }
    }
}
