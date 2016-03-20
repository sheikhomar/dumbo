namespace dumbo.Compiler.AST.Builder
{
    public interface IConcreteTreeNode
    {
        string Name { get; }
        string Data { get; }
        IConcreteTreeNode TryFindFirstChild(string childName);
        IConcreteTreeNode GetFirstChild(string childName);
        IConcreteTreeNode this[int index] { get; }
        int ChildCount { get; }
        bool HasChild(string childName);
    }
}