namespace gec.Application.Common;

public interface IMapper<TInput, TOutput>
{
    TOutput Map(TInput input); // Mapea un solo elemento
    IEnumerable<TOutput> Map(IEnumerable<TInput> inputs); // Mapea una lista a una lista
    TOutput MapListToSingle(IEnumerable<TInput> inputs); // Mapea una lista a un único elemento
    
    // Método genérico que permite dependencias adicionales
    TOutput MapWithDependencies(IEnumerable<TInput> inputs, object dependencies);    
}