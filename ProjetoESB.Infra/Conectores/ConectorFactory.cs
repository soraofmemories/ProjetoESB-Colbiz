using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoESB.Infra.Conectores
{
    public interface IConectorFactory
    {
        IConector Resolve(string tipo);
    }

    public class ConectorFactory : IConectorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _registry;

        public ConectorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // Mapeamento entre o tipo (do banco) e a implementação concreta
            _registry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                {"HTTP", typeof(ConectorRest) },
                // { "SOAP", typeof(ConectorSoap) },
                // { "DB", typeof(ConectorDb) },
                // { "QUEUE", typeof(ConectorQueue) },
                // ...
            };
        }

        public IConector Resolve(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                throw new ArgumentException("Tipo de conector não informado.");

            if (!_registry.TryGetValue(tipo, out var tipoConector))
                throw new NotSupportedException($"Tipo de conector '{tipo}' não suportado.");

            // Cria uma instância do conector via DI (injeção de dependência)
            var conector = _serviceProvider.GetService(tipoConector) as IConector;

            if (conector == null)
                throw new InvalidOperationException($"Conector '{tipo}' não pôde ser resolvido.");

            return conector;
        }
    }
}

