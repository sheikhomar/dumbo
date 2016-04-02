namespace dumbo.Compiler
{
    public interface IVisitable
    {
        VisitResult Accept(IVisitor visitor, VisitorArgs arg);
    }
}