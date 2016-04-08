using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Models
{
    public interface IElement
    {
        int Id { get; }

        string LabelName { get; set; }
    }
}
