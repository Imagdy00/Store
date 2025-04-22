using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions;
public class BasketDeleteBadRequest() : BadRequestException($"Invalid operation whern delete basket")
{

}
