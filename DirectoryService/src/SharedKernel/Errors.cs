using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel;

public class Errors : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public Errors(IEnumerable<Error> errors)
    {
        _errors = [..errors];
    }

    public IEnumerator<Error> GetEnumerator() 
    { 
        return _errors.GetEnumerator(); 
    }

    IEnumerator IEnumerable.GetEnumerator() => _errors.GetEnumerator();

    public static implicit operator Errors(List<Error> errors) => new Errors(errors);

    public static implicit operator Errors(Error error) => new Errors([error]);
}
