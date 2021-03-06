﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApp.DTO
{
    public class ProductCreation
    {
        [Display(Name = "产品名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "{0}的长度应该大于{2}，小于{1}")]
        public string Name { get; set; }

        [Display(Name = "价格")]
        [Range(0, Double.MaxValue, ErrorMessage = "{0}的值必须大于{1}")]
        public float Price { get; set; }
    }
}
