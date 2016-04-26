namespace dumbo.Compiler.AST
{
    /// <summary>
    /// Abstraction over a node that declare one variable.
    /// </summary>
    public interface IVariableDeclNode
    {
        TypeNode Type { get; }
    }
}