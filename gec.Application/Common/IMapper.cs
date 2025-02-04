namespace gec.Application.Common;

public interface IMapper<TInput, TOutput>
{
    TOutput Map(TInput input);
    IEnumerable<TOutput> Map(IEnumerable<TInput> inputs);
}