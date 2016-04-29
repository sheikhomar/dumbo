namespace dumbo.Compiler
{
    public interface IVisitable
    {
        T Accept<T, K>(IVisitor<T, K> visitor, K arg);
    }
}
