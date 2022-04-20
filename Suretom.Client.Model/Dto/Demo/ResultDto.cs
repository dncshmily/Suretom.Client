using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suretom.Client.Entity
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultDto<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public bool Passed { get; set; }
        public string IsEducation { get; set; }
        public List<T> List { get; set; }
    }
}