using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySamplePro.Model
{
    public class DaiBanParaPoco
    {
        /// <summary>
        /// 当前登录用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 每页多少行 
        /// </summary>
        [Required(ErrorMessage = "每页多少行不能为空")]
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页 
        /// </summary>
        [Required(ErrorMessage = "当前页不能为空")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 排许方式：0：紧急程度 1：提交时间 2：创建时间
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 0：升序  1：降序
        /// </summary>
        public int OrderType { get; set; }
    }
}
