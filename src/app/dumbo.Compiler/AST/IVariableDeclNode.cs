namespace dumbo.Compiler.AST
{
    /// <summary>
    /// Abstraction over a node that declare one or more variables.
    /// </summary>
    public interface IVariableDeclNode
    {
        TypeNode Type { get; }
    }
}