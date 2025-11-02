using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Signs.Queries
{
    public class GetSignHandler
    {
        private readonly ISignService? _signService;

        public GetSignHandler(ISignService? signService)
        {  
            _signService = signService;
        }

        public async Task<List<SignDto?>> GetListSignBy(string projecName)
        {
            var listSign = await _signService!.GetListSignAsync(projecName);
            return listSign;
        }
    }
}
