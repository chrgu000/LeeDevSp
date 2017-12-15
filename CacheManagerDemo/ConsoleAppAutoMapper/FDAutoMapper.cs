using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppAutoMapper
{
    public class FDAutoMapper
    {

        public void CreateMap<F,T>()
        {
            AutoMapper.Mapper.Initialize(cfg => cfg.CreateMap<F, T>().ForAllMembers(opt=>);
        }
    }
}
