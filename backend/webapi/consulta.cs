using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;

class Program
{
    static void Main()
    {
        // Conexão com o MongoDB
        var connectionString = "mongodb+srv://tony2k01:CywW2sGwtrvH7MDL@nexmuv.goneei7.mongodb.net/projeto-nexmuv?retryWrites=true&w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("yahoo");
        var collection = database.GetCollection<BsonDocument>("finance");

        // Consulta de dados usando o ID específico
        var objectId = ObjectId.Parse("656834c7911c533ad27f21b5");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
        var projection = Builders<BsonDocument>.Projection
            .Include("chart.result.timestamp")
            .Include("chart.result.indicators.quote.close");

        var result = collection.Find(filter).Project(projection).FirstOrDefault();

        // Exibição do JSON bruto
        if (result != null)
        {
            var timestamps = result["chart"]["result"][0]["timestamp"].AsBsonArray.Select(t => t.ToInt64()).ToList();
            var closes = result["chart"]["result"][0]["indicators"]["quote"][0]["close"].AsBsonArray.Select(c => c.ToDouble()).ToList();

            Console.WriteLine($"Últimos 30 pregões:");

            for (int i = timestamps.Count - 1; i >= timestamps.Count - 30; i--)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(timestamps[i]).DateTime;
                var close = closes[i];

                Console.WriteLine($"Data: {date}, Preço de Fechamento: {close}");
            }

            // Cálculo da variação de preço
            var variacoes = Enumerable.Range(timestamps.Count - 30 + 1, 29)
                .Select(i => closes[i] - closes[i - 1]);

            Console.WriteLine("\nVariação de Preço:");

            foreach (var variacao in variacoes)
            {
                Console.WriteLine($"Variação: {variacao}");
            }
        }
        else
        {
            Console.WriteLine("Documento não encontrado com o ID fornecido.");
        }
    }
}