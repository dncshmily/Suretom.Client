using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suretom.Client.Entity
{
    public class DoingCellsDto
    {
        public int Code { get; set; }
        public Cell Cell { get; set; } = new Cell();
    }
    public class Cell
    { 
        public string DocId { get; set; }
        public string Id { get; set; }
        public double LastTime { get; set; }
        public string ResType { get; set; }
        public bool Status { get; set; }
        public string Title { get; set; }
    }
}