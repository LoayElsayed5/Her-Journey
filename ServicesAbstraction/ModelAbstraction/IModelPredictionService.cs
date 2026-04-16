using Shared.DTos.MlDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstraction.ModelAbstraction
{
    public interface IModelPredictionService
    {
        Task<PredictionResponseDto> PredictAsync(PredictionRequestDto request);
    }
}
