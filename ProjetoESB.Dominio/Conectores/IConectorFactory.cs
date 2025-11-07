namespace ProjetoESB.Dominio.Conectores
{
    public interface IConectorFactory
    {
        IConector Resolve(string tipo);
    }
}
