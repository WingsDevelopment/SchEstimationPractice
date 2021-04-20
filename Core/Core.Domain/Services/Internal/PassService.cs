using Core.Domain.Services.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Services.Internal
{
    public class PassService : IPassService
    {
        Random random = new Random();
        //premestiti u config
        private readonly int PASS_MIN = 0;
        private readonly int PASS_MAX = 0;

        public PassService(string minPassConfig, string maxPassConfig)
        {
            if (!int.TryParse(minPassConfig, out PASS_MIN)) throw new Exception("Invalid minPassConfig string");
            if (!int.TryParse(maxPassConfig, out PASS_MAX)) throw new Exception("Invalid maxPassConfig string");
        }

        public int GeneratePASS()
        {
            return random.Next(PASS_MIN, PASS_MAX);
        }
    }
}
