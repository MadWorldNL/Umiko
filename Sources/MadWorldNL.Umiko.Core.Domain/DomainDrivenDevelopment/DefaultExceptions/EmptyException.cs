namespace MadWorldNL.Umiko.DomainDrivenDevelopment.DefaultExceptions;

public class EmptyException<TType>() : Exception($"{typeof(TType)} cannot contain empty value.")
{
    
}